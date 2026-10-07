using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Wang.Seamas.Shared.AspNetCore.ExceptionHandling;
using Wang.Seamas.Shared.DTOs;

namespace Wang.Seamas.Shared.AspNetCore.Filters;

/// <summary>
/// 全局异常过滤器 - 处理 Controller/Action 中的异常
/// 支持的异常类型通过 IExceptionMapper + SharedAspNetCoreOptions 扩展，公共模块不写死具体异常
/// 统一返回 HTTP 200，错误类型由 ApiResult.Code 表达，方便链路追踪
/// </summary>
public class GlobalExceptionFilter(
    IExceptionMapper exceptionMapper,
    ILogger<GlobalExceptionFilter> logger) : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        var ex = context.Exception;

        var mapping = exceptionMapper.Map(ex)
                      ?? new ExceptionMapping(500, ex.Message, LogLevel.Error);

        logger.Log(mapping.LogLevel, ex, "全局异常过滤器：{Message}", ex.Message);

        context.Result = new ObjectResult(ApiResult.Fail(mapping.Message, mapping.Code))
        {
            StatusCode = StatusCodes.Status200OK
        };
        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}
