-- Postgresql 18 存储过程
DROP PROCEDURE IF EXISTS sp_update_tree_layer;

CREATE OR REPLACE PROCEDURE sp_update_tree_layer()
LANGUAGE plpgsql
AS $$
BEGIN
    WITH RECURSIVE p AS (
        SELECT
            a."Id",
            a."Name"::text AS "Name",
            a."OrderNo"::text AS "OrderNo",
            ''::text AS "ParentName",
            CONCAT('|', a."Id", '|')::text AS "FullId",
            a."Name"::text AS "FullName",
            1 AS "LevelNo",
            a."OrderNo"::text AS "FullOrderNo"
        FROM "Tree" a
        WHERE a."ParentId" = 0

        UNION ALL

        SELECT
            a."Id",
            a."Name"::text AS "Name",
            a."OrderNo"::text AS "OrderNo",
            p."Name" AS "ParentName",
            CONCAT(p."FullId", a."Id", '|')::text AS "FullId",
            CONCAT(p."FullName", '|', a."Name")::text AS "FullName",
            p."LevelNo" + 1 AS "LevelNo",
            CONCAT(p."FullOrderNo", '|', a."OrderNo")::text AS "FullOrderNo"
        FROM "Tree" a
        JOIN p ON a."ParentId" = p."Id"
    )
    UPDATE "Tree"
    SET
        "ParentName" = p."ParentName",
        "FullId" = LEFT(p."FullId", 1000),
        "FullName" = LEFT(p."FullName", 1000),
        "LevelNo" = p."LevelNo",
        "FullOrderNo" = LEFT(p."FullOrderNo", 1000)
    FROM p
    WHERE "Tree"."Id" = p."Id";
END;
$$;
