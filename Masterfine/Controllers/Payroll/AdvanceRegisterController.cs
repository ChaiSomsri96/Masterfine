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

        public ActionResult AdvanceRegister()
        {
            ViewData["month"] = DateTime.UtcNow.ToString("yyyy-MM");
            return View();
        }

        [HttpPost]
        public ActionResult GetAttendanceReigsterDetails(string voucherNo, string employeeCode, string employeeName, string salaryMonth, string voucherType)
        {
            string jsonComboData = string.Empty;
            string jsonTableData = string.Empty;
            try
            {
                DataTable dtblAdvancePayment = new DataTable();
                AdvancePaymentSP spAdvanceRegister = new AdvancePaymentSP();
                if (voucherNo == null)
                    voucherNo = string.Empty;
                if (employeeCode == null)
                    employeeCode = string.Empty;
                if (employeeName == null)
                    employeeName = string.Empty;

                DateTime dateTime = Convert.ToDateTime(salaryMonth);
                salaryMonth = dateTime.ToString("MMMM yyyy");

                dtblAdvancePayment = spAdvanceRegister.AdvanceRegisterSearch(voucherNo, employeeCode, employeeName, salaryMonth, voucherType);
                jsonTableData = Utils.ConvertDataTabletoString(dtblAdvancePayment);

                AdvancePaymentSP spAdvancePaymentSP = new AdvancePaymentSP();
                DataTable dtblVoucherTypeName = new DataTable();
                dtblVoucherTypeName = spAdvancePaymentSP.VoucherTypeNameComboFillAdvanceRegister();
                DataRow dr = dtblVoucherTypeName.NewRow();
                dr[0] = "0";
                dr[1] = "All";
                dtblVoucherTypeName.Rows.InsertAt(dr, 0);
                jsonComboData = Utils.ConvertDataTabletoString(dtblVoucherTypeName);
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "AR2:" + ex.Message, tableData = string.Empty, comboData = string.Empty});
            }
            return Json(new { success = "true", ex= "no", tableData = jsonTableData, comboData = jsonComboData });
        }
    }
}