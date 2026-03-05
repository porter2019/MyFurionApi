namespace MyFurionApi.Application;

/// <summary>
/// 系统字典服务
/// </summary>
public interface ISysDirectoryService
{
    /// <summary>
    /// 构建枚举列表
    /// </summary>
    /// <param name="codes">系统字典的code，多个英文逗号分割</param>
    /// <param name="key">返回对应的key名称，与code的index一一对应</param>
    /// <returns></returns>
    Task<List<dynamic>> BuildEnumList(string codes, params string[] key);
}
