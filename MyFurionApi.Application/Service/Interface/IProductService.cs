using MyFurionApi.Application.Entity;

namespace MyFurionApi.Application
{
    public interface IProductService
    {
        Task<Product> GetInfo(int id);
    }
}
