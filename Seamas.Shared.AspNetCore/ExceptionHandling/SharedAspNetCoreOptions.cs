namespace Wang.Seamas.Shared.AspNetCore.ExceptionHandling;

/// <summary>
/// Shared.AspNetCore 模块选项
/// </summary>
public class SharedAspNetCoreOptions
{
    /// <summary>
    /// 异常类型 -> 映射工厂
    /// 同一类型重复注册时以后注册的为准（用于覆盖默认映射）
    /// </summary>
    internal Dictionary<Type, Func<Exception, ExceptionMapping>> ExceptionMappings { get; } = new();

    /// <summary>
    /// 注册或覆盖指定异常类型的映射
    /// </summary>
    /// <typeparam name="TException">异常类型（匹配时沿继承链向上查找）</typeparam>
    /// <param name="mapper">映射工厂，入参为实际捕获到的异常</param>
    public void Map<TException>(Func<TException, ExceptionMapping> mapper) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(mapper);

        ExceptionMappings[typeof(TException)] = ex => mapper((TException)ex);
    }
}
