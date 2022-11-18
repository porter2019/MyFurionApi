using Furion;
using Furion.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MyFurionApi.Application;
using MyFurionApi.Application.Entity;
using MyFurionApi.Core;
using System.Linq;
using System.Threading.Tasks;

namespace MyFurionApi.Web.Core;

public class JwtHandler : AppAuthorizeHandler
{
    /// <summary>
    /// 授权通过后在这里校验权限
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public override Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
    {
        // 这里写您的授权判断逻辑，授权通过返回 true，否则返回 false

        return CheckAuthorzie(httpContext);
    }

    /// <summary>
    /// 检查权限
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private static Task<bool> CheckAuthorzie(DefaultHttpContext httpContext)
    {
        //用户id
        var currentUserId = (App.User.FindFirst(ClaimConst.UserId)?.Value).ObjToInt();
        //管理员跳过
        var isAdmin = (App.User.FindFirst(ClaimConst.IsSuperAdmin)?.Value).ObjToBool();
        if (isAdmin) return Task.FromResult(true);

        //控制器和Action上的标签信息
        var controllerActionDescriptor = httpContext.GetControllerActionDescriptor();
        var permissionHandlerAttribute = controllerActionDescriptor.ControllerTypeInfo
                                .GetCustomAttributes(true)
                                .Where(p => p.GetType().Equals(typeof(PermissionHandlerAttribute)))
                                .FirstOrDefault() as PermissionHandlerAttribute;
        var permissionAttribute = controllerActionDescriptor.MethodInfo
                                .GetCustomAttributes(true)
                                .Where(p => p.GetType().Equals(typeof(PermissionAttribute)))
                                .FirstOrDefault() as PermissionAttribute;
        if (permissionHandlerAttribute == null || permissionAttribute == null) return Task.FromResult(true);//只要控制器或方法，两者任意为null就不验证了

        //校验权限
        var _userInfoService = App.GetRequiredService<IUserInfoService>();
        return _userInfoService.CheckHasPermissionAsync(currentUserId,
                                    controllerActionDescriptor.ControllerTypeInfo.FullName,
                                    permissionAttribute.OperationName);
    }

    ///// <summary>
    ///// 完全自定义授权
    ///// </summary>
    ///// <param name="context"></param>
    ///// <returns></returns>
    //public override Task HandleAsync(AuthorizationHandlerContext context)
    //{
    //    var isAuthenticated = context.User.Identity.IsAuthenticated;

    //    // 第三方授权自定义
    //    if (是第三方)
    //    {
    //        foreach (var requirement in pendingRequirements)
    //        {
    //            // 授权成功
    //            context.Succeed(requirement);
    //        }
    //    }
    //    // 授权失败
    //    else context.Fail();
    //}
}
