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
    public class PesinGirisController : Controller
    {
        private veritabanil db = new veritabanil();

        // GET: PesinGiris
        public ActionResult PesinGirisEkle()
        {
            var konumlar = db.BosOlans.ToList().Select(b => new
              SelectListItem
            {
                Selected = false,
                Text = b.KonumAdi,
                Value = b.KonumId.ToString()
            });
            ViewBag.KonumID = konumlar;
            return View();
        }


        // POST: PesinGiris/Create
        // Aşırı gönderim saldırılarından korunmak için, lütfen bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=317598 sayfasına bakın.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PesinGirisEkle([Bind(Include = "GPlakaNo,KonumID")] MusteriGunlukBilgisi musteriGunlukBilgisi)
        {
            int aylikID = db.Database.SqlQuery<int>("Select AylikID from MusteriAylıkBilgisi where APlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).FirstOrDefault();
            int GunlukID = db.Database.SqlQuery<int>("Select GunlukID from MusteriGunlukBilgisi where GPlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).FirstOrDefault();
            int vardiyaId = db.Database.SqlQuery<int>("Select VardiyaID from VardiyaBilgisi where GunlukID=@Gunluk", new SqlParameter("@Gunluk", GunlukID)).FirstOrDefault();
            if (aylikID==0)
            {
                if (vardiyaId != 0)
                {
                    Response.Write("<script lang='JavaScript'>alert('Aracınız zaten otoparkta olduğu için tekrar giriş yapamazsınız');</script>");
                }
                else
                {
                    ViewBag.KonumID = new SelectList(db.BosOlans, "KonumId", "KonumAdi", musteriGunlukBilgisi.KonumID);
                    db.Database.ExecuteSqlCommand("INSERT INTO MusteriGunlukBilgisi(GPlakaNo,KonumID) VALUES('" + musteriGunlukBilgisi.GPlakaNo + "','" + musteriGunlukBilgisi.KonumID + "')");
                    int GunlukID1 = db.Database.SqlQuery<int>("Select GunlukID from MusteriGunlukBilgisi where GPlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).FirstOrDefault();
                    db.Database.ExecuteSqlCommand("INSERT INTO VardiyaBilgisi(GunlukID, GirisTarihi) VALUES('" + GunlukID1 + "',GETDATE())");
                    Response.Write("<script lang='JavaScript'>alert('Giriş yaptınız,aracınızı park edebilirsiniz..');</script>");
                }
            }
            else
            {
                Response.Write("<script lang='JavaScript'>alert('Aylik kayıtlı müşteriler bu paneli kullanamaz');</script>");
            }
            
            
            return View(musteriGunlukBilgisi);
        }

        public ActionResult BosAlanlarıGor()
        {
            db.Database.ExecuteSqlCommand("Delete From BosOlans;");
            db.Database.ExecuteSqlCommand("INSERT INTO BosOlans SELECT k.KonumID, k.KonumAdi  FROM KonumBilgisi as k LEFT JOIN MusteriGunlukBilgisi G ON k.KonumID = G.KonumID left join MusteriAylıkBilgisi A ON k.KonumID = A.KonumID  WHERE G.KonumID is null and A.KonumID is null ; ");
            return View(db.BosOlans.ToList());
        }
        public ActionResult PesinGirisCikar()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PesinGirisCikar([Bind(Include = "GPlakaNo")] MusteriGunlukBilgisi musteriGunlukBilgisi)
        {
            int GunlukID = db.Database.SqlQuery<int>("Select GunlukID from MusteriGunlukBilgisi where GPlakaNo=@plaka", new SqlParameter("@plaka", musteriGunlukBilgisi.GPlakaNo)).FirstOrDefault();
            int vardiyaId = db.Database.SqlQuery<int>("Select VardiyaID from VardiyaBilgisi where GunlukID=@Gunluk", new SqlParameter("@Gunluk", GunlukID)).FirstOrDefault();
            if (vardiyaId == 0)
            {
                Response.Write("<script lang='JavaScript'>alert('İçeride bir aracınız olmadığı için çıkış yapamazsınız');</script>");
            }
            else
            {

                db.Database.ExecuteSqlCommand("update VardiyaBilgisi SET CikisTarihi=GETDATE() WHERE GunlukID={0} and CikisTarihi is NULL", GunlukID);
                Response.Write("<script lang='JavaScript'>alert('Çıkış yaptınız,Aracınızı otoparktan çıkartabilirsiniz...');</script>");
                db.Database.ExecuteSqlCommand("update VardiyaBilgisi SET GecenSure=DATEDIFF(MINUTE,GirisTarihi,CikisTarihi)  where  GunlukID={0} and GecenSure is NULL", GunlukID);
                int vardiyaId1 = db.Database.SqlQuery<int>("Select VardiyaID from VardiyaBilgisi where GunlukID=@Gunluk", new SqlParameter("@Gunluk", GunlukID)).FirstOrDefault();
                return RedirectToAction("Fatura",new { vardiyaid = vardiyaId1 });
            }


            return View();
        }
        public ActionResult Fatura(int vardiyaid)
        {
            int gecensure= db.Database.SqlQuery<int>("Select GecenSure from VardiyaBilgisi where VardiyaID=@vardiya", new SqlParameter("@vardiya", vardiyaid)).FirstOrDefault();
            int fiyat;
            if (gecensure>0 && gecensure<=30)
            {
                fiyat = 10;
            }
            else if (gecensure>30 && gecensure<=60)
            {
                fiyat = 20;
            }
            else if (gecensure>60 && gecensure<=120)
            {
                fiyat = 30;
            }
            else if (gecensure>120 && gecensure<=240)
            {
                fiyat = 40;
            }
            else if (gecensure>240)
            {
                int saat = gecensure / 60;
                int dk = gecensure % 60;
                fiyat = (saat * 20)+(dk*1/2);
            }
            else
            {
                fiyat = 0;
            }
            db.Database.ExecuteSqlCommand("insert into KasaBilgisi(VardiyaID,EkUcret,ToplamBorc) values('" + vardiyaid+"','"+ fiyat+"','"+fiyat+"')");
            DateTime girisitarihi= db.Database.SqlQuery<DateTime>("Select GirisTarihi from VardiyaBilgisi where VardiyaID=@vardiya", new SqlParameter("@vardiya", vardiyaid)).FirstOrDefault();
            DateTime cikistarihi= db.Database.SqlQuery<DateTime>("Select CikisTarihi from VardiyaBilgisi where VardiyaID=@vardiya", new SqlParameter("@vardiya", vardiyaid)).FirstOrDefault();
            ViewBag.Giris = girisitarihi.ToString();
            ViewBag.Cikis = cikistarihi.ToString();
            int ucret= db.Database.SqlQuery<int>("Select EkUcret from KasaBilgisi where VardiyaID=@vardiya", new SqlParameter("@vardiya", vardiyaid)).FirstOrDefault();
            ViewBag.Ucret = ucret.ToString();
            ViewBag.Gecen = gecensure.ToString();
           

            return View();
        }

    }
}
