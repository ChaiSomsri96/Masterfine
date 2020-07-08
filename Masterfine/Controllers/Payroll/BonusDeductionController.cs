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

        [HttpPost]
        public ActionResult BonusDeduction_Load()
        {
            string jsonComboData = string.Empty;
            string jsonDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            try
            {
                DataTable employeeCodeCombo = EmployeeCodeComboFill();
                jsonComboData = Utils.ConvertDataTabletoString(employeeCodeCombo);
                
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "BD1:" + ex.Message});
            }
            return Json(new { success = "true", ex= "no", comboData = jsonComboData, today = jsonDate });
        }

        [HttpPost]
        public ActionResult BonusDeductionEdit_Load(string bonusDeductionId)
        {
            string jsonComboData = string.Empty;
            decimal decBonusDeductionId = Convert.ToDecimal(bonusDeductionId);
            BonusDedutionInfo infoBonusDeduction = new BonusDedutionInfo();
            try
            {
                BonusDedutionSP spBonusDeduction = new BonusDedutionSP();
                infoBonusDeduction = spBonusDeduction.BonusDeductionViewForUpdate(decBonusDeductionId);
                DataTable employeeCodeCombo = EmployeeCodeComboFill();
                jsonComboData = Utils.ConvertDataTabletoString(employeeCodeCombo);
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "BD2" + ex.Message });
            }
            return Json(new { success = "true",
                ex = "no",
                data = new {
                    date = infoBonusDeduction.Date.ToString("yyyy-MM-dd"),
                    employeeCodes = jsonComboData,
                    employeeId = infoBonusDeduction.EmployeeId,
                    month = infoBonusDeduction.Month.ToString("yyyy-MM"),
                    bonusAmount = infoBonusDeduction.BonusAmount,
                    deductionAmount = infoBonusDeduction.DeductionAmount,
                    narration = infoBonusDeduction.Narration
                }
            });
        }

        [HttpPost]
        public ActionResult DeleteBonusDeduction(string employeeId, string month)
        {
            string message = string.Empty;
            try
            {                
                BonusDedutionSP spBonusDeduction = new BonusDedutionSP();
                if ((spBonusDeduction.BonusDeductionReferenceDelete(Convert.ToDecimal(employeeId), Convert.ToDateTime(month))) == -1)
                {
                    message = "Can't delete,reference exist";
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "BD3:" + ex.Message });
            }
            return Json(new { success = "true", ex = "no", message });
        }

        public DataTable EmployeeCodeComboFill()
        {
            DataTable dtblEmployeeCode = new DataTable();
            try
            {
                EmployeeSP spEmployee = new EmployeeSP();
                dtblEmployeeCode = spEmployee.EmployeeViewAll();
            }
            catch (Exception ex)
            {
            }
            return dtblEmployeeCode;
        }

        [HttpPost]
        public ActionResult SaveOrEditBonusDeduction(string btnStatus, string employeeId, string date, string month,
            string bonusAmount, string deductionAmount, string narration, string bonusId)
        {
            string message = string.Empty;
            try
            {
                if (btnStatus == "Save")
                {
                    message = SaveFunction(employeeId, date, month, bonusAmount, deductionAmount, narration);
                }
                else
                {
                    message = EditFunction(employeeId, date, month, bonusAmount, deductionAmount, narration, bonusId);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "BD2:" + ex.Message });
            }
            return Json(new { success = "true", ex = "no", message });
        }

        public string SaveFunction(string employeeId, string date, string month,
            string bonusAmount, string deductionAmount, string narration)
        {
            string message = string.Empty;
            try
            {
                BonusDedutionInfo infoBonusDeduction = new BonusDedutionInfo();
                BonusDedutionSP spBonusDeduction = new BonusDedutionSP();
                infoBonusDeduction.Date = Convert.ToDateTime(date);
                infoBonusDeduction.EmployeeId = Convert.ToDecimal(employeeId);
                infoBonusDeduction.Month = Convert.ToDateTime(month);
                infoBonusDeduction.BonusAmount = Convert.ToDecimal(bonusAmount);
                infoBonusDeduction.DeductionAmount = Convert.ToDecimal(deductionAmount);
                infoBonusDeduction.Narration = narration;
                infoBonusDeduction.Extra1 = string.Empty;
                infoBonusDeduction.Extra2 = string.Empty;
                if (!spBonusDeduction.BonusDeductionAddIfNotExist(infoBonusDeduction))
                {
                    message = "Employee bonus deduction already exist";
                }
            }
            catch (Exception ex)
            {
            }
            return message;
        }
        /// <summary>
        /// Function to edit
        /// </summary>
        public string EditFunction(string employeeId, string date, string month,
            string bonusAmount, string deductionAmount, string narration, string bonusId)
        {
            string message = string.Empty;
            try
            {
                BonusDedutionInfo infoBonusDeduction = new BonusDedutionInfo();
                BonusDedutionSP spBonusDeduction = new BonusDedutionSP();
                infoBonusDeduction.Date = Convert.ToDateTime(date);
                infoBonusDeduction.EmployeeId = Convert.ToDecimal(employeeId);
                infoBonusDeduction.Month = Convert.ToDateTime(month);
                infoBonusDeduction.BonusAmount = Convert.ToDecimal(bonusAmount);
                infoBonusDeduction.DeductionAmount = Convert.ToDecimal(deductionAmount);
                infoBonusDeduction.Narration = narration;
                infoBonusDeduction.Extra1 = string.Empty;
                infoBonusDeduction.Extra2 = string.Empty;
                infoBonusDeduction.BonusDeductionId = Convert.ToDecimal(bonusId);
                spBonusDeduction.BonusDedutionEdit(infoBonusDeduction);
            }
            catch (Exception ex)
            {
            }
            return message;
        }
    }
}