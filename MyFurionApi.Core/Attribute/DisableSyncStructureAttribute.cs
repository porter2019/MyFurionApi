namespace MyFurionApi.Core;

/// <summary>
/// 设置禁止SqlSugar Table迁移属性
/// <code>因为官方的SqlSugar.SugarTable这个不能继承拓展新增个属性</code>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DisableSyncStructureAttribute : Attribute { }

/// <summary>
/// 表示字段为主表的外键
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ForeignKeyTagAttribute : Attribute { }