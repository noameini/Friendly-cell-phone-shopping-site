using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjA.Models;
using System.Security.Cryptography;
using System.Collections;
namespace ProjA.Controllers
{
    public class HomeController : Controller
    {
        private Database1Entities3 db = new Database1Entities3();

        public async Task<ActionResult> Index()
        {
            Session["search"] =await db.Items.ToArrayAsync();
            return View();
        }
        
    }
}