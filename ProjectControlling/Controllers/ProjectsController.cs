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
    public IActionResult Index(
        string sortOrder,
        int? priorityFilter,
        DateTime? startDateFrom,
        DateTime? startDateTo,
        string searchString)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["StartDateSortParm"] = sortOrder == "start_date" ? "start_date_desc" : "start_date";
        ViewData["PrioritySortParm"] = sortOrder == "priority" ? "priority_desc" : "priority";
    
        var projects = _context.Projects
            .Include(p => p.ProjectWorkers)
            .ThenInclude(pw => pw.Worker)
            .Include(p => p.ProjectDocuments)
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(searchString))
        {
            projects = projects.Where(p =>
                p.ProjectName.Contains(searchString) ||
                p.ProjectDeveloperCompany.Contains(searchString) ||
                p.ProjectCustomerCompany.Contains(searchString));
        }

        if (priorityFilter.HasValue)
        {
            projects = projects.Where(p => p.ProjectPriority == priorityFilter.Value);
        }

        if (startDateFrom.HasValue)
        {
            projects = projects.Where(p => p.ProjectStartDate >= startDateFrom.Value);
        }
    
        if (startDateTo.HasValue)
        {
            projects = projects.Where(p => p.ProjectStartDate <= startDateTo.Value);
        }
        
        projects = sortOrder switch
        {
            "name_desc" => projects.OrderByDescending(p => p.ProjectName),
            "start_date" => projects.OrderBy(p => p.ProjectStartDate),
            "start_date_desc" => projects.OrderByDescending(p => p.ProjectStartDate),
            "priority" => projects.OrderBy(p => p.ProjectPriority),
            "priority_desc" => projects.OrderByDescending(p => p.ProjectPriority),
            _ => projects.OrderBy(p => p.ProjectName)
        };

        return View(projects.ToList());
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
    
// DELETE PROJECT
    [HttpPost]
    public IActionResult DeleteProject(int id)
    {
        var project = _context.Projects.Find(id);
        if (project == null)
        {
            return NotFound();
        }
        
        var documents = _context.ProjectDocuments.Where(d => d.ProjectId == id).ToList();
        foreach (var document in documents)
        {
            var filePath = document.FilePath;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            _context.ProjectDocuments.Remove(document);
        }
        
        var projectWorkers = _context.ProjectWorkers.Where(pw => pw.ProjectId == id).ToList();
        _context.ProjectWorkers.RemoveRange(projectWorkers);

        _context.Projects.Remove(project);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // REMOVE WORKER FROM PROJECT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public JsonResult RemoveWorkerFromProject(int projectId, int workerId)
    {
        var projectWorker = _context.ProjectWorkers
            .FirstOrDefault(pw => pw.ProjectId == projectId && pw.WorkerId == workerId);

        if (projectWorker == null)
        {
            return Json(new { success = false, message = "Worker not found in project" });
        }

        _context.ProjectWorkers.Remove(projectWorker);
        _context.SaveChanges();

        return Json(new { success = true });
    }
    public IActionResult Edit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var project = _context.Projects
        .Include(p => p.ProjectWorkers)
        .ThenInclude(pw => pw.Worker)
        .FirstOrDefault(p => p.ProjectId == id);
    
    if (project == null)
    {
        return NotFound();
    }
    
    ViewBag.Workers = _context.Workers.ToList();
    return View(project);
}
    
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, Projects project, List<int> assignedWorkerIds)
{
    if (id != project.ProjectId)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(project);
            
            var existingWorkers = _context.ProjectWorkers
                .Where(pw => pw.ProjectId == id)
                .ToList();
            
            foreach (var existingWorker in existingWorkers)
            {
                if (!assignedWorkerIds.Contains(existingWorker.WorkerId))
                {
                    _context.Remove(existingWorker);
                }
            }
            
            foreach (var workerId in assignedWorkerIds)
            {
                if (!existingWorkers.Any(ew => ew.WorkerId == workerId))
                {
                    _context.ProjectWorkers.Add(new ProjectWorkers
                    {
                        ProjectId = id,
                        WorkerId = workerId
                    });
                }
            }
            
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectExists(project.ProjectId))
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
    
    ViewBag.Workers = _context.Workers.ToList();
    return View(project);
}

private bool ProjectExists(int id)
{
    return _context.Projects.Any(e => e.ProjectId == id);
}
}