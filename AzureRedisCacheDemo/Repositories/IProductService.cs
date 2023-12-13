using AzureRedisCacheDemo.Data;

namespace AzureRedisCacheDemo.Repositories
{
    public interface IProductService
    {
        public Task<List<Product>> List();

        public Task<Product> GetById(int productId);

        public Task<bool> Add(Product product);

        public Task<bool> Delete(int productId);
    }
}
