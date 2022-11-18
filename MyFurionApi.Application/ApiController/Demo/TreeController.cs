namespace MyFurionApi.Application.ApiController
{
    /// <summary>
    /// 树型实体
    /// </summary>
    public class TreeController : BaseApiController
    {
        private readonly ILogger<TreeController> _logger;
        private readonly SqlSugarRepository<ProCategory> _proCategoryRep;

        public TreeController(ILogger<TreeController> logger, SqlSugarRepository<ProCategory> proCategoryRep)
        {
            _logger = logger;
            _proCategoryRep = proCategoryRep;
        }

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> Add(ProCategory dto)
        {
            var result = await _proCategoryRep.InsertReturnIdentityAsync(dto);

            //执行存储过程
            await _proCategoryRep.Ado.UseStoredProcedure().ExecuteCommandAsync("sp_update_category_layer");

            return result;
        }

        /// <summary>
        /// 获取树列表
        /// </summary>
        /// <param name="search">搜索关键字</param>
        /// <returns></returns>
        [HttpGet]
        public Task<List<ProCategory>> GetList(string search)
        {
            if (search.IsNull())
            {
                //如果没有搜索条件、或者搜索条件不会破坏树的构造，则可以直接加where
                return _proCategoryRep.Entities
                                      .Includes(x => x.Parent) //同时把父级信息也查出来
                                      .ToTreeAsync(x => x.Childs, x => x.ParentId, 0);
            }
            else
            {
                //如果搜索条件破坏树的构造，如果直接where的话，因为可能找不到PId的数据，导致树不完整
                //1.先根据搜索出所有符合筛选条件的数据
                //2.再将搜索的id数据传入ToTree构造器中,ToTree会自动自动构造出缺少的父级数据，形成最终完整的树数据
                //这种会查两次数据库，第一次筛选条件，第二次查询所有的数据
                var ids = _proCategoryRep.Entities
                                                 .Where(x => x.Name.Contains(search))
                                                 .Select(x => x.Id)
                                                 .ToList()
                                                 .Cast<object>()
                                                 .ToArray();
                return _proCategoryRep.Entities
                                      .ToTreeAsync(x => x.Childs, x => x.ParentId, 0, ids);
            }
        }


    }
}
