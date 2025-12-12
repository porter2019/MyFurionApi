-- 注意：PostgreSQL使用函数替代存储过程

-- 尝试删除函数，如果存在的话
DROP FUNCTION IF EXISTS sp_update_tree_layer() CASCADE;

-- 创建函数
CREATE OR REPLACE FUNCTION sp_update_tree_layer()
RETURNS void AS $$
BEGIN
    -- 使用递归CTE来构建层级结构并更新
    WITH RECURSIVE p AS (
        SELECT
            a.id,
            a.name,
            a.orderno,
            CAST('' AS VARCHAR(255)) AS parentname,           -- 指定具体长度
            CAST(CONCAT('|', a.id, '|') AS VARCHAR(1000)) AS fullid,
            CAST(a.name AS VARCHAR(1000)) AS fullname,
            1 AS levelno,
            CAST(a.orderno AS VARCHAR(2000)) AS fullorderno
        FROM tree a
        WHERE a.parentid = 0
        UNION ALL
        SELECT
            a.id,
            a.name,
            a.orderno,
            CAST(p.name AS VARCHAR(255)) AS parentname,
            CAST(CONCAT(p.fullid, a.id, '|') AS VARCHAR(1000)) AS fullid,
            CAST(CONCAT(p.fullname, '|', a.name) AS VARCHAR(1000)) AS fullname,
            (p.levelno + 1) AS levelno,
            CAST(CONCAT(p.fullorderno, '|', a.orderno) AS VARCHAR(2000)) AS fullorderno
        FROM tree a
        INNER JOIN p ON a.parentid = p.id
    )
    -- 更新tree表中的层级信息
    UPDATE tree q
    SET
        parentname = LEFT(p.parentname, 1000),
        fullid = LEFT(p.fullid, 1000),
        fullname = LEFT(p.fullname, 1000),
        levelno = p.levelno,
        fullorderno = LEFT(p.fullorderno, 1000)
    FROM p
    WHERE q.id = p.id;
END;
$$ LANGUAGE plpgsql;