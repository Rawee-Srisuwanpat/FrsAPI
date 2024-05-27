using field_recording_api.Models.AccountDetailModel;
using field_recording_api.Models.ConfigCloseSystem;
using field_recording_api.Models.HttpModel;

namespace field_recording_api.Services.MasterDataServices
{
    public interface IMasterDataServices
    {
        Task<ResponseContext> getDropdownMaster();

        ConfigCloseSystemResModel GetConfigCloseSystem(ConfigCloseSystemReqModel req );
    }
}
