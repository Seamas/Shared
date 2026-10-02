using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wang.Seamas.Shared.DTOs;

namespace Wang.Seamas.Shared.AspNetCore.Filters;

/// <summary>
/// 模型校验过滤器 - ModelState 无效时直接返回统一错误结构，不进入 Action
/// </summary>
public class ModelValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToArray();

            var message = string.Join("; ", errors);

            context.Result = new ObjectResult(ApiResult.Fail(message, 400))
            {
                StatusCode = StatusCodes.Status200OK
            };
            return;
        }

        await next();
    }
}
