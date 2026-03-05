namespace MyFurionApi.Application;

/// <summary>
/// 系统字典服务
/// </summary>
public class SysDirectoryService : ISysDirectoryService, ITransient
{
    private readonly SqlSugarRepository<SysDirectoryItemFullInfo> _sysDirectoryItemFullInfoRepository;

    public SysDirectoryService(SqlSugarRepository<SysDirectoryItemFullInfo> sysDirectoryItemFullInfoRepository)
    {
        _sysDirectoryItemFullInfoRepository = sysDirectoryItemFullInfoRepository;
    }

    /// <summary>
    /// 构建枚举列表
    /// </summary>
    /// <param name="codes">系统字典的code，多个英文逗号分割</param>
    /// <param name="key">返回对应的key名称，与code的index一一对应</param>
    /// <returns></returns>
    public async Task<List<dynamic>> BuildEnumList(string codes, params string[] key)
    {
        var codeArr = codes.SplitWithComma();
        if (codeArr.Length != key.Length) throw Oops.Bah("构建枚举时，长度不一致");
        var allDirectoryList = await _sysDirectoryItemFullInfoRepository.Where(x => x.Status && codeArr.Contains(x.CategoryCode)).ToListAsync();
        var resultList = new List<dynamic>();

        for (int i = 0; i < codeArr.Length; i++)
        {
            var currList = allDirectoryList.Where(x => x.CategoryCode == codeArr[i]).ToList();
            var optionList = new List<dynamic>();
            foreach (var item in currList)
            {
                optionList.Add(new { Label = item.Name, Value = item.Value });
            }
            resultList.Add(new
            {
                Name = key[i],
                Options = optionList
            });
        }

        return resultList;
    }

    /// <summary>
    /// 调用示例
    /// </summary>
    /// <returns></returns>
    async Task<List<dynamic>> CallExample()
    {
        var enums = new List<dynamic> {
            new { Name = "BusType", Options = new List<dynamic>
                        {
                            new { Label = "送货", Value = "送货" },
                            new { Label = "拉货", Value = "拉货" },
                            new { Label = "内倒", Value = "内倒" },
                            new { Label = "其他", Value = "其他" },
                        }
            },
            //new { Name = "ApplyFlag", Options = typeof(OrderBookApplyFlagEnum).GetEnumOptions() },
        };
        var dny = await BuildEnumList("01,02", "frist", "second");
        return [enums, .. dny];
    }

}
