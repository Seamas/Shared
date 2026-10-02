using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wang.Seamas.Shared.AspNetCore.ExceptionHandling;
using Wang.Seamas.Shared.DTOs;

namespace Wang.Seamas.Shared.AspNetCore.Middlewares;

/// <summary>
/// 全局错误处理中间件（管道最外层）
/// 1. 捕获后续中间件（MVC 之外）抛出的异常，异常类型映射通过 IExceptionMapper 扩展
/// 2. 对非 200 的状态码响应（404/403/401 等）补充统一错误结构
/// 统一返回 HTTP 200，错误类型由 ApiResult.Code 表达
/// </summary>
public class GlobalErrorHandlerMiddleware(
    RequestDelegate next,
    IExceptionMapper exceptionMapper,
    ILogger<GlobalErrorHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);

            if (context.Response.StatusCode != StatusCodes.Status200OK && !context.Response.HasStarted)
            {
                var (message, code) = context.Response.StatusCode switch
                {
                    StatusCodes.Status404NotFound => ("请求地址错误", 404),
                    StatusCodes.Status403Forbidden => ("未授权访问", 403),
                    StatusCodes.Status401Unauthorized => ("用户未登录", 401),
                    _ => ("未知错误", context.Response.StatusCode)
                };

                await WriteErrorResponseAsync(context, message, code);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var mapping = exceptionMapper.Map(ex)
                      ?? new ExceptionMapping(500, ex.Message, LogLevel.Error);

        logger.Log(mapping.LogLevel, ex, "全局错误中间件：{Message}", ex.Message);

        if (context.Response.HasStarted)
        {
            logger.LogError("响应已开始，无法返回错误信息");
            return;
        }

        await WriteErrorResponseAsync(context, mapping.Message, mapping.Code);
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, string message, int code)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsJsonAsync(ApiResult.Fail(message, code));
    }
}
