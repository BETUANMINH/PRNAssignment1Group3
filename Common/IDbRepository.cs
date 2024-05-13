using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WPFAssignment1Group3.Common
{
    public interface IDBRepository : IDisposable
    {

        Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TResult, bool>> expression, bool track = true) where TResult : class;


        Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> expression = null, Expression<Func<T, TResult>> selector = null) where T : class;

        IQueryable<TResult> Filter<T, TResult>(Expression<Func<T, bool>> expression = null, Expression<Func<T, TResult>> selector = null) where T : class;

        IQueryable<TResult> Query<TResult>(Func<DbContext, IQueryable<TResult>> expression);

        void Execute(Action<DbContext> expression);

        IQueryable<T> FromSql<T>(string sql, params object[] param) where T : class;

        IQueryable<T> FromSql<T>(string formattedSql) where T : class;

        Task<IDbContextTransaction> BeginTransactionAsync();




        Task<int> ExecuteSqlCommandAsync(string sql, params object[] param);


        Task<int> ExecuteSqlCommandAsync(string formattedSql);


        Task<T> AddAsync<T>(T entity) where T : class;

        Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class;

        int Update<T>(T entity) where T : class;


        Task<int> UpdateAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateFactory) where T : class;

        Task UpdateFieldsAsync<T>(T entity, List<string> fields) where T : class;

        Task UpdateFieldsRangeAsync<T>(IList<T> entities, List<string> fields) where T : class;

        int Delete<T>(T entity) where T : class;

        int Delete<T>(Expression<Func<T, bool>> predicate) where T : class;

        int DeleteRange<T>(IEnumerable<T> entities) where T : class;

        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class;



        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        DbContext Context { get; }
    }
}
