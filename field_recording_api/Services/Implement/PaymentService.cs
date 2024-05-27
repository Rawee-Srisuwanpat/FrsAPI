using field_recording_api.DataAccess.FieldRecording;
using field_recording_api.Models.AccountDetailModel;
using field_recording_api.Models.ActionCodeModel;
using field_recording_api.Models.GetDropDownListMaster;
using field_recording_api.Models.Payment;
using field_recording_api.Models.ResultCodeModel;
using field_recording_api.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data;
using static field_recording_api.Helpers.JWT.JWT;

namespace field_recording_api.Services.Implement
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly FieldRecordingContext _context;
        private string connectionString = string.Empty;


        public PaymentService(IConfiguration config, FieldRecordingContext _context)
        {
            _config = config;


            this._context = _context;

            this.connectionString = _config.GetSection("ConnectionStrings:SIISConnection").Value;
        }
        public GetPaymentResModel GetPaymentCode(GetPaymentReqModel req)
        {
            var res = new GetPaymentResModel();
            try
            {
                var paymentCode = new GetPaymentDtoModel();
                paymentCode =  GetPaymentCodeFromSIIS(req);
                //using (var ctx = _context)
                //{
                //    var paymentCode = new GetPaymentDtoModel();


                //    //var aa = ctx.tbt_dep_log_statement.AsNoTracking().Where(z => z.contract_no == req.contract_no).OrderByDescending(x => x.statement_channel).FirstOrDefault() ?? throw new Exception($"contact {req.contract_no} not found in tbt_dep_log_statement");


                //    //paymentCode.contract_no = aa.contract_no;
                //    //paymentCode.payment_barcode = aa.payment_barcode;
                //    //paymentCode.payment_qr_code = aa.payment_qr_code;
                //    //paymentCode.statement_channel = aa.statement_channel;

                //    var paymentCodeList = new List<GetPaymentDtoModel>();
                //    paymentCodeList.Add(paymentCode);

                //    res.payload = paymentCodeList;

                //}
                res.payload = new List<GetPaymentDtoModel>() { paymentCode };
                res.status_code = "00";
                res.status_text = "Success";
            }
            catch (Exception ex)
            {
                res.status_code = "99";
                res.status_text = ex.Message;
            }
            return res;
        }



        public GetPaymentDtoModel GetPaymentCodeFromSIIS(GetPaymentReqModel req) 
        {
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();

                string sql = $@"SELECT    TOP 1 contract_no
                                        ,payment_barcode
                                        ,payment_qr_code
                                        ,statement_channel
                                  FROM [dbo].[tbt_dep_log_statement]  ";

                if (!string.IsNullOrEmpty(req.contract_no))
                {
                    sql += $@"  WHERE contract_no = '{req.contract_no}'   ";

                    sql += $@"  Order by [print_date] DESC  ";
                }
                SqlCommand cmd1 = new SqlCommand(sql, con);

                var dataReader = cmd1.ExecuteReader();

                dataTable.Load(dataReader);

            }

            GetPaymentDtoModel returnValue = new GetPaymentDtoModel() ;

            if ((dataTable != null) && (dataTable.Rows.Count == 1))
            {
                returnValue.contract_no = dataTable.Rows[0]["contract_no"].ToString();
                returnValue.payment_barcode = dataTable.Rows[0]["payment_barcode"].ToString();
                returnValue.payment_qr_code = dataTable.Rows[0]["payment_qr_code"].ToString();
                returnValue.statement_channel = dataTable.Rows[0]["statement_channel"].ToString();
            }
            else {
                throw new Exception("contract_no is not found");
            }
            
            return returnValue;
        }
    }
}
