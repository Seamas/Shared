namespace Wang.Seamas.Shared.AspNetCore.ExceptionHandling;

/// <summary>
/// 异常映射器：将异常转换为对外统一响应所需的映射信息
/// Filter 与 Middleware 共用同一实现，保证两条异常处理链路行为一致
/// </summary>
public interface IExceptionMapper
{
    /// <summary>
    /// 按异常的实际类型（沿继承链）查找已注册的映射
    /// </summary>
    /// <param name="exception">实际捕获到的异常</param>
    /// <returns>映射信息；未注册任何匹配类型时返回 null</returns>
    ExceptionMapping? Map(Exception exception);
}
