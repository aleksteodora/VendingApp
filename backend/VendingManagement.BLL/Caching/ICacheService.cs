namespace VendingManagement.BLL.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task<T?> GetAsync<T>(string key, Func<Task<T?>> fallback, TimeSpan? expiry = null) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan expiry) where T : class;
        Task RemoveAsync(string key);
    }
}