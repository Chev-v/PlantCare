using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantCareApp.Data;
using PlantCareApp.Models;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class PlantsController : Controller
{
    private readonly ApplicationDbContext _context;

    public PlantsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Plants.ToListAsync());
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var plant = await _context.Plants
            .FirstOrDefaultAsync(m => m.PlantId == id);
        if (plant == null)
        {
            return NotFound();
        }

        return View(plant);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PlantId,Name,Description")] Plant plant)
    {
        if (ModelState.IsValid)
        {
            _context.Add(plant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(plant);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var plant = await _context.Plants.FindAsync(id);
        if (plant == null)
        {
            return NotFound();
        }
        return View(plant);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PlantId,Name,Description")] Plant plant)
    {
        if (id != plant.PlantId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(plant);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantExists(plant.PlantId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(plant);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var plant = await _context.Plants
            .FirstOrDefaultAsync(m => m.PlantId == id);
        if (plant == null)
        {
            return NotFound();
        }

        return View(plant);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var plant = await _context.Plants.FindAsync(id);
        if (plant == null)
        {
            return NotFound();
        }
        _context.Plants.Remove(plant);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    private bool PlantExists(int id)
    {
        return _context.Plants.Any(e => e.PlantId == id);
    }
}
