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
    /// <param name="codes">系统字典的code</param>
    /// <returns></returns>
    public async Task<List<dynamic>> BuildEnumList(params string[] codes)
    {
        if (codes.Length == 0) return null;
        var allDirectoryList = await _sysDirectoryItemFullInfoRepository.Where(x => x.Status && codes.Contains(x.CategoryCode)).ToListAsync();
        var resultList = new List<dynamic>();

        for (int i = 0; i < codes.Length; i++)
        {
            var currList = allDirectoryList.Where(x => x.CategoryCode == codes[i]).ToList();
            var optionList = new List<dynamic>();
            foreach (var item in currList)
            {
                optionList.Add(new { Label = item.Name, Value = item.Value });
            }
            resultList.Add(new
            {
                Name = codes[i],
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
        var dny = await BuildEnumList("code1", "code2");
        return [enums, .. dny];
    }

}
