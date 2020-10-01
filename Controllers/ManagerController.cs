using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Authentication;
using TakipOtoparkSistemi.Models;
using System.Data.SqlClient;

namespace TakipOtoparkSistemi.Controllers
{
    public class ManagerController : Controller
    {

        private veritabanil db = new veritabanil();
        public ActionResult YoneticiGiris()
        {
            return View();
        }
       [HttpPost]
        public ActionResult YoneticiGiris(Yonetici loginModel)
        {
            var u = db.Yonetici.FirstOrDefault(x => x.Kullanici == loginModel.Kullanici && x.Sifre == loginModel.Sifre);
            if(u!=null)
            {
                return View("LoginYonetici");
            }
            else
            {
                ViewBag.LoginError = "Hatalı kullanıcı adı veya şifre";
            }

            return View();
        }
        
        public ActionResult LoginYonetici()
        {
           
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("YoneticiGiris");
        }
       
        public ActionResult BorcSorgulama()
        {
            ViewBag.GirisTarih = "0";
            ViewBag.CikisTarih = "0";
            ViewBag.Borc = 0;
            ViewBag.Id = 0;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BorcSorgulama([Bind(Include = "GPlakaNo")] MusteriGunlukBilgisi musteriGunlukBilgisi)
        {

            int aylikID = db.Database.SqlQuery<int>("Select AylikID from MusteriAylıkBilgisi where APlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).FirstOrDefault();
            int GunlukID = db.Database.SqlQuery<int>("Select GunlukID from MusteriGunlukBilgisi where GPlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).FirstOrDefault();
            if (aylikID == 0)
            {
                if (GunlukID==0)
                {
                    Response.Write("<script lang='JavaScript'>alert('Girdiğiniz plaka otoparka hiç girmemiştir');</script>");
                }
                else
                {
                   List<DateTime> giristarih = db.Database.SqlQuery<DateTime>("Select GirisTarihi from VardiyaBilgisi join MusteriGunlukBilgisi on  VardiyaBilgisi.GunlukID=MusteriGunlukBilgisi.GunlukID where  VardiyaBilgisi.GunlukID=MusteriGunlukBilgisi.GunlukID and GPlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).ToList();
                   ViewBag.GirisTarih = giristarih;
                    List<DateTime> cikistarih = db.Database.SqlQuery<DateTime>("Select CikisTarihi from VardiyaBilgisi join MusteriGunlukBilgisi on  VardiyaBilgisi.GunlukID=MusteriGunlukBilgisi.GunlukID where  VardiyaBilgisi.GunlukID=MusteriGunlukBilgisi.GunlukID and GPlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).ToList();
                    ViewBag.CikisTarih = cikistarih;
                    int vardiyaId = db.Database.SqlQuery<int>("Select VardiyaID from VardiyaBilgisi where GunlukID=@gunluk", new SqlParameter("@gunluk", GunlukID)).FirstOrDefault();
                    int borc= db.Database.SqlQuery<int>("Select ToplamBorc from KasaBilgisi where VardiyaID=@vardiya", new SqlParameter("@vardiya", vardiyaId)).FirstOrDefault();
                    ViewBag.Borc = borc;
                    ViewBag.Id = vardiyaId;
                    return View();
                }
            }
            else
            {
                List<DateTime> giristarih = db.Database.SqlQuery<DateTime>("Select GirisTarihi from VardiyaBilgisi join MusteriAylıkBilgisi on  VardiyaBilgisi.AylikID=MusteriAylıkBilgisi.AylikID where  VardiyaBilgisi.AylikID=MusteriAylıkBilgisi.AylikID and APlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).ToList();
                ViewBag.GirisTarih = giristarih;
                List<DateTime> cikistarih = db.Database.SqlQuery<DateTime>("Select CikisTarihi from VardiyaBilgisi join MusteriAylıkBilgisi on  VardiyaBilgisi.AylikID=MusteriAylıkBilgisi.AylikID where  VardiyaBilgisi.AylikID=MusteriAylıkBilgisi.AylikID and APlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).ToList();
                ViewBag.CikisTarih = cikistarih;
                int vardiyaId = db.Database.SqlQuery<int>("Select VardiyaID from VardiyaBilgisi where AylikID=@aylik", new SqlParameter("@aylik", aylikID)).FirstOrDefault();
                int borc = db.Database.SqlQuery<int>("Select ToplamBorc from KasaBilgisi where VardiyaID=@vardiya", new SqlParameter("@vardiya", vardiyaId)).FirstOrDefault();
                ViewBag.Borc = borc;
                ViewBag.Id = vardiyaId;
                return View();
            }
            return View();
        }
        public ActionResult OdemeAl(int? Id)
        {
            if (Id!=0)
            {              
                int GunlukID = db.Database.SqlQuery<int>("Select GunlukID  From  VardiyaBilgisi where VardiyaID=@vardiya", new SqlParameter("@vardiya", Id)).FirstOrDefault();
                
                if (GunlukID!=0)
                {
                    db.Database.ExecuteSqlCommand("Delete From  MusteriGunlukBilgisi Where GunlukID='" + GunlukID + "'");
                }
                db.Database.ExecuteSqlCommand("Delete From  KasaBilgisi Where VardiyaID='" + Id + "'");
                db.Database.ExecuteSqlCommand("Delete From  VardiyaBilgisi Where VardiyaID='" + Id + "'");
                ViewBag.GirisTarih = "0";
                ViewBag.CikisTarih = "0";
                ViewBag.Borc = 0;
                Response.Write("<script lang='JavaScript'>alert('Girilen Plakanın Borcu Ödendi.');</script>");
            }
            return View("BorcSorgulama");
        }
        public ActionResult GunSonuRaporu()
        {
            int IceridekiAracSayisi= db.Database.SqlQuery<int>("Select Count(VardiyaID) from VardiyaBilgisi where convert(date,GirisTarihi,103)=convert(date,Getdate(),103) and CikisTarihi is null").FirstOrDefault();
            int CikisYapanAracSayisi= db.Database.SqlQuery<int>("Select Count(VardiyaID) from VardiyaBilgisi where convert(date,CikisTarihi,103)=convert(date,Getdate(),103)").FirstOrDefault();
            int toplamborc= db.Database.SqlQuery<int>("Select SUM(ToplamBorc) From KasaBilgisi join VardiyaBilgisi on KasaBilgisi.VardiyaID=VardiyaBilgisi.VardiyaID Where  KasaBilgisi.VardiyaID=VardiyaBilgisi.VardiyaID and convert(date,CikisTarihi,103)=convert(date,getdate(),103)").FirstOrDefault();
            ViewBag.iceri = IceridekiAracSayisi.ToString();
            ViewBag.cikis = CikisYapanAracSayisi.ToString();
            ViewBag.toplamBorc = toplamborc.ToString();
            return View();
        }
        
        public ActionResult ParkAlaniGoster()

        {
            db.Database.ExecuteSqlCommand("Delete From BosOlans;");
            db.Database.ExecuteSqlCommand("INSERT INTO BosOlans SELECT k.KonumID, k.KonumAdi  FROM KonumBilgisi as k LEFT JOIN MusteriGunlukBilgisi G ON k.KonumID = G.KonumID left join MusteriAylıkBilgisi A ON k.KonumID = A.KonumID  WHERE G.KonumID is null and A.KonumID is null ; ");
            return View(db.BosOlans.ToList());
        }
        
       




    }
}
