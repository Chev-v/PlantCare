using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantCareApp.Data;
using PlantCareApp.Models;
using System.Linq;
using System.Threading.Tasks;

[Authorize] // require user to be authenticated to access any action in this controller
public class PlantsController : Controller
{
    private readonly ApplicationDbContext _context;

    // constructor to initialize the database context
    public PlantsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous] // allow anonymous users to access this action
    public async Task<IActionResult> Index()
    {
        // retrieve all plants from the database and pass them to the view
        return View(await _context.Plants.ToListAsync());
    }

    [AllowAnonymous] // allow anonymous users to access this action
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) // check if the id is null
        {
            return NotFound(); // return a 404 error if id is not provided
        }

        // find the plant by id
        var plant = await _context.Plants
            .FirstOrDefaultAsync(m => m.PlantId == id);
        if (plant == null) // check if the plant exists
        {
            return NotFound(); // return a 404 error if the plant is not found
        }

        return View(plant); // pass the plant to the view
    }

    [Authorize(Roles = "Admin")] // restrict access to users with the "Admin" role
    public IActionResult Create()
    {
        return View(); // return the create view
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // restrict access to users with the "Admin" role
    [ValidateAntiForgeryToken] // prevent cross-site request forgery
    public async Task<IActionResult> Create([Bind("PlantId,Name,Description")] Plant plant)
    {
        if (ModelState.IsValid) // check if the model is valid
        {
            _context.Add(plant); // add the new plant to the database context
            await _context.SaveChangesAsync(); // save changes to the database
            return RedirectToAction(nameof(Index)); // redirect to the index action
        }
        return View(plant); // return the view with the current plant data
    }

    [Authorize(Roles = "Admin")] // restrict access to users with the "Admin" role
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) // check if the id is null
        {
            return NotFound(); // return a 404 error if id is not provided
        }

        // find the plant by id
        var plant = await _context.Plants.FindAsync(id);
        if (plant == null) // check if the plant exists
        {
            return NotFound(); // return a 404 error if the plant is not found
        }
        return View(plant); // return the edit view with the plant data
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // restrict access to users with the "Admin" role
    [ValidateAntiForgeryToken] // prevent cross-site request forgery
    public async Task<IActionResult> Edit(int id, [Bind("PlantId,Name,Description")] Plant plant)
    {
        if (id != plant.PlantId) // check if the provided id matches the plant's id
        {
            return NotFound(); // return a 404 error if ids do not match
        }

        if (ModelState.IsValid) // check if the model is valid
        {
            try
            {
                _context.Update(plant); // update the plant in the database context
                await _context.SaveChangesAsync(); // save changes to the database
            }
            catch (DbUpdateConcurrencyException) // handle concurrency issues
            {
                if (!PlantExists(plant.PlantId)) // check if the plant still exists
                {
                    return NotFound(); // return a 404 error if the plant no longer exists
                }
                else
                {
                    throw; // rethrow the exception if something else went wrong
                }
            }
            return RedirectToAction(nameof(Index)); // redirect to the index action
        }
        return View(plant); // return the view with the current plant data
    }

    [Authorize(Roles = "Admin")] // restrict access to users with the "Admin" role
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) // check if the id is null
        {
            return NotFound(); // return a 404 error if id is not provided
        }

        // find the plant by id
        var plant = await _context.Plants
            .FirstOrDefaultAsync(m => m.PlantId == id);
        if (plant == null) // check if the plant exists
        {
            return NotFound(); // return a 404 error if the plant is not found
        }

        return View(plant); // return the delete confirmation view with the plant data
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken] // prevent cross-site request forgery
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // find the plant by id
        var plant = await _context.Plants.FindAsync(id);
        if (plant == null) // check if the plant exists
        {
            return NotFound(); // return a 404 error if the plant is not found
        }
        _context.Plants.Remove(plant); // remove the plant from the database context
        await _context.SaveChangesAsync(); // save changes to the database
        return RedirectToAction(nameof(Index)); // redirect to the index action
    }

    private bool PlantExists(int id)
    {
        // check if a plant with the specified id exists in the database
        return _context.Plants.Any(e => e.PlantId == id);
    }
}
