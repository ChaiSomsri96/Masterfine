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

namespace Masterfine.Controllers
{
    public partial class PayrollController : Controller
    {

        public ActionResult MonthlySalarySettings()
        {
            ViewData["month"] = DateTime.UtcNow.ToString("yyyy-MM");
            MonthlySalarySP spMonthlySalary = new MonthlySalarySP();
            if (spMonthlySalary.MonthlySalarySettingsMonthlySalaryIdSearchUsingSalaryMonth(DateTime.Now) > 0)
            {
                ViewData["btnText"] = "Update";
            }
            else
            {
                ViewData["btnText"] = "Save";
                ViewData["btnDel"] = "disabled";
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetMonthlySalarySettingsDetails(string month)
        {
            DataTable dtMonthly = GetMonthlySalarySettingsFromDB(month);
            DataTable dtblSalaryPackage = new DataTable();
            SalaryPackageSP spSalaryPackage = new SalaryPackageSP();
            dtblSalaryPackage = spSalaryPackage.SalaryPackageViewAllForMonthlySalarySettings();
            string jsonTableData = Utils.ConvertDataTabletoString(dtMonthly);
            string jsonComboData = Utils.ConvertDataTabletoString(dtblSalaryPackage);

            string jsonBtnStatus = string.Empty;
            MonthlySalarySP spMonthlySalary = new MonthlySalarySP();
            if (spMonthlySalary.MonthlySalarySettingsMonthlySalaryIdSearchUsingSalaryMonth(Convert.ToDateTime(month)) > 0)
                jsonBtnStatus = "Update";
            else
                jsonBtnStatus = "Save";

            return Json(new { success = "true", tableData = jsonTableData, comboData = jsonComboData, btnStatus = jsonBtnStatus });
        }
        
        private DataTable GetMonthlySalarySettingsFromDB(string date)
        {
            DataTable dtblMonthlySalaryDetails = new DataTable();
            try
            {
                MonthlySalarySP spMonthlySalary = new MonthlySalarySP();
                MonthlySalaryInfo infoMonthlySalary = new MonthlySalaryInfo();
                MonthlySalaryDetailsSP spMonthlySalaryDetails = new MonthlySalaryDetailsSP();
                MonthlySalaryDetailsInfo infoMonthlySalaryDetailsInfo = new MonthlySalaryDetailsInfo();
                dtblMonthlySalaryDetails = spMonthlySalary.MonthlySalarySettingsEmployeeViewAll(Convert.ToDateTime(date));
            }
            catch (Exception ex)
            {
            }
            return dtblMonthlySalaryDetails;
        }

        [HttpPost]
        public ActionResult SaveOrEditMonthlySalarySettings(string month, string btnStatus, string tableData)
        {
            string exStr = string.Empty;
            try
            {
                decimal decRowCount = 0;
                List<Dictionary<string, object>> items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tableData);
                decRowCount = items.Count;
                if (btnStatus == "Save")
                {
                    if (decRowCount > 0)
                    {
                        exStr = SaveMonthlySalarySettings(month, tableData);
                    }
                    else
                    {
                        return Json(new { success = "false", method = "save", ex = "no"});
                    }
                }
                else
                {
                    if (decRowCount > 0)
                    {
                        exStr = EditMonthlySalarySettings(month, tableData);
                    }
                    else
                    {
                        //MessageBox.Show("Can't Update Monthly salary settings without atleast one employee with complete details", "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return Json(new { success = "false", method = "update", ex = "no"});
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", method = "all", ex = "MSS9" + ex.Message});
            }
            return Json(new { success = "true", method = "all", ex = exStr });
        }

        public string SaveMonthlySalarySettings(string month, string tableData)
        {
            try
            {
                List<Dictionary<string, object>> items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tableData);
                
                MonthlySalarySP spMonthlySalary = new MonthlySalarySP();
                MonthlySalaryInfo infoMonthlySalary = new MonthlySalaryInfo();
                MonthlySalaryDetailsSP spMonthlySalaryDetails = new MonthlySalaryDetailsSP();
                MonthlySalaryDetailsInfo infoMonthlySalaryDetails = new MonthlySalaryDetailsInfo();
                infoMonthlySalary.SalaryMonth = Convert.ToDateTime(month);
                infoMonthlySalary.Narration = string.Empty;
                infoMonthlySalary.Extra1 = string.Empty;
                infoMonthlySalary.Extra2 = string.Empty;
                decimal decMasterIdForEdit = spMonthlySalary.MonthlySalaryAddWithIdentity(infoMonthlySalary);
                infoMonthlySalaryDetails.MonthlySalaryId = decMasterIdForEdit;
                int RowCount = items.Count;
                for (int i = 0; i < RowCount; i++)
                {
                    if (items[i]["employeeId"] != null)
                    {
                        infoMonthlySalaryDetails.EmployeeId = Convert.ToDecimal(items[i]["employeeId"]);
                        if (items[i]["selectedSalaryPackageId"].ToString() != string.Empty && items[i]["selectedSalaryPackageId"].ToString() != "0")
                        {
                            infoMonthlySalaryDetails.SalaryPackageId = Convert.ToDecimal(items[i]["selectedSalaryPackageId"].ToString());
                            infoMonthlySalaryDetails.Extra1 = string.Empty;
                            infoMonthlySalaryDetails.Extra2 = string.Empty;
                            infoMonthlySalaryDetails.MonthlySalaryId = decMasterIdForEdit;
                            spMonthlySalaryDetails.MonthlySalaryDetailsAddWithMonthlySalaryId(infoMonthlySalaryDetails);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "MSS7" + ex.Message;
            }
            return "no";
        }

        public string EditMonthlySalarySettings(string month, string tableData)
        {
            try
            {
                List<Dictionary<string, object>> items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tableData);
                decimal decMasterIdForEdit = 0;
                MonthlySalarySP spMonthlySalary = new MonthlySalarySP();
                MonthlySalaryInfo infoMonthlySalary = new MonthlySalaryInfo();
                MonthlySalaryDetailsSP spMonthlySalaryDetails = new MonthlySalaryDetailsSP();
                MonthlySalaryDetailsInfo infoMonthlySalaryDetails = new MonthlySalaryDetailsInfo();
                EmployeeSP spEmployee = new EmployeeSP();
                infoMonthlySalary.SalaryMonth = Convert.ToDateTime(month);
                infoMonthlySalary.Narration = string.Empty;
                infoMonthlySalary.Extra1 = string.Empty;
                infoMonthlySalary.Extra2 = string.Empty;
                int RowCount = items.Count;
                for (int i = 0; i < RowCount; i++)
                {
                    if (items[i]["monthlySalaryId"].GetType().Name != "JObject")
                    {
                        decMasterIdForEdit = Convert.ToDecimal(items[i]["monthlySalaryId"].ToString());
                    }
                }
                infoMonthlySalary.MonthlySalaryId = decMasterIdForEdit;
                spMonthlySalary.MonthlySalarySettingsEdit(infoMonthlySalary);
                infoMonthlySalaryDetails.MonthlySalaryId = decMasterIdForEdit;
                for (int i = 0; i <= RowCount - 1; i++)
                {
                    if (items[i]["monthlySalaryDetailsId"].GetType().Name != "JObject")
                    {
                        string st = items[i]["selectedSalaryPackageId"].ToString();
                        if (items[i]["selectedSalaryPackageId"].ToString() != "0")
                        {
                            if (items[i]["employeeId"].GetType().Name != "JObject" && items[i]["employeeId"].ToString() != string.Empty)
                            {
                                infoMonthlySalaryDetails.EmployeeId = Convert.ToDecimal(items[i]["employeeId"].ToString());
                            }
                            if (items[i]["monthlySalaryDetailsId"].GetType().Name != "JObject" && items[i]["monthlySalaryDetailsId"].ToString() != string.Empty)
                            {
                                infoMonthlySalaryDetails.MonthlySalaryDetailsId = Convert.ToDecimal(items[i]["monthlySalaryDetailsId"].ToString());
                            }
                            if (items[i]["monthlySalaryId"].GetType().Name != "JObject" && items[i]["monthlySalaryId"].ToString() != "0")
                            {
                                infoMonthlySalaryDetails.MonthlySalaryId = Convert.ToDecimal(items[i]["monthlySalaryId"].ToString());
                            }
                            if (items[i]["selectedSalaryPackageId"].GetType().Name != "JObject" && items[i]["selectedSalaryPackageId"].ToString() != "0")
                            {
                                infoMonthlySalaryDetails.SalaryPackageId = Convert.ToDecimal(items[i]["selectedSalaryPackageId"].ToString());
                                infoMonthlySalaryDetails.Extra1 = string.Empty;
                                infoMonthlySalaryDetails.Extra2 = string.Empty;
                                spEmployee.EmployeePackageEdit(infoMonthlySalaryDetails.EmployeeId, infoMonthlySalaryDetails.SalaryPackageId);
                                spMonthlySalaryDetails.MonthlySalaryDetailsEditUsingMasterIdAndDetailsId(infoMonthlySalaryDetails);
                            }
                        }
                        else
                        {
                            decimal decMonthlySalaryDetailsId = 0;
                            for (int j = 0; j < RowCount; j++)
                            {
                                if (items[j]["monthlySalaryDetailsId"].GetType().Name != "JObject" && items[j]["monthlySalaryDetailsId"].ToString() != string.Empty)
                                {
                                    if (items[j]["selectedSalaryPackageId"].ToString() == "0")
                                    {
                                        decMonthlySalaryDetailsId = Convert.ToDecimal(items[j]["monthlySalaryDetailsId"].ToString());
                                        spMonthlySalaryDetails.MonthlySalarySettingsDetailsIdDelete(decMonthlySalaryDetailsId);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (items[i]["employeeId"].GetType().Name != "JObject" && items[i]["employeeId"].ToString() != string.Empty)
                        {
                            infoMonthlySalaryDetails.EmployeeId = Convert.ToDecimal(items[i]["employeeId"].ToString());

                            if (items[i]["selectedSalaryPackageId"].GetType().Name != "JObject" && items[i]["selectedSalaryPackageId"].ToString() != "0")
                            {
                                infoMonthlySalaryDetails.SalaryPackageId = Convert.ToDecimal(items[i]["selectedSalaryPackageId"].ToString());
                                infoMonthlySalaryDetails.Extra1 = string.Empty;
                                infoMonthlySalaryDetails.Extra2 = string.Empty;
                                infoMonthlySalaryDetails.MonthlySalaryId = decMasterIdForEdit;
                                spMonthlySalaryDetails.MonthlySalaryDetailsAddWithMonthlySalaryId(infoMonthlySalaryDetails);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "MSS8" + ex.Message;
            }
            return "no";
        }

        [HttpPost]
        public ActionResult DeleteMonthlySalarySettings(string month)
        {
            try
            {
                MonthlySalaryDetailsSP spMonthlySalaryDetails = new MonthlySalaryDetailsSP();
                MonthlySalarySP spMonthlySalary = new MonthlySalarySP();
                spMonthlySalary.MonthlySalaryDeleteAll(spMonthlySalary.MonthlySalarySettingsMonthlySalaryIdSearchUsingSalaryMonth(Convert.ToDateTime(month)));
            }
            catch (Exception ex)
            {
                return Json(new { success = "false", ex = "MSS5" + ex.Message });
            }
            return Json(new { success = "true", ex = "no"});
        }
    }
}