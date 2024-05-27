using Microsoft.AspNetCore.Http;

namespace field_recording_api.Models.FollowUp
{
    public class FollowUpResModel
    {
        public string status_code { get; set; }
        public string status_text { get; set; }

        public List<FollowUpDtoModel>? payload { get; set; }
    }
}
