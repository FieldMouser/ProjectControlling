using Microsoft.AspNetCore.Mvc;
using ProjectControlling.Models;
using ProjectControlling.Data;

namespace ProjectControlling.Controllers;

public class ProjectsController : Controller
{
    private readonly ProjectControllingContext _context;

    public ProjectsController(ProjectControllingContext context)
    {
        _context = context;
    }
    
    // GET
    public IActionResult Index()
    {
        var projects = _context.Projects.ToList();
        return View(projects);
    }
}