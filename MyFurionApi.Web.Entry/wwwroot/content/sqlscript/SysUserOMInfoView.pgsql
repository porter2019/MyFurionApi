-- 系统用户运营管理所需的额外数据
DROP VIEW IF EXISTS SysUserOMInfoView;

CREATE VIEW SysUserOMInfoView AS
SELECT
    a.*,
    COALESCE(STRING_AGG(ru.roleid::text, ',' ORDER BY ru.roleid), '') AS RoleIds, -- 角色ID逗号分隔
    COALESCE(STRING_AGG(sr.name, ',' ORDER BY ru.roleid), '') AS RoleNames -- 角色名称逗号分隔
FROM public.sysuser a
LEFT JOIN public.sysroleuser ru ON a.id = ru.userid AND ru.isdeleted = false
LEFT JOIN public.sysrole sr ON sr.id = ru.roleid AND sr.isdeleted = false
WHERE a.isdeleted = false
GROUP BY a.id;