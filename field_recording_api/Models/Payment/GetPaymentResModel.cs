using field_recording_api.Models.FollowUp;

namespace field_recording_api.Models.Payment
{
    public class GetPaymentResModel
    {
        public string status_code { get; set; }
        public string status_text { get; set; }

        public List<GetPaymentDtoModel>? payload { get; set; }
    }
}
