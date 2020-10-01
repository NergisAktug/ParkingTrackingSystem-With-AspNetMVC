using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TakipOtoparkSistemi.Models;

namespace TakipOtoparkSistemi.Controllers
{
    public class AracIslemController : Controller
    {
        private veritabanil db = new veritabanil();

        // GET: AracIslem
        public ActionResult Index()
        {
            return View(db.MusteriAylıkBilgisi.ToList());
        }

        // GET: AracIslem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusteriAylıkBilgisi musteriAylıkBilgisi = db.MusteriAylıkBilgisi.Find(id);
            if (musteriAylıkBilgisi == null)
            {
                return HttpNotFound();
            }
            return View(musteriAylıkBilgisi);
        }

        // GET: AracIslem/Create
        public ActionResult Create()
        {
            db.Database.ExecuteSqlCommand("Delete From BosOlans;");
     db.Database.ExecuteSqlCommand("INSERT INTO BosOlans SELECT k.KonumID, k.KonumAdi  FROM KonumBilgisi as k LEFT JOIN MusteriGunlukBilgisi G ON k.KonumID = G.KonumID left join MusteriAylıkBilgisi A ON k.KonumID = A.KonumID  WHERE G.KonumID is null and A.KonumID is null ; ");
            var konumlar = db.BosOlans.ToList().Select(b => new
              SelectListItem
            {
                Selected=false,
                Text=b.KonumAdi,
                Value=b.KonumId.ToString()
            });
            var tarife = db.TarifeBilgisi.ToList().Select(b => new
             SelectListItem
            {
                Selected = false,
                Text = b.Tarife,
                Value = b.TarifeID.ToString()
            });
            ViewBag.KonumID = konumlar;
            ViewBag.TaifeID = tarife;
           return View();
        }

        // POST: AracIslem/Create
        // Aşırı gönderim saldırılarından korunmak için, lütfen bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=317598 sayfasına bakın.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AylikID,APlakaNo,TcNo,AdSoyad,Marka,Model,Renk,KonumID,TaifeID")] MusteriAylıkBilgisi musteriAylıkBilgisi)
        {
            if (ModelState.IsValid)
            {
                ViewBag.KonumID = new SelectList(db.BosOlans, "KonumId", "KonumAdi", musteriAylıkBilgisi.KonumID);
                ViewBag.TaifeID = new SelectList(db.TarifeBilgisi, "TarifeID", "Tarife", musteriAylıkBilgisi.TaifeID);
                db.MusteriAylıkBilgisi.Add(musteriAylıkBilgisi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(musteriAylıkBilgisi);
        }

        // GET: AracIslem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusteriAylıkBilgisi musteriAylıkBilgisi = db.MusteriAylıkBilgisi.Find(id);
            if (musteriAylıkBilgisi == null)
            {
                return HttpNotFound();
            }
            return View(musteriAylıkBilgisi);
        }

        // POST: AracIslem/Edit/5
        // Aşırı gönderim saldırılarından korunmak için, lütfen bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=317598 sayfasına bakın.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AylikID,APlakaNo,TcNo,AdSoyad,Marka,Model,Renk,KonumID,TaifeID")] MusteriAylıkBilgisi musteriAylıkBilgisi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(musteriAylıkBilgisi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(musteriAylıkBilgisi);
        }

        // GET: AracIslem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusteriAylıkBilgisi musteriAylıkBilgisi = db.MusteriAylıkBilgisi.Find(id);
            if (musteriAylıkBilgisi == null)
            {
                return HttpNotFound();
            }
            return View(musteriAylıkBilgisi);
        }

        // POST: AracIslem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MusteriAylıkBilgisi musteriAylıkBilgisi = db.MusteriAylıkBilgisi.Find(id);
            db.MusteriAylıkBilgisi.Remove(musteriAylıkBilgisi);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
