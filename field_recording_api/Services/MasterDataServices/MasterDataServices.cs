using field_recording_api.Models.HttpModel;
using System.Dynamic;
using System.Text.Json;
using field_recording_api.Utilities;
using field_recording_api.Models.MasterData;
using field_recording_api.Models.ConfigCloseSystem;
using field_recording_api.DataAccess.FieldRecording;
using System.Data;
using Microsoft.Extensions.Caching.Memory;
using field_recording_api.Models.AccountDetailModel;
using System.Collections.Concurrent;
using System.Linq;
using System.Diagnostics;
using log4net.Config;
using Microsoft.EntityFrameworkCore;

namespace field_recording_api.Services.MasterDataServices
{
    public class MasterDataServices : IMasterDataServices
    {
        private readonly IConfiguration _config;
        public MasterDataServices(IConfiguration config)
        {
            _config = config;
        }


        private readonly FieldRecordingContext _context;
        public MasterDataServices(IConfiguration config, FieldRecordingContext _context)
        {
            _config = config;
            this._context = _context;
        }

        public  ConfigCloseSystemResModel GetConfigCloseSystem(ConfigCloseSystemReqModel req)
        {
            ConfigCloseSystemResModel res = new();
            try
            {
                List<ConfigCloseSystemDtoModel> payload = new();

                //var spcialDay2 = getSpcialDay();
                //_context.tbt_special_day.AsNoTracking().AsParallel().Where(z => z.is_deleted == "n").FirstOrDefault();

                using (var ctx = _context)
                {

                    var d = DateTime.Now;

                    if (DateTime.TryParse(req.dateFromMobile, out d) == false) {
                        d = DateTime.Now;
                    }



                    var spcialDay =   ctx.tbt_special_day.AsNoTracking().AsParallel().Where(z => z.is_deleted.ToLower() == "n" && z.date?.ToString("yyyy-MM-dd") == d.ToString("yyyy-MM-dd")).FirstOrDefault();

                    IQueryable<ConfigCloseSystemDaoModel> filter;
                    if (spcialDay == null)
                        filter = ctx.tbt_schedule_woking.AsNoTracking().Where(z => z.is_active.ToLower() == "y"
                                                                && (d >= z.effecitive_date_from && d <= (z.effecitive_date_to ?? d))
                                                                && z.is_deleted.ToLower() == "n"
                                                                && z.day_type_code != "003");
                    else
                        filter = ctx.tbt_schedule_woking.AsNoTracking().Where(z => z.is_active.ToLower() == "y"
                                                                && (d >= z.effecitive_date_from && d <= (z.effecitive_date_to ?? d))
                                                                && z.is_deleted.ToLower() == "n"
                                                                && z.day_type_code == "003");


                    //Stopwatch sw = new Stopwatch();

                    //long[] time = new long[4];

                    //sw.Start();

                    //List<ConfigCloseSystemDtoModel> payload1 = filter.Select(s => new ConfigCloseSystemDtoModel
                    //{
                    //    id  = s.id ,
                    //    day_type_code = s.day_type_code,
                    //    message_close_system = s.message_close_system,
                    //    start_time = s.start_time,
                    //    end_time = s.end_time,
                    //    effecitive_date_from = s.effecitive_date_from,
                    //    effecitive_date_to = s.effecitive_date_to,
                    //    remind_time_mins = s.remind_time_mins,
                    //    remind_message = s.remind_message,
                    //    message_save_after_closed = s.message_save_after_closed ,
                    //    is_active = s.is_active,
                    //    is_deleted = s.is_deleted
                    //}).ToList();

                    //sw.Stop();

                    //time[0] = sw.ElapsedMilliseconds;
                    //sw.Reset();


                    //payload = new List<ConfigCloseSystemDtoModel>();
                    //sw.Start();
                    var partitioner = Partitioner.Create(filter);
                    Parallel.ForEach(partitioner, item =>
                    {
                        ConfigCloseSystemDtoModel model = new();
                        model.id = item.id;
                        model.day_type_code = item.day_type_code;
                        model.message_close_system = item.message_close_system;
                        model.start_time = item.start_time;
                        model.end_time = item.end_time;
                        model.effecitive_date_from = item.effecitive_date_from;
                        model.effecitive_date_to = item.effecitive_date_to;
                        model.remind_time_mins = item.remind_time_mins;
                        model.remind_message = item.remind_message;
                        model.message_save_after_closed = item.message_save_after_closed;
                        model.is_active = item.is_active;
                        model.is_deleted = item.is_deleted;


                        payload.Add(model);


                    });
                    //sw.Stop();
                    //time[1] = sw.ElapsedMilliseconds;
                    //sw.Reset();
                    //payload = new List<ConfigCloseSystemDtoModel>();
                    //sw.Start();

                    //Parallel.ForEach(filter, item =>
                    //{
                    //    ConfigCloseSystemDtoModel model = new();
                    //    model.id = item.id;
                    //    model.day_type_code = item.day_type_code;
                    //    model.message_close_system = item.message_close_system;
                    //    model.start_time = item.start_time;
                    //    model.end_time = item.end_time;
                    //    model.effecitive_date_from = item.effecitive_date_from;
                    //    model.effecitive_date_to = item.effecitive_date_to;
                    //    model.remind_time_mins = item.remind_time_mins;
                    //    model.remind_message = item.remind_message;
                    //    model.message_save_after_closed = item.message_save_after_closed;
                    //    model.is_active = item.is_active;
                    //    model.is_deleted = item.is_deleted;


                    //    payload.Add(model);


                    //});

                    //sw.Stop();
                    //time[2] = sw.ElapsedMilliseconds;
                    //sw.Reset();
                    //payload = new List<ConfigCloseSystemDtoModel>();
                    //sw.Start();



                    //foreach (var item in filter)
                    //{
                    //    ConfigCloseSystemDtoModel model = new();
                    //    model.id = item.id;
                    //    model.day_type_code = item.day_type_code;
                    //    model.message_close_system = item.message_close_system;
                    //    model.start_time = item.start_time;
                    //    model.end_time = item.end_time;
                    //    model.effecitive_date_from = item.effecitive_date_from;
                    //    model.effecitive_date_to = item.effecitive_date_to;
                    //    model.remind_time_mins = item.remind_time_mins;
                    //    model.remind_message = item.remind_message;
                    //    model.message_save_after_closed = item.message_save_after_closed;
                    //    model.is_active = item.is_active;
                    //    model.is_deleted = item.is_deleted;


                    //    payload.Add(model);
                    //}

                    //sw.Stop();
                    //time[3] = sw.ElapsedMilliseconds;
                    //sw.Reset();


                }
                res.status_code = "00";
                res.status_text = "Success";
                res.payload = payload;
            }
            catch (Exception)
            {
                throw;
            }

            return res;
        }

        //private async Task<tbt_special_day?> getSpcialDay()
        //{
        //    tbt_special_day aa =  _context.tbt_special_day.AsParallel().Where(z => z.is_deleted == "n").FirstOrDefault();
        //    return await aa;
        //}

        public async Task<ResponseContext> getDropdownMaster()
        {
            var _resview = new ResponseContext();


            MasterData masterData = new MasterData();


            Data data = new Data();

            ActionCode actionCode = new ActionCode();
            actionCode.text = "Visit Fee";
            actionCode.value = "VS";

            ResutCode resutCode = new ResutCode();
            resutCode.text = "EVIDENT";
            resutCode.value = "EVID";


            data.ActionCode = new List<ActionCode>() { actionCode };
            data.ResutCode = new List<ResutCode>() { resutCode };
            data.ModelOfContact = new List<ModelOfContact>();
            data.PlaceOfContract = new List<PlaceOfContract>();
            data.PartyContracted = new List<PartyContracted>();



            _resview.statusCode = "200";
            _resview.message = "";

            _resview.data = data;

            return _resview;


            
        }
    }
}
