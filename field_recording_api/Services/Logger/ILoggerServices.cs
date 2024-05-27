using field_recording_api.DataAccess.FieldRecording;
using System.Runtime.CompilerServices;

namespace field_recording_api.Services.Logger
{
    public interface ILoggerServices
    {
        Task dblog(string MessageCode, object InputData, object OutputData, object? remak = null, [CallerMemberName] string methodName = "");
        void Info(string message);
    }
}
