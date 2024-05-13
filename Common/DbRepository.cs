using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WPFAssignment1Group3.Common;
using WPFAssignment1Group3.Models;

public class DBRepository : IDBRepository
{
    private readonly MyStoreContext _context;

    public DBRepository(MyStoreContext context)
    {
        _context = context;
    }

    public Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TResult, bool>> expression, bool track = true) where TResult : class
    {
        if (!track)
            return _context.Set<TResult>().AsNoTracking().FirstOrDefaultAsync(expression);
        return _context.Set<TResult>().FirstOrDefaultAsync(expression);
    }

    public Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> expression = null, Expression<Func<T, TResult>> selector = null) where T : class
    {
        IQueryable<T> query = _context.Set<T>();

        if (expression != null)
            query = query.Where(expression);

        return selector != null ? query.Select(selector).FirstOrDefaultAsync() : query.FirstOrDefaultAsync() as Task<TResult>;
    }

    public IQueryable<TResult> Filter<T, TResult>(Expression<Func<T, bool>> expression = null, Expression<Func<T, TResult>> selector = null) where T : class
    {
        IQueryable<T> query = _context.Set<T>();

        if (expression != null)
            query = query.Where(expression);

        return selector != null ? query.Select(selector) : query as IQueryable<TResult>;
    }

    public IQueryable<TResult> Query<TResult>(Func<DbContext, IQueryable<TResult>> expression)
    {
        return expression(_context);
    }

    public void Execute(Action<DbContext> expression)
    {
        expression(_context);
    }

    public IQueryable<T> FromSql<T>(string sql, params object[] param) where T : class
    {
        return _context.Set<T>().FromSqlRaw(sql, param);
    }

    public IQueryable<T> FromSql<T>(string formattedSql) where T : class
    {
        return _context.Set<T>().FromSqlRaw(formattedSql);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

  

    public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] param)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, param);
    }

    public async Task<int> ExecuteSqlCommandAsync(string formattedSql)
    {
        return await _context.Database.ExecuteSqlRawAsync(formattedSql);
    }

    public async Task<T> AddAsync<T>(T entity) where T : class
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
    {
        await _context.Set<T>().AddRangeAsync(entities);
    }

    public int Update<T>(T entity) where T : class
    {
        _context.Entry(entity).State = EntityState.Modified;
        return _context.SaveChanges();
    }

    public async Task<int> UpdateAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateFactory) where T : class
    {
        var entities = await _context.Set<T>().Where(predicate).ToListAsync();
        entities.ForEach(entity => updateFactory.Compile().Invoke(entity));
        return _context.SaveChanges();
    }

    public async Task UpdateFieldsAsync<T>(T entity, List<string> fields) where T : class
    {
        _context.Entry(entity).State = EntityState.Modified;
        foreach (var field in fields)
        {
            _context.Entry(entity).Property(field).IsModified = true;
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFieldsRangeAsync<T>(IList<T> entities, List<string> fields) where T : class
    {
        foreach (var entity in entities)
        {
            _context.Entry(entity).State = EntityState.Modified;
            foreach (var field in fields)
            {
                _context.Entry(entity).Property(field).IsModified = true;
            }
        }
        await _context.SaveChangesAsync();
    }

    public int Delete<T>(T entity) where T : class
    {
        _context.Set<T>().Remove(entity);
        return _context.SaveChanges();
    }

    public int Delete<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        var entities = _context.Set<T>().Where(predicate).ToList();
        _context.Set<T>().RemoveRange(entities);
        return _context.SaveChanges();
    }

    public int DeleteRange<T>(IEnumerable<T> entities) where T : class
    {
        _context.Set<T>().RemoveRange(entities);
        return _context.SaveChanges();
    }

    public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        var entities = await _context.Set<T>().Where(predicate).ToListAsync();
        _context.Set<T>().RemoveRange(entities);
        return await _context.SaveChangesAsync();
    }


    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public DbContext Context => _context;

    public void Dispose()
    {
        _context.Dispose();
    }
}
