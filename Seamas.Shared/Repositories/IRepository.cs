using System.Linq.Expressions;

namespace Wang.Seamas.Shared.Repositories;

/// <summary>
/// 完整仓储接口：在 <see cref="IQueryRepository{T}"/> 的基础上，提供依赖主键类型的查询与写入方法。
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IRepository<T, TKey> : IQueryRepository<T> where T : class
{
    #region 主键查询

    /// <summary>
    /// 根据主键查询
    /// </summary>
    Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    #endregion

    #region 写入

    /// <summary>
    /// 单条新增
    /// </summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量新增
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 单条更新
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新
    /// </summary>
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 单条删除
    /// </summary>
    Task RemoveAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除
    /// </summary>
    Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件删除
    /// </summary>
    Task RemoveRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion
}
