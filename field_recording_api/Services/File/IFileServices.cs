using field_recording_api.Models.FileModel;
using field_recording_api.Models.HttpModel;

namespace field_recording_api.Services.File
{
    public interface IFileServices
    {
        Task<ResponseContext> Upload(FileModel Req);
    }
}
