using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectControlling.Models;
using ProjectControlling.Data;

namespace ProjectControlling.Controllers;

public class ProjectsController : Controller
{
    private readonly ProjectControllingContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProjectsController(ProjectControllingContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }
    
    public IActionResult Create()
    {
        ViewBag.Workers = _context.Workers
            .AsEnumerable()
            .Select(w => new
            {
                w.WorkerId,
                FullName = $"{w.WorkerName} {w.WorkerLastName}"
            }).OrderBy(w => w.FullName).ToList();
        return View(new Projects());
    }

    [HttpGet]
    public JsonResult SearchEmployees(string term)
    {
        var employees = _context.Workers
            .Where(w => 
                w.WorkerName.Contains(term) ||
                w.WorkerLastName.Contains(term) ||
                w.WorkerEmail.Contains(term))
            .Select(w => new
            {
                id = w.WorkerId,
                text = $"{w.WorkerName} | {w.WorkerLastName} | ({w.WorkerEmail})"
            })
            .Take(10)
            .ToList();
        return Json(employees);
    }
    
    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(
        Projects project,
        List<int> assignedWorkerIds,
        IFormFileCollection projectFiles)
    {
        if (ModelState.IsValid)
        {
            if (assignedWorkerIds != null)
            {
                foreach (var workerId in assignedWorkerIds)
                {
                    project.ProjectWorkers.Add(new ProjectWorkers
                    {
                        WorkerId = workerId
                    }); 
                }
            }
            
            _context.Projects.Add(project);
            _context.SaveChanges();

            if (projectFiles?.Count > 0)
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", project.ProjectId.ToString());
                Directory.CreateDirectory(uploadsPath);
                
                foreach (IFormFile file in projectFiles)
                {
                    if (file.Length > 0)
                    {
                        var filePath = Path.Combine(uploadsPath, file.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    
                        _context.ProjectDocuments.Add(new ProjectDocument
                        {
                            ProjectId = project.ProjectId,
                            FileName = file.FileName,
                            FilePath = filePath,
                            UploadDate = DateTime.Now
                        });
                    }
                }
                _context.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }
        
        ViewBag.Workers = _context.Workers
            .Select(w => new { w.WorkerId, FullName = $"{w.WorkerName} {w.WorkerLastName}" })
            .ToList();
        
        return View(project);
    }
    
    // GET
    public IActionResult Index()
    {
        var projects = _context.Projects
            .Include(p => p.ProjectWorkers)
            .ThenInclude(pw => pw.Worker)
            .Include(p => p.ProjectDocuments)
            .ToList();
    
        return View(projects);
    }
    
    public IActionResult GetProjectDetails(int id)
    {
        var project = _context.Projects
            .Include(p => p.ProjectWorkers)
            .ThenInclude(pw => pw.Worker)
            .Include(p => p.ProjectDocuments)
            .FirstOrDefault(p => p.ProjectId == id);

        if (project == null)
        {
            return NotFound();
        }

        return PartialView("_ProjectDetailsPartial", project);
    }
}