using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Wang.Seamas.Shared.AspNetCore.ExceptionHandling;

/// <summary>
/// 默认异常映射器
/// 查找规则：以异常运行时类型为起点沿继承链向上匹配，最近的已注册基类生效
/// 类型 -> 映射工厂的查找结果会被缓存（工厂本身在每次调用时执行，支持 ex.Code / ex.Message 等实例相关输出）
/// </summary>
public class ExceptionMapper : IExceptionMapper
{
    private readonly ConcurrentDictionary<Type, Func<Exception, ExceptionMapping>?> _factoryCache = new();
    private readonly IReadOnlyDictionary<Type, Func<Exception, ExceptionMapping>> _mappings;

    public ExceptionMapper(IOptions<SharedAspNetCoreOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _mappings = options.Value.ExceptionMappings;
    }

    /// <inheritdoc />
    public ExceptionMapping? Map(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var factory = _factoryCache.GetOrAdd(exception.GetType(), FindFactory);

        return factory?.Invoke(exception);
    }

    private Func<Exception, ExceptionMapping>? FindFactory(Type exceptionType)
    {
        for (var current = exceptionType; current is not null; current = current.BaseType)
        {
            if (_mappings.TryGetValue(current, out var factory))
            {
                return factory;
            }
        }

        return null;
    }
}
