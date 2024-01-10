using Furion.UnifyResult;

namespace MyFurionApi.Application.Controller
{
    /// <summary>
    /// 合同接口
    /// </summary>
    [PermissionHandler("演示", "合同", "Contract", 10)]
    public class ContractController : BaseApiController
    {
        private readonly ILogger<ContractController> _logger;
        private readonly SqlSugarRepository<Contract> _contractRepository;

        public ContractController(ILogger<ContractController> logger, SqlSugarRepository<Contract> contractRepository)
        {
            _logger = logger;
            _contractRepository = contractRepository;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("get/pagelist")]
        public Task<SqlSugarPagedList<Contract>> GetPageList(ContractPageInput req)
        {
            //查询普通列表
            //_contractRepository.ToList(new ContractListInput { Top = 10, Name = "aaa" });

            UnifyContext.Fill(new
            {
                Enums = new List<dynamic> {
                    new { Name = "CType", Options = typeof(ContractTypeEnum).GetEnumOptions() },
                    new { Name = "Flag", Options = typeof(ContractFlagEnum).GetEnumOptions() }
                }
            });

            return _contractRepository.ToPageListAsync(req);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("get/info")]
        [Permission("查看", "show")]
        public async Task<Contract> GetInfo(int id)
        {
            UnifyContext.Fill(new
            {
                Enums = new List<dynamic> {
                    new { Name = "CType", Options = typeof(ContractTypeEnum).GetEnumOptions() },
                    new { Name = "Flag", Options = typeof(ContractFlagEnum).GetEnumOptions() }
                }
            });

            //根据Form实体条件自动构建where条件查询
            //var model = _contractRepository.FirstOrDefault(new ContractSingleInput { Code = "aaa" });
            var entity = await _contractRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                entity.ItemList = await _contractRepository.Change<ContractItem>().ToListAsync(x => x.ContractId == id);
            }
            else
            {
                entity = new Contract();
            }

            return entity;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("add")]
        [Permission("添加", "add")]
        [UnitOfWork]
        public async Task<string> Add(ContractModifyInput dto)
        {
            var data = dto.Adapt<Contract>();

            //添加主体，返回自增id
            var id = await _contractRepository.InsertReturnIdentityAsync(data);
            //添加明细，同edit
            await _contractRepository.Change<ContractItem>().UpdateItemDiffAsync(id, data.ItemList);
            //添加附件，同edit
            await _contractRepository.UpdateAttachFilesAsync(id, CommonAttachType.Demo, data.AttachList);

            return "修改成功";
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("edit")]
        [Permission("修改", "edit")]
        [UnitOfWork]
        public async Task<string> Update(ContractModifyInput dto)
        {
            var data = dto.Adapt<Contract>();

            //更新主体
            await _contractRepository.UpdateAsync(data);
            //更新明细，同add
            await _contractRepository.Change<ContractItem>().UpdateItemDiffAsync(dto.Id, data.ItemList);
            //更新附件，同add
            await _contractRepository.UpdateAttachFilesAsync(dto.Id, CommonAttachType.Demo, data.AttachList);

            return "修改成功";
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Permission("删除", "del")]
        [UnitOfWork]
        public async Task<int> Delete(string ids)
        {
            await _contractRepository.Change<ContractItem>().DeleteWithSoftAsync($"ContractId in({ids})");
            await _contractRepository.Change<CommonAttach>().DeleteWithSoftAsync($"AttachType={(int)CommonAttachType.Demo} and RefId in({ids})");
            var total = await _contractRepository.DeleteWithSoftAsync(ids.SplitWithComma().ConvertIntList());

            return total;
        }
    }
}
