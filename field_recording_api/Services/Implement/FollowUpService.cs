using field_recording_api.DataAccess.FieldRecording;
using field_recording_api.DataAccess.SIIS;
using field_recording_api.Models.ActionCodeModel;
using field_recording_api.Models.DelinquencyReason;
using field_recording_api.Models.FollowUp;
using field_recording_api.Models.GetDropDownListMaster;
using field_recording_api.Models.ModeOfContact;
using field_recording_api.Models.PartyContacted;
using field_recording_api.Models.PlaceOfContact;
using field_recording_api.Models.ResultCodeModel;
using field_recording_api.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace field_recording_api.Services.Implement
{
    public class FollowUpService : IFollowUpService
    {
        private readonly FieldRecordingContext _context;
        public FollowUpService(FieldRecordingContext _context) {
            this._context = _context;
        }

        public GetDropDownListMasterResModel GetDropdownList(GetDropDownListMasterReqModel req)
        {
            var res = new GetDropDownListMasterResModel();
            try
            {
                using (var ctx = _context)
                {
                    var payload_action = new List<ActionCodeDtoModel>();
                    foreach (var row in ctx.t_mst_action_code.Where(z => z.IsActive == "Y"))
                    {
                        var item = new ActionCodeDtoModel();
                        item.action_code = row.Action_Code;
                        item.action_name = row.DescriptionEN;
                        payload_action.Add(item);
                    }

                    res.payload_action_code = payload_action;


                    var payload_result = new List<ResultCodeDtoModel>();
                    foreach (var row in ctx.t_mst_result_code.Where(z => z.IsActive == "Y"))
                    { 
                        var item = new ResultCodeDtoModel();
                        item.result_code = row.result_code;
                        item.result_name = row.DescriptionEN;
                        item.PTPOptionYN = row.PTPOptionYN;

                        payload_result.Add(item);
                    }
                    res.payload_result_code = payload_result;

                    var payload_DelinquencyReason = GetDelinquencyReason();
                    res.payload_delinquency_reason= payload_DelinquencyReason;


                    var payload_ModeOfContact = GetModeOfContact();  
                    res.payload_mode_of_contact = payload_ModeOfContact;


                    var payload_PlaceOfContact = GetPlaceOfContact(); 
                    res.payload_place_of_contact = payload_PlaceOfContact;


                    var payload_PartyContacted = GetPartyContacted();
                    res.payload_party_contacted = payload_PartyContacted;


                }
                res.status_code = "00";
            }
            catch (Exception ex)
            {
                res.status_code = "99";
                res.status_text = ex.Message;
            }
            return res;
        }

        private List<DelinquencyReasonDtoModel>? GetDelinquencyReason()
        {
            var payload_DelinquencyReason = new List<DelinquencyReasonDtoModel>()
            { 
                new DelinquencyReasonDtoModel() { code = "Met with Accident" , value = "Met with Accident" } ,
                //new DelinquencyReasonDtoModel() { code = "Met with Accident" , value = "Met with Accident" } ,
                new DelinquencyReasonDtoModel() { code = "Out of Job" , value = "Out of Job" } ,
                //new DelinquencyReasonDtoModel() { code = "Out of Job" , value = "Out of Job" } ,
                //new DelinquencyReasonDtoModel() { code = "Spouse Deceased" , value = "Spouse Deceased" } ,
                new DelinquencyReasonDtoModel() { code = "Spouse Deceased" , value = "Spouse Deceased" } ,
            };
            return payload_DelinquencyReason;
        }

        private List<PartyContactedDtoModel>? GetPartyContacted()
        {
            var payload_PartyContacted = new List<PartyContactedDtoModel>()
            {
                new PartyContactedDtoModel() { code = "C" , value = "Customer" , valueTh = "ลูกค้า" },
                new PartyContactedDtoModel() { code = "A" , value = "Designated person" , valueTh = "ผู้รับมอบหมายให้ทวงถามหนี้ได้ตามกฏหมาย" },
                new PartyContactedDtoModel() { code = "D" , value = "Dealer" , valueTh = "ร้าน"},
                new PartyContactedDtoModel() { code = "F" , value = "Friend", valueTh = "เพื่อน"},
                new PartyContactedDtoModel() { code = "G" , value = "Guarantor" ,valueTh = "ผู้ค้ำประกัน"},
                new PartyContactedDtoModel() { code = "M" , value = "Marketing", valueTh = "เจ้าหน้าที่การตลาด"},
                new PartyContactedDtoModel() { code = "O" , value = "Other", valueTh = "อื่นๆ"},
                new PartyContactedDtoModel() { code = "P" , value = "Parent", valueTh = "พ่อ แม่"},
                new PartyContactedDtoModel() { code = "R" , value = "Reference", valueTh = "ผู้อ้างอิง"},
                new PartyContactedDtoModel() { code = "S" , value = "Spouse", valueTh = "คู่สมรส"},
                new PartyContactedDtoModel() { code = "U" , value = "Vehicle Usage", valueTh = "ผู้ใช้รถ/ผู้ครอบครองรถ" },

            };
            return payload_PartyContacted;
        }

        private List<PlaceOfContactDtoModel>? GetPlaceOfContact()
        {
            var payload_PlaceOfContact = new List<PlaceOfContactDtoModel>()
            {
                new PlaceOfContactDtoModel() { code = "A", value = "Apartment" , valueTh ="อพาร์ทเม้นท์/ห้องพัก" },
                new PlaceOfContactDtoModel() { code = "B", value = "Business" , valueTh = "ที่ทำงาน"},
                new PlaceOfContactDtoModel() { code = "H", value = "Home" , valueTh = "บ้าน"},
                new PlaceOfContactDtoModel() { code = "I", value = "Operator" , valueTh = "โอเปอร์เรเตอร์"},
                new PlaceOfContactDtoModel() { code = "M", value = "Mobile" , valueTh = "มือถือ"},
                new PlaceOfContactDtoModel() { code = "O", value = "Other" , valueTh = "อื่นๆ"},
                new PlaceOfContactDtoModel() { code = "R", value = "Resident"  , valueTh = "ที่อยู่อาศัย"},

            };
            return payload_PlaceOfContact;
        }

        private List<ModeOfContactDtoModel>? GetModeOfContact()
        {
            var payload_ModeOfContact = new List<ModeOfContactDtoModel>()
            {
                new ModeOfContactDtoModel() { code = "SM" , value = "ข้อความสั้นทางมือถือ" },
                new ModeOfContactDtoModel() { code = "CL" , value = "โทร" },
                new ModeOfContactDtoModel() { code = "VT" , value = "Visit by OA" },
                new ModeOfContactDtoModel() { code = "IN" , value = "กระบวนการภายใน" },
                new ModeOfContactDtoModel() { code = "VTU" , value = "Visit ผู้ใช้รถ/ผู้ครอบครองรถ" },
                new ModeOfContactDtoModel() { code = "VTA" , value = "Visit ผู้รับมอบหมายให้ทวงถามหนี้ได้ตามกฏหมาย" },
                new ModeOfContactDtoModel() { code = "VTC" , value = "Visit ลูกค้า" },
                new ModeOfContactDtoModel() { code = "VTR" , value = "Visit ผู้อ้างอิง" },
                new ModeOfContactDtoModel() { code = "VTP" , value = "Visit พ่อ แม่" },
                new ModeOfContactDtoModel() { code = "VTG" , value = "Visit ผู้ค้ำประกัน" },
                new ModeOfContactDtoModel() { code = "VTD" , value = "Visit ร้าน" },
                new ModeOfContactDtoModel() { code = "VTM" , value = "Visit เจ้าหน้าที่การตลาด" },
                new ModeOfContactDtoModel() { code = "VTS" , value = "Visit คู่สมรส" },
                new ModeOfContactDtoModel() { code = "VTO" , value = "Visit อื่นๆ" },
                new ModeOfContactDtoModel() { code = "VTF" , value = "Visit เพื่อน" },
                new ModeOfContactDtoModel() { code = "LR" , value = "จดหมาย" },
                new ModeOfContactDtoModel() { code = "EM" , value = "อีเมล์" },
            };


            return payload_ModeOfContact;
        }

        public FollowUpResModel SaveFollowUp(FollowUpReqModel req)
        {
            FollowUpResModel res = new ();
            try
            {
                using (var ctx = _context)
                {
                    TbtContactNote item = new();


                    
                    int userid = ctx.TbmUsers.Where(z => z.UserName == req.Create_By).FirstOrDefault()?.UserId ?? 0 ;

                    DateTime nextActionDate = new DateTime();
                    if (req.NextActionDate != "Optional") {

                        string[] d = req.NextActionDate.Split(new char[] { '/' });
                        nextActionDate = new DateTime( int.Parse(d[2]), int.Parse(d[1]),int.Parse(d[0]));
                    }

                    item.ContractNo = req.contract_no;
                    item.Action = req.Action;
                    item.Result = req.Result;
                    item.NextAction = (req.NextAction == "0") ? null : req.NextAction;
                    item.NextActionDate = (req.NextActionDate == "Optional") ? null : nextActionDate  ;
                    item.DelinquencyReason = (req.DelinquencyReason == "0") ? null :  req.DelinquencyReason;
                    item.Remarks = req.Remarks;
                    item.ModeOfContact = (req.ModeOfContact == "0") ? null :  req.ModeOfContact;
                    item.PlaceOfContact = (req.PlaceOfContact == "0") ? null : req.PlaceOfContact;
                    item.PartyContacted = (req.PartyContacted == "0") ? null :  req.PartyContacted;
                    item.ContactNumber = req.ContactNumber;
                    item.Geo = req.Geo;
                    item.SubDistrict = req.SubDistrict;
                    item.District = req.District;
                    item.Province = req.Province;
                    item.ZipCode = req.ZipCode;
                    item.CreateBy = userid.ToString();
                    item.CreateDate = DateTime.Now;
                    item.UpdateDate = DateTime.Now;
                    item.UpdateBy = userid.ToString();
                    item.IsSync = "Y";
                    item.SyncDate = DateTime.Now;
                    item.SyncErrorMessage = (userid == 0) ? req.Create_By : "";

                    item.OD_AMT = req.OD_AMT == "0.0" ? null : Convert.ToDecimal(req.OD_AMT);
                    item.OS_AMT = req.OS_AMT == "0.0" ? null : Convert.ToDecimal(req.OS_AMT);



                    item.IPAddress = req.IPAddress;

                    DateTime PTP_DT = new DateTime();
                    if (req.PTP_DT != "1900-01-01 00:00:00")
                    {
                        PTP_DT = DateTime.ParseExact(req.PTP_DT, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                    }
                    item.PTP_DT = req.PTP_DT == "1900-01-01 00:00:00" ? null : PTP_DT;
                    item.PTP_AMT = req.PTP_AMT == "0.0" ? null : Convert.ToDecimal(req.PTP_AMT);



                    ctx.TbtContactNotes.Add(item);
                    ctx.SaveChanges();
                }
                res.status_code = "00";
                res.status_text = "OK";

            }
            catch (Exception)
            {
                throw;
            }
            return res;
        }




    }
}
