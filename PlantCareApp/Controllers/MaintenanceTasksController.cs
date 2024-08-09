using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlantCareApp.Data;
using PlantCareApp.Models;
using System.Linq;
using System.Threading.Tasks;

[Authorize] // require user to be authenticated
public class MaintenanceTasksController : Controller
{
    private readonly ApplicationDbContext _context;

    public MaintenanceTasksController(ApplicationDbContext context)
    {
        _context = context; // set the database context
    }

    [AllowAnonymous] // allow anonymous access to index
    public async Task<IActionResult> Index()
    {
        var tasks = await _context.MaintenanceTasks.Include(m => m.Plant).ToListAsync(); // get all maintenance tasks with related plant data
        return View(tasks); // pass tasks to view
    }

    [AllowAnonymous] // allow anonymous access to details
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) // check if id is null
        {
            return NotFound(); // return not found if id is not provided
        }

        var maintenanceTask = await _context.MaintenanceTasks
            .Include(m => m.Plant) // include related plant data
            .FirstOrDefaultAsync(m => m.TaskId == id); // find maintenance task by id

        if (maintenanceTask == null) // check if maintenance task exists
        {
            return NotFound(); // return not found if maintenance task does not exist
        }

        return View(maintenanceTask); // pass maintenance task to view
    }

    [Authorize(Roles = "Admin")] // restrict create to admin
    public IActionResult Create()
    {
        ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "Name"); // populate dropdown with plants
        return View(); // return create view
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // restrict create post to admin
    [ValidateAntiForgeryToken] // prevent CSRF attacks
    public async Task<IActionResult> Create([Bind("TaskType,Date,PlantId")] MaintenanceTask maintenanceTask)
    {
        if (ModelState.IsValid) // check if model state is valid
        {
            _context.Add(maintenanceTask); // add new maintenance task to context
            await _context.SaveChangesAsync(); // save changes to database
            return RedirectToAction(nameof(Index)); // redirect to index
        }

        ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "Name", maintenanceTask.PlantId); // repopulate dropdown if model state is invalid
        return View(maintenanceTask); // return view with current maintenance task data
    }

    [Authorize(Roles = "Admin")] // restrict edit to admin
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) // check if id is null
        {
            return NotFound(); // return not found if id is not provided
        }

        var maintenanceTask = await _context.MaintenanceTasks.FindAsync(id); // find maintenance task by id

        if (maintenanceTask == null) // check if maintenance task exists
        {
            return NotFound(); // return not found if maintenance task does not exist
        }

        ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "Name", maintenanceTask.PlantId); // populate dropdown with plants
        return View(maintenanceTask); // return edit view with current maintenance task data
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // restrict edit post to admin
    [ValidateAntiForgeryToken] // prevent CSRF attacks
    public async Task<IActionResult> Edit(int id, [Bind("TaskId,TaskType,Date,PlantId")] MaintenanceTask maintenanceTask)
    {
        if (id != maintenanceTask.TaskId) // check if the provided id matches the task's id
        {
            return NotFound(); // return not found if ids don't match
        }

        if (ModelState.IsValid) // check if model state is valid
        {
            try
            {
                _context.Update(maintenanceTask); // update maintenance task in context
                await _context.SaveChangesAsync(); // save changes to database
            }
            catch (DbUpdateConcurrencyException) // handle concurrency issues
            {
                if (!MaintenanceTaskExists(maintenanceTask.TaskId)) // check if the maintenance task still exists
                {
                    return NotFound(); // return not found if the maintenance task no longer exists
                }
                else
                {
                    throw; // rethrow the exception if something else went wrong
                }
            }
            return RedirectToAction(nameof(Index)); // redirect to index
        }

        ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "Name", maintenanceTask.PlantId); // repopulate dropdown if model state is invalid
        return View(maintenanceTask); // return view with current maintenance task data
    }

    [Authorize(Roles = "Admin")] // restrict delete to admin
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) // check if id is null
        {
            return NotFound(); // return not found if id is not provided
        }

        var maintenanceTask = await _context.MaintenanceTasks
            .Include(m => m.Plant) // include related plant data
            .FirstOrDefaultAsync(m => m.TaskId == id); // find maintenance task by id

        if (maintenanceTask == null) // check if maintenance task exists
        {
            return NotFound(); // return not found if maintenance task does not exist
        }

        return View(maintenanceTask); // return delete confirmation view with maintenance task data
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")] // restrict delete post to admin
    [ValidateAntiForgeryToken] // prevent CSRF attacks
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var maintenanceTask = await _context.MaintenanceTasks.FindAsync(id); // find maintenance task by id
        if (maintenanceTask == null) // check if maintenance task exists
        {
            return NotFound(); // return not found if maintenance task does not exist
        }
        _context.MaintenanceTasks.Remove(maintenanceTask); // remove maintenance task from context
        await _context.SaveChangesAsync(); // save changes to database
        return RedirectToAction(nameof(Index)); // redirect to index
    }

    private bool MaintenanceTaskExists(int id)
    {
        return _context.MaintenanceTasks.Any(e => e.TaskId == id); // check if maintenance task exists by id
    }
}
