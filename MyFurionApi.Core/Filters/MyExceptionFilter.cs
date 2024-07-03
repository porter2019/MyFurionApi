using Furion.FriendlyException;
using Furion.UnifyResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace MyFurionApi.Core;

/// <summary>
/// 异常过滤器
/// </summary>
public class MyExceptionFilter : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        // 如果异常在其他地方被标记了处理，那么这里不再处理
        if (context.ExceptionHandled) return;

        // 获取控制器信息
        //ControllerActionDescriptor? actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        // 获取请求的方法
        //var method = actionDescriptor!.MethodInfo;

        // 获取异常对象
        //var exception = context.Exception;

        // 获取调用堆栈信息，提供更加简单明了的调用和异常堆栈
        //var stackTrace = EnhancedStackTrace.Current();

        // 其他处理~~~
        // 1. MVC 直接返回自定义的错误页面，或者 BadPageResult 类型，如：context.Result = new BadPageResult(StatusCodes.Status500InternalServerError) { }

        // 2. WebAPI 可以直接返回 context.Result = new JsonResult(.....);

        // 3. 记录日志。。。。

        //上面都是官方演示代码，下面才是我要的业务
        var exceptionType = context.Exception.GetType();
        if (exceptionType.Equals(typeof(SqlSugar.VersionExceptions)))
        {
            context.ExceptionHandled = true;

            context.Result = new JsonResult(RESTfulResultProvider.RESTfulResult(400, false, null, "数据已被修改，请刷新页面重新编辑提交"));
        }

        await Task.CompletedTask;
    }
}
