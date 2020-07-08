using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masterfine.Controllers
{
    public partial class PayrollController : Controller
    {
        private readonly IConfiguration configuration;
        private IHostingEnvironment _env;
        protected SqlConnection sqlcon;

        public PayrollController(IConfiguration config, IHostingEnvironment env)
        {
            this.configuration = config;
            string connectionstring = configuration.GetConnectionString("DefaultConnectionString");
            _env = env;
            sqlcon = new SqlConnection(connectionstring);
        }

        public ActionResult BonusDeductionRegister()
        {
            return View();
        }

    }
}