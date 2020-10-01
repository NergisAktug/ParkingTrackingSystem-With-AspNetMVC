using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TakipOtoparkSistemi.Models;
using System.Data.Entity;

namespace TakipOtoparkSistemi.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

      
       
        public ActionResult YoneticiGiris()
        {
            return View();
        }
    }
}