using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Masterfine.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Masterfine.Controllers
{
    public partial class PayrollController : Controller
    {
        public ActionResult SalaryPackage()
        {
            ViewData["NewSalaryPackage"] = "";
            return View();
        }

        public ActionResult NewSalaryPackage()
        {
            ViewData["NewSalaryPackage"] = "NewSalaryPackage";
            return View("SalaryPackage");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalaryPackageSearch(string packageName, string status)
        {
            if (packageName == null)
                packageName = string.Empty;
            if (status == null)
                status = "All";

            DataTable dtbl = SalaryPackageSearchFromDB(packageName, status);
            string jsonResult = Utils.ConvertDataTabletoString(dtbl);
            return Json(new
            {
                success = "true",
                data = jsonResult
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetSalaryPackageDetails(string salaryPackageId)
        {
            int nSalaryPackageId = int.Parse(salaryPackageId);
            SalaryPackageSP spSalaryPackage = new SalaryPackageSP();
            SalaryPackageDetailsSP spSalaryPackageDetils = new SalaryPackageDetailsSP();
            SalaryPackageInfo infoSalaryPackage = new SalaryPackageInfo();
            string jsonDetails = "";
            if (nSalaryPackageId > 0)
            {
                infoSalaryPackage = spSalaryPackage.SalaryPackageView(nSalaryPackageId);
                DataTable dtblSalaryPackageDetails = spSalaryPackageDetils.SalaryPackageDetailsViewWithSalaryPackageId(nSalaryPackageId);
                jsonDetails = Utils.ConvertDataTabletoString(dtblSalaryPackageDetails);
            }
            
            PayHeadSP spPayhead = new PayHeadSP();
            DataTable dtblPayheads = spPayhead.PayHeadViewAll();
            string jsonPayheads = Utils.ConvertDataTabletoString(dtblPayheads);
            return Json(new
            {
                isSuccess = true,
                message = "success",
                data = new
                {
                    salaryPackageId = infoSalaryPackage.SalaryPackageId,
                    salaryPackageName = infoSalaryPackage.SalaryPackageName,
                    narration = infoSalaryPackage.Narration,
                    isActive = infoSalaryPackage.IsActive,
                    salaryPackageDetails = jsonDetails,
                    payHeads = jsonPayheads
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteSalaryPackage(string salaryPackageId)
        {
            int nSalaryPackageId = int.Parse(salaryPackageId);
            SalaryPackageSP spSalaryPackage = new SalaryPackageSP();
            spSalaryPackage.SalaryPackageDeleteAll(nSalaryPackageId);
            return Json(new
            {
                isSuccess = true,
                message = "success",
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveSalaryPackage(string salaryPackageId, string salaryPackageName, string active, string narration, string jsonDetails, string totalAmount)
        {
            bool isSave = true;
            string message = "success";
            SalaryPackageInfo infoSalaryPackage = new SalaryPackageInfo();
            try
            {
                SalaryPackageSP spSalaryPackage = new SalaryPackageSP();
                SalaryPackageDetailsSP spSalaryPackageDetails = new SalaryPackageDetailsSP();
                int nSalaryPackageId = int.Parse(salaryPackageId);
                float fTotalAmount = float.Parse(totalAmount);
                bool isActive = bool.Parse(active);
                infoSalaryPackage.SalaryPackageId = nSalaryPackageId;
                infoSalaryPackage.SalaryPackageName = salaryPackageName;
                infoSalaryPackage.Narration = narration;
                infoSalaryPackage.Extra1 = string.Empty;
                infoSalaryPackage.Extra2 = string.Empty;
                infoSalaryPackage.IsActive = isActive;
                infoSalaryPackage.TotalAmount = Convert.ToDecimal(totalAmount);

                if (nSalaryPackageId > 0) // edit mode
                {
                    spSalaryPackage.SalaryPackageEdit(infoSalaryPackage);
                    spSalaryPackageDetails.SalaryPackageDetailsDeleteWithSalaryPackageId(nSalaryPackageId);
                    isSave = SavePayHeadDetails(spSalaryPackageDetails, jsonDetails, nSalaryPackageId);
                    if (!isSave)
                    {
                        spSalaryPackage.SalaryPackageDeleteAll(nSalaryPackageId);
                    }
                }
                else
                {
                    nSalaryPackageId = (int)spSalaryPackage.SalaryPackageAdd(infoSalaryPackage);
                    if (nSalaryPackageId != -1)
                    {
                        infoSalaryPackage.SalaryPackageId = nSalaryPackageId;
                        isSave = SavePayHeadDetails(spSalaryPackageDetails, jsonDetails, nSalaryPackageId);
                        if (!isSave)
                        {
                            spSalaryPackage.SalaryPackageDeleteAll(nSalaryPackageId);
                        }
                    }
                    else
                    {
                        message = "Package name already exists";
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            

            return Json(new {
                            isSuccess = isSave,
                            messaage = message,
                            data = infoSalaryPackage
                        });
            
        }

        [HttpGet]
        public JsonResult SalaryPackageViewAllForActive()
        {
            bool isSave = true;
            string message = "success";
            string jsonResults = "";
            try
            {
                SalaryPackageSP spSalaryPackage = new SalaryPackageSP();
                DataTable dtblSalaryPackage = new DataTable();
                dtblSalaryPackage = spSalaryPackage.SalaryPackageViewAllForActive();
                jsonResults = Utils.ConvertDataTabletoString(dtblSalaryPackage); 
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new {
                        isSuccess = isSave,
                        messaage = message,
                        data = jsonResults
            });

        }

        private bool SavePayHeadDetails(SalaryPackageDetailsSP spSalaryPackageDetails, string jsonDetails, int nSalaryPackageId)
        {
            bool isSave = false;
            try
            {
                List<Dictionary<string, object>> items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonDetails);

                for (int i = 0; i < items.Count; i++)
                {    
                    SalaryPackageDetailsInfo infoSalaryPackageDetails = new SalaryPackageDetailsInfo();
                    infoSalaryPackageDetails.PayHeadId = Convert.ToDecimal(items[i]["payHeadId"]);
                    infoSalaryPackageDetails.Amount = Convert.ToDecimal(items[i]["Amount"]);
                    infoSalaryPackageDetails.Narration = items[i]["Narration"].ToString();
                    infoSalaryPackageDetails.SalaryPackageId = nSalaryPackageId;
                    infoSalaryPackageDetails.Extra1 = string.Empty;
                    infoSalaryPackageDetails.Extra2 = string.Empty;
                    if (spSalaryPackageDetails.SalaryPackageDetailsAdd(infoSalaryPackageDetails))
                    {
                        isSave = true;
                    }
                    else
                    {
                        isSave = false;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                isSave = false;
            }

            return isSave;
        }

        private DataTable SalaryPackageSearchFromDB(string packageName, string status)
        {
            try
            {
                SalaryPackageSP spSalarayPackage = new SalaryPackageSP();
                DataTable dtbl = spSalarayPackage.SalaryPackageregisterSearch(packageName, status);
                return dtbl;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}