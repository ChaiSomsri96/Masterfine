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
        static bool isAutomatic = false;
        string strVoucherType = "Advance Payment";
        string strAdvancePayment = "AdvancePayment";
        static string strVoucherNo = string.Empty;
        static string strPaymentVoucherTypeId = string.Empty;
        static decimal decAdvancePaymentEditId = 0;
        static decimal decPaymentSuffixPrefixId = 0;
        static decimal decPaymentVoucherTypeId = 11;
        static decimal decAdvancePaymentsId;
        static decimal decAdvancePaymentId;
        static string strInvoiceNo = string.Empty;
        static string strUpdatedVoucherNumber = string.Empty;
        static string strUpdatedInvoiceNumber = string.Empty;
        DateTime dtpDate = DateTime.UtcNow;

        public ActionResult AdvancePayment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAdvancePaymentDetails()
        {
            string jsonCashOrBank = string.Empty;
            string jsonEmployee = string.Empty;
            string jsonDate = string.Empty;
            try
            {
                CallFromVoucherTypeSelection();
                VoucherNoGeneration(DateTime.UtcNow);
                jsonDate = DateTime.UtcNow.ToString("yyyy-MM-dd");                
                TransactionsGeneralFill obj = new TransactionsGeneralFill();
                DataTable cashOrBank = obj.CashOrBankComboFill(false);
                DataTable employee = EmployeeComboFill();
                jsonCashOrBank = Utils.ConvertDataTabletoString(cashOrBank);
                jsonEmployee = Utils.ConvertDataTabletoString(employee);
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "AP1:" + ex.Message});
            }
            return Json(new { success = "true", ex= "no", voucherNo = strVoucherNo, employee = jsonEmployee, cashOrBank = jsonCashOrBank, date = jsonDate});
        }

        [HttpPost]
        public ActionResult CheckBankOrCash()
        {
            bool isBankAccount = false;
            try
            {
                TransactionsGeneralFill obj = new TransactionsGeneralFill();
                DataTable cashOrBank = obj.CashOrBankComboFill(false);
                DataTable employee = EmployeeComboFill();

                if (cashOrBank.Rows.Count > 0)
                {
                    decimal cashOrBankId = (decimal)cashOrBank.Rows[0][1];
                    string cashOrBankName = (string)cashOrBank.Rows[0][0];
                    isBankAccount = CheckWhetherBankOrCash(cashOrBankId, cashOrBankName);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "AP2:" + ex.Message});
            }
            return Json(new { success = "true", ex = "no", isBankAccount });
        }

        [HttpPost]
        public ActionResult GetAdvancePaymentEditDetails(string advancePaymentEditId)
        {
            try
            {
                decAdvancePaymentEditId = Convert.ToDecimal(advancePaymentEditId);
                AdvancePaymentSP spadvance = new AdvancePaymentSP();
                AdvancePaymentInfo infoadvance = new AdvancePaymentInfo();
                VoucherTypeSP spvouchertype = new VoucherTypeSP();
                infoadvance = spadvance.AdvancePaymentView(decAdvancePaymentEditId);
                strVoucherNo = infoadvance.VoucherNo;
                string txtAdvanceVoucherNo = infoadvance.InvoiceNo;
                strInvoiceNo = infoadvance.InvoiceNo;
                string employeeId = infoadvance.EmployeeId.ToString();
                string salaryMonth = infoadvance.SalaryMonth.ToString("yyyy-MM");
                string txtDate = infoadvance.Date.ToString("yyyy-MM-dd");
                string txtChequeDate = infoadvance.Date.ToString("yyyy-MM-dd");
                string ledgerId = infoadvance.LedgerId.ToString();
                string txtCheckNo = infoadvance.Chequenumber;
                string txtAmount = infoadvance.Amount.ToString();
                string txtNarration = infoadvance.Narration;
                bool isBankAccount = false;
                decAdvancePaymentsId = decAdvancePaymentId;

                TransactionsGeneralFill obj = new TransactionsGeneralFill();
                DataTable cashOrBank = obj.CashOrBankComboFill(false);
                DataTable employee = EmployeeComboFill();
                string jsonCashOrBank = Utils.ConvertDataTabletoString(cashOrBank);
                string jsonEmployee = Utils.ConvertDataTabletoString(employee);

                if (cashOrBank.Rows.Count > 0)
                {
                    decimal cashOrBankId = (decimal)cashOrBank.Rows[0][1];
                    string cashOrBankName = (string)cashOrBank.Rows[0][0];
                    isBankAccount = CheckWhetherBankOrCash(cashOrBankId, cashOrBankName);
                }

                decPaymentVoucherTypeId = infoadvance.VoucherTypeId;
                decPaymentSuffixPrefixId = infoadvance.SuffixPrefixId;
                isAutomatic = spvouchertype.CheckMethodOfVoucherNumbering(decPaymentVoucherTypeId);

                return Json(new
                {
                    success = "true",
                    ex = "no",
                    data = new
                    {
                        voucherNo = strVoucherNo,
                        employee = jsonEmployee,
                        employeeId,
                        salaryMonth,
                        chequeNo = txtCheckNo,
                        date = txtDate,
                        amount = txtAmount,
                        cashOrBank = jsonCashOrBank,
                        ledgerId,
                        chequeDate = txtChequeDate,
                        narration = txtNarration,
                        isAutomatic,
                        isBankAccount
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "AP3:" + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAdvancePaymentFunction(string voucherNo, string employeeId, string salaryMonth, string chequeNo, 
            string date, string amount, string ledgerId, string chequeDate, string narration)
        {
            string exception = string.Empty;
            string message = string.Empty;
            string focus = string.Empty;
            string newVoucherNo = string.Empty;
            try
            {
                employeeId = employeeId ?? string.Empty;                
                salaryMonth = salaryMonth ?? string.Empty;
                chequeNo = chequeNo ?? string.Empty;
                date = date ?? string.Empty;
                amount = amount ?? string.Empty;
                ledgerId = ledgerId ?? string.Empty;
                chequeDate = chequeDate ?? string.Empty;
                narration = narration ?? string.Empty;
                AdvancePaymentSP spAdvancepayment = new AdvancePaymentSP();
                AdvancePaymentInfo infoAdvancepayment = new AdvancePaymentInfo();
                LedgerPostingSP spLedgerPosting = new LedgerPostingSP();
                MonthlySalarySP spMonthlySalary = new MonthlySalarySP();

                if (CheckAdvanceAmount(employeeId, amount))
                {
                    if (!spMonthlySalary.CheckSalaryAlreadyPaidOrNotForAdvancePayment(Convert.ToDecimal(employeeId), Convert.ToDateTime(salaryMonth)))
                    {
                        if (!spAdvancepayment.CheckSalaryAlreadyPaidOrNot(Convert.ToDecimal(employeeId), Convert.ToDateTime(salaryMonth)))
                        {
                            if (isAutomatic == true)
                            {
                                infoAdvancepayment.VoucherNo = strVoucherNo;
                            }
                            else
                            {
                                infoAdvancepayment.VoucherNo = voucherNo;
                            }
                            infoAdvancepayment.EmployeeId = Convert.ToDecimal(employeeId);
                            infoAdvancepayment.SalaryMonth = Convert.ToDateTime(salaryMonth);
                            infoAdvancepayment.Chequenumber = chequeNo ?? string.Empty;
                            infoAdvancepayment.Date = Convert.ToDateTime(date);
                            infoAdvancepayment.Amount = Convert.ToDecimal(amount);
                            if (isAutomatic)
                            {
                                infoAdvancepayment.InvoiceNo = strInvoiceNo;
                            }
                            else
                            {
                                infoAdvancepayment.InvoiceNo = voucherNo;
                            }
                            infoAdvancepayment.LedgerId = Convert.ToDecimal(ledgerId);
                            infoAdvancepayment.ChequeDate = Convert.ToDateTime(chequeDate);
                            infoAdvancepayment.Narration = narration;
                            infoAdvancepayment.ExtraDate = DateTime.Now;
                            infoAdvancepayment.Extra1 = string.Empty;
                            infoAdvancepayment.Extra2 = string.Empty;
                            infoAdvancepayment.VoucherTypeId = decPaymentVoucherTypeId;
                            infoAdvancepayment.SuffixPrefixId = decPaymentSuffixPrefixId;
                            infoAdvancepayment.FinancialYearId = PublicVariables._decCurrentFinancialYearId;

                            if (decAdvancePaymentsId != -1)
                            {
                                DataTable dtbl = new DataTable();
                                dtbl = spAdvancepayment.AdvancePaymentAddWithIdentity(infoAdvancepayment, isAutomatic);
                                foreach (DataRow dr in dtbl.Rows)
                                {
                                    decAdvancePaymentId = Convert.ToDecimal(dr.ItemArray[0].ToString());
                                    strUpdatedVoucherNumber = dr.ItemArray[1].ToString();
                                    strUpdatedInvoiceNumber = dr.ItemArray[2].ToString();
                                }
                                if (!isAutomatic)
                                {
                                    strVoucherNo = voucherNo;
                                }
                                if (isAutomatic)
                                {
                                    if (Convert.ToDecimal(strUpdatedVoucherNumber) != Convert.ToDecimal(strVoucherNo))
                                    {
                                        message = "Voucher number changed from  " + strInvoiceNo + "  to  " + strUpdatedInvoiceNumber + ".";
                                        strVoucherNo = strUpdatedVoucherNumber.ToString();
                                        strInvoiceNo = strUpdatedInvoiceNumber;
                                        newVoucherNo = strVoucherNo;
                                    }
                                }
                                focus = "AdvanceVoucherNo";
                            }
                            LedgerPosting(Convert.ToDecimal(ledgerId), decAdvancePaymentId, voucherNo, ledgerId, amount);

                        }
                        else
                        {
                            message = "Advance already paid for this month.";
                            focus = "new_salaryMonth";
                        }
                    }
                    else
                    {
                        message = "Cant pay advance for this month,Salary already paid.";
                        focus = "new_salaryMonth";
                    }
                }
                else
                {
                    message = "Advance of this month exceeds than amount set for the employee";
                    focus = "new_amount";
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = ex.Message });
            }
            return Json(new { success = "true", ex = "no", focus, message, newVoucherNo});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdvancePaymentFunction(string voucherNo, string employeeId, string salaryMonth, string chequeNo,
            string date, string amount, string ledgerId, string chequeDate, string narration, string advancePaymentEditId)
        {
            string message = string.Empty;
            string focus = string.Empty;
            bool amountReadOnly = false;
            try
            {
                employeeId = employeeId ?? string.Empty;
                salaryMonth = salaryMonth ?? string.Empty;
                chequeNo = chequeNo ?? string.Empty;
                date = date ?? string.Empty;
                amount = amount ?? string.Empty;
                ledgerId = ledgerId ?? string.Empty;
                chequeDate = chequeDate ?? string.Empty;
                narration = narration ?? string.Empty;
                decAdvancePaymentEditId = Convert.ToDecimal(advancePaymentEditId);
                MonthlySalarySP spMonthlySalary = new MonthlySalarySP();
                decimal decEmployeeId = Convert.ToDecimal(employeeId);
                DateTime dtSalaryMonth = Convert.ToDateTime(salaryMonth);
                if (!spMonthlySalary.CheckSalaryStatusForAdvancePayment(decEmployeeId, dtSalaryMonth))
                {


                    AdvancePaymentSP spAdvancepayment = new AdvancePaymentSP();
                    AdvancePaymentInfo infoAdvancepayment = new AdvancePaymentInfo();
                    LedgerPostingSP spLedgerPosting = new LedgerPostingSP();

                    if (spAdvancepayment.CheckSalaryAlreadyPaidOrNot(decEmployeeId, dtSalaryMonth))
                    {
                        amountReadOnly = true;
                    }

                    infoAdvancepayment.AdvancePaymentId = decAdvancePaymentEditId;
                    infoAdvancepayment.EmployeeId = decEmployeeId;
                    infoAdvancepayment.SalaryMonth = dtSalaryMonth;
                    infoAdvancepayment.Chequenumber = chequeNo;
                    infoAdvancepayment.Date = Convert.ToDateTime(date);
                    infoAdvancepayment.Amount = Convert.ToDecimal(amount);
                    if (CheckAdvanceAmount(employeeId, amount))
                    {
                        if (isAutomatic)
                        {
                            infoAdvancepayment.VoucherNo = strVoucherNo;
                        }
                        else
                        {
                            infoAdvancepayment.VoucherNo = voucherNo;
                        }
                        if (isAutomatic)
                        {
                            infoAdvancepayment.InvoiceNo = strInvoiceNo;
                        }
                        else
                        {
                            infoAdvancepayment.InvoiceNo = voucherNo;
                        }
                        infoAdvancepayment.LedgerId = Convert.ToDecimal(ledgerId);
                        infoAdvancepayment.ChequeDate = Convert.ToDateTime(chequeDate);
                        infoAdvancepayment.Narration = narration;
                        infoAdvancepayment.ExtraDate = Convert.ToDateTime(DateTime.Now.ToString());
                        infoAdvancepayment.Extra1 = string.Empty;
                        infoAdvancepayment.Extra2 = string.Empty;
                        infoAdvancepayment.VoucherTypeId = decPaymentVoucherTypeId;
                        infoAdvancepayment.SuffixPrefixId = decPaymentSuffixPrefixId;
                        infoAdvancepayment.FinancialYearId = PublicVariables._decCurrentFinancialYearId;
                        message = spAdvancepayment.AdvancePaymentEdit(infoAdvancepayment);
                        message = LedgerUpdate(voucherNo, amount, ledgerId);
                    }
                    else
                    {
                        message = "Advance of this month exceeds than amount set for the employee";
                        focus = "new_amount";
                    }
                }
                else
                {
                    message = "You can't update,reference exist";
                    focus = "edit_salaryMonth";
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = "true", ex = "AP3" + ex.Message });
            }
            return Json(new { success = "true", ex = "no", focus, message, amountReadOnly});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAdvancePaymentFunction(string voucherNo, string employeeId, string salaryMonth)
        {
            string message = string.Empty;
            try
            {
                MonthlySalarySP spMonthlySalary = new MonthlySalarySP();
                if (!spMonthlySalary.CheckSalaryStatusForAdvancePayment(Convert.ToDecimal(employeeId), Convert.ToDateTime(salaryMonth)))
                {
                    AdvancePaymentInfo infoAdvancepayment = new AdvancePaymentInfo();
                    AdvancePaymentSP spAdvancePayment = new AdvancePaymentSP();
                    LedgerPostingSP spLedgerPosting = new LedgerPostingSP();
                    spAdvancePayment.AdvancePaymentDelete(Convert.ToDecimal(decAdvancePaymentEditId.ToString()));
                    spLedgerPosting.LedgerPostDelete(voucherNo, decAdvancePaymentEditId);
                }
                else
                {
                    message = "You can't delete,reference exist";
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = "false", ex = "AP4"+ex.Message, message });
            }
            return Json(new { success = "true", ex = "no", message });
        }

        public string LedgerUpdate(string voucherNo, string amount, string selLedgerId)
        {
            string message = string.Empty;
            try
            {
                decimal decLedgerPostingId = 0;
                LedgerPostingSP spLedgerPosting = new LedgerPostingSP();
                LedgerPostingInfo infoLedgerPosting = new LedgerPostingInfo();
                DataTable dtbl = new DataTable();
                dtbl = spLedgerPosting.GetLedgerPostingIds(strVoucherNo, decAdvancePaymentId);
                int ini = 0;
                foreach (DataRow dr in dtbl.Rows)
                {
                    ini++;
                    if (ini == 2)
                    {
                        decLedgerPostingId = Convert.ToDecimal(dr.ItemArray[0].ToString());
                        infoLedgerPosting.LedgerPostingId = decLedgerPostingId;
                        infoLedgerPosting.Date = PublicVariables._dtCurrentDate;
                        if (isAutomatic)
                        {
                            infoLedgerPosting.VoucherNo = strVoucherNo;
                        }
                        else
                        {
                            infoLedgerPosting.VoucherNo = voucherNo;
                        }
                        infoLedgerPosting.Debit = Convert.ToDecimal(amount);
                        infoLedgerPosting.Credit = 0;
                        infoLedgerPosting.VoucherTypeId = decPaymentVoucherTypeId;
                        infoLedgerPosting.LedgerId = 3;
                        infoLedgerPosting.DetailsId = decAdvancePaymentId;
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

                        spLedgerPosting.LedgerPostingEdit(infoLedgerPosting);
                    }
                    if (ini == 1)
                    {
                        decLedgerPostingId = Convert.ToDecimal(dr.ItemArray[0].ToString());
                        infoLedgerPosting.LedgerPostingId = decLedgerPostingId;
                        infoLedgerPosting.Date = PublicVariables._dtCurrentDate;
                        if (isAutomatic)
                        {
                            infoLedgerPosting.VoucherNo = strVoucherNo;
                        }
                        else
                        {
                            infoLedgerPosting.VoucherNo = voucherNo;
                        }
                        infoLedgerPosting.Debit = 0;
                        infoLedgerPosting.Credit = Convert.ToDecimal(amount);
                        infoLedgerPosting.VoucherTypeId = decPaymentVoucherTypeId;
                        infoLedgerPosting.LedgerId = Convert.ToDecimal(selLedgerId);
                        infoLedgerPosting.DetailsId = decAdvancePaymentId;
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
                        spLedgerPosting.LedgerPostingEdit(infoLedgerPosting);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public DataTable EmployeeComboFill()
        {
            DataTable dtblspAdvancePayment = new DataTable();
            try
            {
                AdvancePaymentSP spAdvancePayment = new AdvancePaymentSP();
                dtblspAdvancePayment = spAdvancePayment.AdvancePaymentEmployeeComboFill();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("AP12:" + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return dtblspAdvancePayment;
        }

        public void CallFromVoucherTypeSelection()
        {
            VoucherTypeSP spVoucherType = new VoucherTypeSP();
            DataTable dtblVouchetType = new DataTable();
            dtblVouchetType = spVoucherType.VoucherTypeSelectionComboFill(strVoucherType);
            decimal decVoucherTypeId = (decimal)dtblVouchetType.Rows[0][0];
            decPaymentVoucherTypeId = decVoucherTypeId;
            strPaymentVoucherTypeId = decVoucherTypeId.ToString();

            isAutomatic = spVoucherType.CheckMethodOfVoucherNumbering(Convert.ToDecimal(decVoucherTypeId.ToString()));
            SuffixPrefixSP spSuffisprefix = new SuffixPrefixSP();
            SuffixPrefixInfo infoSuffixPrefix = new SuffixPrefixInfo();
            infoSuffixPrefix = spSuffisprefix.GetSuffixPrefixDetails(Convert.ToDecimal(strPaymentVoucherTypeId), dtpDate);
            decPaymentSuffixPrefixId = infoSuffixPrefix.SuffixprefixId;
        }

        public void VoucherNoGeneration(DateTime salaryMonth)
        {            
            TransactionsGeneralFill obj = new TransactionsGeneralFill();
            AdvancePaymentSP spAdvancePayment = new AdvancePaymentSP();
            
            if (strVoucherNo == string.Empty)
            {
                strVoucherNo = "0";
            }
            strVoucherNo = obj.VoucherNumberAutomaicGeneration(decPaymentVoucherTypeId, Convert.ToDecimal(strVoucherNo), salaryMonth, strAdvancePayment);
            if (Convert.ToDecimal(strVoucherNo) != spAdvancePayment.AdvancePaymentGetMaxPlusOne(decPaymentVoucherTypeId))
            {
                strVoucherNo = spAdvancePayment.AdvancePaymentGetMax(decPaymentVoucherTypeId).ToString();
                strVoucherNo = obj.VoucherNumberAutomaicGeneration(decPaymentVoucherTypeId, Convert.ToDecimal(strVoucherNo), salaryMonth, strAdvancePayment);
                if (spAdvancePayment.AdvancePaymentGetMax(decPaymentVoucherTypeId) == "0")
                {
                    strVoucherNo = "0";
                    strVoucherNo = obj.VoucherNumberAutomaicGeneration(decPaymentVoucherTypeId, Convert.ToDecimal(strVoucherNo), salaryMonth, strAdvancePayment);
                }
            }
            if (isAutomatic)
            {
                SuffixPrefixSP spSuffixPrefix = new SuffixPrefixSP();
                SuffixPrefixInfo infoSuffixPrefix = new SuffixPrefixInfo();
                infoSuffixPrefix = spSuffixPrefix.GetSuffixPrefixDetails(decPaymentVoucherTypeId, dtpDate);
                string strPrefix = infoSuffixPrefix.Prefix;
                string strSuffix = infoSuffixPrefix.Suffix;
                strInvoiceNo = strPrefix + strVoucherNo + strSuffix;
            }
        }
        public bool CheckWhetherBankOrCash(decimal cashOrBankId, string cashOrBankName)
        {
            bool isBankAcocunt = false;
            try
            {
                //----- To make readonly txtChequeNo and txtChequeDate if selected ledger group is cash-----//
                if (cashOrBankName != null && cashOrBankName != string.Empty && cashOrBankId > 0)
                {
                    decimal decLedger = Convert.ToDecimal(cashOrBankId);
                    
                    AccountGroupSP SpGroup = new AccountGroupSP();
                    DataTable dtbl = new DataTable();
                    dtbl = SpGroup.CheckWheatherLedgerUnderCash();
                    //-------- Checking whether the selected legder is under bank----------//
                    foreach (DataRow dr in dtbl.Rows)
                    {
                        string str = dr.ItemArray[0].ToString();
                        if (decLedger == Convert.ToDecimal(dr.ItemArray[0].ToString()))
                        {
                            isBankAcocunt = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return isBankAcocunt;
        }

        public bool CheckAdvanceAmount(string employeeId, string amount)
        {
            bool Cancel = true;            
            try
            {
                decimal decEmployeeId = 0;
                AdvancePaymentSP spAdvancePayment = new AdvancePaymentSP();
                decimal decEmployeesalary = 0;
                if (employeeId != string.Empty)
                {
                    decEmployeeId = Convert.ToDecimal(employeeId);
                }
                decEmployeesalary = spAdvancePayment.AdvancePaymentAmountchecking(decEmployeeId);

                decimal txtamountvalue = 0;
                if (amount != string.Empty)
                {
                    txtamountvalue = Convert.ToDecimal(amount);
                }
                if (txtamountvalue > decEmployeesalary)
                {                    
                    Cancel = false;
                }
            }
            catch (Exception ex)
            {
            }
            return Cancel;
        }

        public void LedgerPosting(decimal decLedgerPostingId, decimal decAdvancePaymentId, string voucherNo, string selLedgerId, string amount)
        {
            try
            {
                AdvancePaymentSP spAdvancePayment = new AdvancePaymentSP();
                AdvancePaymentInfo infoAdvancePayment = new AdvancePaymentInfo();
                LedgerPostingSP spLedgerPosting = new LedgerPostingSP();
                LedgerPostingInfo infoLedgerPosting = new LedgerPostingInfo();
                infoLedgerPosting.VoucherTypeId = decPaymentVoucherTypeId;
                if (isAutomatic)
                {
                    infoLedgerPosting.VoucherNo = strVoucherNo;
                }
                else
                {
                    infoLedgerPosting.VoucherNo = voucherNo;
                }
                infoLedgerPosting.Date = PublicVariables._dtCurrentDate;
                infoLedgerPosting.LedgerId = Convert.ToDecimal(selLedgerId);
                infoLedgerPosting.DetailsId = decAdvancePaymentId;
                if (isAutomatic)
                {
                    infoLedgerPosting.InvoiceNo = strInvoiceNo;
                }
                else
                {
                    infoLedgerPosting.InvoiceNo = voucherNo;
                }
                infoLedgerPosting.YearId = PublicVariables._decCurrentFinancialYearId;
                infoLedgerPosting.Debit = 0;
                infoLedgerPosting.Credit = Convert.ToDecimal(amount);

                infoLedgerPosting.ChequeNo = string.Empty;
                infoLedgerPosting.ChequeDate = DateTime.Now;

                infoLedgerPosting.ExtraDate = DateTime.Now;
                infoLedgerPosting.Extra1 = string.Empty;
                infoLedgerPosting.Extra2 = string.Empty;
                spLedgerPosting.LedgerPostingAdd(infoLedgerPosting);
                infoLedgerPosting.VoucherTypeId = decPaymentVoucherTypeId;
                if (isAutomatic)
                {
                    infoLedgerPosting.VoucherNo = strVoucherNo;
                }
                else
                {
                    infoLedgerPosting.VoucherNo = voucherNo;
                }
                infoLedgerPosting.Date = PublicVariables._dtCurrentDate;
                infoLedgerPosting.LedgerId = 3;
                infoLedgerPosting.DetailsId = decAdvancePaymentId;
                if (isAutomatic)
                {
                    infoLedgerPosting.InvoiceNo = strInvoiceNo;
                }
                else
                {
                    infoLedgerPosting.InvoiceNo = voucherNo;
                }
                infoLedgerPosting.YearId = PublicVariables._decCurrentFinancialYearId;
                infoLedgerPosting.Debit = Convert.ToDecimal(amount);
                infoLedgerPosting.Credit = 0;

                infoLedgerPosting.ChequeNo = string.Empty;
                infoLedgerPosting.ChequeDate = DateTime.Now;

                infoLedgerPosting.ExtraDate = PublicVariables._dtCurrentDate;
                infoLedgerPosting.Extra1 = string.Empty;
                infoLedgerPosting.Extra2 = string.Empty;
                spLedgerPosting.LedgerPostingAdd(infoLedgerPosting);
            }
            catch (Exception ex)
            {
            }
        }
    }
}