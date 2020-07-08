using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Masterfine.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Primitives;

namespace Masterfine.Controllers
{
    public partial class PayrollController : Controller
    {
        //private IHostingEnvironment _env;
        //public PayrollController(IHostingEnvironment env)
        //{
        //    _env = env;
        //}
        public ActionResult PaySlip()
        {
            ViewData["salaryMonth"] = DateTime.UtcNow.ToString("yyyy-MM");
            return View(FillEmployee());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public DataTable FillEmployee()
        {
            DataTable dtbl = new DataTable();
            try
            {
                EmployeeSP spEmployee = new EmployeeSP();
                dtbl = spEmployee.EmployeeViewForPaySlip();
                DataRow dr = dtbl.NewRow();
                dr[0] = "0";
                dr[1] = "--Select--";
                dtbl.Rows.InsertAt(dr, 0);
                return dtbl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Print(string salaryMonth, string cmbEmployeeID, string cmbEmployeeName)
        {
            try
            {
                //if (CheckUserPrivilege.PrivilegeCheck(PublicVariables._decCurrentUserId, "frmPaySlip", "Print"))
                {
                    if (cmbEmployeeName == string.Empty || cmbEmployeeName == "--Select--")
                    {
                        //Messages.InformationMessage("Select an employee");
                        //cmbEmployee.Focus();
                    }
                    else
                    {
                        SalaryVoucherMasterSP spSalaryVoucherMaster = new SalaryVoucherMasterSP();
                        DateTime dtMon = DateTime.Parse(salaryMonth);
                        DateTime dtSalaryMonth = new DateTime(dtMon.Year, dtMon.Month, 1);
                        decimal decEmployeeId = Convert.ToDecimal(cmbEmployeeID);
                        DataSet dsPaySlip = spSalaryVoucherMaster.PaySlipPrinting(decEmployeeId, dtSalaryMonth, 3);
                        string reportData = string.Empty;
                        foreach (DataTable dtbl in dsPaySlip.Tables)
                        {
                            if (dtbl.TableName == "Table1")
                            {
                                if (dtbl.Rows.Count > 0)
                                {
                                    //frmReport frmReport = new frmReport();
                                    //PaySlipPrinting(dsPaySlip);
                                    reportData = ReplaceReportData(dsPaySlip);
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        success = "failed",
                                        data = "Salary not paid"
                                    });
                                }
                            }
                        }
                        //string jsonResult = Utils.ConvertDataTabletoString(dtbl);
                        return Json(new
                        {
                            success = "success",
                            data = reportData
                        });
                    }
                }

                return Json(new
                {
                    success = "failed",
                    data = "Select one for the report"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = "failed", data = ex.Message });
                //MessageBox.Show("PS 3: " + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /*public Stream PaySlipPrinting(DataSet dsPaySlip)
        {
            try
            {
                decimal decTotalAdd = 0;
                decimal decTotalDed = 0;
                decimal decNetPay = 0;
                //crptPaySlip crptPaySlip = new crptPaySlip();
                //ReportDocument crptPaySlip = new ReportDocument();
                //crptPaySlip.Load("CrystalReports\\Reports\\crptPaySlip.rpt");
                var webRoot = _env.WebRootPath;
                string htmlReportTemplate = System.IO.File.ReadAllText(System.IO.Path.Combine(webRoot, "/ReportTemplate/PaySlip.html"));
                foreach (DataTable dtbl in dsPaySlip.Tables)
                {
                    if (dtbl.TableName == "Table")
                    {
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#Logo#!}", Convert.ToString(dtbl.Rows[0]["logo"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#CompanyName#!}", Convert.ToString(dtbl.Rows[0]["companyName"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#mailingname#!}", Convert.ToString(dtbl.Rows[0]["mailingName"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#address#!}", Convert.ToString(dtbl.Rows[0]["address"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#county#!}", Convert.ToString(dtbl.Rows[0]["country"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#state#!}", Convert.ToString(dtbl.Rows[0]["state"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#pin#!}", Convert.ToString(dtbl.Rows[0]["pin"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#Email#!}", Convert.ToString(dtbl.Rows[0]["emailId"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#Phone#!}", Convert.ToString(dtbl.Rows[0]["Phone"]));
                        //crptPaySlip.Database.Tables["dtblCompanyDetails"].SetDataSource(dtbl);
                    }
                    else if (dtbl.TableName == "Table1")
                    {
                        //crptPaySlip.Database.Tables["dtblEmployeeDetails"].SetDataSource(dtbl);
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#emCode#!}", Convert.ToString(dtbl.Rows[0]["employeeCode"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#salMonth#!}", Convert.ToString(dtbl.Rows[0]["Month"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#empName#!}", Convert.ToString(dtbl.Rows[0]["employeeName"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#salDate#!}", Convert.ToString(dtbl.Rows[0]["Date"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#designation#!}", Convert.ToString(dtbl.Rows[0]["designationName"]));


                        htmlReportTemplate = htmlReportTemplate.Replace("{!#basic#!}", Convert.ToString(dtbl.Rows[0]["Salary"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#dedAmount#!}", Convert.ToString(dtbl.Rows[0]["DEDamount"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#bonusAmount#!}", Convert.ToString(dtbl.Rows[0]["Bonus"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#dedAmount#!}", Convert.ToString(dtbl.Rows[0]["DEDamount"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#dAmount#!}", Convert.ToString(dtbl.Rows[0]["ADDamount"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#advance#!}", Convert.ToString(dtbl.Rows[0]["Advance"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#lop#!}", Convert.ToString(dtbl.Rows[0]["LOP"]));





                        foreach (DataRow drow in dtbl.Rows)
                        {
                            if (drow["ADDamount"].ToString() != string.Empty)
                            {
                                decTotalAdd += Convert.ToDecimal(drow["ADDamount"].ToString());
                            }
                            if (drow["DEDamount"].ToString() != string.Empty)
                            {
                                decTotalDed += Convert.ToDecimal(drow["DEDamount"].ToString());
                            }
                        }

                        foreach (DataRow drow in dtbl.Rows)
                        {
                            if (drow["LOP"].ToString() != string.Empty)
                            {
                                decTotalDed += Convert.ToDecimal(drow["LOP"].ToString());
                            }

                            if (drow["Deduction"].ToString() != string.Empty)
                            {
                                decTotalDed += Convert.ToDecimal(drow["Deduction"].ToString());
                            }

                            if (drow["Advance"].ToString() != string.Empty)
                            {
                                decTotalDed += Convert.ToDecimal(drow["Advance"].ToString());
                            }

                            if (drow["Bonus"].ToString() != string.Empty)
                            {
                                decTotalAdd += Convert.ToDecimal(drow["Bonus"].ToString());
                            }

                            break;

                        }

                        htmlReportTemplate = htmlReportTemplate.Replace("{!#totalEarning#!}", Convert.ToString(decTotalAdd));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#totalDeductions#!}", Convert.ToString(decTotalDed));

                    }
                    else if (dtbl.TableName == "Table2")
                    {
                        DataColumn dtClmn = new DataColumn("AmountInWords");
                        dtbl.Columns.Add(dtClmn);
                        decNetPay = decTotalAdd - decTotalDed;

                        htmlReportTemplate = htmlReportTemplate.Replace("{!#workingDays#!}", Convert.ToString(dtbl.Rows[0]["WorkingDay"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#presentDays#!}", Convert.ToString(dtbl.Rows[0]["PresentDays"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#leaveDays#!}", Convert.ToString(dtbl.Rows[0]["LeaveDays"]));


                        foreach (DataRow drow in dtbl.Rows)
                        {
                            drow["AmountInWords"] = new NumToText().AmountWords(decNetPay, PublicVariables._decCurrencyId);
                        }
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#netPay#!}", Convert.ToString(dtbl.Rows[0]["AmountInWords"]));
                        //crptPaySlip.Database.Tables["dtblOther"].SetDataSource(dtbl);
                    }
                }

                //this.crptViewer.ReportSource = crptPaySlip;
                SettingsSP spSettings = new SettingsSP();

                if (spSettings.SettingsStatusCheck("DirectPrint") == "No")
                {
                    //base.Show();
                    //this.BringToFront();
                }
                else
                {
                    //Stream stream = crptPaySlip.ExportToStream(ExportFormatType.PortableDocFormat);
                    //------------in ordet to print , this below code has to be work!!!
                    //crptPaySlip.PrintToPrinter(1, true, 0, 0);
                    //return stream;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("CRV2 " + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return null;
        }*/

        public string ReplaceReportData(DataSet dsPaySlip)
        {
            string htmlReportTemplate = string.Empty;
            try
            {
                decimal decTotalAdd = 0;
                decimal decTotalDed = 0;
                decimal decNetPay = 0;
                //crptPaySlip crptPaySlip = new crptPaySlip();
                //ReportDocument crptPaySlip = new ReportDocument();
                //crptPaySlip.Load("CrystalReports\\Reports\\crptPaySlip.rpt");
                var webRoot = _env.WebRootPath;
                htmlReportTemplate = System.IO.File.ReadAllText(System.IO.Path.Combine(webRoot, "ReportTemplate/PaySlip.html"));
                htmlReportTemplate = htmlReportTemplate.Replace("{!#currentDate#!}", string.Format("{0:MM/dd/yyyy}", DateTime.Now));
                foreach (DataTable dtbl in dsPaySlip.Tables)
                {
                    if (dtbl.TableName == "Table")
                    {
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#Logo#!}", Convert.ToString(dtbl.Rows[0]["logo"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#CompanyName#!}", Convert.ToString(dtbl.Rows[0]["companyName"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#mailingname#!}", Convert.ToString(dtbl.Rows[0]["mailingName"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#address#!}", Convert.ToString(dtbl.Rows[0]["address"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#county#!}", Convert.ToString(dtbl.Rows[0]["country"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#state#!}", Convert.ToString(dtbl.Rows[0]["state"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#pin#!}", Convert.ToString(dtbl.Rows[0]["pin"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#Email#!}", Convert.ToString(dtbl.Rows[0]["emailId"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#Phone#!}", Convert.ToString(dtbl.Rows[0]["Phone"]));
                        //crptPaySlip.Database.Tables["dtblCompanyDetails"].SetDataSource(dtbl);
                    }
                    else if (dtbl.TableName == "Table1")
                    {
                        //crptPaySlip.Database.Tables["dtblEmployeeDetails"].SetDataSource(dtbl);
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#emCode#!}", Convert.ToString(dtbl.Rows[0]["employeeCode"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#salMonth#!}", Convert.ToString(dtbl.Rows[0]["Month"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#empName#!}", Convert.ToString(dtbl.Rows[0]["employeeName"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#salDate#!}", Convert.ToString(dtbl.Rows[0]["Date"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#designation#!}", Convert.ToString(dtbl.Rows[0]["designationName"]));

                        // htmlReportTemplate = htmlReportTemplate.Replace("{!#AddpayheadName#!}", Convert.ToString(dtbl.Rows[0]["AddpayheadName"]));
                        // htmlReportTemplate = htmlReportTemplate.Replace("{!#ADDamount#!}", Convert.ToString(dtbl.Rows[0]["ADDamount"]));
                        // htmlReportTemplate = htmlReportTemplate.Replace("{!#dedAmount#!}", Convert.ToString(dtbl.Rows[0]["Deduction"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#bonusAmount#!}", Convert.ToString(dtbl.Rows[0]["Bonus"]));
                        //htmlReportTemplate = htmlReportTemplate.Replace("{!#dedAmount#!}", Convert.ToString(dtbl.Rows[0]["Deduction"]));


                        // htmlReportTemplate = htmlReportTemplate.Replace("{!#dAmount#!}", Convert.ToString(dtbl.Rows[0]["ADDamount"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#advance#!}", Convert.ToString(dtbl.Rows[0]["Advance"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#lop#!}", Convert.ToString(dtbl.Rows[0]["LOP"]));
                        //htmlReportTemplate = htmlReportTemplate.Replace("{!#DEDpayheadName#!}", Convert.ToString(dtbl.Rows[0]["DEDpayheadName"]));


                        string detailQuery = "";

                        decimal deductAmount = 0;
                        string[,] array2Db = new string[50, 4];
                        int cntAdd = 0, cntDed = 0;
                        foreach (DataRow drow in dtbl.Rows)
                        {
                            if (drow["ADDamount"].ToString() != string.Empty)
                            {
                                decTotalAdd += Convert.ToDecimal(drow["ADDamount"].ToString());
                            }
                            if (drow["DEDamount"].ToString() != string.Empty)
                            {
                                decTotalDed += Convert.ToDecimal(drow["DEDamount"].ToString());
                                deductAmount += Convert.ToDecimal(drow["DEDamount"].ToString());
                            }
                            if (drow["AddpayheadName"].ToString() != string.Empty)
                            {
                                array2Db[cntAdd, 0] = drow["AddpayheadName"].ToString();
                                array2Db[cntAdd, 1] = drow["ADDamount"].ToString();
                                cntAdd++;
                            }
                            if (drow["DEDpayheadName"].ToString() != string.Empty)
                            {
                                array2Db[cntDed, 2] = drow["DEDpayheadName"].ToString();
                                array2Db[cntDed, 3] = drow["DEDamount"].ToString();
                                cntDed++;
                            }
                        }

                        for (int i = 0; i < Math.Max(cntAdd, cntDed); i ++)
                        {
                            detailQuery += "<tr class='item'><td>" + array2Db[i, 0] + "</td><td style='text-align:right;'>" + array2Db[i, 1] + "</td>";
                            detailQuery += "<td>" + array2Db[i, 2] + "</td><td style='text-align:right;'>" + array2Db[i, 3] + "</td></tr>";
                            
                        }
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#deductionAmount#!}", Convert.ToString(deductAmount));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#tabledetail#!}", detailQuery);

                        foreach (DataRow drow in dtbl.Rows)
                        {
                            if (drow["LOP"].ToString() != string.Empty)
                            {
                                decTotalDed += Convert.ToDecimal(drow["LOP"].ToString());
                            }

                            if (drow["Deduction"].ToString() != string.Empty)
                            {
                                decTotalDed += Convert.ToDecimal(drow["Deduction"].ToString());
                            }

                            if (drow["Advance"].ToString() != string.Empty)
                            {
                                decTotalDed += Convert.ToDecimal(drow["Advance"].ToString());
                            }

                            if (drow["Bonus"].ToString() != string.Empty)
                            {
                                decTotalAdd += Convert.ToDecimal(drow["Bonus"].ToString());
                            }
                            /*if (drow["Salary"].ToString() != string.Empty)
                            {
                                decTotalAdd += Convert.ToDecimal(drow["Salary"].ToString());
                            }*/

                            break;

                        }

                        htmlReportTemplate = htmlReportTemplate.Replace("{!#totalEarning#!}", Convert.ToString(decTotalAdd));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#totalDeductions#!}", Convert.ToString(decTotalDed));

                    }
                    else if (dtbl.TableName == "Table2")
                    {
                        DataColumn dtClmn = new DataColumn("AmountInWords");
                        dtbl.Columns.Add(dtClmn);
                        decNetPay = decTotalAdd - decTotalDed;

                        htmlReportTemplate = htmlReportTemplate.Replace("{!#workingDays#!}", Convert.ToString(dtbl.Rows[0]["WorkingDay"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#presentDays#!}", Convert.ToString(dtbl.Rows[0]["PresentDays"]));
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#leaveDays#!}", Convert.ToString(dtbl.Rows[0]["LeaveDays"]));


                        foreach (DataRow drow in dtbl.Rows)
                        {
                            drow["AmountInWords"] = new NumToText().AmountWords(decNetPay, PublicVariables._decCurrencyId);
                        }
                        htmlReportTemplate = htmlReportTemplate.Replace("{!#netPay#!}", Convert.ToString(dtbl.Rows[0]["AmountInWords"]));
                        //crptPaySlip.Database.Tables["dtblOther"].SetDataSource(dtbl);
                    }
                }

                //this.crptViewer.ReportSource = crptPaySlip;
                SettingsSP spSettings = new SettingsSP();

                if (spSettings.SettingsStatusCheck("DirectPrint") == "No")
                {
                    //base.Show();
                    //this.BringToFront();
                }
                else
                {
                    //Stream stream = crptPaySlip.ExportToStream(ExportFormatType.PortableDocFormat);
                    //------------in ordet to print , this below code has to be work!!!
                    //crptPaySlip.PrintToPrinter(1, true, 0, 0);
                    //return stream;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("CRV2 " + ex.Message, "Masterfine", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return htmlReportTemplate;
        }

    }
}