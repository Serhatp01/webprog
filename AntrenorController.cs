using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OdevDeneme1.Data;
using OdevDeneme1.Models;

namespace OdevDeneme1.Controllers
{
    public class AntrenorController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public AntrenorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            return View(_context.Antrenorler.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Antrenor antrenor)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            if (!ModelState.IsValid)
                return View(antrenor);

            antrenor.MusaitSaatler = string.Join(", ", Request.Form["MusaitSaatler"]);
            antrenor.CalismaGunleri = string.Join(", ", Request.Form["CalismaGunleri"]);

            _context.Antrenorler.Add(antrenor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var ant = _context.Antrenorler.Find(id);
            if (ant == null) return NotFound();

            return View(ant);
        }

        [HttpPost]
        public IActionResult Edit(Antrenor antrenor)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            if (!ModelState.IsValid)
                return View(antrenor);

            antrenor.MusaitSaatler = string.Join(", ", Request.Form["MusaitSaatler"]);
            antrenor.CalismaGunleri = string.Join(", ", Request.Form["CalismaGunleri"]);

            _context.Antrenorler.Update(antrenor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var ant = _context.Antrenorler.Find(id);
            if (ant == null) return NotFound();

            return View(ant);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var ant = _context.Antrenorler.Find(id);
            if (ant == null) return NotFound();

            _context.Antrenorler.Remove(ant);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
