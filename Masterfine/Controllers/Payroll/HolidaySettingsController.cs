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

        public ActionResult HolidaySettings()
        {
            DateTime currentTime = DateTime.Now;
            
            
            return View(GetHolidays(Convert.ToString(currentTime)));
        }
        public DataTable GetHolidays(string date)
        {
            DateTime cur_date = Convert.ToDateTime(date);
            string strMonth = cur_date.ToString("MMMMyyyy");
            string dtpMonth = strMonth.Substring(0, 3);
            string dtpYear = cur_date.Year.ToString();
            HolidaySP spHoliday = new HolidaySP();
            DataTable dtblHolidaySettings = new DataTable();
            dtblHolidaySettings = spHoliday.HoildaySettingsViewAllLimited(dtpMonth.ToString(), dtpYear.ToString());
            return dtblHolidaySettings;
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetHolidayData(string date)
        {
            try
            {
                string jsonResult = Utils.ConvertDataTabletoString(GetHolidays(date));
                return Json(new
                {
                    success = "true",
                    data = jsonResult
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = "false", data = "" });
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SaveAllHoliday(string cur_month, string tableData)
        {
            HolidaySP spHoliday = new HolidaySP();
            try
            {
                List<Dictionary<string, object>> items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tableData);
                if (cur_month == null)
                    cur_month = Convert.ToString(DateTime.Now);
                DateTime curdate = Convert.ToDateTime(cur_month);
                string strMonth = curdate.ToString("MMMMyyyy");
                string dtpMonth = strMonth.Substring(0, 3);
                
                string dtpYear = curdate.Year.ToString();
                spHoliday.HolidaySettingsDeleteByMonth(dtpMonth.ToString(), dtpYear.ToString());
                HolidayInfo infoHoliday = new HolidayInfo();
                int inrowcount = items.Count;
                for (int i = 0; i < inrowcount; i++)
                {
                    DateTime date = Convert.ToDateTime(items[i]["date"]);
                    infoHoliday.Date = date;
                    if (items[i]["narration"] != null)
                    {
                        string strNarration = Convert.ToString(items[i]["narration"]);
                        infoHoliday.Narration = strNarration.Trim();
                    }
                    else
                    {
                        infoHoliday.Narration = string.Empty;
                    }
                    infoHoliday.HolidayName = string.Empty;
                    infoHoliday.Extra1 = string.Empty;
                    infoHoliday.Extra2 = string.Empty;
                    spHoliday.HolidayAddWithIdentity(infoHoliday);
                }
                HolidaySettingsDate(cur_month);
            }
            
            catch (Exception ex)
            {
                return Json(new { error = "failed" });
            }
            return Json(new { error = "success" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AddHoliday(string date, string narration)
        {
            HolidaySP spHoliday = new HolidaySP();
            try
            {
                HolidayInfo infoHoliday = new HolidayInfo();
                {
                    DateTime curdate = Convert.ToDateTime(date);
                    infoHoliday.Date = curdate;
                    if (narration != null)
                    {                        
                        infoHoliday.Narration = narration.Trim();
                    }
                    else
                    {
                        infoHoliday.Narration = string.Empty;
                    }
                    infoHoliday.HolidayName = string.Empty;
                    infoHoliday.Extra1 = string.Empty;
                    infoHoliday.Extra2 = string.Empty;
                    spHoliday.HolidayAddWithIdentity(infoHoliday);
                    
                }
                HolidaySettingsDate(date);
            }

            catch (Exception ex)
            {
                return Json(new { error = "failed" });
            }
            return Json(new { error = "success" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult EditHoliday(string date, string narration)
        {
            HolidaySP spHoliday = new HolidaySP();
            try
            {
                HolidayInfo infoHoliday = new HolidayInfo();
                {
                    DateTime curdate = Convert.ToDateTime(date);
                    string strMonth = curdate.ToString("MMMMyyyydd");
                    string dtpMonth = strMonth.Substring(0, 3);

                    string dtpYear = curdate.Year.ToString();
                    string dtpDay = strMonth.Substring(strMonth.Length-2, 2);
                    decimal id = spHoliday.HolidaySettingsSearchByDate(dtpMonth, dtpYear, dtpDay).HolidayId;
                    
                    infoHoliday.HolidayId = id;
                    infoHoliday.Date = curdate;
                    if (narration != null)
                    {
                        infoHoliday.Narration = narration.Trim();
                    }
                    else
                    {
                        infoHoliday.Narration = string.Empty;
                    }
                    infoHoliday.HolidayName = string.Empty;
                    infoHoliday.ExtraDate = DateTime.Now; 
                    infoHoliday.Extra1 = string.Empty;
                    infoHoliday.Extra2 = string.Empty;
                    spHoliday.HolidayEdit(infoHoliday);
                }
                HolidaySettingsDate(date);
            }

            catch (Exception ex)
            {
                return Json(new { error = "failed" });
            }
            return Json(new { error = "success" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DeleteHoliday(string date)
        {
            HolidaySP spHoliday = new HolidaySP();
            try
            {
                DateTime curdate = Convert.ToDateTime(date);
                string strMonth = curdate.ToString("MMMMyyyydd");
                string dtpMonth = strMonth.Substring(0, 3);

                string dtpYear = curdate.Year.ToString();
                string dtpDay = strMonth.Substring(strMonth.Length - 2, 2);
                decimal id = spHoliday.HolidaySettingsSearchByDate(dtpMonth, dtpYear, dtpDay).HolidayId;
                spHoliday.HolidayDelete(Convert.ToDecimal(id));
            }

            catch (Exception ex)
            {
                return Json(new { error = "failed" });
            }
            return Json(new { error = "success" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ClearHoliday(string cur_month)
        {
            HolidaySP spHoliday = new HolidaySP();
            try
            {
                if (cur_month == null)
                    cur_month = Convert.ToString(DateTime.Now);
                DateTime curdate = Convert.ToDateTime(cur_month);
                string strMonth = curdate.ToString("MMMMyyyy");
                string dtpMonth = strMonth.Substring(0, 3);

                string dtpYear = curdate.Year.ToString();
                spHoliday.HolidaySettingsDeleteByMonth(dtpMonth.ToString(), dtpYear.ToString());
            }

            catch (Exception ex)
            {
                return Json(new { error = "failed" });
            }
            return Json(new { error = "success" });
        }
        public ActionResult HolidaySettingsDate(string cur_month)
        {
            DateTime currentTime = Convert.ToDateTime(cur_month);


            return View(GetHolidays(Convert.ToString(currentTime)));
        }
    }
}