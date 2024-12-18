namespace MyFurionApi.Core;

/// <summary>
/// 表示字段为主表的外键
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ForeignKeyTagAttribute : Attribute { }