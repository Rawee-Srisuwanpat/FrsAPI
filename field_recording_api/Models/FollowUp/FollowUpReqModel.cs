namespace field_recording_api.Models.FollowUp
{
    public class FollowUpReqModel
    {
         public string contract_no { get ; set ; } 
         public string Action { get ; set ; } 
         public string Result { get ; set ; } 
         public string NextAction { get ; set ; } 
         public string NextActionDate { get ; set ; } 
         public string DelinquencyReason { get ; set ; } 
         public string Remarks { get ; set ; } 
         public string ModeOfContact { get ; set ; } 
         public string PlaceOfContact { get ; set ; } 
         public string PartyContacted { get ; set ; } 
         public string ContactNumber { get ; set ; } 
         public string Geo { get ; set ; } 
         public string SubDistrict { get ; set ; } 
         public string District { get ; set ; } 
         public string Province { get ; set ; } 
         public string ZipCode { get ; set ; } 
         public string Create_By { get ; set ; }

        public string PTP_DT { get; set; }
        public string PTP_AMT { get; set; }

        public string OS_AMT { get; set; }

        public string OD_AMT { get; set; }

        public string? IPAddress { get ; set ;}
         
        
    }
}
