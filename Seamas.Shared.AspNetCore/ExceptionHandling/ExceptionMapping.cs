using Microsoft.Extensions.Logging;

namespace Wang.Seamas.Shared.AspNetCore.ExceptionHandling;

/// <summary>
/// 异常映射结果：异常对外转换为统一响应时所需的信息
/// </summary>
/// <param name="Code">业务错误码（写入 ApiResult.Code）</param>
/// <param name="Message">对外提示消息</param>
/// <param name="LogLevel">该异常的日志级别（业务可预期异常通常 Warning，系统异常 Error）</param>
public sealed record ExceptionMapping(int Code, string Message, LogLevel LogLevel);
