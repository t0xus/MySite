using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySite.Models;
using MySite.Pages;

[ApiController]
[Route("api/[controller]")]
public class SuggestionsController : ControllerBase
{
    private readonly ILogger<IndexModel> _logger;

    private readonly resumeContext _context;

    public SuggestionsController(ILogger<IndexModel> logger, resumeContext context)
    {
        _logger = logger;
        _context = context;
    }


    [HttpGet]
    public IActionResult GetSuggestions([FromQuery] string term)
    {
        // Beispielhaft statische Daten; in der Praxis erfolgt hier eine DB-Abfrage etc.
        var allSuggestions = new List<string>();
            
        foreach (var content in _context.ResumeLifestages.ToList())
        {
            allSuggestions.Add(content.LifestageTitle);
        }

        var result = allSuggestions
            .Where(s => s.StartsWith(term, StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        return Ok(result);
    }
}
