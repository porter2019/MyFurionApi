using SqlSugar;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MyFurionApi.Core
{
    /// <summary>
    /// 仓储
    /// </summary>
    public class SqlSugarRepository<TEntity> where TEntity : class, new()
    {
        private readonly string[] UpdateIgnoreColumns = new string[] { "CreatedTime", "CreatedUserId", "CreatedUserName" };

        #region 属性

        /// <summary>
        /// 初始化 SqlSugar 客户端
        /// </summary>
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public virtual SqlSugarScope Context { get; }

        /// <summary>
        /// 独立数据库上下文
        /// </summary>
        public virtual SqlSugarProvider EntityContext { get; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db"></param>
        public SqlSugarRepository(ISqlSugarClient db)
        {
            Context = _db = (SqlSugarScope)db;
            EntityContext = _db.GetConnectionWithAttr<TEntity>();
            Ado = EntityContext.Ado;
        }

        /// <summary>
        /// 实体集合
        /// </summary>
        public virtual ISugarQueryable<TEntity> Entities => EntityContext.Queryable<TEntity>();

        /// <summary>
        /// 原生 Ado 对象
        /// </summary>
        public virtual IAdo Ado { get; }

        #endregion

        #region 查询

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public int Count(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.Count(whereExpression);
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.CountAsync(whereExpression);
        }


        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Count(string where, object parameters = null)
        {
            return Entities.Where(where, parameters).Count();
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<int> CountAsync(string where, object parameters = null)
        {
            return Entities.Where(where, parameters).CountAsync();
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.Any(whereExpression);
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await Entities.AnyAsync(whereExpression);
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="condition"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<TEntity, bool>> whereExpression, bool condition, Expression<Func<TEntity, bool>> exp)
        {
            return Entities.Where(whereExpression).WhereIF(condition, exp).Any();
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="condition"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression, bool condition, Expression<Func<TEntity, bool>> exp)
        {
            return Entities.Where(whereExpression).WhereIF(condition, exp).AnyAsync();
        }

        /// <summary>
        /// 通过主键获取实体
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TEntity Single(dynamic Id)
        {
            return Entities.InSingle(Id);
        }

        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public TEntity Single(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.Single(whereExpression);
        }

        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.SingleAsync(whereExpression);
        }

        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.First(whereExpression);
        }

        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await Entities.FirstAsync(whereExpression);
        }

        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public TEntity FirstOrDefault(BaseSingleQueryModel req)
        {
            string where = req.BuildPageSearchWhere();
            return Entities.Where("1=1" + where).OrderByIF(!string.IsNullOrWhiteSpace(req.OrderBy), req.OrderBy).First();
        }

        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<TEntity> FirstOrDefaultAsync(BaseSingleQueryModel req)
        {
            string where = req.BuildPageSearchWhere();
            return Entities.Where("1=1" + where).OrderByIF(!string.IsNullOrWhiteSpace(req.OrderBy), req.OrderBy).FirstAsync();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public List<TEntity> ToList()
        {
            return Entities.ToList();
        }

        /// <summary>
        /// 使用原始查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<TEntity> ToList(string where)
        {
            return Entities.Where(where).ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.Where(whereExpression).ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return Entities.OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TEntity>> ToListAsync()
        {
            return Entities.ToListAsync();
        }

        /// <summary>
        /// 使用原始查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<List<TEntity>> ToListAsync(string where)
        {
            return Entities.Where(where).ToListAsync();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.Where(whereExpression).ToListAsync();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return Entities.OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToListAsync();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<TEntity> ToList(BaseListQueryModel req)
        {
            //先构建where条件
            string where = req.BuildPageSearchWhere();

            var queryable = Entities.Where("1=1" + where);
            if (req.Top > 0)
            {
                queryable.Skip(req.Top);
            }
            if (req.OrderBy.IsNotNull())
            {
                queryable.OrderBy(req.OrderBy);
            }
            return queryable.ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<List<TEntity>> ToListAsync(BaseListQueryModel req)
        {
            //先构建where条件
            string where = req.BuildPageSearchWhere();

            var queryable = Entities.Where("1=1" + where);
            if (req.Top > 0)
            {
                queryable.Take(req.Top);
            }
            if (req.OrderBy.IsNotNull())
            {
                queryable.OrderBy(req.OrderBy);
            }
            var sql = queryable.ToSqlString();

            return queryable.ToListAsync();
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public SqlSugarPagedList<TEntity> ToPageList(BasePageQueryModel<TEntity> req)
        {
            //先构建where条件
            string where = req.BuildPageSearchWhere();
            //RefAsync<int> totalCount = 0;
            int totalCount = 0;
            var items = Entities.Where("1=1" + where)
                                           .OrderByIF(!string.IsNullOrWhiteSpace(req.PageInfo.OrderBy), req.PageInfo.OrderBy)
                                           .ToPageList(req.PageInfo.PageIndex, req.PageInfo.PageSize, ref totalCount);

            var totalPages = (int)Math.Ceiling(totalCount / (double)req.PageInfo.PageSize);
            return new SqlSugarPagedList<TEntity>
            {
                PageIndex = req.PageInfo.PageIndex,
                PageSize = req.PageInfo.PageSize,
                Items = items,
                TotalCount = (int)totalCount,
                TotalPages = totalPages,
                HasNextPages = req.PageInfo.PageIndex < totalPages,
                HasPrevPages = req.PageInfo.PageIndex - 1 > 0
            };
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<TEntity>> ToPageListAsync(BasePageQueryModel<TEntity> req)
        {
            //先构建where条件
            string where = req.BuildPageSearchWhere();
            RefAsync<int> totalCount = 0;
            var items = await Entities.Where("1=1" + where)
                                           .OrderByIF(!string.IsNullOrWhiteSpace(req.PageInfo.OrderBy), req.PageInfo.OrderBy)
                                           .ToPageListAsync(req.PageInfo.PageIndex, req.PageInfo.PageSize, totalCount);

            var totalPages = (int)Math.Ceiling(totalCount / (double)req.PageInfo.PageSize);
            return new SqlSugarPagedList<TEntity>
            {
                PageIndex = req.PageInfo.PageIndex,
                PageSize = req.PageInfo.PageSize,
                Items = items,
                TotalCount = (int)totalCount,
                TotalPages = totalPages,
                HasNextPages = req.PageInfo.PageIndex < totalPages,
                HasPrevPages = req.PageInfo.PageIndex - 1 > 0
            };
        }

        #endregion

        #region 新增
        public virtual IInsertable<TEntity> AsInsertable(TEntity entity)
        {
            return EntityContext.Insertable(entity);
        }

        public virtual IInsertable<TEntity> AsInsertable(params TEntity[] entities)
        {
            return EntityContext.Insertable(entities);
        }

        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Insert(TEntity entity)
        {
            return EntityContext.Insertable(entity).ExecuteCommand();
        }

        /// <summary>
        /// 新增多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual int Insert(params TEntity[] entities)
        {
            return EntityContext.Insertable(entities).ExecuteCommand();
        }

        /// <summary>
        /// 新增多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual int Insert(IEnumerable<TEntity> entities)
        {
            return EntityContext.Insertable(entities.ToArray()).ExecuteCommand();
        }

        /// <summary>
        /// 新增一条记录返回自增Id
        /// </summary>
        /// <param name="insertObj"></param>
        /// <returns></returns>
        public virtual int InsertReturnIdentity(TEntity insertObj)
        {
            return EntityContext.Insertable(insertObj).ExecuteReturnIdentity();
        }

        /// <summary>
        /// 新增一条记录返回雪花Id
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual long InsertReturnSnowflakeId(TEntity entity)
        {
            return EntityContext.Insertable(entity).ExecuteReturnSnowflakeId();
        }

        /// <summary>
        /// 新增一条记录返回实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity InsertReturnEntity(TEntity entity)
        {
            return EntityContext.Insertable(entity).ExecuteReturnEntity();
        }



        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual Task<int> InsertAsync(TEntity entity)
        {
            return EntityContext.Insertable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 新增多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual Task<int> InsertAsync(params TEntity[] entities)
        {
            return EntityContext.Insertable(entities).ExecuteCommandAsync();
        }

        /// <summary>
        /// 新增多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual Task<int> InsertAsync(IEnumerable<TEntity> entities)
        {
            if (entities != null && entities.Any())
            {
                return EntityContext.Insertable(entities.ToArray()).ExecuteCommandAsync();
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// 新增一条记录返回自增Id
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertReturnIdentityAsync(TEntity entity)
        {
            return await EntityContext.Insertable(entity).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 新增一条记录返回雪花Id
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<long> InsertReturnSnowflakeIdAsync(TEntity entity)
        {
            return await EntityContext.Insertable(entity).ExecuteReturnSnowflakeIdAsync();
        }

        /// <summary>
        /// 新增一条记录返回实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> InsertReturnEntityAsync(TEntity entity)
        {
            return await EntityContext.Insertable(entity).ExecuteReturnEntityAsync();
        }
        #endregion

        #region 更新

        /// <summary>
        /// 更新明细
        /// </summary>
        /// <param name="masterId"></param>
        /// <param name="list"></param>
        public virtual void UpdateItemDiff(int masterId, IEnumerable<TEntity> list)
        {
            var type = typeof(TEntity);
            //获取外键
            var foreignKeyProInfo = type.GetProperties().First(x => x.GetCustomAttribute(typeof(ForeignKeyTagAttribute), true) != null);
            if (foreignKeyProInfo == null) throw new Exception($"{type.FullName}缺少指定{typeof(ForeignKeyTagAttribute).FullName}标记的属性");
            var foreignKeyName = foreignKeyProInfo.Name;

            if (list.IsEmpty())
            {
                //清空
                DeleteWithSoft($"{foreignKeyName} = {masterId}");
                return;
            }

            if (!type.GetProperties().Any(x => x.Name == "Id")) throw new Exception($"{type.FullName}缺少Id主键");

            //原来的列表
            var sourceList = ToList($"{foreignKeyName} = {masterId}");
            //新增的
            var addDataList = new List<TEntity>();
            //要修改的
            var updateDataList = new List<TEntity>();
            var updateIdList = new List<int>();
            //查找新增和修改的
            foreach (var item in list)
            {
                //如果id>0，则修改，否则是添加
                var idValue = item.GetType().GetRuntimeProperty("Id").GetValue(item).ObjToInt();
                if (idValue > 0)
                {
                    updateIdList.Add(idValue);
                    updateDataList.Add(item);
                }
                else
                {
                    //外键赋值
                    item.GetType().GetRuntimeProperty(foreignKeyName).SetValue(item, masterId);
                    addDataList.Add(item);
                }
            }

            //执行添加
            Insert(addDataList);
            //执行修改
            UpdateWithOptLock(updateDataList);
            //查找删除的
            if (sourceList.Any())
            {
                //查找原来的数据Id
                var sourceIdList = new List<int>();
                foreach (var item in sourceList)
                {
                    sourceIdList.Add(item.GetType().GetRuntimeProperty("Id").GetValue(item).ObjToInt());
                }
                //原来的数据和修改的数据取差集
                var deleteIdList = sourceIdList.Except<int>(sourceIdList.Intersect<int>(updateIdList));
                DeleteWithSoft(deleteIdList.ToList<int>());
            }
        }

        /// <summary>
        /// 更新明细
        /// </summary>
        /// <param name="masterId"></param>
        /// <param name="list"></param>
        public virtual async Task UpdateItemDiffAsync(int masterId, IEnumerable<TEntity> list)
        {
            var type = typeof(TEntity);
            //获取外键
            var foreignKeyProInfo = type.GetProperties().First(x => x.GetCustomAttribute(typeof(ForeignKeyTagAttribute), true) != null);
            if (foreignKeyProInfo == null) throw new Exception($"{type.FullName}缺少指定{typeof(ForeignKeyTagAttribute).FullName}标记的属性");
            var foreignKeyName = foreignKeyProInfo.Name;

            if (list.IsEmpty())
            {
                //清空
                DeleteWithSoft($"{foreignKeyName} = {masterId}");
                return;
            }

            if (!type.GetProperties().Any(x => x.Name == "Id")) throw new Exception($"{type.FullName}缺少Id主键");

            //原来的列表
            var sourceList = await ToListAsync($"{foreignKeyName} = {masterId}");
            //新增的
            var addDataList = new List<TEntity>();
            //要修改的
            var updateDataList = new List<TEntity>();
            var updateIdList = new List<int>();
            //查找新增和修改的
            foreach (var item in list)
            {
                //如果id>0，则修改，否则是添加
                var idValue = item.GetType().GetRuntimeProperty("Id").GetValue(item).ObjToInt();
                if (idValue > 0)
                {
                    updateIdList.Add(idValue);
                    updateDataList.Add(item);
                }
                else
                {
                    //外键赋值
                    item.GetType().GetRuntimeProperty(foreignKeyName).SetValue(item, masterId);
                    addDataList.Add(item);
                }
            }

            //执行添加
            await InsertAsync(addDataList);
            //执行修改
            await UpdateWithOptLockAsync(updateDataList);
            //查找删除的
            if (sourceList.Any())
            {
                //查找原来的数据Id
                var sourceIdList = new List<int>();
                foreach (var item in sourceList)
                {
                    sourceIdList.Add(item.GetType().GetRuntimeProperty("Id").GetValue(item).ObjToInt());
                }
                //原来的数据和修改的数据取差集
                var deleteIdList = sourceIdList.Except<int>(sourceIdList.Intersect<int>(updateIdList));
                await DeleteWithSoftAsync(deleteIdList.ToList<int>());
            }
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Update(TEntity entity)
        {
            return EntityContext.Updateable(entity).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommand();
        }

        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual int Update(params TEntity[] entities)
        {
            return EntityContext.Updateable(entities).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommand();
        }
        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual int Update(IEnumerable<TEntity> entities)
        {
            return EntityContext.Updateable(entities.ToArray()).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommand();
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            return await EntityContext.Updateable(entity).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandAsync();
        }
        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="predicate">更新的条件</param>
        /// <param name="content">更新的内容</param>
        /// <returns></returns>
        public virtual int Update(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> content)
        {
            return EntityContext.Updateable(content).Where(predicate).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommand();
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="predicate">更新的条件</param>
        /// <param name="content">更新的内容</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> content)
        {
            return await EntityContext.Updateable(content).Where(predicate).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual Task<int> UpdateAsync(params TEntity[] entities)
        {
            return EntityContext.Updateable(entities).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual Task<int> UpdateAsync(IEnumerable<TEntity> entities)
        {
            return EntityContext.Updateable(entities.ToArray()).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandAsync();
        }

        public virtual IUpdateable<TEntity> AsUpdateable(TEntity entity)
        {
            return EntityContext.Updateable(entity).IgnoreColumns(UpdateIgnoreColumns);
        }

        public virtual IUpdateable<TEntity> AsUpdateable(IEnumerable<TEntity> entities)
        {
            return EntityContext.Updateable<TEntity>(entities).IgnoreColumns(UpdateIgnoreColumns);
        }
        #endregion

        #region 更新 带乐观锁 Version

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int UpdateWithOptLock(TEntity entity)
        {
            return EntityContext.Updateable(entity).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandWithOptLock(true);
        }

        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual int UpdateWithOptLock(params TEntity[] entities)
        {
            return EntityContext.Updateable(entities).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandWithOptLock(true);
        }
        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual int UpdateWithOptLock(IEnumerable<TEntity> entities)
        {
            return EntityContext.Updateable(entities.ToArray()).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandWithOptLock(true);
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateWithOptLockAsync(TEntity entity)
        {
            return await EntityContext.Updateable(entity).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandWithOptLockAsync(true);
        }
        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="predicate">更新的条件</param>
        /// <param name="content">更新的内容</param>
        /// <returns></returns>
        public virtual int UpdateWithOptLock(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> content)
        {
            return EntityContext.Updateable(content).Where(predicate).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandWithOptLock(true);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="predicate">更新的条件</param>
        /// <param name="content">更新的内容</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateWithOptLockAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> content)
        {
            return await EntityContext.Updateable(content).Where(predicate).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandWithOptLockAsync(true);
        }

        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual Task<int> UpdateWithOptLockAsync(params TEntity[] entities)
        {
            return EntityContext.Updateable(entities).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandWithOptLockAsync(true);
        }

        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual Task<int> UpdateWithOptLockAsync(IEnumerable<TEntity> entities)
        {
            return EntityContext.Updateable(entities.ToArray()).IgnoreColumns(UpdateIgnoreColumns).ExecuteCommandWithOptLockAsync(true);
        }

        #endregion

        #region 删除

        #region 物理删除

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Delete(TEntity entity)
        {
            return EntityContext.Deleteable(entity).ExecuteCommand();
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual int Delete(object key)
        {
            return EntityContext.Deleteable<TEntity>().In(key).ExecuteCommand();
        }

        /// <summary>
        /// 删除多条记录
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual int Delete(params object[] keys)
        {
            return EntityContext.Deleteable<TEntity>().In(keys).ExecuteCommand();
        }

        /// <summary>
        /// 自定义条件删除记录
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public int Delete(Expression<Func<TEntity, bool>> whereExpression)
        {
            return EntityContext.Deleteable<TEntity>().Where(whereExpression).ExecuteCommand();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        public int Delete(string whereSql)
        {
            if (whereSql.IsNull()) return 0;
            return EntityContext.Deleteable<TEntity>().Where(whereSql).ExecuteCommand();
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual Task<int> DeleteAsync(TEntity entity)
        {
            return EntityContext.Deleteable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual Task<int> DeleteAsync(object key)
        {
            return EntityContext.Deleteable<TEntity>().In(key).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除多条记录
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual Task<int> DeleteAsync(params object[] keys)
        {
            return EntityContext.Deleteable<TEntity>().In(keys).ExecuteCommandAsync();
        }

        /// <summary>
        /// 自定义条件删除记录
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await EntityContext.Deleteable<TEntity>().Where(whereExpression).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(string whereSql)
        {
            if (whereSql.IsNull()) return Task.FromResult(0);
            return EntityContext.Deleteable<TEntity>().Where(whereSql).ExecuteCommandAsync();
        }

        #endregion

        #region 软删除，更新IsDeleted=1

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteWithSoft(int id)
        {
            if (id < 1) return 0;
            return EntityContext.Updateable<TEntity>().SetColumns("IsDeleted", 1).SetColumns("Version", "-1").Where($"Id={id}").ExecuteCommand();
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public int DeleteWithSoft(List<int> idList)
        {
            if (idList.IsEmpty()) return 0;
            return EntityContext.Updateable<TEntity>().SetColumns("IsDeleted", 1).SetColumns("Version", "-1").Where($"Id in ({idList.Join()})").ExecuteCommand();
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="where">额外条件</param>
        /// <returns></returns>
        public int DeleteWithSoft(List<int> idList, Expression<Func<TEntity, bool>> where)
        {
            if (idList.IsEmpty()) return 0;
            return EntityContext.Updateable<TEntity>().SetColumns("IsDeleted", 1).SetColumns("Version", "-1").Where($"Id in ({idList.Join()})").Where(where).ExecuteCommand();
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        public int DeleteWithSoft(string whereSql)
        {
            if (whereSql.IsNull()) return 0;
            return EntityContext.Updateable<TEntity>().SetColumns("IsDeleted", 1).SetColumns("Version", "-1").Where(whereSql).ExecuteCommand();
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> DeleteWithSoftAsync(int id)
        {
            if (id < 1) return Task.FromResult(0);
            return EntityContext.Updateable<TEntity>().SetColumns("IsDeleted", 1).SetColumns("Version", "-1").Where($"Id={id}").ExecuteCommandAsync();
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public Task<int> DeleteWithSoftAsync(List<int> idList)
        {
            if (idList.IsEmpty()) return Task.FromResult(0);
            return EntityContext.Updateable<TEntity>().SetColumns("IsDeleted", 1).SetColumns("Version", "-1").Where($"Id in ({idList.Join()})").ExecuteCommandAsync();
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="where">额外条件</param>
        /// <returns></returns>
        public Task<int> DeleteWithSoftAsync(List<int> idList, Expression<Func<TEntity, bool>> where)
        {
            if (idList.IsEmpty()) return Task.FromResult(0);
            return EntityContext.Updateable<TEntity>().SetColumns("IsDeleted", 1).SetColumns("Version", "-1").Where($"Id in ({idList.Join()})").Where(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        public Task<int> DeleteWithSoftAsync(string whereSql)
        {
            if (whereSql.IsNull()) return Task.FromResult(0);
            return EntityContext.Updateable<TEntity>().SetColumns("IsDeleted", 1).SetColumns("Version", "-1").Where(whereSql).ExecuteCommandAsync();
        }

        #endregion

        #endregion

        #region 其他

        /// <summary>
        /// 根据表达式查询多条记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual ISugarQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return AsQueryable(predicate);
        }

        /// <summary>
        /// 根据表达式查询多条记录
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual ISugarQueryable<TEntity> Where(bool condition, Expression<Func<TEntity, bool>> predicate)
        {
            return AsQueryable().WhereIF(condition, predicate);
        }

        /// <summary>
        /// 使用Sql语句进行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual ISugarQueryable<TEntity> Where(string sql)
        {
            return AsQueryable().Where(sql);
        }

        /// <summary>
        /// 使用Sql语句进行查询
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual ISugarQueryable<TEntity> Where(bool condition, string sql)
        {
            return AsQueryable().WhereIF(condition, sql);
        }

        /// <summary>
        /// 构建查询分析器
        /// </summary>
        /// <returns></returns>
        public virtual ISugarQueryable<TEntity> AsQueryable()
        {
            return Entities;
        }

        /// <summary>
        /// 构建查询分析器
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual ISugarQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Where(predicate);
        }

        /// <summary>
        /// 直接返回数据库结果
        /// </summary>
        /// <returns></returns>
        public virtual List<TEntity> AsEnumerable()
        {
            return AsQueryable().ToList();
        }

        /// <summary>
        /// 直接返回数据库结果
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<TEntity> AsEnumerable(Expression<Func<TEntity, bool>> predicate)
        {
            return AsQueryable(predicate).ToList();
        }

        /// <summary>
        /// 直接返回数据库结果
        /// </summary>
        /// <returns></returns>
        public virtual Task<List<TEntity>> AsAsyncEnumerable()
        {
            return AsQueryable().ToListAsync();
        }

        /// <summary>
        /// 直接返回数据库结果
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual Task<List<TEntity>> AsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate)
        {
            return AsQueryable(predicate).ToListAsync();
        }

        public virtual bool IsExists(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.Any(whereExpression);
        }

        public virtual Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return Entities.AnyAsync(whereExpression);
        }
        #endregion

        #region 仓储事务

        /// <summary>
        /// 切换仓储(注意使用环境)
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>仓储</returns>
        public virtual SqlSugarRepository<T> Change<T>()
            where T : class, new()
        {
            return App.GetService<SqlSugarRepository<T>>();
        }
        /// <summary>
        /// 当前db
        /// </summary>
        public void CurrentBeginTran()
        {
            Ado.BeginTran();
        }
        /// <summary>
        /// 当前db
        /// </summary>
        public void CurrentCommitTran()
        {
            Ado.CommitTran();
        }
        /// <summary>
        /// 当前db
        /// </summary>
        public void CurrentRollbackTran()
        {
            Ado.RollbackTran();
        }
        /// <summary>
        /// 所有db
        /// </summary>
        public void BeginTran()
        {
            Context.BeginTran();
        }
        /// <summary>
        /// 所有db
        /// </summary>
        public void CommitTran()
        {
            Context.CommitTran();
        }
        /// <summary>
        /// 所有db
        /// </summary>
        public void RollbackTran()
        {
            Context.RollbackTran();
        }


        #endregion
    }
}
