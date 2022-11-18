namespace MyFurionApi.Application.ApiController
{
    /// <summary>
    /// 系统角色组
    /// </summary>
    [Route("api/[controller]")]
    public class SysRoleController : BaseApiController
    {
        private readonly ILogger<SysRoleController> _logger;
        private readonly ISysRoleService _sysRoleService;

        public SysRoleController(ILogger<SysRoleController> logger, ISysRoleService sysRoleService)
        {
            _logger = logger;
            _sysRoleService = sysRoleService;
        }

        /// <summary>
        /// 根据组Id获取组的权限配置数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("get/permit/list")]
        public List<Dto.SysRoleModuleGroupOutput> GetRolePermitList(int roleId)
        {
            return _sysRoleService.GetPermitListByRoleId(roleId);
        }

        /// <summary>
        /// 设置用户组权限
        /// </summary>
        /// <param name="roleId">组id</param>
        /// <param name="permits">权限PermitId，英文逗号分开</param>
        /// <returns></returns>
        [HttpGet("set/permit")]
        public Task<bool> SetRolePermit(int roleId, string permits)
        {
            return _sysRoleService.SetRolePermitAsync(roleId, permits);
        }
    }
}
