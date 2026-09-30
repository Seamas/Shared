namespace Wang.Seamas.Shared.Attributes;

/// <summary>标记方法需要在事务中执行（由宿主侧的 AOP 拦截器识别）。</summary>
[AttributeUsage(AttributeTargets.Method)]
public class TransactionalAttribute : Attribute
{
}
