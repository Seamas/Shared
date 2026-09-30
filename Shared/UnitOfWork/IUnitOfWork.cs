namespace Wang.Seamas.Shared.UnitOfWork;

/// <summary>
/// 工作单元：统一管理变更提交与数据库事务，与具体持久化技术无关。
/// </summary>
public interface IUnitOfWork
{
    bool HasActiveTransaction { get; }

    /// <summary>提交所有待保存的变更（不开启事务）。</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>开启数据库事务。</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>保存变更并提交事务。</summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>回滚事务。</summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
