
using MyFurionApi.Application.Dto;

namespace MyFurionApi.Application.ApiController
{
    /// <summary>
    /// 产品
    /// </summary>
    [PermissionHandler("系统", "产品", "Produt", 10)]
    public class ProducController : BaseApiController
    {
        private readonly ILogger<ProducController> _logger;
        private readonly SqlSugarRepository<Product> _productRepository;
        private readonly IProductService _productService;

        public ProducController(ILogger<ProducController> logger, SqlSugarRepository<Product> productRepository, IProductService productService)
        {
            _logger = logger;
            _productRepository = productRepository;
            _productService = productService;
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Permission("查看", "show")]
        public async Task<Product> GetInfo(int id)
        {
            var entity = await _productRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                entity.ItemList = await _productRepository.Change<ProductItem>().ToListAsync(x => x.ProductId == id);
            }
            return entity;
        }

        /// <summary>
        /// 获取一条
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Product> GetInfo(ProductSingleInput req)
        {
            var data = await _productRepository.FirstOrDefaultAsync(req);
            if (data != null)
            {
                data.ItemList = await _productRepository.Change<ProductItem>().ToListAsync(x => x.ProductId == req.Id);
            }

            return data;
        }

        /// <summary>
        /// 更新-使用DTO
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Permission("修改", "edit")]
        public async Task<string> Update(ProductUpdateInput dto)
        {
            var entity = dto.Adapt<Product>();
            var count = await _productRepository.UpdateWithOptLockAsync(entity);
            return $"更新数量：{count}";
        }

        /// <summary>
        /// 修改-使用Entity并且更新ItemList
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Update2(Product dto)
        {
            try
            {
                _productRepository.CurrentBeginTran();
                var itemRep = _productRepository.Change<ProductItem>();
                await _productRepository.UpdateWithOptLockAsync(dto);
                await itemRep.UpdateItemDiffAsync(dto.Id, dto.ItemList);

                _productRepository.CurrentCommitTran();

                return "OK";
            }
            catch (Exception)
            {
                _productRepository.CurrentRollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Permission("添加", "add")]
        public async Task<int> Add()
        {
            var index = Random.Shared.Next(555);
            var entity = new Product()
            {
                Name = $"中文AAA-{index}",
                Num = index,
                Type = ProductType.Custom,
                Value = index + (decimal)5.36
            };

            try
            {
                _productRepository.CurrentBeginTran();
                int productId = await _productRepository.InsertReturnIdentityAsync(entity);

                var _productItemRepo = _productRepository.Change<ProductItem>();
                #region 明细

                var itemList = new List<ProductItem>();
                itemList.Add(new ProductItem()
                {
                    ProductId = productId,
                    Money = (decimal)1.1 + index,
                    Str = "明细1-" + index,
                    Num = 11,
                });
                itemList.Add(new ProductItem()
                {
                    ProductId = productId,
                    Money = (decimal)2.1 + index,
                    Str = "明细2-" + index,
                    Num = 22,
                });
                itemList.Add(new ProductItem()
                {
                    ProductId = productId,
                    Money = (decimal)3.1 + index,
                    Str = "明细3-" + index,
                    Num = 33,
                });

                #endregion

                await _productItemRepo.InsertAsync(itemList);

                _productRepository.CurrentCommitTran();

                return productId;
            }
            catch (Exception ex)
            {
                _productRepository.CurrentRollbackTran();
                throw Oops.Oh(ex.Message);
            }
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Permission("删除", "del")]
        public Task<int> Delete(int id)
        {
            return _productRepository.DeleteWithSoftAsync(id);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<SqlSugarPagedList<Product>> GetPageList(Dto.ProductPageInput req)
        {
            return _productRepository.ToPageListAsync(req);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<List<Product>> GetList(Dto.ProductListInput req)
        {
            return _productRepository.ToListAsync(req);
        }

    }
}
