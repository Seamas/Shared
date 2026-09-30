using System.Linq.Expressions;
using Wang.Seamas.Shared.DTOs;

namespace Wang.Seamas.Shared.Repositories;

/// <summary>
/// 只读仓储接口：包含所有不依赖主键类型的查询方法。
/// 用于在不需要主键类型的场景（如分页查询扩展方法）中解除对 TKey 的依赖。
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IQueryRepository<T> where T : class
{
    /// <summary>
    /// 查询单条数据
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询列表
    /// </summary>
    Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查是否存在
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 计数
    /// </summary>
    Task<long> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResult<T>> GetPagedListAsync(
        Expression<Func<T, bool>>? predicate,
        PagedQuery? pagedQuery,
        CancellationToken cancellationToken = default);
}
