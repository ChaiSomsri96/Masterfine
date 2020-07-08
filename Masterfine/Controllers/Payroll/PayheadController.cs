using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace Masterfine.Controllers
{
    public partial class PayrollController : Controller
    {
        public ActionResult PayHead()
        {
            DataTable dtbl = new DataTable();
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                dtbl.Columns.Add("slNo", typeof(int));
                dtbl.Columns["slNo"].AutoIncrement = true;
                dtbl.Columns["slNo"].AutoIncrementSeed = 1;
                dtbl.Columns["slNo"].AutoIncrementStep = 1;
                SqlDataAdapter sdaadapter = new SqlDataAdapter("PayHeadViewAll", sqlcon);
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
        public ActionResult DeletePayhead(string id)
        {
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand sccmd = new SqlCommand("PayHeadDeleteVoucherTypeCheckReference", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@PayHeadId", SqlDbType.Decimal);
                sprmparam.Value = id;
                int ina = sccmd.ExecuteNonQuery();
                if (ina == -1)
                {
                    return Content("error");
                }
                else
                {
                    return Content("success");
                }
            }
            catch (Exception ex)
            {
                return Content("failed");
            }
            finally
            {
                sqlcon.Close();
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetPayhead(string id)
        {
            SqlDataReader sdrreader = null;
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                SqlCommand sccmd = new SqlCommand("PayHeadView", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@payHeadId", SqlDbType.Decimal);
                sprmparam.Value = id;
                sdrreader = sccmd.ExecuteReader();
                while (sdrreader.Read())
                {
                    return Json(new
                    {
                        error = "false",
                        payheadId = decimal.Parse(sdrreader[0].ToString()),
                        payheadName = sdrreader[1].ToString(),
                        type = sdrreader[2].ToString(),
                        narration = sdrreader[3].ToString(),
                        extraDate = DateTime.Parse(sdrreader[4].ToString()),
                        extra1 = sdrreader[5].ToString(),
                        extra2 = sdrreader[6].ToString()
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
        public ActionResult AddPayhead(string payheadName, string type, string narration)
        {
            DataTable dtbl = new DataTable();
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
                SqlCommand sccmd = new SqlCommand("PayHeadAdd", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@payHeadName", SqlDbType.VarChar);
                sprmparam.Value = payheadName.Trim();
                sprmparam = sccmd.Parameters.Add("@type", SqlDbType.VarChar);
                sprmparam.Value = type;
                sprmparam = sccmd.Parameters.Add("@narration", SqlDbType.VarChar);
                sprmparam.Value = narration.Trim();
                sprmparam = sccmd.Parameters.Add("@extraDate", SqlDbType.DateTime);
                sprmparam.Value = DateTime.Parse(DateTime.Now.ToString());
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
            return RedirectToAction("PayHead");
        }

        [HttpPost]
        public ActionResult EditPayhead(string e_payheadId, string e_payheadName, string e_type, string e_narration)
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
                SqlCommand sccmd = new SqlCommand("PayHeadEdit", sqlcon);
                sccmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sprmparam = new SqlParameter();
                sprmparam = sccmd.Parameters.Add("@payHeadId", SqlDbType.Decimal);
                sprmparam.Value = e_payheadId;
                sprmparam = sccmd.Parameters.Add("@payHeadName", SqlDbType.VarChar);
                sprmparam.Value = e_payheadName;
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
            return RedirectToAction("PayHead");
        }

    }
}