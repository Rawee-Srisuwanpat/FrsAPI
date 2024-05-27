namespace field_recording_api.Utilities
{
    public interface IUnitOfWorkDB : IDisposable
    {
        IRepositoryEfcore<TEntity> SIISRepository<TEntity>() where TEntity : class;

        //IRepositoryEfcore<TEntity> FieldRecordingRepository<TEntity>() where TEntity : class;

        //IRepositoryEfcore<TEntity> SQLingRepository<TEntity>() where TEntity : class;

        void Save();
    }
}
