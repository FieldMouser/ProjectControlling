using System.ComponentModel.DataAnnotations;
 
 namespace ProjectControlling.Models;
 
 public class Projects
 {
     [Key]
     public int ProjectId { get; set; }
     [Required]
     public string ProjectName { get; set; } = null!;
     [Required]
     public string ProjectDeveloperCompany  { get; set; } = null!;
     [Required]
     public string ProjectCustomerCompany  { get; set; } = null!;
     [Required]
     public int ProjectLeaderId { get; set; }
     [Required]
     public DateTime ProjectStartDate { get; set; }
     [Required] 
     public DateTime ProjectEndDate { get; set; }
     public int ProjectPriority { get; set; }
     
     public ICollection<ProjectWorkers> ProjectWorkers { get; set; } = new List<ProjectWorkers>();
     public ICollection<ProjectDocument> ProjectDocuments { get; set; } = new List<ProjectDocument>();
 }