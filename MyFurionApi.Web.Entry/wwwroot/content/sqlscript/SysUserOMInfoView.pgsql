-- 系统用户运营管理所需的额外数据
DROP VIEW IF EXISTS "SysUserOMInfoView";

CREATE VIEW "SysUserOMInfoView" AS
SELECT
    a.*,
    COALESCE(STRING_AGG(ru."RoleId"::text, ',' ORDER BY ru."RoleId"), '') AS "RoleIds", -- 角色ID逗号分隔
    COALESCE(STRING_AGG(sr."Name", ',' ORDER BY ru."RoleId"), '') AS "RoleNames" -- 角色名称逗号分隔
FROM public."SysUser" a
LEFT JOIN "SysRoleUser" ru ON a."Id" = ru."UserId" AND ru."IsDeleted" = false
LEFT JOIN "SysRole" sr ON sr."Id" = ru."RoleId" AND sr."IsDeleted" = false
WHERE a."IsDeleted" = false
GROUP BY a."Id";