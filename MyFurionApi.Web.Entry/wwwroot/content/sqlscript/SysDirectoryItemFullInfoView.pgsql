-- 字典完整信息
DROP VIEW IF EXISTS SysDirectoryItemFullInfoView;

CREATE VIEW SysDirectoryItemFullInfoView AS
SELECT
    a.*,
    b.name CategoryName,
    b.code CategoryCode
FROM public.sysdirectoryitem a
LEFT JOIN public.sysdirectorycategory b ON a.categoryid = b.id AND b.isdeleted = false
WHERE a.isdeleted = false;