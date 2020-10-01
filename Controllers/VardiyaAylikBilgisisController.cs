using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

using System.Web.Mvc;
using TakipOtoparkSistemi.Models;

namespace TakipOtoparkSistemi.Controllers
{
    public class VardiyaAylikBilgisisController : Controller
    {
        private veritabanil db = new veritabanil();
        public ActionResult AylikMusteriGiris()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AylikMusteriGiris([Bind(Include = "APlakaNo")] MusteriAylıkBilgisi musteriAylıkBilgisi)
        {
            int musteriID = db.Database.SqlQuery<int>("Select AylikID from MusteriAylıkBilgisi where APlakaNo=@plaka", new SqlParameter("@plaka", musteriAylıkBilgisi.APlakaNo)).FirstOrDefault();
            if (musteriID==0)
            {
                Response.Write("<script lang='JavaScript'>alert('Günlük(Peşin)girişi kullanın yada üyelik işlemleri için yönetici ile görüşün.');</script>");
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO VardiyaBilgisi(AylikID, GirisTarihi) VALUES('" + musteriID + "',GETDATE())");
                Response.Write("<script lang='JavaScript'>alert('Giriş yaptınız,aracınızı park edebilirsiniz..');</script>");
            }
           
            return View();
        }
        public ActionResult AylikMusteriCikis()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AylikMusteriCikis([Bind(Include = "APlakaNo")] MusteriAylıkBilgisi musteriAylıkBilgisi)
        {
            int AylikID = db.Database.SqlQuery<int>("Select AylikID from MusteriAylıkBilgisi where APlakaNo=@plaka", new SqlParameter("@plaka", musteriAylıkBilgisi.APlakaNo)).FirstOrDefault();
            int vardiyaId = db.Database.SqlQuery<int>("Select VardiyaID from VardiyaBilgisi where AylikID=@Aylik", new SqlParameter("@Aylik", AylikID)).FirstOrDefault();
            if (vardiyaId==0)
            {
                Response.Write("<script lang='JavaScript'>alert('İçeride bir aracınız olmadığı için çıkış yapamazsınız');</script>");
            }
            else
            {
               
                    db.Database.ExecuteSqlCommand("update VardiyaBilgisi SET CikisTarihi=GETDATE() WHERE AylikID={0} and CikisTarihi is NULL", AylikID);
                    Response.Write("<script lang='JavaScript'>alert('Çıkış yaptınız,Aracınızı otoparktan çıkartabilirsiniz...');</script>");
                    db.Database.ExecuteSqlCommand("update VardiyaBilgisi SET GecenSure=DATEDIFF(MINUTE,GirisTarihi,CikisTarihi)  where  AylikID={0} and GecenSure is NULL", AylikID);
                int tarifeID = db.Database.SqlQuery<int>("Select TaifeID from MusteriAylıkBilgisi where APlakaNo=@plaka", new SqlParameter("@plaka", musteriAylıkBilgisi.APlakaNo)).FirstOrDefault();
                int Borc= db.Database.SqlQuery<int>("Select Borc from TarifeBilgisi where TarifeID=@tarife", new SqlParameter("@tarife", tarifeID)).FirstOrDefault();
                db.Database.ExecuteSqlCommand("insert into KasaBilgisi(VardiyaID,AylikBorc,ToplamBorc) values('" + vardiyaId + "','" + Borc + "','" + Borc + "')");

            }
                
           
           // 
           
            return View("AylikMusteriGiris");
        }
    }
}
