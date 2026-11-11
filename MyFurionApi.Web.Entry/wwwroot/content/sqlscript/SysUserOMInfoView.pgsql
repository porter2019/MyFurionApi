-- 系统用户运营管理所需的额外数据
DROP VIEW IF EXISTS SysUserOMInfoView;

CREATE VIEW SysUserOMInfoView AS
SELECT
    a.*,
    ARRAY_TO_STRING(r.role_ids, ',') AS RoleIds,
    ARRAY_TO_STRING(r.role_names, ',') AS RoleNames,
    ARRAY_TO_STRING(p.post_ids, ',') AS PostIds,
    ARRAY_TO_STRING(p.post_names, ',') AS PostNames
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
LEFT JOIN LATERAL (
    SELECT
        ARRAY_AGG(pu.postid ORDER BY pu.postid) AS post_ids,
        ARRAY_AGG(sp.name ORDER BY pu.postid) AS post_names
    FROM public.syspostuser pu
    LEFT JOIN public.syspost sp
        ON sp.id = pu.postid
        AND sp.isdeleted = false
    WHERE pu.userid = a.id
      AND pu.isdeleted = false
) p ON true
WHERE a.isdeleted = false;