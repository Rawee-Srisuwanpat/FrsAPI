using field_recording_api.Models.VisitFeeModel;

namespace field_recording_api.Models.AccountDetailModel
{
    public class AllocationModel
    {
        public string result {  get; set; }
        public string errorcode { get; set; }
        public string message { get; set; }

        public List<allocations> allocations { get; set;}
    }

    public class allocations 
    {
        public string applicationid { get; set; }
        public string fullsync { get; set; }
        public string applicantid { get; set; }
        public string screenid { get; set; }
        public data data { get; set; }


    }

    public class data 
    {
        public fields fields { get; set; }
        public List<grids> grids { get; set; }
    }

    public class fields
    { 
               public string ORG            { get  ;set;}
               public string ANO            { get  ;set;}
               public string LCNO           { get  ;set;}
               public string ASEQ           { get  ;set;}
               public string LANO           { get  ;set;}
               public string ODDAYS         { get  ;set;}
               public string BC             { get  ;set;}
               public string ODAMT          { get  ;set;}
               public string OSAMT          { get  ;set;}
               public string ADNO           { get  ;set;}
               public string ACST           { get  ;set;}
               public string AON            { get  ;set;}
               public string DE             { get  ;set;}
               public string DD             { get  ;set;}
               public string PRIORITY       { get  ;set;}
               public string REM            { get  ;set;}
               public string FN             { get  ;set;}
               public string MN             { get  ;set;}
               public string LN             { get  ;set;}
               public string AN             { get  ;set;}
               public string GEN            { get  ;set;}
               public string DOB            { get  ;set;}
               public string CMS            { get  ;set;}
               public string OCC            { get  ;set;}
               public string NM             { get  ;set;}
               public string LNAMT          { get  ;set;}
               public string EMI            { get  ;set;}
               public string MOA            { get  ;set;}
               public string CRSK           { get  ;set;}
               public string WFC            { get  ;set;}
               public string ADDR           { get  ;set;}
               public string LICNUM         { get  ;set;}
               public string CARPROV        { get  ;set;}
               public string ENGNUM         { get  ;set;}
               public string CHASISNUM      { get  ;set;}
               public string BRAND          { get  ;set;}
               public string MODEL          { get  ;set;}
               public string APPCOL         { get  ;set;}
               public string PHONE1         { get  ;set;}
               public string PHONE2         { get  ;set;}
               public string PHONE3         { get  ;set;}
               public string PHONE4         { get  ;set;}
               public string EMAIL          { get  ;set;}
               public string PAN            { get  ;set;}
               public string POSAMT         { get  ;set;}
               public string PDC            { get  ;set;}
               public string PF             { get  ;set;}
               public string DS             { get  ;set;}
               public string CYDAY          { get  ;set;}
               public string PDESC          { get  ;set;}
               public string DISDATE        { get  ;set;}
               public string LSTPMTDT       { get  ;set;}
               public string LSTAMTPAID     { get  ;set;}
               public string INSTDT         { get  ;set;}
               public string TPENALCHRG     { get  ;set;}
               public string CANCELFEE      { get  ;set;}
               public string ODPERIOD       { get  ;set;}
               public string CTRUNK         { get  ;set;}
               public string LATLONG        { get  ;set;}
               //public string ORG            { get  ;set;}
               //public string ANO            { get  ;set;}
               public string CNO            { get;  set; }
    }

    public class grids
    { 
        public string id { get ;set;}
        public string tbseq { get ;set;}
        public string stbseq { get ;set;}
        public string gridrowseq { get ;set;}
        public fieldsIn_grids fields { get ;set;}
    }

    public class fieldsIn_grids
    {
        public string TYP {get ;set;}
        public string AD1 {get ;set;}
        public string AD2 {get ;set;}
        public string LM  {get ;set;}
        public string ST  {get ;set;}
        public string CT  {get ;set;}
        public string AR  {get ;set;}
        public string ZP { get; set; }

        //////////////////////////////////////
        ///contact_note

        public string AC { get; set; }
        public string DTA { get; set; }
        public string RCD { get; set; }
        public string NA { get; set; }
        public string DNA { get; set; }
        public string RMK { get; set; }
        public string PROMISEDATE { get; set; }
        public string PROMISEAMOUNT { get; set; }
        public string POC { get; set; }
        public string MODC { get; set; }
        public string ICONTACTNO { get; set; }

        //////////////////////////////////////
        ///billing-invoice-detail
        ///
        public string DTTR   { get; set; }
        public string MD     { get; set; }
        public string AMT    { get; set; }
        public string DTCH   { get; set; }
        public string RMK_INVOICE { get; set; }





    }
}
