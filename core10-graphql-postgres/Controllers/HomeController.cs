using Microsoft.AspNetCore.Mvc;

namespace core10_graphql_postgres.Controllers;

public class HomeController : Controller
{

    [HttpGet("/")]
    public async Task<IActionResult> GetHtmlFromFile()
    {

        var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Views", "index.html");
        var html = await System.IO.File.ReadAllTextAsync(path);
        return Content(html, "text/html");        
    
    }
}