using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wang.Seamas.Shared.DTOs;

namespace Wang.Seamas.Shared.AspNetCore.Filters;

/// <summary>
/// 统一返回值包装过滤器 - 将 ObjectResult 包装为 ApiResult&lt;T&gt;
/// ActionResult
/// ├── ObjectResult（数据/JSON）
/// ├── FileResult（文件下载）
/// ├── RedirectResult
/// └── StatusCodeResult
/// 只处理 ObjectResult，其余结果类型保持原样
/// </summary>
public class ApiResultFilter : IAsyncResultFilter
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> SuccessMethods = new();

    private static MethodInfo GetSuccessMethod(Type dataType)
    {
        return SuccessMethods.GetOrAdd(dataType, type =>
        {
            // 动态构造 ApiResult<T> 类型并取其静态 Ok 方法
            var apiResultType = typeof(ApiResult<>).MakeGenericType(type);
            return apiResultType.GetMethod(nameof(ApiResult.Ok), BindingFlags.Public | BindingFlags.Static)!;
        });
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var declaredType = objectResult.DeclaredType;
            var originalValue = objectResult.Value;

            // 获取返回值的真实类型（int / string / User / List<User> 等）
            var dataType = declaredType ?? originalValue?.GetType() ?? typeof(object);

            var successMethod = GetSuccessMethod(dataType);

            // 动态调用 ApiResult<T>.Ok(originalValue)，得到强类型结果而非 ApiResult<object>
            var apiResult = successMethod.Invoke(null, [originalValue]);

            context.Result = new ObjectResult(apiResult)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        await next();
    }
}
