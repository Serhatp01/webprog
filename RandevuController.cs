using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OdevDeneme1.Data;
using OdevDeneme1.Models;
using System.Globalization;

namespace OdevDeneme1.Controllers
{
    public class RandevuController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminList()
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var randevular = _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .OrderByDescending(r => r.Tarih)
                .ToList();

            return View(randevular);
        }
        public IActionResult OnayBekleyenler()
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var randevular = _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r =>
                    r.Durum == RandevuDurum.Beklemede &&
                    r.Tarih >= DateTime.Now).OrderBy(r => r.Tarih)
                .ToList();

            return View(randevular);
        }

        public IActionResult AktifRandevular()
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var randevular = _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r =>
                    r.Durum == RandevuDurum.Onaylandi &&
                    r.Tarih >= DateTime.Now)
                .OrderBy(r => r.Tarih)
                .ToList();

            return View(randevular);
        }

        public IActionResult GecmisRandevular()
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var randevular = _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r =>r.Tarih < DateTime.Now)
                .OrderByDescending(r => r.Tarih)
                .ToList();

            return View(randevular);
        }

        [HttpPost]
        public IActionResult Onayla(int id)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var r = _context.Randevular.Find(id);
            if (r == null) return NotFound();

            if (r.Tarih <= DateTime.Now)
            {
                TempData["Hata"] = "Geçmiş randevular için işlem yapılamaz.";
                return RedirectToAction("AdminList");
            }

            r.Durum = RandevuDurum.Onaylandi;
            _context.SaveChanges();

            return RedirectToAction(nameof(AdminList));
        }

        [HttpPost]
        public IActionResult Reddet(int id)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var r = _context.Randevular.Find(id);
            if (r == null) return NotFound();

            if (r.Tarih <= DateTime.Now)
            {
                TempData["Hata"] = "Geçmiş randevular için işlem yapılamaz.";
                return RedirectToAction("AdminList");
            }

            r.Durum = RandevuDurum.Reddedildi;
            _context.SaveChanges();

            return RedirectToAction(nameof(AdminList));
        }

        public IActionResult MyRandevular()
        {
            var uyeId = HttpContext.Session.GetInt32("UyeId");
            if (uyeId == null)
                return RedirectToAction("Index", "Login");

            var randevular = _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Where(r => r.UyeId == uyeId)
                .OrderByDescending(r => r.Tarih)
                .ToList();

            return View(randevular);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UyeId") == null)
                return RedirectToAction("Index", "Login");

            ViewBagDoldur();

            return View(new Randevu
            {
                Tarih = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Randevu r)
        {
            var uyeId = HttpContext.Session.GetInt32("UyeId");
            if (uyeId == null)
                return RedirectToAction("Index", "Login");

            r.UyeId = uyeId.Value;
            SaatVeTarihBirlestir(r);

            var saat = Request.Form["Saat"].ToString();

            if (string.IsNullOrEmpty(saat))
            {
                ModelState.AddModelError("Saat", "Saat seçmek zorunludur.");

                ViewBag.Hizmetler = _context.Hizmetler.ToList();
                ViewBag.Antrenorler = _context.Antrenorler.ToList();
                ViewBag.MusaitSaatler = SaatleriGetir();

                return View(r);
            }

            if (r.Tarih <= DateTime.Now)
            {
                ModelState.AddModelError("", "Geçmiş bir tarih veya saat için randevu alınamaz.");
            }

            if (!AntrenorUygunMu(r))
            {
                ViewBagDoldur();
                return View(r);
            }

            if (SaatDoluMu(r))
            {
                ModelState.AddModelError("", "Bu antrenör bu saat için uygun değil.");
            }

            if (!ModelState.IsValid)
            {
                ViewBagDoldur();
                return View(r);
            }

            r.Durum = RandevuDurum.Beklemede;

            _context.Randevular.Add(r);
            _context.SaveChanges();

            return RedirectToAction(nameof(MyRandevular));
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var uyeId = HttpContext.Session.GetInt32("UyeId");
            if (uyeId == null)
                return RedirectToAction("Index", "Login");

            var r = _context.Randevular
                .Include(x => x.Hizmet)
                .Include(x => x.Antrenor)
                .FirstOrDefault(x => x.Id == id && x.UyeId == uyeId);

            if (r == null) return NotFound();

            if (r.Tarih <= DateTime.Now)
                return RedirectToAction("MyRandevular");

            ViewBag.Hizmetler = _context.Hizmetler.ToList();
            ViewBag.Antrenorler = _context.Antrenorler.ToList();
            ViewBag.MusaitSaatler = SaatleriGetir();

            return View(r);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Randevu r)
        {
            var uyeId = HttpContext.Session.GetInt32("UyeId");
            if (uyeId == null)
                return RedirectToAction("Index", "Login");

            var dbRandevu = _context.Randevular.FirstOrDefault(x => x.Id == r.Id);
            if (dbRandevu == null) return NotFound();

            var saat = Request.Form["Saat"].ToString();
            if (string.IsNullOrEmpty(saat))
            {
                ModelState.AddModelError("", "Saat seçmelisiniz.");
            }
            else
            {
                dbRandevu.Tarih = DateTime.Parse($"{r.Tarih:yyyy-MM-dd} {saat}");
            }

            if (dbRandevu.Tarih <= DateTime.Now)
            {
                ModelState.AddModelError("", "Geçmiş tarih/saat için randevu güncellenemez.");
            }

            var cakismaVarMi = _context.Randevular.Any(x =>
                x.Id != dbRandevu.Id &&
                x.AntrenorId == r.AntrenorId &&
                x.Tarih == dbRandevu.Tarih &&
                x.Durum == RandevuDurum.Onaylandi
            );

            if (cakismaVarMi)
            {
                ModelState.AddModelError("", "Bu saat dolu.");
            }

            if (!ModelState.IsValid)
            {
                ViewBagDoldur();
                return View(r);
            }

            dbRandevu.HizmetId = r.HizmetId;
            dbRandevu.AntrenorId = r.AntrenorId;
            dbRandevu.Aciklama = r.Aciklama;
            dbRandevu.Durum = RandevuDurum.Beklemede;

            _context.SaveChanges();

            return RedirectToAction(nameof(MyRandevular));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var uyeId = HttpContext.Session.GetInt32("UyeId");
            if (uyeId == null)
                return RedirectToAction("Index", "Login");

            var r = _context.Randevular
                .Include(x => x.Hizmet)
                .Include(x => x.Antrenor)
                .FirstOrDefault(x => x.Id == id && x.UyeId == uyeId);

            if (r == null) return NotFound();

            if (r.Tarih <= DateTime.Now)
                return RedirectToAction("MyRandevular");

            return View(r);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var uyeId = HttpContext.Session.GetInt32("UyeId");
            if (uyeId == null)
                return RedirectToAction("Index", "Login");

            var r = _context.Randevular.FirstOrDefault(x => x.Id == id && x.UyeId == uyeId);
            if (r == null) return NotFound();

            if (r.Tarih <= DateTime.Now)
                return RedirectToAction("MyRandevular");

            _context.Randevular.Remove(r);
            _context.SaveChanges();

            return RedirectToAction("MyRandevular");
        }

        private void ViewBagDoldur()
        {
            ViewBag.Hizmetler = _context.Hizmetler.ToList();
            ViewBag.Antrenorler = _context.Antrenorler.ToList();
            ViewBag.MusaitSaatler = SaatleriGetir();
        }

        private void SaatVeTarihBirlestir(Randevu r)
        {
            var saat = Request.Form["Saat"].ToString();
            if (!string.IsNullOrEmpty(saat))
                r.Tarih = DateTime.Parse($"{r.Tarih:yyyy-MM-dd} {saat}");
        }

        private bool SaatDoluMu(Randevu r)
        {
            return _context.Randevular.Any(x =>
                x.AntrenorId == r.AntrenorId &&
                x.Tarih == r.Tarih &&
                x.Durum == RandevuDurum.Onaylandi
            );
        }

        private bool AntrenorUygunMu(Randevu r)
        {
            var antrenor = _context.Antrenorler.Find(r.AntrenorId);
            if (antrenor == null)
            {
                ModelState.AddModelError("", "Antrenör bulunamadı.");
                return false;
            }

            var gun = r.Tarih.ToString("dddd", new CultureInfo("tr-TR"));
            if (!string.IsNullOrEmpty(antrenor.CalismaGunleri) &&
                !antrenor.CalismaGunleri.Contains(gun))
            {
                ModelState.AddModelError("", $"Antrenör {gun} günü çalışmıyor.");
                return false;
            }

            return true;
        }

        private string[] SaatleriGetir()
        {
            return new[]
            {
                "09:00","10:00","11:00","12:00","13:00",
                "14:00","15:00","16:00","17:00","18:00"
            };
        }
    }
}
