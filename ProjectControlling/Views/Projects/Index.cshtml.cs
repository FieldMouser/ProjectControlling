using System.Collections;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjectControlling.Views.Projects;

public class Index : PageModel, IEnumerable
{
    public void OnGet()
    {
        
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}