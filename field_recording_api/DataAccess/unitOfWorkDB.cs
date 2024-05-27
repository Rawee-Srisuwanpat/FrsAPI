using AutoMapper;
using field_recording_api.DataAccess.FieldRecording;
using field_recording_api.DataAccess.SIIS;
using field_recording_api.Utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Reflection;

namespace field_recording_api.DataAccess
{
    public class unitOfWorkDB : IUnitOfWorkDB
    {
        private DbContext _context;
        //private IDbContext _context;
        //private readonly FieldRecordingContext _fieldRecordingContext;
        //private readonly SiisUatContext _siisUatContext;

        private bool _disposed;
        private Hashtable _repositories;
        private readonly IMapper _mapper;

        public unitOfWorkDB(
            //IDbContext context,
            //FieldRecordingContext fieldRecordingContext,
            //SiisUatContext siisUatContext,
            IMapper mapper
        ) {
            //_context = context;
            //_fieldRecordingContext = fieldRecordingContext;
            //_siisUatContext = siisUatContext;
            _mapper = mapper;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing && _context != null)
                    _context.Dispose();

            _disposed = true;
        }

        public IRepositoryEfcore<TEntity> SIISRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            //_context = _siisUatContext;

            if (_repositories.ContainsKey(type)) return (IRepositoryEfcore<TEntity>)_repositories[type];

            var repositoryType = typeof(EfCoreRepository<>);

            var repositoryInstance =
                Activator.CreateInstance(repositoryType
                    .MakeGenericType(typeof(TEntity)), _context, _mapper);

            _repositories.Add(type, repositoryInstance);

            //var instance = new EfRepository<TEntity>((DbContext)_siisUatContext, _mapper);

            return (IRepositoryEfcore<TEntity>)_repositories[type];
        }

        //public IRepositoryEfcore<TEntity> FieldRecordingRepository<TEntity>() where TEntity : class
        //{
        //    if (_repositories == null)
        //        _repositories = new Hashtable();

        //    var type = typeof(TEntity).Name;

        //    _context = _fieldRecordingContext;

        //    if (_repositories.ContainsKey(type)) return (IRepositoryEfcore<TEntity>)_repositories[type];

        //    var repositoryType = typeof(EfCoreRepository<>);

        //    var repositoryInstance =
        //        Activator.CreateInstance(repositoryType
        //            .MakeGenericType(typeof(TEntity)), _context, _mapper);

        //    _repositories.Add(type, repositoryInstance);

        //    return (IRepositoryEfcore<TEntity>)_repositories[type];
        //}


        //public IRepositoryEfcore<TEntity> SQLingRepository<TEntity>() where TEntity : class
        //{
        //    if (_repositories == null)
        //        _repositories = new Hashtable();

        //    var listcontext = Assembly.GetExecutingAssembly()
        //        .GetTypes()
        //        .Where(a => a.Name.EndsWith("Context") && a.IsClass && typeof(DbContext).IsAssignableFrom(a))
        //        .Select(a => new { assignedType = a })
        //        .ToList();

        //    var type = typeof(TEntity).Name;
        //    //var ggg = ((DbContext)listcontext[0].GetType());
        //    //var ccc = (
        //    //            from a in listcontext 
        //    //            where (DbContext)
        //    //          )
        //    //var nnn = (from j in _fieldRecordingContext.Model.GetEntityTypes()
        //    //           where j.ClrType.Name == type
        //    //           select j.ClrType.Name).FirstOrDefault();

        //    //if (nnn)

        //    //_context = _fieldRecordingContext;
        //    //_context = listcontext[0];

        //    if (_repositories.ContainsKey(type)) return (IRepositoryEfcore<TEntity>)_repositories[type];

        //    var repositoryType = typeof(EfCoreRepository<>);

        //    var repositoryInstance =
        //        Activator.CreateInstance(repositoryType
        //            .MakeGenericType(typeof(TEntity)), _context, _mapper);

        //    _repositories.Add(type, repositoryInstance);

        //    return (IRepositoryEfcore<TEntity>)_repositories[type];
        //}
    }
}
