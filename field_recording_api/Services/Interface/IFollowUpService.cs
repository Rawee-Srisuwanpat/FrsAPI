using field_recording_api.Models.ActionCodeModel;
using field_recording_api.Models.FollowUp;
using field_recording_api.Models.GetDropDownListMaster;

namespace field_recording_api.Services.Interface
{
    public interface IFollowUpService
    {
        public GetDropDownListMasterResModel GetDropdownList(GetDropDownListMasterReqModel req);

        public FollowUpResModel SaveFollowUp(FollowUpReqModel req);
    }
}
