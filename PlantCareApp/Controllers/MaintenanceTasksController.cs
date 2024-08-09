using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlantCareApp.Data;
using PlantCareApp.Models;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class MaintenanceTasksController : Controller
{
    private readonly ApplicationDbContext _context;

    public MaintenanceTasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var tasks = await _context.MaintenanceTasks.Include(m => m.Plant).ToListAsync();
        return View(tasks);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var maintenanceTask = await _context.MaintenanceTasks
            .Include(m => m.Plant)
            .FirstOrDefaultAsync(m => m.TaskId == id);

        if (maintenanceTask == null)
        {
            return NotFound();
        }

        return View(maintenanceTask);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "Name");
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TaskType,Date,PlantId")] MaintenanceTask maintenanceTask)
    {
        if (ModelState.IsValid)
        {
            _context.Add(maintenanceTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "Name", maintenanceTask.PlantId);
        return View(maintenanceTask);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var maintenanceTask = await _context.MaintenanceTasks.FindAsync(id);

        if (maintenanceTask == null)
        {
            return NotFound();
        }

        ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "Name", maintenanceTask.PlantId);
        return View(maintenanceTask);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("TaskId,TaskType,Date,PlantId")] MaintenanceTask maintenanceTask)
    {
        if (id != maintenanceTask.TaskId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(maintenanceTask);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceTaskExists(maintenanceTask.TaskId))
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

        ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "Name", maintenanceTask.PlantId);
        return View(maintenanceTask);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var maintenanceTask = await _context.MaintenanceTasks
            .Include(m => m.Plant)
            .FirstOrDefaultAsync(m => m.TaskId == id);

        if (maintenanceTask == null)
        {
            return NotFound();
        }

        return View(maintenanceTask);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var maintenanceTask = await _context.MaintenanceTasks.FindAsync(id);
        if (maintenanceTask == null)
        {
            return NotFound();
        }
        _context.MaintenanceTasks.Remove(maintenanceTask);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    private bool MaintenanceTaskExists(int id)
    {
        return _context.MaintenanceTasks.Any(e => e.TaskId == id);
    }
}
