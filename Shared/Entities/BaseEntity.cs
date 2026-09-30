namespace Wang.Seamas.Shared.Entities;

/// <summary>
/// 实体内核基类：仅包含与具体用户体系无关的审计时间戳。
/// 非泛型形式使集成 DbContext 可通过 <c>ChangeTracker.Entries&lt;BaseEntity&gt;()</c> 统一填充审计字段。
/// </summary>
public abstract class BaseEntity
{
    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }
}


/// <summary>
/// 带主键的实体基类。主键类型由各模块自行决定（如 <see langword="int"/>、<see langword="long"/>）。
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class BaseEntity<TKey> : BaseEntity
{
    public TKey Id { get; set; } = default!;
}