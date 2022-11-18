namespace MyFurionApi.Application;

/// <summary>
/// 自定义映射规则
/// <code>该文件可以放在任何项目或文件夹中，Furion 会在程序启动的时候自动扫描并注入配置。</code>
/// </summary>
public class Mapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //config.ForType<Entity, Dto>()
        //        .Map(dest => dest.FullName, src => src.FirstName + src.LastName)
        //        .Map(dest => dest.IdCard, src => src.IdCard.Replace("1234", "****"));
    }
}
