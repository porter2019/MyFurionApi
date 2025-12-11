using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Security.Claims;

namespace MyFurionApi.Core;

/// <summary>
/// 方法过滤器
/// </summary>
public class MyActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //============== 这里是执行方法之前获取数据 ====================
        #region 官方示例

        //// 获取控制器、路由信息
        //var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        //// 获取请求的方法
        //var method = actionDescriptor!.MethodInfo;

        //// 获取 HttpContext 和 HttpRequest 对象
        //var httpContext = context.HttpContext;
        //var httpRequest = httpContext.Request;

        //// 获取客户端 Ipv4 地址
        //var remoteIPv4 = httpContext.GetClientIpAddress();

        //// 获取请求的 Url 地址
        //var requestUrl = httpRequest.GetRequestUrlAddress();

        //// 获取来源 Url 地址
        //var refererUrl = httpRequest.GetRefererUrlAddress();

        //// 获取请求参数（写入日志，需序列化成字符串后存储），可以自由篡改！！！！！！
        //var parameters = context.ActionArguments;

        //// 获取操作人（必须授权访问才有值）"userId" 为你存储的 claims type，jwt 授权对应的是 payload 中存储的键名
        //var userId = httpContext.User?.FindFirstValue(ClaimConst.UserId);

        //// 请求时间
        //var requestedTime = DateTimeOffset.Now; 

        #endregion
        //为请求参数附加上下文用户数据
        var currentUserId = (context.HttpContext.User?.FindFirstValue(ClaimConst.UserId)).ObjToInt();
        if (currentUserId > 0)
        {
            object postModelValue = context.ActionArguments.FirstOrDefault().Value;
            if (postModelValue is IEnumerable<object> enumerable)
            {
                var list = enumerable.ToList();
                if (list.Count != 0) postModelValue = list[0];
            }

            var currentUserName = context.HttpContext.User?.FindFirstValue(ClaimConst.UserName);

            //所有post model对象需要继承的实体
            if (postModelValue is BaseFormPostModel && context.HttpContext.Request.Method == "POST")
            {
                var baseModel = (BaseFormPostModel)postModelValue;
                baseModel.CurrentUserId = currentUserId;
                baseModel.CurrentUserName = currentUserName;
            }
        }

        //============== 这里是执行方法之后获取数据 ====================
        var actionContext = await next();

        #region 官方示例

        // 获取返回的结果
        var returnResult = actionContext.Result;

        // 判断是否请求成功，没有异常就是请求成功
        var isRequestSucceed = actionContext.Exception == null;

        // 获取调用堆栈信息，提供更加简单明了的调用和异常堆栈
        var stackTrace = EnhancedStackTrace.Current();


        #endregion
    }
}
