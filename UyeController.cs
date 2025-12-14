using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdevDeneme1.Data;
using OdevDeneme1.Models;

public class UyeController : Controller
{
    private readonly ApplicationDbContext _context;

    public UyeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Uyeler.ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Uye uye)
    {
        if (!ModelState.IsValid)
            return View(uye);

        _context.Uyeler.Add(uye);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var uye = _context.Uyeler.Find(id);
        if (uye == null) return NotFound();
        return View(uye);
    }

    [HttpPost]
    public IActionResult Edit(Uye uye)
    {
        if (!ModelState.IsValid)
            return View(uye);

        _context.Uyeler.Update(uye);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var uye = _context.Uyeler.Find(id);
        if (uye == null) return NotFound();
        return View(uye);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var uye = _context.Uyeler.Find(id);
        _context.Uyeler.Remove(uye);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
