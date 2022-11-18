using System.ComponentModel.DataAnnotations;

namespace MyFurionApi.Application.Dto
{
    /// <summary>
    /// 测试表单验证输入参数
    /// </summary>
    public class TestFormValidateInput
    {
        /// <summary>
        /// 账户名
        /// </summary>
        [Required(ErrorMessage = "账户名必填"), DataValidation(MyValidationTypes.AccountName)]
        public string AccountName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码必填"), MinLength(6, ErrorMessage = "最少6个字符"), MaxLength(30, ErrorMessage = "最多30个字符")]
        public string Password { get; set; }

    }
}
