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

        public ActionResult BonusDeduction()
        {
            ViewData["month"] = DateTime.UtcNow.ToString("yyyy-MM");
            return View();
        }

        [HttpPost]
        public ActionResult GetBonusDeductionRegister(string employeeName, string month)
        {
            string jsonComboData = string.Empty;
            string jsonTableData = string.Empty;
            try
            {
                DataTable dtblBonusDeduction = new DataTable();
                BonusDedutionSP spBonusDeduction = new BonusDedutionSP();
                dtblBonusDeduction = spBonusDeduction.BonusDeductionSearch(employeeName, Convert.ToDateTime(month));
                jsonTableData = Utils.ConvertDataTabletoString(dtblBonusDeduction);
                DataTable employeeNameCombo = EmployeeNameComboFill();
                jsonComboData = Utils.ConvertDataTabletoString(employeeNameCombo);
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "BDR1:" + ex.Message});
            }
            return Json(new { success = "true", ex= "no", tableData = jsonTableData, comboData = jsonComboData });
        }

        public DataTable EmployeeNameComboFill()
        {
            DataTable dtblEmployeeName = new DataTable();
            try
            {
                EmployeeSP spEmployee = new EmployeeSP();
                dtblEmployeeName = spEmployee.EmployeeViewAll();
            }
            catch (Exception ex)
            {
            }
            return dtblEmployeeName;
        }
    }
}