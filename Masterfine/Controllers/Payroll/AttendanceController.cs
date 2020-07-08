using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Masterfine.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Masterfine.Controllers
{
    public partial class PayrollController : Controller
    {

        public ActionResult Attendance()
        {
            ViewData["date"] = DateTime.UtcNow.ToString("yyyy-MM-dd");
            DailyAttendanceMasterSP spDailyAttendanceMaster = new DailyAttendanceMasterSP();

            if (spDailyAttendanceMaster.DailyAttendanceMasterMasterIdSearch(DateTime.Now.ToShortDateString().ToString()))
                ViewData["btnText"] = "Update";
            else
            {
                ViewData["btnText"] = "Save";
                ViewData["btnDel"] = "disabled";
            }

            return View();
        }

        private DataTable GetAttendenceFromDB(string strDate)
        {
            DataTable dtblAttendance = new DataTable();

            try
            {
                DailyAttendanceDetailsInfo infoDailyAttendanceDetails = new DailyAttendanceDetailsInfo();
                DailyAttendanceDetailsSP spDailyAttendanceDetails = new DailyAttendanceDetailsSP();
                DailyAttendanceMasterInfo infoDailyAttendanceMaster = new DailyAttendanceMasterInfo();                

                infoDailyAttendanceMaster.Date = Convert.ToDateTime(strDate);
                dtblAttendance = spDailyAttendanceDetails.DailyAttendanceDetailsSearchGridFill(strDate);


                for (int i = 0; i < dtblAttendance.Rows.Count; i++)
                {
                    string status = dtblAttendance.Rows[i]["status"].ToString();
                    if (status == string.Empty)
                        dtblAttendance.Rows[i]["status"] = "Present";
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("A1" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return dtblAttendance;
        }

        [HttpPost]
        public JsonResult GetAttendence(string date)
        {
            DateTime cur_date = Convert.ToDateTime(date);
            DataTable dtblAttendence = GetAttendenceFromDB(cur_date.ToShortDateString().ToString());
            
            string jsonResult = Utils.DataTableToJsonString(dtblAttendence);
            decimal decResult = HolidaySettings(date);
            if (decResult == 1)
                return Json(new { success = "true", holiday = "true", data = jsonResult});
            else
                return Json(new { success = "true", holiday = "false", data = jsonResult });

        }
        
        [HttpPost]
        public ActionResult SaveOrEditAttendance(string date, string btnStatus, string tableData)
        {
            bool isSuccess = true;
            bool isHoliday = false;
            string message = string.Empty;
            string jsonResult = string.Empty;
            try
            {
                decimal decResult = HolidaySettings(date);
                if (decResult != 1)
                {
                    if (btnStatus == "Save")
                    {
                        SaveFunction(date, tableData);
                    }
                    else
                    {
                        EditFunction(date, tableData);
                    }
                    DataTable dtblAttendence = GetAttendenceFromDB(date);
                    jsonResult = Utils.DataTableToJsonString(dtblAttendence);
                }
                else
                {
                    isHoliday = true;
                }
            }
            catch(Exception ex)
            {
                isSuccess = false;
                message = "AC1" + ex.Message;
            }
            return Json(new { isSuccess, message, isHoliday, data = jsonResult });
        }

        public bool SaveFunction(string date, string tableData)
        {
            try
            {
                List<Dictionary<string, object>> items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tableData);

                DailyAttendanceDetailsInfo infoDailyAttendanceDetails = new DailyAttendanceDetailsInfo();
                DailyAttendanceDetailsSP spDailyAttendanceDetails = new DailyAttendanceDetailsSP();
                DailyAttendanceMasterInfo infoDailyAttendanceMaster = new DailyAttendanceMasterInfo();
                DailyAttendanceMasterSP spDailyAttendanceMaster = new DailyAttendanceMasterSP();
                infoDailyAttendanceMaster.Date = Convert.ToDateTime(date);
                infoDailyAttendanceMaster.Narration = string.Empty;
                infoDailyAttendanceMaster.Extra1 = string.Empty;
                infoDailyAttendanceMaster.Extra2 = string.Empty;
                int inrowcount = items.Count;
                var decMasterIdForEdit = spDailyAttendanceMaster.DailyAttendanceAddToMaster(infoDailyAttendanceMaster);  // calling @@identity
                infoDailyAttendanceDetails.DailyAttendanceMasterId = decMasterIdForEdit;
                for (int i = 0; i < inrowcount; i++)
                {
                    string employeeId = items[i]["employeeId"].ToString() ?? string.Empty;
                    string status = items[i]["status"].ToString() ?? string.Empty;
                    string narration = items[i]["narration"].ToString() ?? string.Empty;
                    narration = narration.Replace("\n", "\\n");

                    if (employeeId != null && employeeId != string.Empty)
                    {
                        infoDailyAttendanceDetails.EmployeeId = Convert.ToDecimal(employeeId);
                    }
                    if (status != null && status != string.Empty)
                    {
                        infoDailyAttendanceDetails.Status = status;
                    }
                    else
                    {
                        infoDailyAttendanceDetails.Status = "Present";
                    }
                    if (narration != null && narration != "")
                    {
                        infoDailyAttendanceDetails.Narration = narration;
                    }
                    else
                    {
                        infoDailyAttendanceDetails.Narration = "";
                    }
                    infoDailyAttendanceDetails.Extra1 = string.Empty;
                    infoDailyAttendanceDetails.Extra2 = string.Empty;
                    infoDailyAttendanceDetails.DailyAttendanceMasterId = decMasterIdForEdit;
                    spDailyAttendanceDetails.DailyAttendanceDetailsAddUsingMasterId(infoDailyAttendanceDetails);
                }                    
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool EditFunction(string date, string tableData)
        {
            try
            {
                List<Dictionary<string, object>> items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tableData);

                DailyAttendanceDetailsInfo infoDailyAttendanceDetails = new DailyAttendanceDetailsInfo();
                DailyAttendanceDetailsSP spDailyAttendanceDetails = new DailyAttendanceDetailsSP();
                DailyAttendanceMasterInfo infoDailyAttendanceMaster = new DailyAttendanceMasterInfo();
                DailyAttendanceMasterSP spDailyAttendanceMaster = new DailyAttendanceMasterSP();
                infoDailyAttendanceMaster.Date = Convert.ToDateTime(date);
                infoDailyAttendanceMaster.Narration = string.Empty;
                infoDailyAttendanceMaster.Extra1 = string.Empty;
                infoDailyAttendanceMaster.Extra2 = string.Empty;
                decimal decMasterIdForEdit = 0;
                int inrowcount = items.Count;
                for (int i = 0; i < inrowcount; i++)
                {
                    string masterId = items[i]["masterId"].ToString() ?? string.Empty;
                    if (masterId != "")
                    {
                        decMasterIdForEdit = Convert.ToDecimal(masterId);   //storing Dailymasterid
                    }
                }
                infoDailyAttendanceMaster.DailyAttendanceMasterId = decMasterIdForEdit;
                spDailyAttendanceMaster.DailyAttendanceEditMaster(infoDailyAttendanceMaster);
                infoDailyAttendanceDetails.DailyAttendanceMasterId = decMasterIdForEdit;
                for (int i = 0; i < inrowcount; i++)
                {
                    string detailsId = items[i]["detailsId"].ToString() ?? string.Empty;
                    string masterId = items[i]["masterId"].ToString() ?? string.Empty;
                    string employeeId = items[i]["employeeId"].ToString() ?? string.Empty;
                    string status = items[i]["status"].ToString() ?? string.Empty;                    
                    string narration = items[i]["narration"].ToString() ?? string.Empty;
                    narration = narration.Replace("\n", "\\n");

                    if (detailsId != "")
                    {
                        // for updation of saved employees
                        if (employeeId != null && employeeId != "")
                        {
                            infoDailyAttendanceDetails.EmployeeId = Convert.ToDecimal(employeeId);
                        }
                        if (detailsId != null && detailsId != "")
                        {
                            infoDailyAttendanceDetails.DailyAttendanceDetailsId = Convert.ToDecimal(detailsId);
                        }
                        if (masterId != null && masterId != "")
                        {
                            infoDailyAttendanceDetails.DailyAttendanceMasterId = Convert.ToDecimal(masterId);
                        }
                        if (status != null && status != "")
                        {
                            infoDailyAttendanceDetails.Status = status;
                        }
                        else
                        {
                            infoDailyAttendanceDetails.Status = "Present";
                        }
                        if (narration != null && narration != "")
                        {
                            infoDailyAttendanceDetails.Narration = narration;
                        }
                        else
                        {
                            infoDailyAttendanceDetails.Narration = "";
                        }
                        infoDailyAttendanceDetails.Extra1 = string.Empty;
                        infoDailyAttendanceDetails.Extra2 = string.Empty;
                        spDailyAttendanceDetails.DailyAttendanceDetailsEditUsingMasterId(infoDailyAttendanceDetails);
                    }
                    else
                    {
                        // for new employees to add
                        if (employeeId != null && employeeId != "")
                        {
                            infoDailyAttendanceDetails.EmployeeId = Convert.ToDecimal(employeeId);
                        }
                        if (status != null && status != "")
                        {
                            infoDailyAttendanceDetails.Status = status;
                        }
                        else
                        {
                            infoDailyAttendanceDetails.Status = "Present";
                        }
                        if (narration != null && narration != "")
                        {
                            infoDailyAttendanceDetails.Narration = narration;
                        }
                        else
                        {
                            infoDailyAttendanceDetails.Narration = "";
                        }
                        infoDailyAttendanceDetails.Extra1 = string.Empty;
                        infoDailyAttendanceDetails.Extra2 = string.Empty;
                        infoDailyAttendanceDetails.DailyAttendanceMasterId = decMasterIdForEdit;
                        spDailyAttendanceDetails.DailyAttendanceDetailsAddUsingMasterId(infoDailyAttendanceDetails);
                    }
                } //   updation of old employees & addition of new employees closes here
            }
            catch (Exception ex)
            {
                return false;    
            }
            return true;
        }

        [HttpPost]
        public ActionResult DeleteFunction(string date, string masterId)
        {
            bool isSuccess = true;
            string message = string.Empty;
            string jsonResult = string.Empty;
            try
            {
                DailyAttendanceDetailsSP spDailyAttendanceDetails = new DailyAttendanceDetailsSP();
                spDailyAttendanceDetails.DailyAttendanceDetailsDeleteAll(Convert.ToDecimal(masterId));

                DataTable dtblAttendence = GetAttendenceFromDB(date);
                jsonResult = Utils.DataTableToJsonString(dtblAttendence);
            }
            catch(Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
            }
            return Json(new { isSuccess, message, data = jsonResult });
        }

        private decimal HolidaySettings(string date)
        {
            HolidaySP spHoliday = new HolidaySP();
            DateTime cur_date = Convert.ToDateTime(date);
            decimal decResult = spHoliday.HolliDayChecking(cur_date);
            return decResult;
        }
    }
}