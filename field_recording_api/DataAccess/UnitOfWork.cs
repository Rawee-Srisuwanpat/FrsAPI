using field_recording_api.DataAccess.FieldRecording;
using field_recording_api.Utilities;

namespace field_recording_api.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FieldRecordingContext _dataContext;

        public UnitOfWork(FieldRecordingContext dataContext)
        {
            _dataContext = dataContext;
        }
        public void BeginTransaction()
        {
            _dataContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _dataContext.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _dataContext.Database.RollbackTransaction();
        }
    }
}
