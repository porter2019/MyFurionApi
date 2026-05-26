using Furion.DatabaseAccessor;
using Microsoft.AspNetCore.Mvc.Filters;
using SqlSugar;

namespace MyFurionApi.Core;

/// <summary>
/// SqlSugar 工作单元实现
/// </summary>
public sealed class SqlSugarUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// SqlSugar 对象
    /// </summary>
    private readonly ISqlSugarClient _sqlSugarClient;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sqlSugarClient"></param>
    public SqlSugarUnitOfWork(ISqlSugarClient sqlSugarClient)
    {
        _sqlSugarClient = sqlSugarClient;
    }

    /// <summary>
    /// 开启工作单元处理
    /// </summary>
    /// <param name="context"></param>
    /// <param name="unitOfWork"></param>
    /// <exception cref="NotImplementedException"></exception>
    Task IUnitOfWork.BeginTransactionAsync(FilterContext context, UnitOfWorkAttribute unitOfWork)
    {
        return _sqlSugarClient.AsTenant().BeginTranAsync();
    }

    /// <summary>
    /// 提交工作单元处理
    /// </summary>
    /// <param name="resultContext"></param>
    /// <param name="unitOfWork"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task CommitTransactionAsync(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        return _sqlSugarClient.AsTenant().CommitTranAsync();
    }

    /// <summary>
    /// 回滚工作单元处理
    /// </summary>
    /// <param name="resultContext"></param>
    /// <param name="unitOfWork"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task RollbackTransactionAsync(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        return _sqlSugarClient.AsTenant().RollbackTranAsync();
    }

    /// <summary>
    /// 执行完毕（无论成功失败）
    /// </summary>
    /// <param name="context"></param>
    /// <param name="resultContext"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task OnCompletedAsync(FilterContext context, FilterContext resultContext)
    {
        _sqlSugarClient.Dispose();
        return Task.CompletedTask;
    }
}