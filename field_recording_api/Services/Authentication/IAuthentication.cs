using field_recording_api.Models.AccountDetailModel;
using field_recording_api.Models.AuthenticationModel;
using field_recording_api.Models.HttpModel;

namespace field_recording_api.Services.Authentication
{
    public interface IAuthentication
    {
        Task<ResponseContext> authen(AuthenticationModel Req);
    }
}
