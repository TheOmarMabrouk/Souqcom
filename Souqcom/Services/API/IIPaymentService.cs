namespace First_core_project.Services.API
{
    public interface IIPaymentService
    {
        // خليها تستقبل الـ OrderId بس، وهي تتصرف جوه
        Task<string> GetPaymentKeyForApiAsync(int orderId);
    }
}
