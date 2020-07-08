using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Masterfine.Models;

namespace Masterfine.Controllers
{
    public partial class PayrollController : Controller
    {
        [HttpGet]
        public ActionResult Designation()
        {
            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("slNo", typeof(int));
            dtbl.Columns["slNo"].AutoIncrement = true;
            dtbl.Columns["slNo"].AutoIncrementSeed = 1;
            dtbl.Columns["slNo"].AutoIncrementStep = 1;
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlDataAdapter sdaadapter = new SqlDataAdapter("DesignationViewAll", sqlcon);
                sdaadapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sdaadapter.Fill(dtbl);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            finally
            {
                sqlcon.Close();
            }
            return View(dtbl);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteDesignation(string id)
        {
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand sccmd = new SqlCommand("DesignationDelete", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@designationId", SqlDbType.Decimal);
                sprmparam.Value = id;
                int inEffectedRow = sccmd.ExecuteNonQuery();
                if (inEffectedRow > 0)
                {
                    return Content("success");
                }
                else
                {
                    return Content("error");
                }
            }
            catch (Exception ex)
            {
                return Content("success");
            }
            finally
            {
                sqlcon.Close();
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetDesignation(string id)
        {
            SqlDataReader sdrreader = null;
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand sccmd = new SqlCommand("DesignationView", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@designationId", SqlDbType.Decimal);
                sprmparam.Value = id;
                sdrreader = sccmd.ExecuteReader();
                while (sdrreader.Read())
                {
                    return Json(new
                    {
                        error = "false",
                        designationId = decimal.Parse(sdrreader[0].ToString()),
                        designationName = sdrreader[1].ToString(),
                        leaveDays = decimal.Parse(sdrreader[2].ToString()),
                        advanceAmount = decimal.Parse(sdrreader[3].ToString()),
                        narration = sdrreader[4].ToString(),
                        extraDate = DateTime.Parse(sdrreader[5].ToString()),
                        extra1 = sdrreader[6].ToString(),
                        extra2 = sdrreader[7].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = "true" });
            }
            finally
            {
                sdrreader.Close();
                sqlcon.Close();
            }
            return Json(new { error = "true" });
        }

        [HttpPost]
        public ActionResult AddDesignation(string designName, string leaveDays, string advanceAmount, string narration)
        {
            DataTable dtbl = new DataTable();
            decimal advance_amount, leave_days;
            if (advanceAmount == string.Empty || advanceAmount == null)
            {
                advance_amount = 0;
            }
            else
            {
                advance_amount = Convert.ToDecimal(advanceAmount.Trim());
            }
            if (leaveDays == string.Empty || leaveDays == null)
            {
                leave_days = 0;
            }
            else
            {
                leave_days = Convert.ToDecimal(leaveDays.Trim());
            }
            if (narration == string.Empty || narration == null)
            {
                narration = "";
            }
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand sccmd = new SqlCommand("DesignationAddIfNotExistsDesignation", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@designationName", SqlDbType.VarChar);
                sprmparam.Value = designName;
                sprmparam = sccmd.Parameters.Add("@leaveDays", SqlDbType.Decimal);
                sprmparam.Value = leave_days;
                sprmparam = sccmd.Parameters.Add("@advanceAmount", SqlDbType.Decimal);
                sprmparam.Value = advance_amount;
                sprmparam = sccmd.Parameters.Add("@narration", SqlDbType.VarChar);
                sprmparam.Value = narration;
                sprmparam = sccmd.Parameters.Add("@extra1", SqlDbType.VarChar);
                sprmparam.Value = string.Empty;
                sprmparam = sccmd.Parameters.Add("@extra2", SqlDbType.VarChar);
                sprmparam.Value = string.Empty;
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
            return RedirectToAction("Designation");
        }

        [HttpPost]
        public ActionResult EditDesignation(string e_designId, string e_designName, string e_leaveDays, string e_advanceAmount, string e_narration)
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
                SqlCommand sccmd = new SqlCommand("DesignationEdit", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@designationId", SqlDbType.Decimal);
                sprmparam.Value = e_designId;
                sprmparam = sccmd.Parameters.Add("@designationName", SqlDbType.VarChar);
                sprmparam.Value = e_designName;
                sprmparam = sccmd.Parameters.Add("@leaveDays", SqlDbType.Decimal);
                sprmparam.Value = e_leaveDays;
                sprmparam = sccmd.Parameters.Add("@advanceAmount", SqlDbType.Decimal);
                sprmparam.Value = e_advanceAmount;
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
            return RedirectToAction("Designation");
        }

        [HttpGet]
        public JsonResult GetDesignation()
        {
            bool isSuccess = true;
            string message = "success";
            string jsonResults = "";
            try
            {
                DataTable dtblDesignation = GetDesignationFromDB();
                jsonResults = Utils.ConvertDataTabletoString(dtblDesignation);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
                data = jsonResults
            });
        }

        /// <summary>
        /// Function to fill Designation combobox
        /// </summary>
        private DataTable GetDesignationFromDB()
        {
            DataTable dtblDesignation = new DataTable();
            try
            {
                DesignationSP SpDesignation = new DesignationSP();

                dtblDesignation = SpDesignation.DesignationViewAll();
                

            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
            }

            return dtblDesignation;
        }

    }
}