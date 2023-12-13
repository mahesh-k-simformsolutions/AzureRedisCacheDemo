using AzureRedisCacheDemo.Data;
using AzureRedisCacheDemo.Repositories;
using AzureRedisCacheDemo.Repositories.AzureRedisCache;
using Microsoft.AspNetCore.Mvc;

namespace AzureRedisCacheDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IRedisCache _redisCache;

        public ProductsController(IProductService productService, IRedisCache redisCache)
        {
            _productService = productService;
            _redisCache = redisCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> List()
        {
            var cacheData = _redisCache.GetCacheData<List<Product>>("product-cache");
            if (cacheData != null)
            {
                return new List<Product>(cacheData);
            }

            var productList = await _productService.List();
            if (productList != null)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                _ = _redisCache.SetCacheData<List<Product>>("product-cache", productList, expirationTime);
                return Ok(productList);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetById(int productId)
        {
            var cacheData = _redisCache.GetCacheData<List<Product>>("product-cache");
            if (cacheData != null)
            {
                Product filteredData = cacheData.Where(x => x.Id == productId).FirstOrDefault();
                return new ActionResult<Product>(filteredData);
            }

            var product = await _productService.GetById(productId);
            return product != null ? (ActionResult<Product>)Ok(product) : (ActionResult<Product>)NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            var isProductInserted = await _productService.Add(product);
            _ = _redisCache.RemoveData("product-cache");
            return isProductInserted ? Ok(isProductInserted) : BadRequest();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int productId)
        {
            var isProductDeleted = await _productService.Delete(productId);
            _ = _redisCache.RemoveData("product-cache");
            return isProductDeleted ? Ok(isProductDeleted) : BadRequest();
        }
    }
}
