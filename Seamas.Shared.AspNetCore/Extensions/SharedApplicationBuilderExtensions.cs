using Microsoft.AspNetCore.Builder;
using Wang.Seamas.Shared.AspNetCore.Middlewares;

namespace Wang.Seamas.Shared.AspNetCore.Extensions;

public static class SharedApplicationBuilderExtensions
{
    /// <summary>
    /// 全局错误处理中间件，应放在管道最外层（UseRouting 之前）
    /// </summary>
    public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseMiddleware<GlobalErrorHandlerMiddleware>();
    }

    /// <summary>
    /// 当前用户中间件，在认证完成之后、业务权限判断之前使用
    /// </summary>
    public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseMiddleware<CurrentUserMiddleware>();
    }
}
