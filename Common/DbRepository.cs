using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WPFAssignment1Group3.Common;
using WPFAssignment1Group3.Models;

public class DBRepository : IDBRepository
{
    private const int BatchSize = 1000;

    public MyStoreContext Context { get; }

    public DBRepository(MyStoreContext context)
    {
        context.Database.SetCommandTimeout(180);
        Context = context;
    }

    public async Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TResult, bool>> expression, bool track = true) where TResult : class
    {
        return track
            ? await Context.Set<TResult>().FirstOrDefaultAsync(expression)
            : await Context.Set<TResult>().Where(expression).AsNoTracking().FirstOrDefaultAsync();
    }


    public async Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> expression = null,
        Expression<Func<T, TResult>> selector = null) where T : class
    {
        var set = Context.Set<T>();
        var entity = expression == null ? (await set.FirstOrDefaultAsync()) : (await set.FirstOrDefaultAsync(expression));
        if (entity == null) return default(TResult);
        if (selector != null) return selector.Compile()(entity);
        selector = EF.Functions.GetSelector<T, TResult>();
        return selector.Compile()(entity);
    }

    public IQueryable<TResult> Filter<T, TResult>(Expression<Func<T, bool>> expression = null,
        Expression<Func<T, TResult>> selector = null) where T : class
    {
        var set = Context.Set<T>();
        var entities = expression == null ? set : set.Where(expression);
        if (selector != null) return entities.Select(selector);
        selector = EF.Functions.GetSelector<T, TResult>();
        return entities.Select(selector);
    }

    public IQueryable<TResult> Query<TResult>(Func<DbContext, IQueryable<TResult>> expression)
    {
        return expression(Context);
    }

    public void Execute(Action<DbContext> expression)
    {
        expression(Context);
    }

    public IQueryable<T> FromSql<T>(string sql, params object[] param) where T : class
    {
        return Context.Set<T>().FromSqlRaw(sql, param);
    }

    public IQueryable<T> FromSql<T>(string formattedSql) where T : class
    {
        return Context.Set<T>().FromSqlRaw(formattedSql);
    }


    public async Task<List<T>> SqlQueryAsync<T>(string sql, params object[] param)
    {
        using (var command = Context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (param != null && param.Any())
            {
                foreach (var p in param)
                {
                    command.Parameters.Add(p);
                }
            }
            await Context.Database.OpenConnectionAsync();
            using (var result = await command.ExecuteReaderAsync())
            {
                List<T> list = new List<T>();
                while (await result.ReadAsync())
                {
                    var typeInfo = typeof(T);
                    if (typeInfo.Name == "String")
                    {
                        list.Add((T)result[0]);
                    }
                    else
                    {
                        var obj = Activator.CreateInstance<T>();
                        if (typeof(T).IsClass)
                        {
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                if (!Equals(result[prop.Name], DBNull.Value))
                                {
                                    prop.SetValue(obj, result[prop.Name], null);
                                }
                            }
                        }
                        else
                        {
                            obj = (T)result[0];
                        }

                        list.Add(obj);
                    }
                }
                return list;
            }
        }
    }



    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await Context.Database.BeginTransactionAsync();
    }


    public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] param)
    {
        return await Context.Database.ExecuteSqlRawAsync(sql, param);
    }


    public async Task<int> ExecuteSqlCommandAsync(string formattedSql)
    {
        return await Context.Database.ExecuteSqlRawAsync(formattedSql);
    }

    [Obsolete]
    public virtual T Add<T>(T entity) where T : class
    {
        var entry = Context.Set<T>().Add(entity);
        return entry.Entity;
    }

    public async virtual Task<T> AddAsync<T>(T entity) where T : class
    {
        var entry = await Context.Set<T>().AddAsync(entity);
        return entry.Entity;
    }


    public async virtual Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
    {
        await Context.Set<T>().AddRangeAsync(entities);
    }

    public virtual int Update<T>(T entity) where T : class
    {
        Context.Set<T>().Update(entity);
        return 1;
    }

    public virtual async Task<int> UpdateAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateFactory) where T : class
    {
        var updateList = await Context.Set<T>().Where(predicate).ToListAsync();
        var memberInitExpression = updateFactory.Body as MemberInitExpression;
        if (memberInitExpression == null)
            throw new ArgumentException("The update expression must be of type MemberInitExpression.", "updateExpression");
        foreach (MemberBinding binding in memberInitExpression.Bindings)
        {
            string propertyName = binding.Member.Name;
            var memberAssignment = binding as MemberAssignment;
            if (memberAssignment == null)
                throw new ArgumentException("The update expression MemberBinding must only by type MemberAssignment.", "updateExpression");

            object value;
            if (memberAssignment.Expression.NodeType == ExpressionType.Constant)
            {
                var constantExpression = memberAssignment.Expression as ConstantExpression;
                if (constantExpression == null)
                    throw new ArgumentException("The MemberAssignment expression is not a ConstantExpression.", "updateExpression");

                value = constantExpression.Value;
            }
            else
            {
                LambdaExpression lambda = Expression.Lambda(memberAssignment.Expression, null);
                value = lambda.Compile().DynamicInvoke();
            }
            updateList.ForEach(t => t.GetType().GetProperty(propertyName).SetValue(t, value));
        }
        return updateList.Count();
    }

    public async Task UpdateFieldsAsync<T>(T entity, List<string> fields) where T : class
    {
        Context.Set<T>().Attach(entity);
        fields.ForEach(f => Context.Entry(entity).Property(f).IsModified = true);
    }

    public async Task UpdateFieldsRangeAsync<T>(IList<T> entities, List<string> fields) where T : class
    {
        foreach (var entity in entities)
        {
            await UpdateFieldsAsync(entity, fields);
        }
    }

    public int Delete<T>(T entity) where T : class
    {
        Context.Set<T>().Remove(entity);
        return 1;
    }

    public int Delete<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        var removeList = Context.Set<T>().Where(predicate);
        Context.Set<T>().RemoveRange(removeList);
        return removeList.Count();
    }

    public int DeleteRange<T>(IEnumerable<T> entities) where T : class
    {
        Context.Set<T>().RemoveRange(entities);
        return entities.Count();
    }

    public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        return await Task.Run(() => Delete(predicate), default(CancellationToken));
    }


    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
    }

    public void BulkInsertOrUpdate<T>(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

}
public static class EFUtil
{
    public static Expression<Func<T, TResult>> GetSelector<T, TResult>(this DbFunctions _)
    {
        if (!(Activator.CreateInstance<TResult>() is ISelector<T, TResult> resultObj))
        {
            throw new Exception(
                $"The type {typeof(TResult)} is invalid, please implement interface ISelector<T1, T2>.");
        }
        return resultObj.Selector;
    }
    public interface ISelector<T1, T2>
    {
        [JsonIgnore]
        Expression<Func<T1, T2>> Selector { get; }
    }
}