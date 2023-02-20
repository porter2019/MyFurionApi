﻿using System.Text.Json.Serialization;

namespace MyFurionApi.Core;


/// <summary>
/// 数据库实体基类
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键自增Id
    /// <c>[SugarColumn(ColumnDescription = "Id主键", IsIdentity = true, IsPrimaryKey = true)]</c>
    /// </summary>
    [FsColumn("Id主键", IsPK = true)]
    public int Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [FsColumn("创建时间", true)]
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [FsColumn("更新时间", true)]
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 乐观锁
    /// </summary>
    [FsColumn("乐观锁", IsEnableUpdateVersionValidation = true)]
    public long Version { get; set; }

    /// <summary>
    /// 软删除
    /// <code>CreateTableFieldSort设置后无效，不知道为啥</code>
    /// </summary>
    [FsColumn("软删除", false, CreateTableFieldSort = 99), JsonIgnore]
    public bool IsDeleted { get; set; } = false;

}

/// <summary>
/// 数据库实体通用基类
/// </summary>
public abstract class BaseEntityStandard : BaseEntity
{
    #region 创建者、修改者信息

    /// <summary>
    /// 创建者Id
    /// </summary>
    [FsColumn("创建者Id", true)]
    public int? CreatedUserId { get; set; }

    /// <summary>
    /// 创建者名称
    /// </summary>
    [FsColumn("创建者名称", true)]
    public string CreatedUserName { get; set; }

    /// <summary>
    /// 修改者Id
    /// </summary>
    [FsColumn("修改者Id", true)]
    public int? UpdatedUserId { get; set; }

    /// <summary>
    /// 修改者名称
    /// </summary>
    [FsColumn("修改者名称", true)]
    public string UpdatedUserName { get; set; }

    #endregion


    /// <summary>
    /// 更新信息列
    /// </summary>
    /// <returns></returns>
    public virtual string[] UpdateColumn()
    {
        var result = new[] { nameof(UpdatedUserId), nameof(UpdatedUserName), nameof(UpdatedTime) };
        return result;
    }

    /// <summary>
    /// 假删除的列，包含更新信息
    /// </summary>
    /// <returns></returns>
    public virtual string[] FalseDeleteColumn()
    {
        var updateColumn = UpdateColumn();
        var deleteColumn = new[] { nameof(IsDeleted) };
        var result = new string[updateColumn.Length + deleteColumn.Length];
        deleteColumn.CopyTo(result, 0);
        updateColumn.CopyTo(result, deleteColumn.Length);
        return result;
    }

}
