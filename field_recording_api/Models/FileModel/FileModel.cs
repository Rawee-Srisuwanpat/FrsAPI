using field_recording_api.Models.AccountDetailModel;

namespace field_recording_api.Models.FileModel
{
    public class FileModel
    {
        public string contract_no { get; set; }

        public string contract_note_id { get; set; }
        public string user_name { get; set; }
        public IList<IFormFile> inputFile { get; set; }

        public AccountDetaiAddresslModel adress { get; set; }
        public DateTime fileDate { get; set; } 
    }
}
