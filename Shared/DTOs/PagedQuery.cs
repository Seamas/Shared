namespace Wang.Seamas.Shared.DTOs;

/// <summary>
/// 分页查询
/// </summary>
public class PagedQuery
{
    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    
    public int SkipCount => (PageIndex  - 1) * PageSize;
    
    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortField { get; set; }
    /// <summary>
    /// 是否升序号
    /// </summary>
    public bool IsAscending { get; set; } = true;
}