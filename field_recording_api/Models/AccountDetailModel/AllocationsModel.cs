using System.Dynamic;
using static field_recording_api.Models.AccountDetailModel.AllocationsModel;

namespace field_recording_api.Models.AccountDetailModel
{
    public class AllocationsModel
    {
        public class AllocationsData
        {
            public AllocationsFields fields { get; set; }
            public List<AllocationsGrid> grids { get; set; }
        }

        public class AllocationsFields
        {
            public string ORG { get; set; }
            public string ANO { get; set; }
            public string LCNO { get; set; }
            public string ASEQ { get; set; }
            public string LANO { get; set; }
            public string ODDAYS { get; set; }
            public string BC { get; set; }
            public string ODAMT { get; set; }
            public string OSAMT { get; set; }
            public string ADNO { get; set; }
            public string ACST { get; set; }
            public string AON { get; set; }
            public string DE { get; set; }
            public string DD { get; set; }
            public string PRIORITY { get; set; }
            public string REM { get; set; }
            public string FN { get; set; }
            public string MN { get; set; }
            public string LN { get; set; }
            public string AN { get; set; }
            public string GEN { get; set; }
            public string DOB { get; set; }
            public string CMS { get; set; }
            public string OCC { get; set; }
            public string NM { get; set; }
            public string LNAMT { get; set; }
            public string EMI { get; set; }
            public string MOA { get; set; }
            public string CRSK { get; set; }
            public string WFC { get; set; }
            public string ADDR { get; set; }
            public string LICNUM { get; set; }
            public string CARPROV { get; set; }
            public string ENGNUM { get; set; }
            public string CHASISNUM { get; set; }
            public string BRAND { get; set; }
            public string MODEL { get; set; }
            public string APPCOL { get; set; }
            public string PHONE1 { get; set; }
            public string PHONE2 { get; set; }
            public string PHONE3 { get; set; }
            public string PHONE4 { get; set; }
            public string EMAIL { get; set; }
            public string PAN { get; set; }
            public string POSAMT { get; set; }
            public string PDC { get; set; }
            public string PF { get; set; }
            public string DS { get; set; }
            public string CYDAY { get; set; }
            public string PDESC { get; set; }
            public string DISDATE { get; set; }
            public string LSTPMTDT { get; set; }
            public string LSTAMTPAID { get; set; }
            public string INSTDT { get; set; }
            public string TPENALCHRG { get; set; }
            public string CANCELFEE { get; set; }
            public string ODPERIOD { get; set; }
            public string CTRUNK { get; set; }
            public string LATLONG { get; set; }
            public string CNO { get; set; }

            // public string CNO { get; set; }

            public string? INSTALLMENT_AMOUNT { get; set; }
            public string FVISITFEE { get; set; }
            public string FMINDUEVISITFEE { get; set; }
            public string FMINODAMOUNT { get; set; }
            public string SZLATESTVISITFEETYPE { get; set; }


            public string ITYPE { get; set; }
            public string IGROUPCUSTOMER { get; set; }
            public string SZRULE1 { get; set; }
            public string SZRULE2 { get; set; }
            public string SZRULE3 { get; set; }
            public string SZRULE4 { get; set; }
            public string SZRULE5 { get; set; }
            public string SZRULE6 { get; set; }
            public string SZRULE7 { get; set; }
            public string SZRULE8 { get; set; }
            public string SZRULE9 { get; set; }
            public string SZRULE10 { get; set; }

        }

        public class AllocationsGrid
        {
            public string id { get; set; }
            public string tbseq { get; set; }
            public string stbseq { get; set; }

            public string gridrowseq { get; set; }


            //public ExpandoObject fields { get; set; }

            public fieldsIn_grids fields { get; set; }
        }

        

    }
}
