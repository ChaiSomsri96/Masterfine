using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Masterfine.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Masterfine.Controllers
{
    public partial class PayrollController : Controller
    {
        public ActionResult Employee()
        {
            ViewData["nowDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd");
            DataTable dtblDesignation = GetDesignationFromDB();
            if (dtblDesignation.Rows.Count > 0)
            {
                DataRow drRow = dtblDesignation.NewRow();
                drRow["designationId"] = "0";
                drRow["designationName"] = "All";
                dtblDesignation.Rows.InsertAt(drRow, 0);
            }
            ViewData["designations"] = dtblDesignation;

            DataTable dtblSalaryPackages = new SalaryPackageSP().SalaryPackageViewAllForActive();
            ViewData["salaryPackages"] = dtblSalaryPackages;

            EmployeeInfo infoEmployee = new EmployeeInfo();
            infoEmployee.EmployeeCode = "";
            infoEmployee.EmployeeName = "";
            infoEmployee.DesignationId = 0;
            infoEmployee.SalaryType = "All";
            infoEmployee.BankAccountNumber = "";
            infoEmployee.PassportNo = "";
            infoEmployee.LabourCardNumber = "";
            infoEmployee.VisaNumber = "";
            
            DataTable dtblEmployees = SearchEmployeesFromDB(infoEmployee);
            return View(dtblEmployees);
        }

        [HttpPost]
        public ActionResult EditEmployee(string e_employeeId, string e_employeeName, string e_type, string e_narration)
        {
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                if (e_narration == string.Empty || e_narration == null)
                {
                    e_narration = "";
                }
                SqlCommand sccmd = new SqlCommand("EmployeeEdit", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@employeeId", SqlDbType.Decimal);
                sprmparam.Value = e_employeeId;
                sprmparam = sccmd.Parameters.Add("@employeeName", SqlDbType.VarChar);
                sprmparam.Value = e_employeeName;
                sprmparam = sccmd.Parameters.Add("@type", SqlDbType.VarChar);
                sprmparam.Value = e_type;
                sprmparam = sccmd.Parameters.Add("@narration", SqlDbType.VarChar);
                sprmparam.Value = e_narration;
                sccmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            finally
            {
                sqlcon.Close();
            }
            return RedirectToAction("Employee");
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchEmployee(string code, string name, int designationId, string salaryType, string bankNo, string passportNo, string labCardNo, string visaNo)
        {
            bool isSuccess = true;
            string message = "success";
            string jsonResults = "";
            try
            {
                EmployeeInfo infoEmployee = new EmployeeInfo();
                EmployeeSP spEmployee = new EmployeeSP();
                infoEmployee.EmployeeCode = (code == null ? string.Empty : code);
                infoEmployee.EmployeeName = (name == null ? string.Empty : name);
                infoEmployee.DesignationId = designationId;
                infoEmployee.SalaryType = salaryType;
                infoEmployee.BankAccountNumber = (bankNo == null ? string.Empty: bankNo);
                infoEmployee.PassportNo = (passportNo == null ? string.Empty: passportNo);
                infoEmployee.LabourCardNumber = (labCardNo == null ? string.Empty : labCardNo);
                infoEmployee.VisaNumber = (visaNo == null ? string.Empty : visaNo);
                DataTable tblEmployees = spEmployee.EmployeeSearch(infoEmployee);
                jsonResults = Utils.ConvertDataTabletoString(tblEmployees);
            }
            catch(Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
            }

            return Json(new  {
                            isSuccess = isSuccess,
                            message = message,
                            data = jsonResults
                        });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetEmployee(string employeeId)
        {
            bool isSuccess = true;
            string message = "success";
            EmployeeInfo info = new EmployeeInfo();
            try
            {
                EmployeeSP spEmployee = new EmployeeSP();
                int nEmployeeId = int.Parse(employeeId);
                info = spEmployee.EmployeeView(nEmployeeId);
                
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new  {
                            isSuccess = isSuccess,
                            message = message,
                            data = info
                        });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveEmployee(string employeeId, string strEmployee)
        {
            bool isSuccess = true;
            string message = "success";
            EmployeeInfo info = new EmployeeInfo();
            try
            {
                EmployeeSP spEmployee = new EmployeeSP();
                int nEmployeeId = int.Parse(employeeId);
                Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(strEmployee);
                info.EmployeeId = int.Parse(dict["employeeId"].ToString());
                info.EmployeeCode = dict["employeeCode"] == null ? string.Empty : dict["employeeCode"].ToString();
                info.EmployeeName = dict["employeeName"] == null ? string.Empty : dict["employeeName"].ToString();
                info.SalaryType = dict["salaryType"] == null ? string.Empty : dict["salaryType"].ToString();
                if (info.SalaryType == "Daily wage")
                {
                    info.DailyWage = decimal.Parse(dict["dailyWage"].ToString());
                }
                else
                {
                    info.DefaultPackageId = int.Parse(dict["salaryPackage"].ToString());
                }
                info.DesignationId = int.Parse(dict["designationId"].ToString());
                info.Dob = Convert.ToDateTime(dict["dob"].ToString());
                info.MaritalStatus = dict["maritalStatus"] == null ? "Single" : dict["maritalStatus"].ToString();
                info.Gender = dict["gender"] == null ? "Male" : dict["gender"].ToString();
                info.Qualification = dict["qualification"] == null ? string.Empty : dict["qualification"].ToString();
                info.BloodGroup = dict["bloodGroup"] == null ? string.Empty : dict["bloodGroup"].ToString();
                info.JoiningDate = Convert.ToDateTime(dict["joiningDate"].ToString());
                info.TerminationDate = Convert.ToDateTime(dict["terminationDate"].ToString());
                info.Address = dict["address"] == null ? string.Empty : dict["address"].ToString();
                info.PhoneNumber = dict["phoneNumber"] == null ? string.Empty : dict["phoneNumber"].ToString();
                info.MobileNumber = dict["mobileNumber"] == null ? string.Empty : dict["mobileNumber"].ToString();
                info.Email = dict["email"] == null ? string.Empty : dict["email"].ToString();
                info.IsActive = bool.Parse(dict["isActive"].ToString());
                info.Narration = dict["narration"] == null ? string.Empty : dict["narration"].ToString();
                info.BankName = dict["bankName"] == null ? string.Empty : dict["bankName"].ToString();
                info.BankAccountNumber = dict["bankNumber"] == null ? string.Empty : dict["bankNumber"].ToString();
                info.BranchName = dict["branchName"] == null ? string.Empty : dict["branchName"].ToString();
                info.BranchCode = dict["branchCode"] == null ? string.Empty : dict["branchCode"].ToString();
                info.PanNumber = dict["panNumber"] == null ? string.Empty : dict["panNumber"].ToString();
                info.PfNumber = dict["pfNumber"] == null ? string.Empty : dict["pfNumber"].ToString();
                info.EsiNumber = dict["esiNumber"] == null ? string.Empty : dict["esiNumber"].ToString();
                info.PassportNo = dict["passportNo"] == null ? string.Empty : dict["passportNo"].ToString();
                info.PassportExpiryDate = Convert.ToDateTime(dict["passportExpiryDate"].ToString());
                info.LabourCardNumber = dict["labourCardNumber"] == null ? string.Empty : dict["labourCardNumber"].ToString();
                info.LabourCardExpiryDate = Convert.ToDateTime(dict["labourCardExpiryDate"].ToString());
                info.VisaNumber = dict["visaNumber"] == null ? string.Empty : dict["visaNumber"].ToString();
                info.VisaExpiryDate = Convert.ToDateTime(dict["visaExpiryDate"].ToString());
                info.ExtraDate = DateTime.Now;
                info.Extra1 = string.Empty;
                info.Extra2 = string.Empty;
                if (spEmployee.EmployeeCodeCheckExistance(info.EmployeeCode, nEmployeeId) == false)
                {
                    if (nEmployeeId > 0) // edit mode
                    {
                        isSuccess = spEmployee.EmployeeEdit(info);
                    }
                    else //create mode
                    {
                        nEmployeeId = (int)spEmployee.EmployeeAddWithReturnIdentity(info);
                        if (nEmployeeId <= 0)
                        {
                            isSuccess = false;
                            message = "Can not save this employee with any issues.";
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                    message = "Employee code already Exist.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
                data = info
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteEmployee(string employeeId)
        {
            bool isSuccess = true;
            string message = "success";
            try
            {
                int nEmployeeId = int.Parse(employeeId);
                EmployeeSP spEmployee = new EmployeeSP();
                if (spEmployee.EmployeeCheckReferences(nEmployeeId) == -1)
                {
                    isSuccess = false;
                    message = "You can't delete,reference exist";
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message
            });
        }

        private DataTable SearchEmployeesFromDB(EmployeeInfo info)
        {
            EmployeeSP spEmployee = new EmployeeSP();
            DataTable tblEmployees = spEmployee.EmployeeSearch(info);

            return tblEmployees;
        }

        [HttpPost]
        public JsonResult AddEmployee(string data)
        {
            return null;
        }

    }
}