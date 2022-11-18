---获取明细内容
if exists (select 1 from  sysobjects where  id = object_id('ProductItemView') and type = 'V')
   drop view ProductItemView
go

create view ProductItemView 
as 

select * from ProductItem 

go