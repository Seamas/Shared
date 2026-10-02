using Microsoft.AspNetCore.Mvc;
using Wang.Seamas.Shared.AspNetCore.Filters;

namespace Wang.Seamas.Shared.AspNetCore.Extensions;

public static class MvcOptionsExtensions
{
    /// <summary>
    /// 挂载 Shared 内置全局过滤器，顺序与执行优先级：
    /// ModelValidationFilter（进 Action 前校验）→ GlobalExceptionFilter（兜底异常）→ ApiResultFilter（出参包装）
    /// </summary>
    public static MvcOptions AddSharedFilters(this MvcOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Filters.Add<ModelValidationFilter>();
        options.Filters.Add<GlobalExceptionFilter>();
        options.Filters.Add<ApiResultFilter>();

        return options;
    }
}
