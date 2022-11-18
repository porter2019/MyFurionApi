using Furion.UnifyResult;
using Microsoft.Extensions.Logging;

namespace MyFurionApi.Application.ApiController
{
    /// <summary>
    /// 表单验证
    /// </summary>
    public class FormValidateController : BaseApiController
    {
        private readonly ILogger<FormValidateController> _logger;

        public FormValidateController(ILogger<FormValidateController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 手动验证某个字段
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool Manual(string account)
        {
            //官方自带验证规则：ValidationTypes.PhoneNumber
            //填充
            UnifyContext.Fill(new { Enum = "枚举列表" });
            return account.TryValidate(MyValidationTypes.AccountName).IsValid;
        }

        /// <summary>
        /// 表单验证
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool Form(Dto.TestFormValidateInput dto)
        {
            return true;
        }

    }
}
