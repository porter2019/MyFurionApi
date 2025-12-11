if exists (select 1
            from  sysobjects
           where  id = object_id('sp_update_tree_layer')
            and   type = 'P')
   drop proc sp_update_tree_layer
go

--更新树级层级关系
create  proc [dbo].[sp_update_tree_layer]
as

with p as (
	select a.Id, a.Name, a.OrderNo, 
	    cast('' as varchar(8000)) as ParentName, 						
	    cast('|' + cast(a.Id as varchar) + '|' as varchar(8000)) as FullId, 
	    cast(a.Name as varchar(8000)) as FullName,						
	    1 as LevelNo,
	    cast(a.OrderNo as varchar(8000)) as FullOrderNo
	from Tree a where a.ParentId = 0
	union all
	select a.Id, a.Name, a.OrderNo,
	    cast(p.Name as varchar(8000)) as ParentName,						
	    cast(p.FullId + cast(a.Id as varchar) + '|' as varchar(8000)) as FullId, 
	    cast(p.FullName + '|' + a.Name as varchar(8000)) as FullName,
	    (p.LevelNo + 1) as LevelNo,
	    cast(p.FullOrderNo + '|' + a.OrderNo as varchar(8000)) as FullOrderNo
	from Tree a 
	inner join p on a.ParentId = p.Id
	where a.Id > 0
)

update q set 
    q.ParentName = p.ParentName, 					
    q.FullId = p.FullId, 
    q.FullName = p.FullName, 					
    q.LevelNo = p.LevelNo, 
    q.FullOrderNo = p.FullOrderNo
from Tree q 
inner join p on q.Id = p.Id

go