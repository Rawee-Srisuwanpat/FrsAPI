using Azure;
using field_recording_api.DataAccess.FieldRecording;
using field_recording_api.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net;
using System.Text;
using AutoMapper;
using System;
using System.Drawing;

namespace field_recording_api.DataAccess
{
    public class EfCoreRepository<T> : IRepositoryEfcore<T> where T : class
    {
        //private readonly IDbContext _dbContext;
        private readonly DbContext _dbContext;
        internal DbSet<T> _dbSet;
        private readonly IMapper _mapper;

        public EfCoreRepository(DbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            _mapper = mapper;
        }
        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }
        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }
        public async Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            // fetch a Queryable that includes all expression-based includes
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(_dbContext.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            // return the result of the query using the specification's criteria expression
            return await secondaryResult
                            .Where(spec.Criteria)
                            .ToListAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            var primaryId = GetKey(entity);
            var Series = await _dbContext.Set<T>().FindAsync(primaryId);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<T, T>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) =>
                    {
                        if (srcMember != null)
                        {
                            return srcMember != null;
                        }
                        return srcMember != null;
                    }
                ));
            });
            var mapper = new Mapper(config);
            mapper.Map(entity, Series);
            _dbContext.Entry(Series).CurrentValues.SetValues(Series);
            //_dbContext.Entry(found).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            //if (await _dbContext.Set<T>().FindAsync(primaryId) is T found) {}
        }

        public virtual long GetKey<T>(T entity)
        {
            var keyName = _dbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
                .Select(x => x.Name).Single();

            return (long)entity.GetType().GetProperty(keyName).GetValue(entity, null);
        }

        public async Task<IQueryable<T>> Query()
        {
            try
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                IQueryable<T> query = _dbSet;
                return query;
            }
            finally
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }

        }

        public async Task<IEnumerable<T>> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            try
            {

                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

                IQueryable<T> query = _dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (orderBy != null)
                {
                    return orderBy(query);
                }

                return query;
            }
            finally
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }
    }
}
