namespace Wang.Seamas.Shared.Exceptions;

/// <summary>
/// 业务异常
/// </summary>
/// <param name="message"></param>
/// <param name="code"></param>
/// <param name="innerException"></param>
public class BizException(string message, int code = 400, Exception? innerException = null )
    : Exception(message, innerException)
{
    public int Code { get; } = code;
}