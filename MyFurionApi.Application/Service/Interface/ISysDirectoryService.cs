namespace MyFurionApi.Application;

/// <summary>
/// 系统字典服务
/// </summary>
public interface ISysDirectoryService
{
    /// <summary>
    /// 构建枚举列表
    /// </summary>
    /// <param name="codes">系统字典的code</param>
    /// <returns></returns>
    Task<List<dynamic>> BuildEnumList(params string[] codes);
}
