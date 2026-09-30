namespace Wang.Seamas.Shared.Exceptions;

/// <summary>
/// 认证异常
/// </summary>
/// <param name="message"></param>
/// <param name="innerException"></param>
public class AuthException(string message, Exception? innerException = null) : Exception(message, innerException);