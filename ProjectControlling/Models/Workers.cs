using System.ComponentModel.DataAnnotations;

namespace ProjectControlling.Models;

public class Workers
{
    [Key]
    public int WorkerId { get; set; }
    
    [Required]
    public string WorkerName { get; set; } = null!;
    [Required] 
    public string WorkerLastName { get; set; } = null!;
    public string WorkerMiddleName { get; set; }
    [Required] [EmailAddress] 
    public string WorkerEmail { get; set; } = null!;
    
    public ICollection<ProjectWorkers> ProjectWorkers { get; set; } = new List<ProjectWorkers>();
}