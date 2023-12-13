using AzureRedisCacheDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace AzureRedisCacheDemo.Repositories
{
    public class ProductService : IProductService
    {
        private readonly DataContext dbContextClass;

        public ProductService(DataContext dbContextClass)
        {
            this.dbContextClass = dbContextClass;
        }

        public async Task<List<Product>> List()
        {
            return await dbContextClass.Products.ToListAsync();
        }

        public async Task<Product> GetById(int productId)
        {
            return await dbContextClass.Products.Where(ele => ele.Id == productId).FirstOrDefaultAsync();
        }

        public async Task<bool> Add(Product product)
        {
            _ = await dbContextClass.Products.AddAsync(product);
            var result = await dbContextClass.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(int productId)
        {
            var findProductData = dbContextClass.Products.Where(_ => _.Id == productId).FirstOrDefault();
            if (findProductData != null)
            {
                _ = dbContextClass.Products.Remove(findProductData);
                var result = await dbContextClass.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }
    }
}
