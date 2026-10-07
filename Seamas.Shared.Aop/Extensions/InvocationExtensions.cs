using System.Collections.Concurrent;
using System.Reflection;
using Castle.DynamicProxy;

namespace Wang.Seamas.Shared.Aop.Extensions;

public static class InvocationExtensions
{
    /// <summary>
    /// 判断当前被拦截的方法上是否标记了指定特性，反射查找结果按 (方法, 特性类型) 缓存。
    /// 该方法只用于判断 代码中调用类型的方法上是否具有目标特性标记
    /// 例如 IService.Method 接口方法， Service.Method 实际方法，如果在代码中的对象是 IService 声明的，就取 IService.Method 上的特性标记，如果使用的是 Service.Method 调用，就取 Service.Method上的标记
    /// public class A(IService x);   public class B(Service x);
    /// 考虑到对于实现类的 AOP 还有很多限制， 实际上是通过继承来实现的，比如类不能是 sealed, 方法必须是 virtual 等条件
    /// 因此 AOP 应该统一使用接口来实现
    /// </summary>
    public static bool HasAttributeOnMethod<TAttribute>(this IInvocation invocation)
        where TAttribute : Attribute
    {
        var method = invocation.Method;

        return MethodAttributeCache<TAttribute>.Methods.GetOrAdd(method, HasAttributeCore<TAttribute>);
    }

    private static bool HasAttributeCore<TAttribute>(MethodInfo method)
        where TAttribute : Attribute
    {
        var declaringType = method.DeclaringType;
        if (declaringType == null)
            return false;

        var methodName = method.Name;
        var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

        var targetMethod = declaringType.GetMethod(
            methodName,
            BindingFlags.Public | BindingFlags.Instance,
            null,
            parameterTypes,
            null
        );

        return targetMethod?.GetCustomAttribute<TAttribute>() != null;
    }

    /// <summary>
    /// 每个特性类型对应一个独立的封闭泛型类型，各自持有一份缓存，
    /// 避免同一个方法查询不同特性时命中错误的缓存结果
    /// </summary>
    private static class MethodAttributeCache<TAttribute>
        where TAttribute : Attribute
    {
        public static readonly ConcurrentDictionary<MethodInfo, bool> Methods = new();
    }
}
