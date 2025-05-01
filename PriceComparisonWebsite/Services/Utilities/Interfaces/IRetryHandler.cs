using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.Utilities.Interfaces
{
    public interface IRetryHandler
    {
        Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, string operationName, int maxRetries = 3);
        Task ExecuteWithRetryAsync(Func<Task> operation, string operationName, int maxRetries = 3);
    }
}