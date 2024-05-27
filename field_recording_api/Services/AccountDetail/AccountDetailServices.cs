using field_recording_api.Models.AccountDetailModel;
using field_recording_api.Models.HttpModel;
using System.Dynamic;
using System.Text.Json;
using System.IO;
using field_recording_api.Utilities;
using field_recording_api.Services.File;
using field_recording_api.Models.FileModel;
using System.Data;
using field_recording_api.DataAccess.FieldRecording;
using log4net;
using field_recording_api.Services.Logger;
using field_recording_api.Models.VisitFeeModel;
using static field_recording_api.Models.AccountDetailModel.AllocationsModel;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Writers;
using System.ComponentModel;
using field_recording_api.DataAccess;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System;
using Microsoft.VisualBasic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Runtime.Intrinsics.Arm;
using System.Xml.Linq;
using System.Linq;

namespace field_recording_api.Services.AccountDetail
{
    public class AccountDetailServices: IAccountDetailServices
    {
        private readonly IConfiguration _config;
        private readonly IFileServices _file;
        private readonly IRepository<TbtContactNote> _tbt_contract_note;
        private readonly IRepository<TbtFetchAlloc> _tbt_fetch_alloc;
        private readonly IRepository<TLog> _tLog;
        private readonly IRepository<TbmUser> _tbm_user;
        private readonly IRepository<TbtContractTermination> _tbt_contract_terminations;
        private readonly ILoggerServices _logService;
        //private readonly IUnitOfWorkDB _uow;
        private readonly IRepository<TbmExceptUsers> _tbm_except_users;


        private string token_addLocationTracking = string.Empty;
        private string connectionString = string.Empty;

        public AccountDetailServices(
            IConfiguration config,
            IFileServices file,
            IRepository<TbtContactNote> tbt_contract_note,
            IRepository<TbtFetchAlloc> tbt_fetch_alloc,
            IRepository<TLog> tLog,
            IRepository<TbmUser> tbm_user,
            IRepository<TbtContractTermination> tbt_contract_terminations,
            ILoggerServices logService,
            ILoggerFactory loggerFactory,
            //IUnitOfWorkDB uow
            IRepository<TbmExceptUsers> tbm_except_users
            ) {
            _config = config;
            _file = file;
            _tbt_contract_note = tbt_contract_note;
            _tbt_fetch_alloc = tbt_fetch_alloc;
            _tbm_user = tbm_user;
            _tLog = tLog;
            _tbt_contract_terminations = tbt_contract_terminations;
            _logService = logService;
            //_uow = uow;
            _tbm_except_users = tbm_except_users;

            this.token_addLocationTracking = _config.GetSection("LocationTracking:token").Value;
            this.connectionString = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;


        }

        public async Task<ResponseContext> syncDataFile(CollecctionModel Req)
        {
            string jsonText = string.Empty;
            string urlGetJson =string.Empty;
            var _resview = new ResponseContext();
            try
            {                
                _logService.Info("Service syncDataFile: Start");
                var fetchAllocData = new TbtFetchAlloc();
                fetchAllocData.UserName = Req.user_name;
                fetchAllocData.SyncDate = DateTime.Now;
                var resultInsert = await _tbt_fetch_alloc.AddAsync(fetchAllocData).ConfigureAwait(false);
                if (resultInsert == null || resultInsert.Id == 0) {
                    _resview.statusCode = "201";
                    _resview.message = "Cannot Insert Contract Note";
                    await _logService.dblog("201", Req, _resview, fetchAllocData);
                    return _resview;
                }


                var fetchAllExceptUsers = new TbmExceptUsers();

                var resultAllExceptUsers = await _tbm_except_users.ListAllAsync().ConfigureAwait(false);

                bool Is_skip = false;
                if (resultAllExceptUsers.Count != 0) {
                    var skipUser = resultAllExceptUsers.FirstOrDefault(z => z.USER_ID == Req.user_name.ToUpper());

                    if (skipUser != null)
                    {
                        Is_skip = true;
                    }
                }



                if (!string.IsNullOrEmpty(Req.user_name))
                    Req.user_name = Req.user_name.ToUpper();

                var ORG_CODE = _config.GetSection("ICS:ORG_CODE").Value;
                var DID = _config.GetSection("ICS:DID").Value;
                var url = _config.GetSection("ApiSetting:IC5Doc").Value;
                var Endpoint = string.Format("{0}?ORG_CODE={1}&DID={2}&USER_ID={3}", url, ORG_CODE, DID, Req.user_name);
                urlGetJson = Endpoint;

                if (!Is_skip)
                {
                    var apiResponse = await CallApi.get(Endpoint, new HttpOption() { timeouts = _config.GetSection("ApiSetting:timeouts").Value });

                    if (apiResponse.data.ToString().Contains("\"result\":\"success\""))
                    {
                        jsonText = apiResponse.data.ToString();
                        AllocationModel bb = Newtonsoft.Json.JsonConvert.DeserializeObject<AllocationModel>(jsonText);


                        SaveJsonToDB(apiResponse.data.ToString(), Req.user_name);
                    }
                    else
                        throw new Exception(apiResponse.data.ToString());

                }
                else {
                    string jsonFixed = " { \"result\":\"success\", \"errorcode\":\"null\",\"message\":\"null\",\"allocations\": [] }";
                    SaveJsonToDB(jsonFixed, Req.user_name);

                }



            
                fetchAllocData.FileName =  string.Format("{0}.json", Req.user_name);

                fetchAllocData.ActiveFlag = "N";
                fetchAllocData.SyncDate = DateTime.Now;
                await _tbt_fetch_alloc.UpdateAsync(fetchAllocData).ConfigureAwait(false);
                //var allocations = resultObj.allocations as List<dynamic>;
                //if (allocations != null && allocations.Count == 0)
                //{
                //    _resview.data = resultObj;
                //    throw new Exception("Data allocations has 0");
                //}

                //var result = await setCollection(allocations,Req.user_name, "syncDataFile");
                var result = await setCollection2(null, Req.user_name, "syncDataFile");
                if (result.statusCode != "200")
                    throw new Exception(result.message);

                _resview.data = result.data;
                _logService.Info("Service syncDataFile: End");
            }
            catch (Exception ex)
            {   
                _resview.statusCode = "201";
                _resview.message = string.Format("syncDataFile Error => {0}", ex.Message);
                _logService.Info(string.Format("Service syncDataFile Error: {0}", ex.Message));

                _logService.Info(urlGetJson);
               await _logService.dblog("201", Req, _resview);
            }

            return _resview;
        }

        private void SaveJsonToDB(string? jsonText,string userName)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;

            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();
                using (SqlTransaction sqlTransaction = con.BeginTransaction())
                {
                    try 
                    {
                        jsonText = jsonText.Replace("'","");
                        string sql = $@"UPDATE [FieldRecording].[dbo].[T_Contract_Sync_Json] 
                                        SET [Json]='{jsonText}' 
                                            ,[Update_Date] = GETDATE()
                                            , [Update_By] = '{userName}'
                                        WHERE [user_name]='{userName}';
                                    IF @@ROWCOUNT = 0
                                        INSERT INTO [FieldRecording].[dbo].[T_Contract_Sync_Json] 
                                         ([Json]
                                        ,[user_name]
                                        ,[Create_Date]
                                        ,[Create_By]
                                        ,[Update_Date]
                                        ,[Update_By]) 
                                        VALUES ( '{jsonText}'
                                                ,'{userName}'
                                                ,GETDATE()
                                                ,'{userName}'
                                                ,GETDATE()
                                                ,'{userName}')";
                        SqlCommand cmd1 = new SqlCommand(sql, sqlTransaction.Connection);

                        cmd1.Transaction = sqlTransaction;
                        cmd1.ExecuteNonQuery();

                        sqlTransaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                    }
                }
            }
        }

        private string GetJsonFromDB(string userName)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand($"SELECT TOP 1 JSON FROM [FieldRecording].[dbo].[T_Contract_Sync_Json] WHERE [user_name] = '{userName}'", con);

                var dataReader = cmd1.ExecuteReader();
                if (dataReader.HasRows)
                    dataTable.Load(dataReader);
                else
                    return "";

            }
            return dataTable.Rows[0]["Json"].ToString() ?? "";
        }

        public async Task<ResponseContext> getCollections(CollecctionModel Req)
        {
            var _resview = new ResponseContext();
            try
            {
                _logService.Info(string.Format("Service getCollections: Start {0}", Req.user_name));

               // var result = await setCollection2(allocations , Req.user_name , "getCollections");
                var result = await setCollection2(null, Req.user_name, "getCollections");
                if (result.statusCode != "200")
                    throw new Exception(result.message);

                _resview.data = result.data;

              



                _logService.Info(string.Format("Service getCollections: End {0}", Req.user_name));
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("getCollections Error => {0}", ex.Message);
                _logService.Info(string.Format("Service getCollections Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }

            return _resview;
        }



        public async Task<ResponseContext> getCollectionsNew(CollecctionModel Req)
        {
            var _resview = new ResponseContext();
            try
            {
                _logService.Info(string.Format("Service getCollections: Start {0}", Req.user_name));

               
                var result = await getCollectionsFromT_Contract_Sync(Req.user_name);
                if (result.statusCode != "200")
                    throw new Exception(result.message);

                _resview.data = result.data;


                _logService.Info(string.Format("Service getCollections: End {0}", Req.user_name));
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("getCollections Error => {0}", ex.Message);
                _logService.Info(string.Format("Service getCollections Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }

            return _resview;
        }

        private async Task<ResponseContext> getCollectionsFromT_Contract_Sync(string userName) 
        {
            var _resview = new ResponseContext();
            try
            {
                //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
                string _connectionStr = this.connectionString;
                var dataTable = new DataTable();
                using (SqlConnection con = new SqlConnection(_connectionStr))
                {
                    con.Open();

                    string sql = $@"SELECT     [applicationid]
                                              , [NM]
                                              , [OSAMT]
                                              , [ODAMT]
                                              , [ADDR]
                                              , [IsActive]
                                  FROM [FieldRecording].[dbo].[T_Contract_Sync]  ";
                    
                    if (!string.IsNullOrEmpty(userName)) {
                        sql += $@"  WHERE [FileName] = '{userName}'  ";

                        sql += $@"  Order by [IsActive]  ";
                    }
                    SqlCommand cmd1 = new SqlCommand(sql, con);

                    var dataReader = cmd1.ExecuteReader();

                    dataTable.Load(dataReader);

                }
                IList returnList = new List<TContractSyncModel>();
                returnList = dataTable.AsEnumerable().Select(row => new
                {
                    applicationid = row.Field<string>("applicationid") ,
                    NM = row.Field<string>("NM"),
                    OSAMT = row.Field<string>("OSAMT"),
                    ODAMT = row.Field<string>("ODAMT"),
                    ADDR = row.Field<string>("ADDR"),
                    IsActive = row.Field<string>("IsActive"),

                }).ToList();

                _resview.data = returnList;



            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("getCollectionsFromT_Contract_Sync Error: {0}", ex.Message);
                _logService.Info(string.Format("Service getCollectionsFromT_Contract_Sync Error: {0}", ex.Message));
                await _logService.dblog("201", userName, _resview);
            }
            return _resview;
        }

        private IList searchCollectionsWithcondition(SearchCollectionsModel Req) 
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();

                string sql = $@"SELECT    Top 10 [applicationid]
                                              , [NM]
                                              , [OSAMT]
                                              , [ODAMT]
                                              , [ADDR]
                                              , [IsActive]
                                  FROM [FieldRecording].[dbo].[T_Contract_Sync]  ";

                if (!string.IsNullOrEmpty(Req.searchText))
                {
                    sql += $@"  WHERE [applicationid] LIKE '%{Req.searchText}%'  OR [NM] LIKE '%{Req.searchText}%'  ";

                    sql += $@"  Order by [IsActive]  ";
                }
                SqlCommand cmd1 = new SqlCommand(sql, con);

                var dataReader = cmd1.ExecuteReader();

                dataTable.Load(dataReader);

            }
            IList returnList = new List<TContractSyncModel>();
            returnList = dataTable.AsEnumerable().Select(row => new
            {
                applicationid = row.Field<string>("applicationid"),
                NM = row.Field<string>("NM"),
                OSAMT = row.Field<string>("OSAMT"),
                ODAMT = row.Field<string>("ODAMT"),
                ADDR = row.Field<string>("ADDR"),
                IsActive = row.Field<string>("IsActive"),

            }).ToList();
            return returnList;
        }

        public async Task<ResponseContext> searchCollectionsNew(SearchCollectionsModel Req) 
        {
            var _resview = new ResponseContext();
            try
            {
                if (String.IsNullOrEmpty(Req.searchText))
                {

                    var tmp = getCollectionsNew(new CollecctionModel() { user_name = Req.user_name });
                    _resview = tmp.Result;

                }
                else
                {

                    var result = searchCollectionsWithcondition(Req);

                    _resview.data = result;
                }

            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("searchCollections Error => {0}", ex.Message);
                _logService.Info(string.Format("Service searchCollections Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }

            return _resview;
        }

        public async Task<ResponseContext> searchCollections(SearchCollectionsModel Req)
        {
            var _resview = new ResponseContext();
            try
            {
                DataTable dt = GetJsonDataFromDB(Req.user_name);

                if (String.IsNullOrEmpty(Req.searchText))
                {

                    var result = await setCollection2(null, Req.user_name, "getCollections");
                    if (result.statusCode != "200")
                        throw new Exception(result.message);

                    _resview.data = result.data;

                }
                else
                {
                    DataTable userName = GetUserNameFromSearchText(Req.searchText);

                    if (userName.Rows.Count > 0)
                    {
                        string query = string.Empty;
                        foreach (DataRow row in userName.Rows) {
                            query +=  $" user_name = '" + row["FileName"].ToString() + $"'" ;
                            query += " OR ";
                        }

                        if (query.EndsWith(" OR "))
                        {
                            query = query.Substring(0, query.Length -3);
                        }

                        dt = dt.Select(query).CopyToDataTable();
                    }

                    

                    List<dynamic>? resultListActive = new List<dynamic>();
                    foreach (DataRow row in dt.Rows)
                    {
                        var result = await setCollection2(null, row["user_name"].ToString(), "getCollections");

                        if (result.statusCode == "201") {
                            continue;
                        }

                        var a = new List<dynamic>();

                        a = (List<dynamic>)result.data;

                        var b = a.Where(z => (z.applicationid.Contains(Req.searchText)) ||
                                            (z.NM.Contains(Req.searchText)));

                        foreach (var item in b)
                        {
                            resultListActive.Add(item);
                        }
                    }
                    _resview.data = resultListActive;
                }

            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("searchCollections Error => {0}", ex.Message);
                _logService.Info(string.Format("Service searchCollections Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }

            return _resview;
        }

        private DataTable GetUserNameFromSearchText(string searchText)
        {
            DataTable result = new DataTable();
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand($"SELECT  *  FROM [FieldRecording].[dbo].[T_Contract_Sync] WHERE [applicationid] LIKE '%{searchText}%' OR [NM]  LIKE '%{searchText}%' ", con);

                var dataReader = cmd1.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataTable.Load(dataReader);
                    result = dataTable;
                }

            }
            return result;
        }

        private DataTable GetJsonDataFromDB(string user_name)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();
                //SqlCommand cmd1 = new SqlCommand($"SELECT *  FROM [FieldRecording].[dbo].[T_Contract_Sync_Json] WHERE [user_name] != '{user_name}'", con);
                SqlCommand cmd1 = new SqlCommand($"SELECT *  FROM [FieldRecording].[dbo].[T_Contract_Sync_Json] WHERE [Json] Like '%\"result\":\"success\"%'", con);

                var dataReader = cmd1.ExecuteReader();

                dataTable.Load(dataReader);

            }
            return dataTable;
        }


        public async Task<ResponseContext> getAccountDetailDataNew(AccountDetailModel Req)
        {
            var _resview = new ResponseContext();
            try
            {
                _logService.Info("Service getAccountDetailData: Start");



                string userName = GetUserNameFrom_T_Contract_Sync(Req.contract_no);

                userName = String.IsNullOrEmpty(userName) ? Req.user_name.ToString() : userName;


                var contact_note = GetDataFrom_tbt_contact_note_allocated(Req.contract_no);

                var data = GetDataFrom_tbt_agreement(Req.contract_no);

                var address = GetDataFrom_tbt_address(Req.contract_no);

                var invoice = GetDataFrom_tbt_invoice_details(Req.contract_no);




                VisitFeeModel visitFeeModel = new VisitFeeModel();
                List<Allocations> allocations = new List<Allocations>();


                AllocationsFields allocationsFields = new AllocationsFields();
                // Tab Customer-detail
                allocationsFields.LANO = data[0].LANO; // Contract No
                allocationsFields.LCNO = data[0].LCNO; //Contract ID
                allocationsFields.FN = data[0].FN; //First Name
                allocationsFields.LN = data[0].LN; //Last Name
                allocationsFields.GEN = data[0].GEN; //Gender
                allocationsFields.CMS = data[0].CMS;  //Marital Status
                allocationsFields.OCC = data[0].OCC; //Occupation
                allocationsFields.ADDR = data[0].ADDR;  //Address
                allocationsFields.PHONE1 = data[0].PHONE1;
                allocationsFields.PHONE2 = data[0].PHONE2;
                allocationsFields.PHONE3 = data[0].PHONE3;
                allocationsFields.PHONE4 = data[0].PHONE4;

                // Tab Accoout-summary
                allocationsFields.OSAMT = data[0].OSAMT; //OS Amount
                allocationsFields.ODAMT = data[0].ODAMT; //OD Amount
                allocationsFields.LNAMT = data[0].LNAMT; // Principle Outstanding Amount
                allocationsFields.ODPERIOD = data[0].ODPERIOD; //Overdue Period
                allocationsFields.PDC = data[0].PDC; // Product Code
                allocationsFields.PF = data[0].PF; // Portforio
                allocationsFields.DD = data[0].DD;  // Aging DP
                allocationsFields.BC = data[0].BC; // Bucket
                allocationsFields.CTRUNK = data[0].CTRUNK; // C TRUNK
                allocationsFields.LATLONG = data[0].LATLONG; // LAT LONG
                allocationsFields.DS = data[0].DS; // Bucket History
                allocationsFields.CYDAY = data[0].CYDAY; // Cycle Day
                allocationsFields.PDESC = data[0].PDESC; // Product Type
                allocationsFields.LICNUM = data[0].LICNUM; // License Number
                allocationsFields.CARPROV = data[0].CARPROV; // Car Province
                allocationsFields.ENGNUM = data[0].ENGNUM; // Engine Number
                allocationsFields.CHASISNUM = data[0].CHASISNUM;  // Chassis Number
                allocationsFields.BRAND = data[0].BRAND; // Brand
                allocationsFields.MODEL = data[0].MODEL; // Mobile Name
                allocationsFields.APPCOL = data[0].APPCOL; // App color
                allocationsFields.LNAMT = data[0].LNAMT; // Finance Amount

                allocationsFields.INSTALLMENT_AMOUNT = data[0].INSTALLMENT_AMOUNT;
                allocationsFields.FVISITFEE = data[0].FVISITFEE;
                allocationsFields.FMINDUEVISITFEE = data[0].FMINDUEVISITFEE;
                allocationsFields.FMINODAMOUNT = data[0].FMINODAMOUNT;
                allocationsFields.SZLATESTVISITFEETYPE = data[0].SZLATESTVISITFEETYPE;
                allocationsFields.TPENALCHRG = data[0].TPENALCHRG;
                allocationsFields.CANCELFEE = data[0].CANCELFEE;
                allocationsFields.DISDATE = data[0].DISDATE;
                allocationsFields.LSTAMTPAID = data[0].LSTAMTPAID;
                allocationsFields.LSTPMTDT = data[0].LSTPMTDT;
                allocationsFields.INSTDT = data[0].INSTDT;

                //Tab Analyze
                allocationsFields.ITYPE = data[0].ITYPE;
                allocationsFields.IGROUPCUSTOMER = data[0].IGROUPCUSTOMER;
                allocationsFields.SZRULE1 = data[0].SZRULE1;
                allocationsFields.SZRULE2 = data[0].SZRULE2;
                allocationsFields.SZRULE3 = data[0].SZRULE3;
                allocationsFields.SZRULE4 = data[0].SZRULE4;
                allocationsFields.SZRULE5 = data[0].SZRULE5;
                allocationsFields.SZRULE6 = data[0].SZRULE6;
                allocationsFields.SZRULE7 = data[0].SZRULE7;
                allocationsFields.SZRULE8 = data[0].SZRULE8;
                allocationsFields.SZRULE9 = data[0].SZRULE9;
                allocationsFields.SZRULE10 = data[0].SZRULE10;












                List<AllocationsGrid> list_allocationsGrid = new List<AllocationsGrid>();
                AllocationsGrid allocationsGrid = new AllocationsGrid();

                


                



                //Tab billing-invoice-detail
                //DTTR :  Transaction Date
                //MD : Mode
                //AMT : Amount
                // DTCH : Date
                //RMK : Remark

                foreach (var item in address) 
                {
                    allocationsGrid = new AllocationsGrid();
                    fieldsIn_grids _fieldsIn_grids = new fieldsIn_grids();

                    //TabAddressDetail
                    _fieldsIn_grids.TYP = item.TYP;
                    _fieldsIn_grids.AD1 = item.AD1;
                    _fieldsIn_grids.AD2 = item.AD2;
                    _fieldsIn_grids.CT = item.CT;

                    allocationsGrid.fields = _fieldsIn_grids;
                    allocationsGrid.id = "addrgrid";
                    allocationsGrid.tbseq = "-1";
                    allocationsGrid.stbseq = "-1";

                    list_allocationsGrid.Add(allocationsGrid);

                }

                foreach (var item in address.Where(z => z.TYP == "CO")) // only CO
                {
                    allocationsGrid = new AllocationsGrid();
                    fieldsIn_grids _fieldsIn_grids = new fieldsIn_grids();

                    //Tab collection-address-detail
                    _fieldsIn_grids.TYP = item.TYP;
                    _fieldsIn_grids.AD1 = item.AD1;
                    _fieldsIn_grids.AD2 = item.AD2;
                    _fieldsIn_grids.CT = item.CT;

                    allocationsGrid.fields = _fieldsIn_grids;
                    allocationsGrid.id = "addresstab1";
                    allocationsGrid.tbseq = "-1";
                    allocationsGrid.stbseq = "0";

                    list_allocationsGrid.Add(allocationsGrid);

                }

                if (address.Where(z => z.TYP == "CO").Count() == 0) {
                    allocationsGrid = new AllocationsGrid();
                    fieldsIn_grids _fieldsIn_grids = new fieldsIn_grids();

                    //Tab collection-address-detail
                    _fieldsIn_grids.TYP = "CO";
                    _fieldsIn_grids.AD1 = "-";
                    _fieldsIn_grids.AD2 = "-";
                    _fieldsIn_grids.CT = "-";

                    allocationsGrid.fields = _fieldsIn_grids;
                    allocationsGrid.id = "addresstab1";
                    allocationsGrid.tbseq = "-1";
                    allocationsGrid.stbseq = "0";

                    list_allocationsGrid.Add(allocationsGrid);
                }

                foreach (var item in contact_note)
                {
                    allocationsGrid = new AllocationsGrid();
                    fieldsIn_grids _fieldsIn_grids = new fieldsIn_grids();

                    //Tab follow-up-history
                    // DTA  : Action Date
                    //RCD : Result
                    //NA :  Next Action
                    //DNA : Next Action Date
                    // RMK : Remark

                    //_fieldsIn_grids.AC = item.Action;
                    //_fieldsIn_grids.DTA = item.NextActionDate?.ToString();
                    //_fieldsIn_grids.RCD = item.Result;
                    //_fieldsIn_grids.NA = item.NextAction;
                    //_fieldsIn_grids.DNA = item.NextActionDate?.ToString();
                    //_fieldsIn_grids.RMK = item.Remarks;
                    //_fieldsIn_grids.PROMISEDATE = "";
                    //_fieldsIn_grids.PROMISEAMOUNT = "";
                    //_fieldsIn_grids.POC = "";
                    //_fieldsIn_grids.MODC = "";
                    //_fieldsIn_grids.ICONTACTNO = "";

                    _fieldsIn_grids.AC = item.AC;
                    _fieldsIn_grids.DTA = item.DTA;
                    _fieldsIn_grids.RCD = item.RCD;
                    _fieldsIn_grids.NA = item.NA;
                    _fieldsIn_grids.DNA = item.DNA;
                    _fieldsIn_grids.RMK = item.RMK;
                    _fieldsIn_grids.PROMISEDATE = item.PROMISEDATE;
                    _fieldsIn_grids.PROMISEAMOUNT = item.PROMISEAMOUNT;
                    _fieldsIn_grids.POC = item.POC;
                    _fieldsIn_grids.MODC = item.MODC;
                    _fieldsIn_grids.ICONTACTNO = item.ICONTACTNO;


                    allocationsGrid.fields = _fieldsIn_grids;
                    allocationsGrid.id = "followupgrid";
                    allocationsGrid.tbseq = "-1";
                    allocationsGrid.stbseq = "-1";

                    list_allocationsGrid.Add(allocationsGrid);
                }

                //Tab billing-invoice-detail 
                foreach (var item in invoice) {
                    // pmtgrid
                    allocationsGrid = new AllocationsGrid();
                    fieldsIn_grids _fieldsIn_grids = new fieldsIn_grids();

                    _fieldsIn_grids.DTTR = item.DTTR;
                    _fieldsIn_grids.MD = item.MD;
                    _fieldsIn_grids.AMT = item.AMT;
                    _fieldsIn_grids.DTCH = item.DTCH;
                    _fieldsIn_grids.RMK_INVOICE = item.RMK_INVOICE;

                    allocationsGrid.fields = _fieldsIn_grids;
                    allocationsGrid.id = "pmtgrid";
                    allocationsGrid.tbseq = "-1";
                    allocationsGrid.stbseq = "-1";

                    list_allocationsGrid.Add(allocationsGrid);

                }






                AllocationsData allocationsData = new AllocationsData();
                allocationsData.fields = allocationsFields;
                allocationsData.grids = list_allocationsGrid;


                Allocations allocations1 = new Allocations();
                allocations1.applicationid = Req.contract_no; // Contract No
                allocations1.fullsync = "";
                allocations1.applicantid = Req.contract_no; // Contract No
                allocations1.screenid = "";
                allocations1.data = allocationsData;
                allocations.Add(allocations1);


                visitFeeModel.message = "";
                visitFeeModel.errorcode = "";
                visitFeeModel.result = "";
                visitFeeModel.allocations = allocations;





              
                _resview.data = allocations1;
                


               


                _logService.Info("Service getAccountDetailData: End");
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("getAccountDetailData Error => {0}", ex.Message);
                _logService.Info(string.Format("Service getAccountDetailData Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }

            return _resview;
        }

        private List<tbt_invoice_detailsModel> GetDataFrom_tbt_invoice_details(string contract_no)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();

                string sql = $@"SELECT    *
                                    FROM [FieldRecording].[dbo].[tbt_invoice_details]  ";

                if (1 == 1)
                {
                    sql += $@"  WHERE [LANO] = '{contract_no}'   ";
                }
                SqlCommand cmd1 = new SqlCommand(sql, con);

                var dataReader = cmd1.ExecuteReader();

                dataTable.Load(dataReader);

            }
            var returnList = new List<tbt_invoice_detailsModel>();


            //returnList = dataTable.AsEnumerable().Select(row => new
            foreach (DataRow row in dataTable.Rows)
            {
                tbt_invoice_detailsModel t = new tbt_invoice_detailsModel();
                t.TRSQ = row.Field<string>("TRSQ");
                t.DTTR = row.Field<string>("DTTR");
                t.TRCD = row.Field<string>("TRCD");
                t.MD = row.Field<string>("MD");
                t.AMT = row.Field<string>("AMT");
                t.DTCH = row.Field<string>("DTCH");
                t.CHN = row.Field<string>("CHN");
                t.RMK_INVOICE = row.Field<string>("RMK");
                t.LANO = row.Field<string>("LANO");

                returnList.Add(t);
            };

            return returnList;
        }

        private List<tbt_addressModel> GetDataFrom_tbt_address(string contract_no)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();

                string sql = $@"SELECT    *
                                    FROM [FieldRecording].[dbo].[tbt_address]  ";

                if (1 == 1)
                {
                    sql += $@"  WHERE [LANO] = '{contract_no}'   ";
                }
                SqlCommand cmd1 = new SqlCommand(sql, con);

                var dataReader = cmd1.ExecuteReader();

                dataTable.Load(dataReader);

            }
            var returnList = new List<tbt_addressModel>();
            

            //returnList = dataTable.AsEnumerable().Select(row => new
            foreach (DataRow row in dataTable.Rows)
            {
                tbt_addressModel t = new tbt_addressModel();
                t.ID = row.Field<long>("ID");
                t.TYP = row.Field<string?>("TYP");
                t.AD1 = row.Field<string?>("AD1");
                t.AD2 = row.Field<string?>("AD2");
                t.LM = row.Field<string?>("LM");
                t.ST = row.Field<string?>("ST");
                t.CT = row.Field<string?>("CT");
                t.AR = row.Field<string?>("AR");
                t.ZP = row.Field<string?>("ZP");
                t.LANO = row.Field<string?>("LANO");
                //t.Sync_Date = row.Field<DateTime?>("Sync_Date");
                returnList.Add(t);
            };
            
            return returnList;
        }
        private List<tbt_agreementModel> GetDataFrom_tbt_agreement(string contract_no)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();

                string sql = $@"SELECT    *
                                    FROM [FieldRecording].[dbo].[tbt_agreement]  ";

                if (1 == 1)
                {
                    sql += $@"  WHERE [LANO] = '{contract_no}'   ";
                }
                SqlCommand cmd1 = new SqlCommand(sql, con);

                var dataReader = cmd1.ExecuteReader();

                dataTable.Load(dataReader);

            }
            var returnList = new List<tbt_agreementModel>();
            tbt_agreementModel t = new tbt_agreementModel ();

            //returnList = dataTable.AsEnumerable().Select(row => new
            foreach (DataRow row in dataTable.Rows)
            {
                t.ID = row.Field<long>("ID");
                t.ORG = row.Field<string?>("ORG");
                t.ANO = row.Field<string?>("ANO");
                t.LCNO = row.Field<string?>("LCNO");
                t.ASEQ = row.Field<string?>("ASEQ");
                t.LANO = row.Field<string?>("LANO");
                t.ODDAYS = row.Field<string?>("ODDAYS");
                t.BC = row.Field<string?>("BC");
                t.ODAMT = row.Field<string?>("ODAMT");
                t.OSAMT = row.Field<string?>("OSAMT");
                t.ADNO = row.Field<string?>("ADNO");
                t.ACST = row.Field<string?>("ACST");
                t.AON = row.Field<string?>("AON");
                t.DE = row.Field<string?>("DE");
                t.DD = row.Field<string?>("DD");
                t.PRIORITY = row.Field<string?>("PRIORITY");
                t.REM = row.Field<string?>("REM");
                t.FN = row.Field<string?>("FN");
                t.MN = row.Field<string?>("MN");
                t.LN = row.Field<string?>("LN");
                t.AN = row.Field<string?>("AN");
                t.GEN = row.Field<string?>("GEN");
                t.DOB = row.Field<string?>("DOB");
                t.CMS = row.Field<string?>("CMS");
                t.OCC = row.Field<string?>("OCC");
                t.NM = row.Field<string?>("NM");
                t.LNAMT = row.Field<string?>("LNAMT");
                t.EMI = row.Field<string?>("EMI");
                t.MOA = row.Field<string?>("MOA");
                t.CRSK = row.Field<string?>("CRSK");
                t.WFC = row.Field<string?>("WFC");
                t.ADDR = row.Field<string?>("ADDR");
                t.LICNUM = row.Field<string?>("LICNUM");
                t.CARPROV = row.Field<string?>("CARPROV");
                t.ENGNUM = row.Field<string?>("ENGNUM");
                t.CHASISNUM = row.Field<string?>("CHASISNUM");
                t.BRAND = row.Field<string?>("BRAND");
                t.MODEL = row.Field<string?>("MODEL");
                t.APPCOL = row.Field<string?>("APPCOL");
                t.PHONE1 = row.Field<string?>("PHONE1");
                t.PHONE2 = row.Field<string?>("PHONE2");
                t.PHONE3 = row.Field<string?>("PHONE3");
                t.PHONE4 = row.Field<string?>("PHONE4");
                t.EMAIL = row.Field<string?>("EMAIL");
                t.PAN = row.Field<string?>("PAN");
                t.POSAMT = row.Field<string?>("POSAMT");
                t.PDC = row.Field<string?>("PDC");
                t.PF = row.Field<string?>("PF");
                t.DS = row.Field<string?>("DS");
                t.CYDAY = row.Field<string?>("CYDAY");
                t.PDESC = row.Field<string?>("PDESC");
                t.DISDATE = row.Field<string?>("DISDATE");
                t.LSTPMTDT = row.Field<string?>("LSTPMTDT");
                t.LSTAMTPAID = row.Field<string?>("LSTAMTPAID");
                t.INSTDT = row.Field<string?>("INSTDT");
                t.TPENALCHRG = row.Field<string?>("TPENALCHRG");
                t.CANCELFEE = row.Field<string?>("CANCELFEE");
                t.ODPERIOD = row.Field<string?>("ODPERIOD");
                t.CTRUNK = row.Field<string?>("CTRUNK");
                t.LATLONG = row.Field<string?>("LATLONG");
                t.CNO = row.Field<string?>("CNO");
                t.Sync_Date = row.Field<DateTime?>("Sync_Date");

                t.SZCONTACTADDTYPE = row.Field<string?>("SZCONTACTADDTYPE");
                t.OCCID = row.Field<string?>("OCCID");
                t.INSTALLMENT_AMOUNT = row.Field<string?>("INSTALLMENT_AMOUNT");

                t.ITYPE = row.Field<string?>("ITYPE");
                t.IGROUPCUSTOMER = row.Field<string?>("IGROUPCUSTOMER");
                t.SZRULE1 = row.Field<string?>("SZRULE1");
                t.SZRULE2 = row.Field<string?>("SZRULE2");
                t.SZRULE3 = row.Field<string?>("SZRULE3");
                t.SZRULE4 = row.Field<string?>("SZRULE4");
                t.SZRULE5 = row.Field<string?>("SZRULE5");
                t.SZRULE6 = row.Field<string?>("SZRULE6");
                t.SZRULE7 = row.Field<string?>("SZRULE7");
                t.SZRULE8 = row.Field<string?>("SZRULE8");
                t.SZRULE9 = row.Field<string?>("SZRULE9");
                t.SZRULE10 = row.Field<string?>("SZRULE10");

                t.FVISITFEE = row.Field<string?>("FVISITFEE");
                t.FMINDUEVISITFEE = row.Field<string?>("FMINDUEVISITFEE");
                t.FMINODAMOUNT = row.Field<string?>("FMINODAMOUNT");
                t.SZLATESTVISITFEETYPE = row.Field<string?>("SZLATESTVISITFEETYPE");

            };
            returnList.Add(t);
            return returnList;
        }

        private List<tbt_contact_note_allocatedModel> GetDataFrom_tbt_contact_note_allocated(string contract_no)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();

                string sql = $@"SELECT    [AC]
                                          ,CONVERT(nvarchar(10) ,DTA,103)  as [DTA]
                                          ,[RCD]
                                          ,[NA]
                                          ,CONVERT(nvarchar(10) ,DNA,103)  as [DNA]
                                          ,[RMK]
                                          ,[PROMISEDATE]
                                          ,[PROMISEAMOUNT]
                                          ,[POC]
                                          ,[MODC]
                                          ,[ICONTACTNO] 
                                FROM ( ";
                sql += $@" SELECT          [AC]
                                          ,[DTA]
                                          ,[RCD]
                                          ,[NA]
                                          ,[DNA]
                                          ,[RMK]
                                          ,[PROMISEDATE]
                                          ,[PROMISEAMOUNT]
                                          ,[POC]
                                          ,[MODC]
                                          ,[ICONTACTNO]

                                FROM [FieldRecording].[dbo].[tbt_contact_note_allocated] 
                                WHERE [ICONTACTNO] = '{contract_no}'
                                UNION 
                             SELECT  
			                             [Action] AS AC
			                            ,convert(nvarchar(10),Create_Date,103) AS DTA 
			                            ,Result AS RCD 
			                            ,NextAction AS NA
			                            ,convert(nvarchar(10),NextActionDate,103) AS DNA
			                            ,Remarks AS RMK 
			                            ,NULL AS PROMISEDATE
			                            ,NULL AS PROMISEAMOUNT
			                            ,NULL AS POC 
			                            ,NULL AS MODC 
			                            ,contract_no AS ICONTACTNO 
                            FROM [FieldRecording].[dbo].[tbt_contact_note]
                            WHERE [contract_no] = '{contract_no}'
                         ";
                sql += $@" ) AS X ";

                if (1 == 1)
                {
                    sql += $@"  order by CONVERT(datetime ,DTA,103)  DESC ";
                }
                SqlCommand cmd1 = new SqlCommand(sql, con);

                var dataReader = cmd1.ExecuteReader();

                dataTable.Load(dataReader);

            }
            var returnList = new List<tbt_contact_note_allocatedModel>();
            tbt_contact_note_allocatedModel t = new tbt_contact_note_allocatedModel();
            //returnList = dataTable.AsEnumerable().Select(row => new
            foreach (DataRow row in dataTable.Rows)
            {
                t = new tbt_contact_note_allocatedModel();
                t.AC = row.Field<string>("AC");
                t.DTA = row.Field<string>("DTA");
                t.RCD = row.Field<string>("RCD");
                t.NA = row.Field<string>("NA");
                t.DNA = row.Field<string>("DNA");
                t.RMK = row.Field<string>("RMK");
                t.PROMISEDATE = row.Field<string>("PROMISEDATE");
                t.PROMISEAMOUNT = row.Field<string>("PROMISEAMOUNT");
                t.POC = row.Field<string>("POC");
                t.MODC = row.Field<string>("MODC");
                t.ICONTACTNO = row.Field<string>("ICONTACTNO");

                returnList.Add(t);

            }

            return returnList;
        }

        private List<tbt_contact_noteModel> GetDataFrom_tbt_contact_note(string contract_no)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();

                string sql = $@"SELECT    DISTINCT [contract_no] 
                                          , [contact_note_id]
                                          ,[Action]
                                          ,[Result]
                                          ,[NextAction]
                                          ,[NextActionDate]
                                          ,[DelinquencyReason]
                                          ,[Remarks]
                                          ,[ModeOfContact]
                                          ,[PlaceOfContact]
                                          ,[PartyContacted]
                                          ,[ContactNumber]
                                          ,[Geo]
                                          ,[SubDistrict]
                                          ,[District]
                                          ,[Province]
                                          ,[ZipCode]
                                          ,[Create_Date]
                                          ,[Create_By]
                                          ,[Update_Date]
                                          ,[Update_By]
                                          ,[IS_Sync]
                                          ,[Sync_Date]
                                          ,[Sync_Error_Message] 
                                    FROM [FieldRecording].[dbo].[tbt_contact_note]  ";

                if (1 == 1)
                {
                    sql += $@"  WHERE [contract_no] = '{contract_no}'   ";
                    sql += $@"  ORDER BY  Create_Date DESC   ";
                }
                SqlCommand cmd1 = new SqlCommand(sql, con);

                var dataReader = cmd1.ExecuteReader();

                dataTable.Load(dataReader);

            }
            var returnList = new List<tbt_contact_noteModel>();
            tbt_contact_noteModel t = new tbt_contact_noteModel();
            //returnList = dataTable.AsEnumerable().Select(row => new
            foreach (DataRow row in dataTable.Rows)
            {
                t = new tbt_contact_noteModel();
                t.contract_no = row.Field<string>("contract_no");
                t.contact_note_id = row.Field<long>("contact_note_id");
                t.Action = row.Field<string>("Action");
                t.Result = row.Field<string>("Result");
                t.NextAction = row.Field<string>("NextAction");
                t.NextActionDate = row.Field<DateTime?>("NextActionDate");
                t.DelinquencyReason = row.Field<string>("DelinquencyReason");
                t.Remarks = row.Field<string>("Remarks");
                t.ModeOfContact = row.Field<string>("ModeOfContact");
                t.PlaceOfContact = row.Field<string>("PlaceOfContact");
                t.PartyContacted = row.Field<string>("PartyContacted");
                t.ContactNumber = row.Field<string>("ContactNumber");
                t.Geo = row.Field<string>("Geo");
                t.SubDistrict = row.Field<string>("SubDistrict");
                t.District = row.Field<string>("District");
                t.Province = row.Field<string>("Province");
                t.ZipCode = row.Field<string>("ZipCode");
                t.Create_Date = row.Field<DateTime?>("Create_Date");
                t.Create_By = row.Field<string>("Create_By");
                t.Update_Date = row.Field<DateTime?>("Update_Date");
                t.Update_By = row.Field<string>("Update_By");
                t.IS_Sync = row.Field<string>("IS_Sync");
                t.Sync_Date = row.Field<DateTime?>("Sync_Date");
                t.Sync_Error_Message = row.Field<string>("Sync_Error_Message");
                returnList.Add(t);

            }
            
            return returnList;
        }

        public async Task<ResponseContext> getAccountDetailData(AccountDetailModel Req)
        {       
            var _resview = new ResponseContext();
            bool isFound= false;
            try
            {
                _logService.Info("Service getAccountDetailData: Start");
              
                StringBuilder jsonString = new StringBuilder();


                string userName = GetUserNameFrom_T_Contract_Sync(Req.contract_no);

                userName  = String.IsNullOrEmpty(userName) ? Req.user_name.ToString() : userName;


                jsonString.Append(GetJsonFromDB(userName));
                var visitFeeData0 = Newtonsoft.Json.JsonConvert.DeserializeObject<VisitFeeModel>(jsonString.ToString());
                var allocationItem0 = visitFeeData0?.allocations.Where(data => data.applicationid == Req.contract_no)?.FirstOrDefault();
                if (allocationItem0 == null)
                {
                    jsonString = new StringBuilder();
                }
                else
                {
                    var getfollowupResult0 = await getFollowUpDB(allocationItem0, Req);
                    if (getfollowupResult0.statusCode != "200")
                    {
                        throw new Exception(getfollowupResult0.message);
                    }
                    _resview.data = getfollowupResult0.data;
                    isFound = true;
                }


                if (isFound == false)
                {

                    jsonString = new StringBuilder();
                    DataTable dt = GetJsonDataFromDB(Req.user_name);
                    foreach (DataRow row in dt.Rows)
                    {
                        jsonString.Append(GetJsonFromDB(row["user_name"].ToString()));

                        var visitFeeData = Newtonsoft.Json.JsonConvert.DeserializeObject<VisitFeeModel>(jsonString.ToString());
                        var allocationItem = visitFeeData.allocations.Where(data => data.applicationid == Req.contract_no).FirstOrDefault();

                        if (allocationItem == null)
                        {
                            jsonString = new StringBuilder();
                            continue;
                        }
                        var getfollowupResult = await getFollowUpDB(allocationItem, Req);


                        if (getfollowupResult.statusCode != "200")
                            throw new Exception(getfollowupResult.message);

                        _resview.data = getfollowupResult.data;
                        break;
                    }
                }

               

               
                _logService.Info("Service getAccountDetailData: End");
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("getAccountDetailData Error => {0}", ex.Message);
                _logService.Info(string.Format("Service getAccountDetailData Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }

            return _resview;
        }

        private string GetUserNameFrom_T_Contract_Sync(string contract_no)
        {
            string result = string.Empty;
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand($"SELECT Top 1 *  FROM [FieldRecording].[dbo].[T_Contract_Sync] WHERE [applicationid] = '{contract_no}' ", con);

                var dataReader = cmd1.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataTable.Load(dataReader);
                    result = dataTable.Rows[0]["FileName"].ToString();
                }
                else
                    result = string.Empty;

            }
            return result;
        }

        public async Task<ResponseContext> saveAccountDetailData(SaveAccountDetailModel Req)
        {
            var _resview = new ResponseContext();
            try
            {
                _logService.Info("Service saveAccountDetailData: Start");

                if (Req.inputFile == null) {
                    throw new Exception("กรุณาอัปโหลดรูปภาพ");
                }

                foreach (IFormFile postedFile in Req.inputFile)
                {
                    if (postedFile.Length == 0) throw new Exception("รูปภาพมีขนาด 0 byte กรุณาอัปโหลดรูปภาพใหม่");
                }


                var Sync_Date = DateTime.Now;
                var resultInsert = await InsContactNote(Req);
                if (resultInsert.statusCode != "200")
                    throw new Exception(resultInsert.message);

                var contract_note_id = ((TbtContactNote)resultInsert.data).ContactNoteId.ToString();
                var productType = "";
                if ((Req.contract_no.Substring(0, 2) == "05"))
                    productType = "HP";
                else if ((Req.contract_no.Substring(0, 2) == "11"))
                    productType = "PL";

                dynamic IC5body = new ExpandoObject();
                IC5body.tokenID = _config.GetSection("ICS:tokenID").Value;
                IC5body.transactionID = String.Format("{0}{1}{2}", productType, Sync_Date.ToString("yyyyMMddHHmmss"), Req.contract_no);
                IC5body.productType = productType;
                IC5body.contractNo = Req.contract_no;
                IC5body.actionCode = Req.action;//"IV";
                IC5body.resultCode = Req.result; //"VPTP";
                IC5body.contactDate = Sync_Date.ToString("yyyy-MM-dd HH:mm:ss");
                IC5body.telephoneNo = (String.IsNullOrEmpty(Req.contact_number)) ? "" : Req.contact_number;
                IC5body.note = (String.IsNullOrEmpty(Req.remark)) ? "" : Req.remark;
                IC5body.CollectorCode = Req.user_name;

                //string json = Newtonsoft.Json.JsonConvert.SerializeObject(IC5body);

                var url = _config.GetSection("ApiSetting:IC5").Value;

                //var url = @"https://localhost:44327/api/VoiceBlaster/Insert";
                //https://cbs.summitcapital.co.th/api/VoiceBlaster/Insert

                /**
                var apiResponse = await CallApi.post(url, IC5body);

                if (apiResponse.statusCode != "200") {
                    await _logService.dblog(_resview.statusCode, IC5body, apiResponse);
                    //return apiResponse;

                    throw new Exception(apiResponse.statusCode + ":" +  apiResponse.message);
                }
                dynamic resultObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(apiResponse.data);

                //var jsonObject = JsonSerializer.Serialize(IC5body);
                //StringContent httpContent = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json");
                //var url = _config.GetSection("ApiSetting:IC5").Value;
                //dynamic resultObj = new ExpandoObject();

                //HttpClientHandler clientHandler = new HttpClientHandler();
                //clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                //using (var httpClient = new HttpClient(clientHandler))
                //{
                //    using (var response = await httpClient.PostAsync(url, httpContent))
                //    {
                //        var apiResponse = await response.Content.ReadAsStringAsync();
                //        resultObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(apiResponse);
                //    }
                //}

                if (!string.IsNullOrEmpty(ValidatesResultIC5(resultObj)))
                {
                    _resview.data = resultObj;
                    var resultUpdfail = await UpdContactNote(contract_note_id, Sync_Date, ValidatesResultIC5(resultObj), "N", Req);
                    throw new Exception((resultUpdfail.statusCode != "200") ? resultUpdfail.message : "IC5 Service Response: " + ValidatesResultIC5(resultObj));
                }
                **/

                update_T_Contract_Sync(Req.contract_no);

                var resultUpdSucc = await UpdContactNote(contract_note_id, Sync_Date, "", "Y", Req);
                if (resultUpdSucc.statusCode != "200")
                    throw new Exception(resultUpdSucc.message);

                var address = new AccountDetaiAddresslModel()
                {
                    district = Req.district,
                    latlong = Req.latlong,
                    province = Req.province,
                    zipCode = Req.zipCode,
                    subDistrict = Req.subDistrict,
                };

                var fileModel = new FileModel()
                {
                    adress = address,
                    contract_no = Req.contract_no,
                    contract_note_id = contract_note_id,
                    inputFile = Req.inputFile,
                    user_name = Req.user_name,
                    fileDate = Sync_Date
                };
                var resultfileUpload = await _file.Upload(fileModel);
                if (resultfileUpload.statusCode != "200")
                    throw new Exception(resultfileUpload.message);

                _logService.Info("Service saveAccountDetailData: End");
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message  = string.Format("saveAccountDetailData Error => {0}", ex.Message);
                _logService.Info(string.Format("Service saveAccountDetailData Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }

            return _resview;
        }

        private void update_T_Contract_Sync(string contract_no)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand($"UPDATE [FieldRecording].[dbo].[T_Contract_Sync] SET [IsActive] = 'Y' WHERE [applicationid] = '{contract_no}' ", con);

                cmd1.ExecuteNonQuery();

            }
        }

        private async Task<ResponseContext> InsContactNote(SaveAccountDetailModel Req) {
            var _resview = new ResponseContext();
            try
            {
                _logService.Info("Service InsContactNote: Start");
                //var userid = (from b in await _uow.SIISRepository<TbmUser>().Get(d => d.UserName == Req.user_name).ConfigureAwait(false) select b.UserId).FirstOrDefault();
                var userid = (from b in await _tbm_user.Get(d => d.UserName == Req.user_name).ConfigureAwait(false) select b.UserId).FirstOrDefault();

                var contractNoteData = new TbtContactNote();
                contractNoteData.ContractNo = Req.contract_no;
                contractNoteData.Action = Req.action;
                contractNoteData.Result = Req.result;
                contractNoteData.Geo = Req.latlong;
                contractNoteData.SubDistrict = Req.subDistrict;
                contractNoteData.District = Req.district;
                contractNoteData.Province = Req.province;
                contractNoteData.ZipCode = Req.zipCode;
                contractNoteData.ContactNumber = String.IsNullOrEmpty(Req.contact_number) ? "" : Req.contact_number;
                contractNoteData.Remarks = String.IsNullOrEmpty(Req.remark) ? "" : Req.remark;
                contractNoteData.CreateBy = userid.ToString();// Req.user_name;
                contractNoteData.CreateDate = DateTime.Now;
                var resultInsert = await _tbt_contract_note.AddAsync(contractNoteData).ConfigureAwait(false);
                if (resultInsert == null || resultInsert.ContactNoteId == 0)
                    throw new Exception("Cannot Insert Contract Note");

                _resview.data = resultInsert;
                _logService.Info("Service InsContactNote: End");
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("InsContactNote Error: {0}", ex.ToString());
                _logService.Info(string.Format("Service InsContactNote Error: {0}", ex.Message));
            }

            return _resview;
        }

        private async Task<ResponseContext> UpdContactNote(string contract_note_id, DateTime? Sync_Date, string Sync_Error_Msg, string Is_sync, SaveAccountDetailModel Req)
        {
            var _resview = new ResponseContext();
            try
            {
                _logService.Info("Service UpdContactNote: Start");
                var userid = (from b in await _tbm_user.Get(d => d.UserName == Req.user_name).ConfigureAwait(false) select b.UserId).FirstOrDefault();

                var contractNoteData = new TbtContactNote();
                contractNoteData.ContactNoteId = Int32.Parse(contract_note_id);
                contractNoteData.ContractNo = Req.contract_no;
                contractNoteData.Action = Req.action;
                contractNoteData.Result = Req.result;
                contractNoteData.Geo = Req.latlong;
                contractNoteData.SyncDate = Sync_Date;
                contractNoteData.SyncErrorMessage = Sync_Error_Msg;
                contractNoteData.IsSync = Is_sync;

                contractNoteData.UpdateBy = userid.ToString();
                contractNoteData.UpdateDate = DateTime.Now;
                await _tbt_contract_note.UpdateAsync(contractNoteData).ConfigureAwait(false);
                _logService.Info("Service UpdContactNote: End");
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("UpdContactNote Error: {0}", ex.Message);
                _logService.Info(string.Format("Service UpdContactNote Error: {0}", ex.Message));
            }

            return _resview;
        }
        private async Task<ResponseContext> setCollection(List<dynamic> Listallocations , string userName , string method)
        {
            var _resview = new ResponseContext();
            try
            {
               

                _logService.Info("Service setCollection: Start");
                var allocationsListString = Listallocations.Select(data => data.applicationid).ToList();
               

                var resultListActive = (from li in Listallocations
                                        join ctn in (from ctn in await _tbt_contract_note.Get(d => allocationsListString.Cast<string>().Contains(d.ContractNo) && d.IsSync == "Y").ConfigureAwait(false)
                                                     select ctn.ContractNo).Distinct() on li.applicationid equals ctn into ctnl
                                        from ContractNo in ctnl.DefaultIfEmpty()
                                        join tct in (from tct in await _tbt_contract_terminations.Get().ConfigureAwait(false)
                                                     select tct.ContractNo).Distinct() on li.applicationid equals tct into tctl
                                        from ctnD in tctl.DefaultIfEmpty()
                                        where
                                         // decimal.Parse(li.data.fields.ODAMT) > 1000 && Int32.Parse(li.data.fields.BC) >= 2 &&
                                         li.applicationid != ctnD
                                        select new { data = li, IsActive = (ContractNo == li.applicationid) ? "Y" : "N" }).Select(data =>
                                        {
                                            dynamic obj = new ExpandoObject();
                                            obj.applicationid = data.data.applicationid;
                                            obj.NM = data.data.data.fields.NM;
                                            obj.OSAMT = data.data.data.fields.OSAMT;
                                            obj.ODAMT = data.data.data.fields.ODAMT;
                                            obj.ADDR = data.data.data.fields.ADDR;
                                            obj.IsActive = data.IsActive;
                                            return obj;
                                        })
                                       .OrderBy(d => d.IsActive)
                                       .ToList();

                _resview.data = resultListActive;


                if (method == "getCollections")
                {
                    DataTable dt =GetSyncDataFromDB(userName);

                    
                    foreach (DataRow row in dt.Rows)
                    {
                        dynamic a = new ExpandoObject();

                        a.Id = null;
                        a.applicationid = row["applicationid"].ToString();
                        a.NM = row["NM"].ToString();
                        a.OSAMT = row["OSAMT"].ToString();
                        a.ODAMT = row["ODAMT"].ToString();
                        a.ADDR = row["ADDR"].ToString();
                        a.IsActive = row["IsActive"].ToString();
                        a.FileName = row["FileName"].ToString();

                        resultListActive.Add(a);
                    }

                   

                }
                else
                {
                    SaveSyncDataToDB(resultListActive, userName);
                }

                _logService.Info("Service setCollection: End");
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("setCollection Error: {0}", ex.Message);
                _logService.Info(string.Format("Service setCollection Error: {0}", ex.Message));
            }

            return _resview;
        }


        private async Task<ResponseContext> setCollection2(List<allocations> Listallocations, string userName, string method)
        {
            var _resview = new ResponseContext();
            try
            {
                string jsonText = GetJsonFromDB(userName);

                if (!jsonText.Contains("\"result\":\"success\""))
                {
                    throw new Exception("Please Re sync data." + jsonText);
                }

                AllocationModel bb = Newtonsoft.Json.JsonConvert.DeserializeObject<AllocationModel>(jsonText);



                _logService.Info("Service setCollection: Start");
                var allocationsListString = bb.allocations.Select(data => data.applicationid).ToList();

                Listallocations = bb.allocations;


                var resultListActive = (from li in Listallocations
                                        join ctn in (from ctn in await _tbt_contract_note.Get(d => allocationsListString.Cast<string>().Contains(d.ContractNo) && d.IsSync == "Y").ConfigureAwait(false)
                                                     select ctn.ContractNo).Distinct() on li.applicationid equals ctn into ctnl
                                        from ContractNo in ctnl.DefaultIfEmpty()
                                        join tct in (from tct in await _tbt_contract_terminations.Get().ConfigureAwait(false)
                                                     select tct.ContractNo).Distinct() on li.applicationid equals tct into tctl
                                        from ctnD in tctl.DefaultIfEmpty()
                                        where
                                         // decimal.Parse(li.data.fields.ODAMT) > 1000 && Int32.Parse(li.data.fields.BC) >= 2 &&
                                         li.applicationid != ctnD
                                        select new { data = li, IsActive = (ContractNo == li.applicationid) ? "Y" : "N" }).Select(data =>
                                        {
                                            dynamic obj = new ExpandoObject();
                                            obj.applicationid = data.data.applicationid;
                                            obj.NM = data.data.data.fields.NM;
                                            obj.OSAMT = data.data.data.fields.OSAMT;
                                            obj.ODAMT = data.data.data.fields.ODAMT;
                                            obj.ADDR = data.data.data.fields.ADDR;
                                            obj.IsActive = data.IsActive;
                                            return obj;
                                        })
                                       .OrderBy(d => d.IsActive)
                                       .ToList();

                _resview.data = resultListActive;


                if (method == "getCollections")
                {
                    //DataTable dt = GetSyncDataFromDB(userName);


                    //foreach (DataRow row in dt.Rows)
                    //{
                    //    dynamic a = new ExpandoObject();

                    //    a.Id = null;
                    //    a.applicationid = row["applicationid"].ToString();
                    //    a.NM = row["NM"].ToString();
                    //    a.OSAMT = row["OSAMT"].ToString();
                    //    a.ODAMT = row["ODAMT"].ToString();
                    //    a.ADDR = row["ADDR"].ToString();
                    //    a.IsActive = row["IsActive"].ToString();
                    //    a.FileName = row["FileName"].ToString();

                    //    resultListActive.Add(a);
                    //}



                }
                else
                {
                    SaveSyncDataToDB(resultListActive, userName);
                }

                _logService.Info("Service setCollection: End");
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";

                if (ex.Message == "There is no row at position 0.")
                {
                    _resview.message = "Data Not Found, Please Sync data";
                }
                else {
                    _resview.message = string.Format("setCollection Error: {0}", ex.Message);
                }

                
                _logService.Info(string.Format("Service setCollection Error: {0}", ex.Message));
            }

            return _resview;
        }

        private  DataTable GetSyncDataFromDB(string userName)
        {
            //MyRepository myRepository = new MyRepository(_config.GetSection("ConnectionStrings:FieldRecordingConnection").Value);
            //DataTable dt_re = myRepository.RunSQLTextToDatatable($"SELECT *  FROM [FieldRecording].[dbo].[T_Contract_Sync] WHERE [FileName] != '{userName}'");


            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand($"SELECT *  FROM [FieldRecording].[dbo].[T_Contract_Sync] WHERE [FileName] != '{userName}'", con);

                var dataReader = cmd1.ExecuteReader();
                
                dataTable.Load(dataReader);
                    
            }
            return dataTable;

        }

     

        private async Task<ResponseContext> getFollowUpDB(Allocations followupData, AccountDetailModel req)
        {
            var _resview = new ResponseContext();
            try
            {
                _logService.Info("Service getFollowUpDB: Start");
                var dateSync = (from fa in await _tbt_fetch_alloc.Get(d => d.UserName == req.user_name).ConfigureAwait(false)
                                group fa by fa.SyncDate into g
                                select new { Date = g.Max(t => t.SyncDate) }).Select(x => x.Date).FirstOrDefault();

                var resultListfolloupHis = (from ctn in (from ctn in await _tbt_contract_note.Get(d => d.CreateDate > dateSync && d.ContractNo == req.contract_no).ConfigureAwait(false) select ctn).Distinct().OrderByDescending(ctn => ctn.CreateDate)
                                            select new { data = ctn }).Select(data => {
                                                dynamic obj = new ExpandoObject();
                                                dynamic fields = new ExpandoObject();
                                                obj.id = "followupgrid";
                                                obj.tbseq = -1;
                                                obj.stbseq = -1;
                                                obj.gridrowseq = -1;
                                                fields.AC = data.data.Action;
                                                fields.DTA = (data.data.CreateDate != null) ? ((DateTime)data.data.CreateDate).ToString("dd/MM/yyyy") : "";
                                                fields.RCD = data.data.Result;
                                                fields.NA = data.data.NextAction;
                                                fields.DNA = data.data.NextActionDate;
                                                fields.RMK = data.data.Remarks;
                                                fields.PROMISEDATE = "";
                                                fields.PROMISEAMOUNT = "";
                                                fields.POC = "";
                                                fields.MODC = "";
                                                fields.ICONTACTNO = "";
                                                obj.fields = fields;
                                                return obj;
                                            })
                                            .ToList<dynamic>();

                if (resultListfolloupHis.Count != 0)
                {
                    //string eeeeeeeee = Newtonsoft.Json.JsonConvert.SerializeObject(resultListfolloupHis);
                    //var grids = followupData.data.grids.Where(data => data.id == "followupgrid").ToList();
                    var mergefollowupTosrc = followupData.data.grids.Concat(resultListfolloupHis).ToList();
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(mergefollowupTosrc);
                    followupData.data.grids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AllocationsGrid>>(json);// src.Concat(mergefollowupTosrc).ToList<dynamic>();  
                }

                _resview.data = followupData;

                _logService.Info("Service getFollowUpDB: End");
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("getFollowUpDB Error: {0}", ex.Message);
                _logService.Info(string.Format("Service getFollowUpDB Error: {0}", ex.Message));
            }

            return _resview;
        }
        private string ValidatesResult(dynamic obj) {
            var resultCode = (obj == null || !Util.HasAttr(obj, "result")) ? "" : obj.result;
            var resultMessage = (obj == null || !Util.HasAttr(obj, "message")) ? "" : obj.message;
            resultMessage = resultMessage != "" ? resultMessage : "Not found result status from source";
            return (resultCode != "success") ? resultMessage : "";
        }

        private string ValidatesResultIC5(dynamic obj)
        {
            var resultstatucCode = (obj == null || !Util.HasAttr(obj, "status")) ? "" : obj.status;
            var resultMessage = (obj == null || !Util.HasAttr(obj, "message")) ? "" : obj.message;
            resultMessage = resultMessage != "" ? resultMessage : "Not found result status from source";
            return (resultstatucCode != "200") ? resultMessage : "";
        }

        private ResponseContext checkfile(string fullpath)
        {
            var _resview = new ResponseContext();
            try
            {
                _logService.Info(string.Format("Service checkfile: Start - path check is {0}", fullpath));

                FileInfo f = new FileInfo(fullpath);
                string drive = Path.GetPathRoot(f.FullName);
                var disk = new DriveInfo(drive);
                if (!disk.IsReady)
                {
                    _resview.statusCode = "201";
                    _resview.message = string.Format("path {0}: Cannot write file because disk is not ready", fullpath);
                }

                var freeSpacePerc = disk.AvailableFreeSpace / 1024 / 1024; // mb
                if (freeSpacePerc < 100)
                {
                    _resview.statusCode = "201";
                    _resview.message = string.Format("path {0}: Cannot write file because disk is full", fullpath);
                }

                _resview.data = freeSpacePerc;
                _logService.Info(string.Format("Service checkfile: End - path check is {0}", fullpath));
            } 
            catch(Exception ex) 
            {
                _resview.statusCode = "201";
                _resview.message = ex.Message;
                _logService.Info(string.Format("Service checkfile Error: {0}", ex.Message));
            }

            return _resview;
        }

        private void SaveSyncDataToDB(List<dynamic>? resultListActive ,string userName) 
        {
            List<TContractSyncModel> all = new List<TContractSyncModel>();
            foreach (dynamic obj in resultListActive)
            {
                TContractSyncModel a = new TContractSyncModel();
                a.Id = null;
                a.applicationid = obj.applicationid;
                a.NM = obj.NM;
                a.OSAMT = obj.OSAMT;
                a.ODAMT = obj.ODAMT;
                a.ADDR = obj.ADDR;
                a.IsActive = obj.IsActive;
                a.FileName = userName;
                all.Add(a);
            }

            DataTable dt = new DataTable();
            dt = this.ConvertToDataTable(all);

            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;

            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();
                using (SqlTransaction sqlTransaction = con.BeginTransaction())
                {
                    SqlCommand cmd1 = new SqlCommand($"DELETE FROM [FieldRecording].[dbo].[T_Contract_Sync] WHERE [FileName] = '{userName}'", sqlTransaction.Connection);

                    cmd1.Transaction = sqlTransaction;
                    cmd1.ExecuteNonQuery();

                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, sqlTransaction))
                    {
                        try
                        {
                            sqlBulkCopy.BatchSize = 10000;
                            sqlBulkCopy.BulkCopyTimeout = 0;
                            sqlBulkCopy.DestinationTableName = "[dbo].[T_Contract_Sync]";
                            sqlBulkCopy.WriteToServer(dt);

                            sqlTransaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            sqlTransaction.Rollback();
                            //throw new InvalidOperationException($"Error at sqlBulkCopy tbt_ic5_mapping : {ex.Message}");
                        }

                    }
                }
            }



        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public async Task<ResponseContext> getLocationTracking(LocationTracking Req)
        {
            var _resview = new ResponseContext();
            try
            {
                DataTable dt = GetLocationTrackingFromDB(Req.CreatedBy);

                LocationTracking locationTracking = new LocationTracking();
                foreach (DataRow row in dt.Rows)
                {
                    locationTracking.id = row["id"].ToString();
                    locationTracking.lat = row["lat"].ToString();
                    locationTracking.lng = row["lng"].ToString();
                    locationTracking.CreatedBy = row["CreatedBy"].ToString();
                    locationTracking.CreatedDate = Convert.ToDateTime(row["CreatedDate"].ToString());
                    locationTracking.UpdatedBy = row["UpdatedBy"].ToString();
                    locationTracking.UpdatedDate = Convert.ToDateTime(row["UpdatedDate"].ToString());
                    locationTracking.IsActive = row["IsActive"].ToString();


                }

                if (dt.Rows.Count == 0)
                    throw new Exception("Data not found");
                _resview.data = locationTracking;

            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("getLocationTracking Error => {0}", ex.Message);
                _logService.Info(string.Format("Service getLocationTracking Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }
            return _resview;
        }

        private DataTable GetLocationTrackingFromDB(string? createdBy)
        {
            //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
            string _connectionStr = this.connectionString;
            var dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionStr))
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand($"SELECT TOP 1 *  FROM [FieldRecording].[dbo].[M_Location_Tracking] " +
                                                $"  WHERE [CreatedBy] = '{createdBy}' " +
                                                $"  Order By CreatedDate DESC ", con);

                var dataReader = cmd1.ExecuteReader();

                dataTable.Load(dataReader);

            }
            return dataTable;
        }

        public async Task<ResponseContext> AddLocationTracking(AddLocationTrackingReq Req)
        {
            var _resview = new ResponseContext();
            try
            {
               // bool is_validToken = (Req.token == _config.GetSection("LocationTracking:token").Value) ? true : throw new Exception("token invalid");


                bool is_validToken = (Req.token == this.token_addLocationTracking) ? true : throw new Exception("token invalid");


                //string _connectionStr = _config.GetSection("ConnectionStrings:FieldRecordingConnection").Value;
                string _connectionStr = this.connectionString;


                using (SqlConnection con = new SqlConnection(_connectionStr))
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand($"INSERT INTO M_Location_Tracking(id,lat,lng,CreatedBy,UpdatedBy,IsActive) values(" +
                                                    $"   newid() ," +
                                                    $"  '{Req.lat}' ," +
                                                    $"  '{Req.lng}' ," +
                                                    $"  '{Req.username}' ," +
                                                    $"  '{Req.username}' ," +
                                                     $"  '1' " +
                                                    $"  )", con);

                    int i = cmd1.ExecuteNonQuery();
                }


                _resview.statusCode = "200";
                _resview.message = string.Format("ok");

            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("AddLocationTracking Error => {0}", ex.Message);
                _logService.Info(string.Format("Service AddLocationTracking Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }
            return _resview;
        }
    }
}
