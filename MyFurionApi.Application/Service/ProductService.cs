using MyFurionApi.Application.Entity;

namespace MyFurionApi.Application;

/// <summary>
/// 产品
/// </summary>
public class ProductService : IProductService, ITransient
{
    private readonly ILogger<ProductService> _logger;
    private readonly SqlSugarRepository<Product> _productRepository;

    public ProductService(ILogger<ProductService> logger, SqlSugarRepository<Product> productRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
    }


    /// <summary>
    /// 获取一条信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<Product> GetInfo(int id)
    {
        return _productRepository.Where(x => x.Id == id).FirstAsync();
    }


}
