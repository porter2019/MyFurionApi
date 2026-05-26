-- 系统用户运营管理所需的额外数据
DROP VIEW IF EXISTS SysUserOMInfoView;

CREATE VIEW SysUserOMInfoView AS
SELECT
    a.*,
    ARRAY_TO_STRING(r.role_ids, ',') AS RoleIds,
    ARRAY_TO_STRING(r.role_names, ',') AS RoleNames
FROM public.sysuser a
LEFT JOIN LATERAL (
    SELECT
        ARRAY_AGG(ru.roleid ORDER BY ru.roleid) AS role_ids,
        ARRAY_AGG(sr.name ORDER BY ru.roleid) AS role_names
    FROM public.sysroleuser ru
    LEFT JOIN public.sysrole sr
        ON sr.id = ru.roleid
        AND sr.isdeleted = false
    WHERE ru.userid = a.id
      AND ru.isdeleted = false
) r ON true
WHERE a.isdeleted = false;