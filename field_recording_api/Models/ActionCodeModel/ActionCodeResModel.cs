using field_recording_api.Models.DelinquencyReason;
using field_recording_api.Models.ModeOfContact;
using field_recording_api.Models.PartyContacted;
using field_recording_api.Models.PlaceOfContact;
using field_recording_api.Models.ResultCodeModel;

namespace field_recording_api.Models.ActionCodeModel
{
    public class ActionCodeResModel
    {
        public string status_code { get; set; }
        public string status_text { get; set; }
        public List<ActionCodeDtoModel>? payload { get; set; }

        public List<ResultCodeDtoModel>? payload_result_code { get; set; }

        public List<DelinquencyReasonDtoModel>? payload_delinquency_reason { get; set; }

        public List<ModeOfContactDtoModel>? payload_mode_of_contact { get; set; }
        public List<PlaceOfContactDtoModel>? payload_place_of_contact { get; set; }
        public List<PartyContactedDtoModel>? payload_party_contacted { get; set; }
    }
}
