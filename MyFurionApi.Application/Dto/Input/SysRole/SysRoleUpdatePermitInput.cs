namespace MyFurionApi.Application.Dto;
/// <summary>
/// 修改权限所需
/// </summary>
public class SysRoleUpdatePermitInput
{
    public int RoleId { get; set; }

    public string Permits { get; set; }
}
