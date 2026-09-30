namespace Wang.Seamas.Shared.Exceptions;

/// <summary>
/// 参数验证异常
/// </summary>
public class ValidateException(string message, Exception? innerException = null) : Exception(message, innerException);