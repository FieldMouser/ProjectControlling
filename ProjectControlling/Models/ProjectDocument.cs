using System.ComponentModel.DataAnnotations;

namespace ProjectControlling.Models;

public class ProjectDocument
{
    [Key]
    public int ProjectDocumentId { get; set; }
        
    [Required]
    public int ProjectId { get; set; }
        
    [Required]
    public string FileName { get; set; }
        
    [Required]
    public string FilePath { get; set; }
        
    [Required]
    public DateTime UploadDate { get; set; }
        
    public Projects Project { get; set; }
}