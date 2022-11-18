using Furion.DataValidation;
using System.Text.RegularExpressions;

namespace MyFurionApi.Core
{
    [ValidationType]
    public enum MyValidationTypes
    {
        /// <summary>
        /// 账户名称
        /// </summary>
        [ValidationItemMetadata(@"^[a-zA-Z0-9_]*$", "只能数字、字母、下划线组成")]
        AccountName,

        /// <summary>
        /// 11位手机号码
        /// </summary>
        [ValidationItemMetadata(@"^1[3456789]\d{9}$", "手机号码格式不正确")]
        CellPhone,

        /// <summary>
        /// 邮箱
        /// </summary>
        [ValidationItemMetadata(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", "邮箱格式不正确", RegexOptions.IgnoreCase)]
        Email,

    }
}
