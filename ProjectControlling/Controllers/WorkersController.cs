using Microsoft.AspNetCore.Mvc;
using ProjectControlling.Data;
using ProjectControlling.Models;

namespace ProjectControlling.Controllers;

public class WorkersController : Controller
{
    private readonly ProjectControllingContext _context;

    public WorkersController(ProjectControllingContext context)
    {
        _context = context;
    }
    // GET
    public IActionResult Index()
    {
        var workers = _context.Workers.ToList();
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Workers worker)
    {
        if (ModelState.IsValid)
        {
            _context.Workers.Add(worker);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(worker);
    }
}