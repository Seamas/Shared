using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wang.Seamas.Shared.AspNetCore.ExceptionHandling;

namespace Wang.Seamas.Shared.AspNetCore.Extensions;

public static class SharedAspNetCoreServiceCollectionExtensions
{
    /// <summary>
    /// 注册 Shared.AspNetCore 的基础服务：异常映射器与异常映射选项
    /// 内置默认映射先生效，<paramref name="configure"/> 中注册的同类型映射会覆盖默认值
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">自定义异常类型映射等选项</param>
    public static IServiceCollection AddSharedAspNetCore(
        this IServiceCollection services,
        Action<SharedAspNetCoreOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var optionsBuilder = services.AddOptions<SharedAspNetCoreOptions>()
            .Configure(DefaultExceptionMappings.Apply);

        if (configure is not null)
        {
            optionsBuilder.Configure(configure);
        }

        services.TryAddSingleton<IExceptionMapper, ExceptionMapper>();

        return services;
    }
}
