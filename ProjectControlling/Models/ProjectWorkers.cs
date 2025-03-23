namespace ProjectControlling.Models;

public class ProjectWorkers
{
    public int ProjectWorkersId { get; set; }
    
    public int ProjectId { get; set; }
    public Projects Project { get; set; } = null!;
    
    public int WorkerId { get; set; }
    public Workers Worker { get; set; } = null!;
}