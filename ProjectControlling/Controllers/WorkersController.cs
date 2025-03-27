using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        return View(workers);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Workers worker)
    {
        if (ModelState.IsValid)
        {
            _context.Workers.Add(worker);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(worker);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var worker = _context.Workers.Find(id);
        if (worker == null)
        {
            return NotFound();
        }
        
        _context.Workers.Remove(worker);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var worker = _context.Workers.Find(id);
        if (worker == null)
        {
            return NotFound();
        }
        return View(worker);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("WorkerId,WorkerName,WorkerLastName,WorkerMiddleName,WorkerEmail")] Workers worker)
    {
        if (id != worker.WorkerId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(worker);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkerExists(worker.WorkerId))
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
        return View(worker);
    }

    private bool WorkerExists(int id)
    {
        return _context.Workers.Any(e => e.WorkerId == id);
    }
}