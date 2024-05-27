using static field_recording_api.Models.AccountDetailModel.AllocationsModel;

namespace field_recording_api.Models.VisitFeeModel
{
    public class VisitFeeModel
    {
        public string result { get; set; }
        public string errorcode { get; set; }
        public string message { get; set; }

        public List<Allocations> allocations { get; set; }
    }

    public class Allocations {
        public string applicationid { get; set; }
        public string fullsync { get; set; }
        public string applicantid { get; set; }
        public string screenid { get; set; }
        public AllocationsData data { get; set; }
    }


}  
