using Fluid;

namespace MyFurionApi.Application.Controller;

/// <summary>
/// 代码生成
/// </summary>
public class FluidController : BaseApiController
{

    private readonly FluidParser _parser;

    public FluidController()
    {
        _parser = new FluidParser();
    }

    /// <summary>
    /// 根据实体生成控制器类
    /// </summary>
    /// <param name="name">实体名称</param>
    /// <param name="lowerName">实体首字母小写的名称</param>
    /// <param name="desc">名称</param>
    /// <returns></returns>
    [HttpGet, Route("generate/controller"), AllowAnonymous]
    public string GenerateController(string name, string lowerName, string desc)
    {
        var controllerFileName = "ApiController.txt";
        var pageInputFileName = "PageInput.txt";

        //文件保存在wwwroot/uploads目录下了
        var baseIODirectory = App.WebHostEnvironment.WebRootPath;
        var model = new { Name = name, Name2 = lowerName, Desc = desc };

        var templatePath = Path.Combine(baseIODirectory, "content", "fluid", controllerFileName);
        if (!File.Exists(templatePath)) return "模板文件不存在";
        if (_parser.TryParse(File.ReadAllText(templatePath), out var template, out var error))
        {
            var context = new TemplateContext(model);
            var outStr = template.Render(context);
            //将内容写入文件
            WriteFile(Path.Combine(baseIODirectory, "uploads", $"{name}Controller.cs"), outStr);
        }
        else
        {
            return "错误：" + error;
        }

        templatePath = Path.Combine(baseIODirectory, "content", "fluid", pageInputFileName);
        if (!File.Exists(templatePath)) return "模板文件不存在";
        if (_parser.TryParse(File.ReadAllText(templatePath), out var template2, out var error2))
        {
            var context = new TemplateContext(model);
            var outStr = template2.Render(context);
            //将内容写入文件
            WriteFile(Path.Combine(baseIODirectory, "uploads", $"{name}PageInput.cs"), outStr);
        }
        else
        {
            return "错误：" + error2;
        }

        return "成功";
    }

    /// <summary>
    /// 写入文件
    /// </summary>
    /// <param name="path">完整路径</param>
    /// <param name="text"></param>
    /// <returns></returns>
    bool WriteFile(string path, string text)
    {
        if (File.Exists(path)) File.Delete(path);
        Encoding utf8Bom = new UTF8Encoding(true);
        using (StreamWriter writer = new StreamWriter(path, false, utf8Bom))
        {
            writer.WriteLine(text); // 写入数据
        }
        return true;
    }

}
