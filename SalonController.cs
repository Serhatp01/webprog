using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OdevDeneme1.Data;
using OdevDeneme1.Models;

namespace OdevDeneme1.Controllers
{
    public class SalonController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public SalonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔐 ADMIN KONTROLÜ
        private IActionResult AdminKontrol()
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var salonlar = await _context.Salonlar.ToListAsync();
            return View(salonlar);
        }

        public IActionResult Create()
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Salon salon)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            if (!ModelState.IsValid)
                return View(salon);

            _context.Salonlar.Add(salon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var salon = await _context.Salonlar.FindAsync(id);
            if (salon == null) return NotFound();

            return View(salon);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Salon salon)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            if (!ModelState.IsValid)
                return View(salon);

            _context.Salonlar.Update(salon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var salon = await _context.Salonlar.FindAsync(id);
            if (salon == null) return NotFound();

            return View(salon);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kontrol = AdminKontrol();
            if (kontrol != null) return kontrol;

            var salon = await _context.Salonlar.FindAsync(id);
            if (salon == null) return NotFound();

            _context.Salonlar.Remove(salon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
