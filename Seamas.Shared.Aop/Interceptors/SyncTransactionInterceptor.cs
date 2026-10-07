using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Wang.Seamas.Shared.Aop.Extensions;
using Wang.Seamas.Shared.Attributes;
using Wang.Seamas.Shared.UnitOfWork;

namespace Wang.Seamas.Shared.Aop.Interceptors;

public class SyncTransactionInterceptor(
    IUnitOfWork unitOfWork,
    ILogger<SyncTransactionInterceptor> logger) : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        if (!invocation.HasAttributeOnMethod<TransactionalAttribute>())
        {
            invocation.Proceed();
            return;
        }

        if (unitOfWork.HasActiveTransaction)
        {
            invocation.Proceed();
            if (invocation.ReturnValue is Task task)
            {
                task.GetAwaiter().GetResult();
            }
            return;
        }

        HandleTransactionAsync(invocation).GetAwaiter().GetResult();
    }

    private async Task HandleTransactionAsync(IInvocation invocation)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();
            invocation.Proceed();

            if (invocation.ReturnValue is Task task)
            {
                await task;
            }

            await unitOfWork.CommitAsync();
            logger.LogDebug("Transaction committed successfully");
        }
        catch
        {
            try
            {
                await unitOfWork.RollbackAsync();
                logger.LogDebug("Transaction rolled back successfully");
            }
            catch (Exception rollbackEx)
            {
                logger.LogError(rollbackEx, "Failed to rollback transaction");
            }
            throw;
        }
    }
}
