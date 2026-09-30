using System;

namespace Wang.Seamas.Shared.DTOs;

public class ApiResult<T>
{
    public int Code { get; set; } = 200;
    public string? Message { get; set; }
    public T? Data { get; set; }
    
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public bool Success => Code == 200;

    public static ApiResult<T> Ok(T data) => new() { Code = 200, Data = data };
    
}

public class ApiResult
{
    public int Code { get; set; } = 200;
    public string? Message { get; set; }
    
    public long Timestamp => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public bool Success => Code == 200;
    
    public static ApiResult Ok() => new();
    public static ApiResult Fail(string message, int code = 500 ) => new() { Message = message, Code = code };
}