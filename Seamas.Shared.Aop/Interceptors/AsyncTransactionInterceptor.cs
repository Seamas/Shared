using Castle.DynamicProxy;
using Wang.Seamas.Shared.Aop.Extensions;
using Wang.Seamas.Shared.Attributes;
using Wang.Seamas.Shared.UnitOfWork;

namespace Wang.Seamas.Shared.Aop.Interceptors;

/// <summary>
/// 异步拦截版本
/// 但是不 autofac 不支持，留着备用
/// </summary>
/// <param name="unitOfWork"></param>
public class AsyncTransactionInterceptor(IUnitOfWork unitOfWork) : AsyncInterceptorBase
{
    
     #region 拦截器方法
    
    protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
    {
        await HandleTransactionAsync<object>(invocation, async (x) =>
        {
            invocation.Proceed();
            if (invocation.ReturnValue is Task task)
            {
                await task;
            }
            return null!;
        });
    }

    protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
    {
        return await HandleTransactionAsync<TResult>(invocation, async (x) =>
        {
            invocation.Proceed();
            if (invocation.ReturnValue is Task<TResult> task)
            {
                return await task;
            }
            return (TResult) invocation.ReturnValue!;
        });
    }
    

    #endregion

    #region 私有方法
    
    
    
    private async Task<T> HandleTransactionAsync<T>(IInvocation invocation, Func<IInvocation, Task<T>> proceed)
    {
        if (!invocation.HasAttributeOnMethod<TransactionalAttribute>())
        {
            return await proceed(invocation);
        }

        try
        {
            await unitOfWork.BeginTransactionAsync();
            var result = await proceed(invocation);
            await unitOfWork.CommitAsync();
            return result;
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    #endregion
}