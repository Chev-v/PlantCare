using Microsoft.AspNetCore.Mvc;
using PlantCareApp.Models;

public interface IMaintenanceTasksController
{
    IActionResult Create();
    Task<IActionResult> Create([Bind(new[] { "TaskType,Date,PlantId" })] MaintenanceTask maintenanceTask);
    Task<IActionResult> Delete(int? id);
    Task<IActionResult> DeleteConfirmed(int id);
    Task<IActionResult> Details(int? id);
    Task<IActionResult> Edit(int id, [Bind(new[] { "TaskId,TaskType,Date,PlantId" })] MaintenanceTask maintenanceTask);
    Task<IActionResult> Edit(int? id);
    Task<IActionResult> Index();
}