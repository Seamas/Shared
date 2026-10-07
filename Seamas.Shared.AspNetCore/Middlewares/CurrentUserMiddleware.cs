using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Wang.Seamas.Shared;

namespace Wang.Seamas.Shared.AspNetCore.Middlewares;

/// <summary>
/// 当前用户中间件 - 从已认证的 ClaimsPrincipal 中解析用户 ID 写入 CurrentUserContext
/// 请求结束时清理 AsyncLocal，避免上下文泄漏
/// </summary>
public class CurrentUserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userId, out var id))
                {
                    CurrentUserContext.UserId = id;
                }
            }

            await next(context);
        }
        finally
        {
            CurrentUserContext.Clear();
        }
    }
}
