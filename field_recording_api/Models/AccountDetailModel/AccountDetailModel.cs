using System.ComponentModel.DataAnnotations;

namespace field_recording_api.Models.AccountDetailModel
{
    public class AccountDetailModel
    {
        public string user_name { get; set; }
        [Required]
        public string contract_no { get; set; }
    }

    public class SaveAccountDetailModel
    {
        public string contract_no { get; set; }
        public string action { get; set; }
        public string result { get; set; }
        public string user_name { get; set; }
        public string subDistrict { get; set; }
        public string district { get; set; }
        public string province { get; set; }
        public string zipCode { get; set; }
        public string latlong { get; set; }
        public string? contact_number { get; set; }
        public string? remark { get; set; }
        public IList<IFormFile> inputFile { get; set; }
    }

    public class AccountDetaiAddresslModel 
    {
        public string subDistrict { get; set; }
        public string district { get; set; }
        public string province { get; set; }
        public string zipCode { get; set; }
        public string latlong { get; set; }
    }
}
