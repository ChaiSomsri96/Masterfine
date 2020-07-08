using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Masterfine.Models;
using Newtonsoft.Json;

namespace Masterfine.Controllers
{
    public partial class PayrollController : Controller
    {

        public ActionResult DailySalaryVoucher()
        {
            ViewData["date"] = DateTime.UtcNow.ToString("yyyy-MM-dd");
            return View();
        }
        
        //added by KimJK
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetDailySalaryVoucher(string dailySalaryMasterId)
        {
            bool isSuccess = true;
            string message = "success";
            string jsonResults = "";
            string jsonCombo = "";
            DailySalaryVoucherMasterInfo info = new DailySalaryVoucherMasterInfo();
            bool isAutomatic = false;
            try
            {
                DailySalaryVoucherMasterSP SpMaster = new DailySalaryVoucherMasterSP();
                VoucherTypeSP spVoucherType = new VoucherTypeSP();
                DataTable dtbl = new DataTable();
                DailySalaryVoucherDetailsSP spDetails = new DailySalaryVoucherDetailsSP();
                int nDailySalaryMasterId = int.Parse(dailySalaryMasterId);
                info = SpMaster.DailySalaryVoucherViewFromRegister(nDailySalaryMasterId);
                info.DailySalaryVoucehrMasterId = nDailySalaryMasterId;
                isAutomatic = spVoucherType.CheckMethodOfVoucherNumbering(info.VoucherTypeId);
                bool isEditMode = nDailySalaryMasterId > 0 ? true : false;
                dtbl = spDetails.DailySalaryVoucherDetailsGridViewAll(info.SalaryDate.ToString(), isEditMode, info.VoucherNo);
                jsonResults = Utils.ConvertDataTabletoString(dtbl);

                DailySalaryVoucherMasterSP spMaster = new DailySalaryVoucherMasterSP();
                DataTable newDtbl = new DataTable();
                newDtbl = spMaster.DailySalaryVoucherCashOrBankLedgersComboFill();
                jsonCombo = Utils.ConvertDataTabletoString(newDtbl);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
            }

            return Json(new  {
                            isSuccess = isSuccess,
                            message = message,
                            data = new
                            {
                                dailySalaryMasterId = info.DailySalaryVoucehrMasterId,
                                voucherNo = info.VoucherNo,
                                invoiceNo = info.InvoiceNo,
                                date = info.Date.ToString("yyyy-MM-dd"),
                                salaryDate = info.SalaryDate.ToString("yyyy-MM-dd"),
                                totalAmount = info.TotalAmount,
                                narration = info.Narration,
                                suffixPrefixId = info.SuffixPrefixId,
                                voucherTypeId = info.VoucherTypeId,
                                ledgerId = info.LedgerId,
                                jsonDetails = jsonResults,
                                jsonCombo = jsonCombo
                            }
                        });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrEditDailySalaryVoucher(string dailyMasterId, string voucherTypeId, string voucherNo, string date,
            string salaryDate, string cash, string narration, string totalAmount, string details)
        {
            bool isSuccess = true;
            string message = string.Empty;
            try
            {
                int nDailyMasterId = int.Parse(dailyMasterId);
                if (nDailyMasterId > 0)
                    message = EditDailySalaryVoucher(dailyMasterId, voucherTypeId, voucherNo, date, salaryDate, cash, narration, totalAmount, details);
                else
                    message = SaveDailySalaryVoucher(dailyMasterId, voucherTypeId, voucherNo, date, salaryDate, cash, narration, totalAmount, details);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
            }
            return Json(new { isSuccess, message});
        }


        public string  EditDailySalaryVoucher(string dailyMasterId, string voucherTypeId, string voucherNo, string date, 
            string salaryDate, string cash, string narration, string totalAmount, string details)
        {
            string message = string.Empty;
            try
            {
                DailySalaryVoucherMasterInfo infoMaster = new DailySalaryVoucherMasterInfo();
                DailySalaryVoucherMasterSP spMaster = new DailySalaryVoucherMasterSP();
                DailySalaryVoucherDetailsInfo infoDetails = new DailySalaryVoucherDetailsInfo();
                DailySalaryVoucherDetailsSP spDetails = new DailySalaryVoucherDetailsSP();
                LedgerPostingSP spLedgerPosting = new LedgerPostingSP();

                infoMaster.DailySalaryVoucehrMasterId = Convert.ToDecimal(dailyMasterId);
                infoMaster.Date = Convert.ToDateTime(date);
                infoMaster.LedgerId = Convert.ToDecimal(cash);
                infoMaster.Narration = narration;
                infoMaster.TotalAmount = Convert.ToDecimal(totalAmount);
                infoMaster.VoucherNo = voucherNo;
                infoMaster.InvoiceNo = voucherNo;

                infoMaster.Extra1 = string.Empty;
                infoMaster.Extra2 = string.Empty;
                infoMaster.SuffixPrefixId = 0;//decDailySuffixPrefixId;
                infoMaster.VoucherTypeId = Convert.ToDecimal(voucherTypeId);
                infoMaster.FinancialYearId = PublicVariables._decCurrentFinancialYearId;
                infoMaster.SalaryDate = Convert.ToDateTime(salaryDate);
                infoDetails.Extra1 = string.Empty;// Fields not in design//
                infoDetails.Extra2 = string.Empty;// Fields not in design//
                spMaster.DailySalaryVoucherMasterEdit(infoMaster);
                List<Dictionary<string, object>> detailsData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(details);

                int inRowCount = detailsData.Count;
                for (int i = 0; i < inRowCount; i++)
                {
                    if (detailsData[i]["employeeId"].GetType().Name != "JObject"  && detailsData[i]["employeeId"].ToString() != string.Empty)
                    {
                        infoDetails.EmployeeId = Convert.ToDecimal(detailsData[i]["employeeId"].ToString());
                    }
                    if (detailsData[i]["dailyWage"].GetType().Name != "JObject" && detailsData[i]["dailyWage"].ToString() != string.Empty)
                    {
                        infoDetails.Wage = Convert.ToDecimal(detailsData[i]["dailyWage"].ToString());
                    }
                    if (detailsData[i]["status"].GetType().Name != "JObject" && detailsData[i]["status"].ToString() == "paid")
                    {
                        infoDetails.Status = detailsData[i]["status"].ToString();
                    }

                    if (detailsData[i]["status"].ToString() == "pending" && detailsData[i]["masterId"].GetType().Name != "JObject")
                    {
                        decimal DailySalaryVoucherDetailsId = Convert.ToDecimal(detailsData[i]["detailsId"].ToString());
                        spDetails.DailySalaryVoucherDetailsDelete(DailySalaryVoucherDetailsId);

                        LedgerUpdate(infoMaster);
                    }
                    else if (detailsData[i]["status"].ToString() == "pending" && detailsData[i]["masterId"].GetType().Name == "JObject")
                    {
                        spMaster.DailySalaryVoucherMasterEdit(infoMaster);
                        LedgerUpdate(infoMaster);
                    }
                    else if (detailsData[i]["status"].ToString() == "paid" && detailsData[i]["masterId"].GetType().Name == "JObject")
                    {
                        infoDetails.DailySalaryVocherMasterId = infoMaster.DailySalaryVoucehrMasterId;
                        spDetails.DailySalaryVoucherDetailsAdd(infoDetails);
                        //spMaster.DailySalaryVoucherMasterEdit(infoMaster);
                        LedgerUpdate(infoMaster);
                    }
                    
                }
                if (spDetails.DailySalaryVoucherDetailsCount(infoMaster.DailySalaryVoucehrMasterId) == 0)
                {
                    spMaster.DailySalaryVoucherMasterDelete(infoMaster.DailySalaryVoucehrMasterId);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public string SaveDailySalaryVoucher(string dailyMasterId, string voucherTypeId, string voucherNo, string date,
            string salaryDate, string cash, string narration, string totalAmount, string details)
        {
            string message = string.Empty;
            try
            {
                decimal decDailyVoucherTypeId = 0;
                VoucherTypeSP spVoucherType = new VoucherTypeSP();
                DataTable dtblVouchetType = new DataTable();
                dtblVouchetType = spVoucherType.VoucherTypeSelectionComboFill("Daily Salary Voucher");
                int nCount = dtblVouchetType.Rows.Count;
                string voucherType = string.Empty;
                if (nCount == 1)
                {
                    voucherType = dtblVouchetType.Rows[0].ItemArray[0].ToString();
                    decDailyVoucherTypeId = decimal.Parse(voucherType);
                }

                string strEployeeNames = string.Empty;
                DailySalaryVoucherMasterInfo infoMaster = new DailySalaryVoucherMasterInfo();
                DailySalaryVoucherMasterSP spMaster = new DailySalaryVoucherMasterSP();
                DailySalaryVoucherDetailsInfo infoDetails = new DailySalaryVoucherDetailsInfo();
                DailySalaryVoucherDetailsSP spDetails = new DailySalaryVoucherDetailsSP();

                //-------------In multi user case check whether salary is paying for the same persone--------------//
                List<Dictionary<string, object>> detailsData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(details);
                int inCounts = detailsData.Count;
                int incont = 0;
                decimal decVal = 0;
                DateTime dtSalaryDate = Convert.ToDateTime(salaryDate);
                for (int i = 0; i < inCounts; i++)
                {
                    decVal = Convert.ToDecimal(detailsData[i]["employeeId"].ToString());
                    if (spDetails.CheckWhetherDailySalaryAlreadyPaid(decVal, dtSalaryDate) != "0")
                    {
                        strEployeeNames = strEployeeNames + spDetails.CheckWhetherDailySalaryAlreadyPaid(decVal, dtSalaryDate) + ",";
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
                if (spDetails.CheckWhetherDailySalaryAlreadyPaid(decVal, dtSalaryDate) != "0")
                {
                    message = "Salary already paid for - " + " " + strEployeeNames;
                }
                else
                {
                    DateTime dtDate = Convert.ToDateTime(date);
                    if (isAutomatic)
                    {
                        infoMaster.VoucherNo = strVoucherNo;
                    }
                    else
                    {
                        infoMaster.VoucherNo = voucherNo;
                    }
                    infoMaster.Date = dtDate;
                    infoMaster.SalaryDate = dtSalaryDate;
                    infoMaster.LedgerId = Convert.ToDecimal(cash);
                    infoMaster.Narration = narration ?? string.Empty;
                    infoMaster.TotalAmount = Convert.ToDecimal(totalAmount);
                    infoMaster.Extra1 = string.Empty; // Fields not in design//
                    infoMaster.Extra2 = string.Empty; // Fields not in design//
                    if (isAutomatic)
                    {
                        infoMaster.InvoiceNo = strInvoiceNo;
                    }
                    else
                    {
                        infoMaster.InvoiceNo = voucherNo;
                    }
                    infoMaster.SuffixPrefixId = 0;//decDailySuffixPrefixId;
                    infoMaster.VoucherTypeId = decDailyVoucherTypeId;
                    infoMaster.FinancialYearId = PublicVariables._decCurrentFinancialYearId;

                    int inval = 0;
                    int inCount = detailsData.Count;
                    for (int i = 0; i < inCount; i++)
                    {
                        if (detailsData[i]["status"].ToString() == "paid")
                        {
                            inval++;
                        }

                    }
                    if (inval >= 0)
                    {
                        decimal decMasterId = 0;
                        string strUpdatedVoucherNo = string.Empty;
                        string strUpdatedInvoiceNo = string.Empty;
                        //-------------checks Voucher No. repeating in Multi user case----------//
                        DataTable dtbl = new DataTable();
                        dtbl = spMaster.DailySalaryVoucherMasterAddWithIdentity(infoMaster, true);
                        foreach (DataRow dr in dtbl.Rows)
                        {
                            decMasterId = Convert.ToDecimal(dr.ItemArray[0].ToString());
                            strUpdatedVoucherNo = dr.ItemArray[1].ToString();
                            strUpdatedInvoiceNo = dr.ItemArray[2].ToString();
                        }
                        if (!isAutomatic)
                        {
                            strVoucherNo = voucherNo;
                        }
                        if (isAutomatic)
                        {
                            if (Convert.ToDecimal(strUpdatedVoucherNo) != Convert.ToDecimal(strVoucherNo))
                            {
                                message = "Voucher number changed from  " + strInvoiceNo + "  to  " + strUpdatedInvoiceNo;
                                strVoucherNo = strUpdatedVoucherNo.ToString();
                                strInvoiceNo = strUpdatedInvoiceNo;
                            }
                        }
                        //-------------------------------------//
                        LedgerPostingForDailySalary(Convert.ToDecimal(cash), totalAmount, decDailyVoucherTypeId, voucherNo, date);

                        infoDetails.DailySalaryVocherMasterId = decMasterId;
                        infoDetails.Extra1 = string.Empty;// Fields not in design//
                        infoDetails.Extra2 = string.Empty;// Fields not in design//
                        int inRowCount = detailsData.Count;
                        for (int i = 0; i < inRowCount; i++)
                        {

                            if (detailsData[i]["employeeId"].GetType().Name != "JObject" && detailsData[i]["employeeId"].ToString() != string.Empty)
                            {
                                infoDetails.EmployeeId = Convert.ToDecimal(detailsData[i]["employeeId"].ToString());
                            }
                            if (detailsData[i]["dailyWage"].GetType().Name != "JObject" && detailsData[i]["dailyWage"].ToString() != string.Empty)
                            {
                                infoDetails.Wage = Convert.ToDecimal(detailsData[i]["dailyWage"].ToString());

                            }
                            if (detailsData[i]["status"].GetType().Name != "JObject" && detailsData[i]["status"].ToString() == "paid")
                            {
                                infoDetails.Status = detailsData[i]["status"].ToString();

                            }

                            if (detailsData[i]["status"].ToString() == "paid" && detailsData[i]["masterId"].ToString() == string.Empty)
                            {
                                infoDetails.DailySalaryVocherMasterId = decMasterId;
                                spDetails.DailySalaryVoucherDetailsAdd(infoDetails);
                            }
                        }
                    }
                    else
                    {
                        strVoucherNo = spMaster.DailySalaryVoucherMasterGetMax(Convert.ToDecimal(decDailyVoucherTypeId));
                        message = "Can't save without at least one employee";
                    }
                }
            }
            catch (Exception ex)
            {
                message = "DSV11:" + ex.Message;
            }
            return message;
        }

        public void LedgerPostingForDailySalary(decimal decid, string totalAmount, decimal decVoucherTypeId, string voucherNo, string date)
        {
            try
            {
                LedgerPostingSP spLedgerPosting = new LedgerPostingSP();
                LedgerPostingInfo infoLedgerPosting = new LedgerPostingInfo();

                infoLedgerPosting.Debit = 0;
                infoLedgerPosting.Credit = Convert.ToDecimal(totalAmount);
                infoLedgerPosting.VoucherTypeId = decVoucherTypeId;
                if (isAutomatic)
                {
                    infoLedgerPosting.VoucherNo = strVoucherNo;
                }
                else
                {
                    infoLedgerPosting.VoucherNo = voucherNo;
                }
                infoLedgerPosting.Date = Convert.ToDateTime(date);
                infoLedgerPosting.LedgerId = decid;
                infoLedgerPosting.DetailsId = 0;
                if (isAutomatic)
                {
                    infoLedgerPosting.InvoiceNo = strInvoiceNo;
                }
                else
                {
                    infoLedgerPosting.InvoiceNo = voucherNo;
                }
                infoLedgerPosting.ChequeNo = string.Empty;
                infoLedgerPosting.ChequeDate = DateTime.Now;
                infoLedgerPosting.YearId = PublicVariables._decCurrentFinancialYearId;
                infoLedgerPosting.Extra1 = string.Empty;
                infoLedgerPosting.Extra2 = string.Empty;

                spLedgerPosting.LedgerPostingAdd(infoLedgerPosting);

                infoLedgerPosting.Debit = Convert.ToDecimal(totalAmount);
                infoLedgerPosting.Credit = 0;
                infoLedgerPosting.VoucherTypeId = decVoucherTypeId;
                if (isAutomatic)
                {
                    infoLedgerPosting.VoucherNo = strVoucherNo;
                }
                else
                {
                    infoLedgerPosting.VoucherNo = voucherNo;
                }
                infoLedgerPosting.Date = PublicVariables._dtCurrentDate;
                infoLedgerPosting.LedgerId = 4;
                infoLedgerPosting.DetailsId = 0;
                if (isAutomatic)
                {
                    infoLedgerPosting.InvoiceNo = strInvoiceNo;
                }
                else
                {
                    infoLedgerPosting.InvoiceNo = voucherNo;
                }
                infoLedgerPosting.ChequeNo = string.Empty;
                infoLedgerPosting.ChequeDate = DateTime.Now;
                infoLedgerPosting.YearId = PublicVariables._decCurrentFinancialYearId;
                infoLedgerPosting.Extra1 = string.Empty;
                infoLedgerPosting.Extra2 = string.Empty;
                spLedgerPosting.LedgerPostingAdd(infoLedgerPosting);

            }
            catch (Exception ex)
            {
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDailySalaryVoucherForModal(string salaryDate)
        {
            bool isSuccess = true;
            string message = string.Empty;
            string jsonResult = string.Empty;
            string jsonCombo = string.Empty;
            string strVoucherNo = string.Empty;
            string strTableName = "DailySalaryVoucherMaster";
            try
            {
                VoucherTypeSP spVoucherType = new VoucherTypeSP();
                DataTable dtblVouchetType = new DataTable();
                dtblVouchetType = spVoucherType.VoucherTypeSelectionComboFill("Daily Salary Voucher");
                int inCount = dtblVouchetType.Rows.Count;
                string voucherType = string.Empty;
                if (inCount == 1)
                {
                    voucherType = dtblVouchetType.Rows[0].ItemArray[0].ToString();
                    decimal decDailyVoucherTypeId = decimal.Parse(voucherType);

                    TransactionsGeneralFill TransactionsGeneralFillobj = new TransactionsGeneralFill();
                    DailySalaryVoucherMasterSP spmaster = new DailySalaryVoucherMasterSP();
                    strVoucherNo = spmaster.SalaryVoucherMasterGetMaxPlusOne(decDailyVoucherTypeId).ToString();
                    strVoucherNo = TransactionsGeneralFillobj.VoucherNumberAutomaicGeneration(decDailyVoucherTypeId, Convert.ToDecimal(strVoucherNo), Convert.ToDateTime(salaryDate), strTableName);
                }

                DataTable dtbl = DailySalaryVoucherDetailsGridfill(false, "0", salaryDate);
                jsonResult = Utils.ConvertDataTabletoString(dtbl);

                DailySalaryVoucherMasterSP spMaster = new DailySalaryVoucherMasterSP();
                DataTable newDtbl = new DataTable();
                newDtbl = spMaster.DailySalaryVoucherCashOrBankLedgersComboFill();
                jsonCombo = Utils.ConvertDataTabletoString(newDtbl);
            }
            catch(Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
            }
            return Json(new { isSuccess, message, voucherNo = strVoucherNo, data = jsonResult, jsonCombo });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDailySalaryVoucher(string dailyMasterId, string voucherTypeId, string voucherNo, string salaryDate)
        {
            bool isSuccess = true;
            string message = string.Empty;
            try
            {
                DailySalaryVoucherMasterSP spMaster = new DailySalaryVoucherMasterSP();
                DailySalaryVoucherDetailsSP spDetails = new DailySalaryVoucherDetailsSP();
                LedgerPostingSP spLedgerPosting = new LedgerPostingSP();

                spMaster.DailySalaryVoucherMasterDelete(Convert.ToDecimal(dailyMasterId));
                spDetails.DailySalaryVoucherDetailsDeleteUsingMasterId(Convert.ToDecimal(dailyMasterId));
                spLedgerPosting.LedgerPostDelete(voucherNo, Convert.ToDecimal(voucherTypeId));
                DataTable dtbl = DailySalaryVoucherDetailsGridfill(true, voucherNo, salaryDate);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = "DSV13:" + ex.Message;
            }
            return Json(new { isSuccess, message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetTotalWageAmount(string details)
        {
            bool isSuccess = true;
            string message = string.Empty;
            string result = string.Empty;
            try
            {
                decimal decPayTotal = 0;
                List<Dictionary<string, object>> detailsData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(details);

                if (detailsData.Count > 0)
                {
                    for (int i = 0; i < detailsData.Count; i ++)
                    {

                        if (detailsData[i]["dailyWage"].GetType().Name != "JObject" && detailsData[i]["dailyWage"].ToString() != string.Empty)
                        {
                            if (detailsData[i]["status"].GetType().Name != "JObject")
                            {
                                if (detailsData[i]["status"].ToString() == "paid")
                                {
                                    if (detailsData[i]["attendance"].ToString() == "Present")
                                    {
                                        decPayTotal = decPayTotal + Convert.ToDecimal(detailsData[i]["dailyWage"].ToString());
                                    }
                                    else
                                    {
                                        decPayTotal = decPayTotal + ((Convert.ToDecimal(detailsData[i]["dailyWage"].ToString())) * decimal.Parse("0.5".ToString()));
                                    }
                                }

                            }
                        }
                    }
                }
                result = Math.Round(Convert.ToDecimal(decPayTotal.ToString("0.00000")), PublicVariables._inNoOfDecimalPlaces).ToString();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = "DSV7:" + ex.Message;
            }
            return Json(new { isSuccess, message, totalAmout = result });
        }

        public DataTable DailySalaryVoucherDetailsGridfill(bool @isEditmode, string voucherNo, string salaryDate)
        {
            DataTable dtbl = new DataTable();
            try
            {
                DailySalaryVoucherDetailsSP spDetails = new DailySalaryVoucherDetailsSP();
                dtbl = spDetails.DailySalaryVoucherDetailsGridViewAll(salaryDate, isEditmode, voucherNo);
            }
            catch (Exception ex)
            {
            }
            return dtbl;
        }


        /// <summary>
        /// Function to edit LedgerPosting table
        /// </summary>
        private void LedgerUpdate(DailySalaryVoucherMasterInfo infoMaster)
        {
            try
            {
                decimal decLedgerPostingId = 0;
                LedgerPostingSP spLedgerPosting = new LedgerPostingSP();
                LedgerPostingInfo infoLedgerPosting = new LedgerPostingInfo();
                DataTable dtbl = new DataTable();
                dtbl = spLedgerPosting.GetLedgerPostingIds(infoMaster.VoucherNo, infoMaster.VoucherTypeId);
                int ini = 0;
                foreach (DataRow dr in dtbl.Rows)
                {
                    ini++;

                    if (ini == 2)
                    {
                        decLedgerPostingId = Convert.ToDecimal(dr.ItemArray[0].ToString());
                        infoLedgerPosting.LedgerPostingId = decLedgerPostingId;
                        infoLedgerPosting.Date = infoMaster.Date;
                        infoLedgerPosting.VoucherNo = infoMaster.VoucherNo;
                        infoLedgerPosting.Debit = 0;
                        infoLedgerPosting.Credit = infoMaster.TotalAmount;
                        infoLedgerPosting.VoucherTypeId = infoMaster.VoucherTypeId;
                        infoLedgerPosting.LedgerId = 4;
                        infoLedgerPosting.DetailsId = 0;
                        infoLedgerPosting.InvoiceNo = infoMaster.InvoiceNo;
                        infoLedgerPosting.ChequeNo = string.Empty;
                        infoLedgerPosting.ChequeDate = DateTime.Now;
                        infoLedgerPosting.YearId = 0; //PublicVariables._decCurrentFinancialYearId;
                        infoLedgerPosting.Extra1 = string.Empty;
                        infoLedgerPosting.Extra2 = string.Empty;

                        spLedgerPosting.LedgerPostingEdit(infoLedgerPosting);
                    }
                    if (ini == 1)
                    {
                        decLedgerPostingId = Convert.ToDecimal(dr.ItemArray[0].ToString());
                        infoLedgerPosting.LedgerPostingId = decLedgerPostingId;
                        infoLedgerPosting.Date = infoMaster.Date;
                        infoLedgerPosting.VoucherNo = infoMaster.VoucherNo;
                        infoLedgerPosting.Debit = infoMaster.TotalAmount;
                        infoLedgerPosting.Credit = 0;
                        infoLedgerPosting.VoucherTypeId = infoMaster.VoucherTypeId;
                        infoLedgerPosting.LedgerId = infoMaster.LedgerId;
                        infoLedgerPosting.DetailsId = 0;
                        infoLedgerPosting.InvoiceNo = infoMaster.InvoiceNo;
                        infoLedgerPosting.ChequeNo = string.Empty;
                        infoLedgerPosting.ChequeDate = DateTime.Now;
                        infoLedgerPosting.YearId = PublicVariables._decCurrentFinancialYearId;
                        infoLedgerPosting.Extra1 = string.Empty;
                        infoLedgerPosting.Extra2 = string.Empty;

                        spLedgerPosting.LedgerPostingEdit(infoLedgerPosting);
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        //added by KimJK

        //added by Hongb
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDailySalarySearch(string voucherDateFrom, string voucherDateTo, string salaryDateFrom, string salaryDateTo, string voucherNo)
        {
            bool isSuccess = true;
            string message = "success";
            string jsonResults = "";
            try
            {
                voucherNo = voucherNo ?? string.Empty;
                DataTable dtblDailySalaryRegister = new DataTable();
                DailySalaryVoucherMasterSP spDailySalaryVoucherMasterSP = new DailySalaryVoucherMasterSP();
                dtblDailySalaryRegister = spDailySalaryVoucherMasterSP.DailySalaryRegisterSearch(Convert.ToDateTime(voucherDateFrom), Convert.ToDateTime(voucherDateTo),
                    Convert.ToDateTime(salaryDateFrom), Convert.ToDateTime(salaryDateTo), voucherNo);
                jsonResults = Utils.ConvertDataTabletoString(dtblDailySalaryRegister);
            }
            catch(Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
            }

            return Json(new
            {
                isSuccess,
                message,
                data = jsonResults
            });
        }
        //added by Hongb
    }
}