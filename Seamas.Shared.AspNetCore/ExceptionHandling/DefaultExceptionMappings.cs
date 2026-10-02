using Microsoft.Extensions.Logging;
using Wang.Seamas.Shared.Exceptions;

namespace Wang.Seamas.Shared.AspNetCore.ExceptionHandling;

/// <summary>
/// 内置默认异常映射
/// 消费方可通过 SharedAspNetCoreOptions.Map&lt;TException&gt; 覆盖其中任意一项
/// </summary>
public static class DefaultExceptionMappings
{
    public static void Apply(SharedAspNetCoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Map<LoginLockoutException>(ex =>
            new ExceptionMapping(429, ex.Message, LogLevel.Warning));

        options.Map<UnauthorizedAccessException>(ex =>
            new ExceptionMapping(401, ex.Message, LogLevel.Warning));

        options.Map<AuthException>(ex =>
            new ExceptionMapping(401, ex.Message, LogLevel.Warning));

        options.Map<ValidateException>(ex =>
            new ExceptionMapping(400, ex.Message, LogLevel.Warning));

        options.Map<BizException>(ex =>
            new ExceptionMapping(ex.Code, ex.Message, LogLevel.Warning));

        // 兜底：其余异常按系统错误处理
        options.Map<Exception>(ex =>
            new ExceptionMapping(500, ex.Message, LogLevel.Error));
    }
}
