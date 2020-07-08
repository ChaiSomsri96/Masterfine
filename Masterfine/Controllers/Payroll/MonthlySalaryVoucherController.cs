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
        //---------Register
        public ActionResult MonthlySalaryVoucher()
        {
            ViewData["date"] = DateTime.UtcNow.ToString("yyyy-MM-dd");
            ViewData["dateMonth"] = DateTime.UtcNow.ToString("yyyy-MM");
            ViewData["decMonthlyVoucherTypeId"] = 27;
            ViewData["decMonthlySuffixPrefixId"] = 0;           
            return View(RegisterClear());
        }
      
        public DataTable RegisterClear()
        {
            DataTable dtbl = new DataTable();
            try
            {
                VoucherTypeNameComboFill();
                //TransactionsGeneralFill obj = new TransactionsGeneralFill();
                //obj.CashOrBankComboFill(cmbCashBankAC, false);
               
                DateTime txtVoucherDateFrom = DateTime.Now;
                DateTime txtVoucherDateTo = DateTime.Now;
                string strSalaryMonth = DateTime.Now.ToString("yyyy-MM");
                DateTime dtpSalaryMonth = Convert.ToDateTime(strSalaryMonth);
                SalaryVoucherMasterSP spSalaryVoucherMaster = new SalaryVoucherMasterSP();
                dtbl = spSalaryVoucherMaster.MonthlySalaryRegisterSearch(txtVoucherDateFrom, txtVoucherDateTo, dtpSalaryMonth, "", "All", "All");
                return dtbl;
            }
            catch (Exception ex)
            {
               // MessageBox.Show("MSR2:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return dtbl;
        }
       
        public void VoucherTypeNameComboFill()
        {
            try
            {
                DataTable dtblVoucherTyeName = new DataTable();
                VoucherTypeSP spVoucherType = new VoucherTypeSP();
                dtblVoucherTyeName = spVoucherType.VoucherTypeNameComboFill();
                DataRow dr = dtblVoucherTyeName.NewRow();
                dr[1] = "All";
                dtblVoucherTyeName.Rows.InsertAt(dr, 0);
                //cmbVoucherTypeName.DataSource = dtblVoucherTyeName;
                //cmbVoucherTypeName.ValueMember = "voucherTypeId";
                //cmbVoucherTypeName.DisplayMember = "voucherTypeName";
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MSR1:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalaryVoucherSearch(string dateFrom, string salaryMonth, string cash, string dateTo, string no, string type)
        {
            if (dateFrom == null)
                dateFrom = string.Empty;
            if (salaryMonth == null)
                salaryMonth = string.Empty;
            if (cash == null)
                cash = "All";
            if (dateTo == null)
                dateTo = string.Empty;
            if (no == null)
                no = string.Empty;
            if (type == null)
                type = "All";
            try
            {
                SalaryVoucherMasterSP spSalaryVoucherMaster = new SalaryVoucherMasterSP();
                DataTable dtbl = spSalaryVoucherMaster.MonthlySalaryRegisterSearch(Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateTo), Convert.ToDateTime(salaryMonth), no, cash, type);
                string jsonResult = Utils.ConvertDataTabletoString(dtbl);
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
        //-------------Voucher
        public DataTable GridFillVoucher(bool isEditMode, DateTime dtpMonth, string strVoucherNo)
        {
            DataTable dtbl = new DataTable();
            try
            {
                SalaryVoucherDetailsSP spSalaryVoucherDetails = new SalaryVoucherDetailsSP();
                
                string strMonth = dtpMonth.ToString("MMMMyyyy");
                string Month = strMonth.Substring(0, 3);
                string strmonthYear = Convert.ToDateTime(strMonth.ToString()).Year.ToString();
                string monthYear = Month + " " + strmonthYear;
                dtbl = spSalaryVoucherDetails.MonthlySalaryVoucherDetailsViewAll(strMonth, Month, monthYear, isEditMode, strVoucherNo);
                return dtbl;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MSV1:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return dtbl;

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetVoucherNo(string voucherNo, string voucherDate, string editMode)
        {
            DataTable dtbl = new DataTable();
            if (voucherNo == null)
                voucherNo = "0";
            
            try
            {
                //TransactionsGeneralFill Obj = new TransactionsGeneralFill();
                //Obj.CashOrBankComboFill(cash, false);

                if (Convert.ToBoolean(editMode) == false)
                {
                    voucherNo = VoucherClear(voucherNo, voucherDate, editMode);
                }
               
                return Json(new
                {
                    success = "true",
                    voucherNo
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = "false"});
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteMonthlySalaryVoucher(string id, string strVoucherNo, string dptMonth, string dptVoucherDate, string cash, string editMode)
        {
            DataTable dtbl = new DataTable();
            try
            {
                SalaryVoucherMasterSP spMaster = new SalaryVoucherMasterSP();
                SalaryVoucherDetailsSP spDetails = new SalaryVoucherDetailsSP();
                LedgerPostingSP spLedgerPosting = new LedgerPostingSP();
                decimal masterId = Convert.ToDecimal(id);
                spMaster.SalaryVoucherMasterDelete(masterId);
                spDetails.SalaryVoucherDetailsDeleteUsingMasterId(masterId);
                int decMonthlyVoucherTypeId = 27;
                spLedgerPosting.LedgerPostDelete(strVoucherNo, Convert.ToDecimal(decMonthlyVoucherTypeId));
                
                VoucherClear(strVoucherNo, dptVoucherDate, editMode);
                //dtbl = GridFillVoucher(Convert.ToBoolean(editMode), Convert.ToDateTime(dptMonth), strVoucherNo);
                return Json(new { error = "success" });
            }
            catch (Exception ex)
            {
                
                //MessageBox.Show("MSV16:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return Json(new { error = "failed" });
        }
        public string VoucherClear(string strVoucherNo, string dptVoucherDate, string editMode)
        {
            DataTable dtbl = new DataTable();
            try
            {
                if (Convert.ToBoolean(editMode) == false)
                {
                    decimal typeId = 27;
                    VoucherTypeSP spVoucherType = new VoucherTypeSP();
                    var isAutomatic = spVoucherType.CheckMethodOfVoucherNumbering(typeId);
                    return voucherNumberGeneration(typeId, strVoucherNo, dptVoucherDate);
                    /*if (isAutomatic)
                    {
                        return voucherNumberGeneration(typeId, strVoucherNo, dptVoucherDate);
                    }
                    else
                    {
                        return string.Empty;
                    }*/
                }

                //int inCount = dgvMonthlySalary.RowCount;
                //for (int i = 0; i < inCount; i++)
                //{
                //    dgvMonthlySalary.Rows[i].Cells["cmbStatus"].Value = null;
                //}
                
                return string.Empty;
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MSR2:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return string.Empty;
        }
        public string voucherNumberGeneration(decimal decMonthlyVoucherTypeId, string strVoucherNo, string dptVoucherDate)
        {
            try
            {
                //TransactionsGeneralFill obj = new TransactionsGeneralFill();
                SalaryVoucherMasterSP spMaster = new SalaryVoucherMasterSP();
                if (strVoucherNo == string.Empty || strVoucherNo == null)
                {
                    strVoucherNo = "0";
                }
                string tableName = "SalaryVoucherMaster";
                //-----------------------------------------Voucher number Automatic generation ------------------------------------------------//
                //strVoucherNo = obj.VoucherNumberAutomaicGeneration(decMonthlyVoucherTypeId, Convert.ToDecimal(strVoucherNo), dtpVoucherDate.Value, tableName);
                if (Convert.ToDecimal(strVoucherNo) != spMaster.SalaryVoucherMasterGetMaxPlusOne(decMonthlyVoucherTypeId))
                {
                    strVoucherNo = spMaster.SalaryVoucherMasterGetMax(decMonthlyVoucherTypeId).ToString();
                    //strVoucherNo = obj.VoucherNumberAutomaicGeneration(decMonthlyVoucherTypeId, Convert.ToDecimal(strVoucherNo), dtpVoucherDate.Value, tableName);
                    if (spMaster.SalaryVoucherMasterGetMax(decMonthlyVoucherTypeId) == "0")
                    {
                        strVoucherNo = "0";
                       // strVoucherNo = obj.VoucherNumberAutomaicGeneration(decMonthlyVoucherTypeId, Convert.ToDecimal(strVoucherNo), dptVoucherDate.Value, tableName);
                    }
                }
                VoucherTypeSP spVoucherType = new VoucherTypeSP();
                //var isAutomatic = spVoucherType.CheckMethodOfVoucherNumbering(decMonthlyVoucherTypeId);
                //if (isAutomatic)
                {
                    SuffixPrefixSP spSuffisprefix = new SuffixPrefixSP();
                    SuffixPrefixInfo infoSuffixPrefix = new SuffixPrefixInfo();

                    infoSuffixPrefix = spSuffisprefix.GetSuffixPrefixDetails(decMonthlyVoucherTypeId, Convert.ToDateTime(dptVoucherDate));
                    var strPrefix = infoSuffixPrefix.Prefix;
                    var strSuffix = infoSuffixPrefix.Suffix;
                    var strInvoiceNo = strPrefix + strVoucherNo + strSuffix;
                    return strInvoiceNo;
                    //txtVoucherNo.ReadOnly = true;
                }
                //return string.Empty;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MSV14:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return string.Empty;
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FillFunction(string id)
        {
            try
            {
                DataTable dtbl = new DataTable();
                SalaryVoucherMasterSP SpMaster = new SalaryVoucherMasterSP();
                SalaryVoucherMasterInfo InfoMaster = new SalaryVoucherMasterInfo();
                VoucherTypeSP spVoucherType = new VoucherTypeSP();
                decimal masterId = Convert.ToDecimal(id);
                InfoMaster = SpMaster.SalaryVoucherMasterView(masterId);

                var strVoucherNoforEdit = InfoMaster.VoucherNo;
                var strVoucherNo = InfoMaster.VoucherNo;
                var strInvoiceNo = InfoMaster.InvoiceNo;

                var txtVoucherDate = InfoMaster.Date.ToString("yyyy-MM-dd");
                var dtpMonth = Convert.ToDateTime(InfoMaster.Month.ToString("yyyy-MM"));
                
                var txtNarration = InfoMaster.Narration;
                string txtMonth = InfoMaster.Month.ToString("yyyy-MM");
                decimal decTotalAmont = Math.Round(Convert.ToDecimal(InfoMaster.TotalAmount.ToString()), PublicVariables._inNoOfDecimalPlaces);
                var lblTotalAmount = decTotalAmont.ToString();
                var decMonthlySuffixPrefixId = InfoMaster.SuffixPrefixId;
                var decMonthlyVoucherTypeId = InfoMaster.VoucherTypeId;
                ViewData["decMonthlyVoucherTypeId"] = decMonthlyVoucherTypeId;
                ViewData["decMonthlySuffixPrefixId"] = decMonthlySuffixPrefixId;
                var cmbCashOrBankAcc = InfoMaster.LedgerId;
                var isAutomatic = spVoucherType.CheckMethodOfVoucherNumbering(decMonthlyVoucherTypeId);
                bool txtInVoiceNoEnabled = false;
                if (isAutomatic)
                {
                    txtInVoiceNoEnabled = false;
                }
                else
                {
                    txtInVoiceNoEnabled = true;
                }
                bool dtpMonthEnabled = false;
                dtbl = GridFillVoucher(true, dtpMonth, strVoucherNo);
                string jsonResult = Utils.ConvertDataTabletoString(dtbl);
                return Json(new
                {
                    error = "false",
                    data = jsonResult,
                    voucherNo = strVoucherNo,
                    strInvoiceNo,
                    voucherDate = txtVoucherDate,
                    month = txtMonth,
                    cash = cmbCashOrBankAcc,
                    totalAmount = lblTotalAmount,
                    narration = txtNarration,
                    txtInVoiceNoEnabled,
                    dtpMonthEnabled,
                    masterID = masterId

                });
            }
            catch (Exception ex)
            {

            }
            return Json(new{error = "true"});
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefreshModalTable(string voucherNo, string month)
        {
            try
            {
                DataTable dtbl = new DataTable();
         
                DateTime dtpMonth = Convert.ToDateTime(month);
                //string strMonth = dtpMonth.ToString("MMMMyyyy");
               // string Month = strMonth.Substring(0, 3);
                dtbl = GridFillVoucher(false, dtpMonth, voucherNo);
                string jsonResult = Utils.ConvertDataTabletoString(dtbl);
                return Json(new
                {
                    error = "false",
                    data = jsonResult,
                });
            }
            catch (Exception ex)
            {

            }
            return Json(new { error = "true" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveOrEdit(string masterId, string tableData, string editMode)
        {
            try
            {
                List<Object> dgvMonthlySalary = JsonConvert.DeserializeObject<List<Object>>(tableData);
                JArray itemFirst = (JArray)dgvMonthlySalary[0];
                decimal masterID = Convert.ToDecimal(masterId);
                SalaryVoucherMasterSP spMaster = new SalaryVoucherMasterSP();
                bool isEditMode = Convert.ToBoolean(editMode);
                int decMonthlyVoucherTypeId = 27;
                decimal typeId = Convert.ToDecimal(decMonthlyVoucherTypeId);
                VoucherTypeSP spVoucherType = new VoucherTypeSP();
                var isAutomatic = spVoucherType.CheckMethodOfVoucherNumbering(typeId);
                
                string voucherNo = Convert.ToString(itemFirst[0]["voucherNo"].Value<string>());
                if (isEditMode == false)
                {
                    
                    if (PublicVariables.isMessageAdd)
                    {
                        if (!isAutomatic)
                        {
                            if (spMaster.MonthlySalaryVoucherCheckExistence(voucherNo, typeId, masterID) == false)
                            {
                                SaveFunction_MSV(tableData, editMode);
                            }
                        }
                        else
                        {
                            SaveFunction_MSV(tableData, editMode);
                        }
                    }
                    else
                    {
                        if (!isAutomatic)
                        {
                            if (spMaster.MonthlySalaryVoucherCheckExistence(voucherNo, typeId, masterID) == false)
                            {
                                SaveFunction_MSV(tableData, editMode);
                            }
                        }
                        else
                        {
                            SaveFunction_MSV(tableData, editMode);
                        }
                    }

                }
                //------ Update-------------//
                else if (isEditMode)
                {
                    if (PublicVariables.isMessageEdit)
                    {
                        if (!isAutomatic)
                        {
                            if (spMaster.MonthlySalaryVoucherCheckExistence(voucherNo, typeId, masterID) == false)
                            {
                                EditFunction_MSV(masterId, tableData, editMode);
                            }
                        }
                        else
                        {
                            EditFunction_MSV(masterId, tableData, editMode);
                        }
                    }
                    else
                    {
                        if (!isAutomatic)
                        {
                            if (spMaster.MonthlySalaryVoucherCheckExistence(voucherNo, typeId, masterID) == false)
                            {
                                EditFunction_MSV(masterId, tableData, editMode);
                            }
                        }
                        else
                        {
                            EditFunction_MSV(masterId, tableData, editMode);
                        }
                    }
                }
                return Json(new { error = "success" });
            }
            catch (Exception ex)
            {
               // MessageBox.Show("MSV6:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return Json(new { error = "error" });
        }
        public void SaveFunction_MSV(string tableData, string editMode)
        {
            try
            {
                List<Object> dgvMonthlySalary = JsonConvert.DeserializeObject<List<Object>>(tableData);
                JArray itemFirst = (JArray)dgvMonthlySalary[0];
                string voucherNo = Convert.ToString(itemFirst[0]["voucherNo"].Value<string>());
                string voucherDate = Convert.ToString(itemFirst[0]["voucherDate"].Value<string>());
                string month = Convert.ToString(itemFirst[0]["month"].Value<string>());
                string cash = Convert.ToString(itemFirst[0]["cash"].Value<string>());
                string totalAmount = Convert.ToString(itemFirst[0]["totalAmount"].Value<string>());
                string narration = Convert.ToString(itemFirst[0]["narration"].Value<string>());
                SalaryVoucherMasterSP spMaster = new SalaryVoucherMasterSP();
                SalaryVoucherMasterInfo infoMaster = new SalaryVoucherMasterInfo();
                SalaryVoucherDetailsSP spDetails = new SalaryVoucherDetailsSP();
                SalaryVoucherDetailsInfo infoDetails = new SalaryVoucherDetailsInfo();
                DateTime dtpMonth = Convert.ToDateTime(month);
                bool isEditMode = Convert.ToBoolean(editMode);
                int decMonthlyVoucherTypeId = 27;
                decimal typeId = Convert.ToDecimal(decMonthlyVoucherTypeId);
                VoucherTypeSP spVoucherType = new VoucherTypeSP();
                var isAutomatic = spVoucherType.CheckMethodOfVoucherNumbering(typeId);
                
                //------------------------------- In the case of multi user check whether salary is paying for the sam person ----------------//
                int inCounts = dgvMonthlySalary.Count;
                int incont = 0;
                decimal decVal = 0;
                string strEployeeNames = "";
                for (int i = 0; i < inCounts; i++)
                {
                    JArray item = (JArray)dgvMonthlySalary[i];
                    decVal = Convert.ToDecimal(item[0]["txtEmployeeId"].Value<string>());
                    if (spDetails.CheckWhetherSalaryAlreadyPaid(decVal, dtpMonth) != "0")
                    {
                        strEployeeNames = strEployeeNames + spDetails.CheckWhetherSalaryAlreadyPaid(decVal, dtpMonth) + ",";
                        foreach (char ch in strEployeeNames)
                        {
                            if (ch == ',')
                            {
                                incont++;
                            }
                        }
                        if (incont == 15)
                        {
                            incont = 0;
                            strEployeeNames = strEployeeNames + Environment.NewLine;
                        }

                    }
                }
                if (spDetails.CheckWhetherSalaryAlreadyPaid(decVal, dtpMonth) != "0")
                {

                   // Messages.InformationMessage("Salary already paid for - " + " " + strEployeeNames);
                    //GridFillVoucher(isEditMode, dtpMonth, voucherNo);
                }
                infoMaster.LedgerId = Convert.ToDecimal(cash);
                infoMaster.VoucherNo = voucherNo;
                infoMaster.Month = Convert.ToDateTime(dtpMonth);
                infoMaster.Date = Convert.ToDateTime(voucherDate);
                infoMaster.Narration = narration;
                infoMaster.InvoiceNo = voucherNo;
                if (totalAmount != string.Empty)
                {
                    infoMaster.TotalAmount = Math.Round(Convert.ToDecimal(totalAmount), PublicVariables._inNoOfDecimalPlaces);
                }
                infoMaster.Extra1 = string.Empty; // Fields not in design//
                infoMaster.Extra2 = string.Empty; // Fields not in design//
                infoMaster.SuffixPrefixId = Convert.ToDecimal(ViewData["decMonthlySuffixPrefixId"]);
                infoMaster.VoucherTypeId = typeId;
                infoMaster.FinancialYearId = PublicVariables._decCurrentFinancialYearId;

                int inCount = dgvMonthlySalary.Count;
                int inValue = 0;
                for (int i = 0; i < inCount; i++)
                {
                    JArray item = (JArray)dgvMonthlySalary[i];
                    if (Convert.ToString(item[0]["cmbStatus"].Value<string>()) == "Paid")
                    {
                        inValue++;
                    }
                }
                if (inValue > 0)
                {
                    //-------------------------In the case of Multi-User Check the VoucherNo. again (Max of VoucherNumber )---------------------//
                    DataTable dtbl = new DataTable();
                    decimal decMasterId = 0;
                    string strUpdatedVoucherNo = "", strUpdatedInvoiceNo = "";
                    dtbl = spMaster.MonthlySalaryVoucherMasterAddWithIdentity(infoMaster, isAutomatic);
                    foreach (DataRow dr in dtbl.Rows)
                    {
                        decMasterId = Convert.ToDecimal(dr.ItemArray[0].ToString());
                        strUpdatedVoucherNo = dr.ItemArray[1].ToString();
                        strUpdatedInvoiceNo = dr.ItemArray[2].ToString();
                    }
                    string strVoucherNo = "";
                    string strInvoiceNo = "";
                    if (!isAutomatic)
                    {
                        strVoucherNo = voucherNo;
                    }
                    if (isAutomatic)
                    {
                        if (strUpdatedVoucherNo != "" && Convert.ToDecimal(strUpdatedVoucherNo) != Convert.ToDecimal(voucherNo))
                        {
                            //Messages.InformationMessage("Voucher number changed from  " + strInvoiceNo + "  to  " + strUpdatedInvoiceNo);
                            strVoucherNo = strUpdatedVoucherNo.ToString();
                            strInvoiceNo = strUpdatedInvoiceNo;


                        }
                    }
                    //LedgerPosting(Convert.ToDecimal(cash));
                    infoDetails.Extra1 = string.Empty;
                    infoDetails.Extra2 = string.Empty;
                    infoDetails.SalaryVoucherMasterId = decMasterId;

                    int inRowCount = dgvMonthlySalary.Count;
                    for (int i = 0; i < inRowCount; i++)
                    {
                        JArray item = (JArray)dgvMonthlySalary[i];
                        if (item[0]["txtEmployeeId"] != null)
                        {
                            infoDetails.EmployeeId = Convert.ToDecimal(item[0]["txtEmployeeId"].Value<string>());
                        }
                        if (item[0]["txtBonus"] != null)
                        {
                            infoDetails.Bonus = Convert.ToDecimal(item[0]["txtBonus"].Value<string>());
                        }
                        if (item[0]["txtDeduction"] != null)
                        {
                            infoDetails.Deduction = Convert.ToDecimal(item[0]["txtDeduction"].Value<string>());
                        }
                        if (item[0]["txtAdvance"] != null)
                        {
                            infoDetails.Advance = Convert.ToDecimal(item[0]["txtAdvance"].Value<string>());
                        }
                        if (item[0]["txtLop"] != null)
                        {
                            infoDetails.Lop = Convert.ToDecimal(item[0]["txtLop"].Value<string>());
                        }
                        if (item[0]["txtSalary"] != null)
                        {
                            infoDetails.Salary = Convert.ToDecimal(item[0]["txtSalary"].Value<string>());
                        }
                        if (item[0]["cmbStatus"] != null)
                        {
                            infoDetails.Status = Convert.ToString(item[0]["cmbStatus"].Value<string>()) == "Paid"? "Paid": string.Empty;
                        }
                        if (Convert.ToString(item[0]["cmbStatus"].Value<string>()) == "Paid" && Convert.ToString(item[0]["txtMasterId"].Value<string>()) == "0")
                        {
                            infoDetails.SalaryVoucherMasterId = decMasterId;
                            spDetails.MonthlySalaryVoucherDetailsAdd(infoDetails);
                        }
                    }

                    //GridFillVoucher(isEditMode,dtpMonth, voucherNo);
                    VoucherClear(strVoucherNo, voucherDate, editMode);
                }
                else
                {
                    //Messages.InformationMessage("Can't save without atleast one employee");
                    voucherNo = spMaster.SalaryVoucherMasterGetMax(typeId);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MSV7:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// Function for edit
        /// </summary>
        public void EditFunction_MSV(string masterId, string tableData, string editMode)
        {
            try
            {
                List<Object> dgvMonthlySalary = JsonConvert.DeserializeObject<List<Object>>(tableData);
                JArray itemFirst = (JArray)dgvMonthlySalary[0];
                string voucherNo = Convert.ToString(itemFirst[0]["voucherNo"].Value<string>());
                string voucherDate = Convert.ToString(itemFirst[0]["voucherDate"].Value<string>());
                string month = Convert.ToString(itemFirst[0]["month"].Value<string>());
                string cash = Convert.ToString(itemFirst[0]["cash"].Value<string>());
                string totalAmount = Convert.ToString(itemFirst[0]["totalAmount"].Value<string>());
                string narration = Convert.ToString(itemFirst[0]["narration"].Value<string>());
                SalaryVoucherMasterSP spMaster = new SalaryVoucherMasterSP();
                SalaryVoucherMasterInfo infoMaster = new SalaryVoucherMasterInfo();
                SalaryVoucherDetailsSP spDetails = new SalaryVoucherDetailsSP();
                SalaryVoucherDetailsInfo infoDetails = new SalaryVoucherDetailsInfo();
                LedgerPostingSP spLedgerPosting = new LedgerPostingSP();
                decimal masterID = Convert.ToDecimal(masterId);
                infoMaster.SalaryVoucherMasterId = masterID;
                infoMaster.Date = Convert.ToDateTime(voucherDate);
                infoMaster.LedgerId = Convert.ToDecimal(cash);
                infoMaster.Narration = narration;
                infoMaster.TotalAmount = Convert.ToDecimal(totalAmount);
                bool isEditMode = Convert.ToBoolean(editMode);
                VoucherTypeSP spVoucherType = new VoucherTypeSP();
                int decMonthlyVoucherTypeId = 27;
                decimal typeId = Convert.ToDecimal(decMonthlyVoucherTypeId);
                var isAutomatic = spVoucherType.CheckMethodOfVoucherNumbering(typeId);
                infoMaster.VoucherNo = voucherNo;
                infoMaster.InvoiceNo = voucherNo;
            
                infoMaster.Extra1 = string.Empty;
                infoMaster.Extra2 = string.Empty;
                infoMaster.SuffixPrefixId = Convert.ToDecimal(ViewData["decMonthlySuffixPrefixId"]);
                infoMaster.VoucherTypeId = typeId;
                infoMaster.FinancialYearId = PublicVariables._decCurrentFinancialYearId;
                infoMaster.Month = Convert.ToDateTime(month);

                infoDetails.Extra1 = string.Empty;
                infoDetails.Extra2 = string.Empty;

                int inRowCount = dgvMonthlySalary.Count;
                for (int i = 0; i < inRowCount; i++)
                {

                    JArray item = (JArray)dgvMonthlySalary[i];
                    if (item[0]["txtEmployeeId"] != null)
                    {
                        infoDetails.EmployeeId = Convert.ToDecimal(item[0]["txtEmployeeId"].Value<string>());
                    }
                    if (item[0]["txtBonus"] != null)
                    {
                        infoDetails.Bonus = Convert.ToDecimal(item[0]["txtBonus"].Value<string>());
                    }
                    if (item[0]["txtDeduction"] != null)
                    {
                        infoDetails.Deduction = Convert.ToDecimal(item[0]["txtDeduction"].Value<string>());
                    }
                    if (item[0]["txtAdvance"] != null)
                    {
                        infoDetails.Advance = Convert.ToDecimal(item[0]["txtAdvance"].Value<string>());
                    }
                    if (item[0]["txtLop"] != null)
                    {
                        infoDetails.Lop = Convert.ToDecimal(item[0]["txtLop"].Value<string>());
                    }
                    if (item[0]["txtSalary"] != null)
                    {
                        infoDetails.Salary = Convert.ToDecimal(item[0]["txtSalary"].Value<string>());
                    }
                    if (item[0]["cmbStatus"] != null && Convert.ToString(item[0]["cmbStatus"].Value<string>()) == "Paid")
                    {
                        infoDetails.Status = Convert.ToString(item[0]["cmbStatus"].Value<string>());
                    }


                    if (Convert.ToString(item[0]["cmbStatus"].Value<string>()) == "Pending" && Convert.ToString(item[0]["txtMasterId"].Value<string>()) != "0")
                    {
                        decimal SalaryVoucherDetailsId = Convert.ToDecimal(item[0]["txtDetailsId"].Value<string>());
                        spDetails.SalaryVoucherDetailsDelete(SalaryVoucherDetailsId);
                        spMaster.SalaryVoucherMasterEdit(infoMaster);

                        //LedgerUpdate();

                    }

                    if (Convert.ToString(item[0]["cmbStatus"].Value<string>()) == "Paid" && Convert.ToString(item[0]["txtMasterId"].Value<string>()) == "0")
                    {
                        infoDetails.SalaryVoucherMasterId = masterID;
                        spDetails.MonthlySalaryVoucherDetailsAdd(infoDetails);
                        spMaster.SalaryVoucherMasterEdit(infoMaster);

                        //LedgerUpdate();
                    }
                    if (Convert.ToString(item[0]["cmbStatus"].Value<string>()) == "Pending" && Convert.ToString(item[0]["txtMasterId"].Value<string>()) == "0")
                    {
                        spMaster.SalaryVoucherMasterEdit(infoMaster);
                        //LedgerUpdate();
                    }
                }
                if (spDetails.SalaryVoucherDetailsCount(masterID) == 0)
                {
                    spMaster.SalaryVoucherMasterDelete(masterID);
                }
                //GridFillVoucher(isEditMode, Convert.ToDateTime(month), voucherNo);

            }
            catch (Exception ex)
            {
                //MessageBox.Show("MSV8:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetTotalAmount(string tableData)
        {
            try
            {
                string totalAmount = "";
                List<Object> dgvMonthlySalary = JsonConvert.DeserializeObject<List<Object>>(tableData);
                decimal decTotal = 0;
                int inCounts = dgvMonthlySalary.Count;
                if (inCounts > 0)
                {
                    for (int i = 0; i < inCounts; i++)
                    {
                        JArray item = (JArray)dgvMonthlySalary[i];
                        if (item[0]["txtSalary"] != null && Convert.ToString(item[0]["txtSalary"].Value<string>()) != string.Empty)
                        {

                            if (item[0]["cmbStatus"] != null)
                            {
                                if (Convert.ToString(item[0]["cmbStatus"].Value<string>()) == "Paid")
                                {
                                    decTotal += Convert.ToDecimal(item[0]["txtSalary"].Value<string>());
                                }
                            }
                        }

                    }
                }

                if (decTotal == 0)
                {
                    totalAmount = "0.00";
                }
                else
                {
                    totalAmount = Math.Round(Convert.ToDecimal(decTotal.ToString()), PublicVariables._inNoOfDecimalPlaces).ToString();
                }
                return Json(new { error = "success", total = totalAmount });
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MSV15:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return Json(new { error = "error"});
        }
    }
}