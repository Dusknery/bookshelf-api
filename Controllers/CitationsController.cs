using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/citations")]
public class CitationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CitationsController(AppDbContext context)
    {
        _context = context;
    }

    //GET all citations

    [HttpGet]
    public async Task<ActionResult<List<Citation>>> GetCitations()
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        var citations = await _context.Citations
        .Where(b => b.UserId == userId)
        .ToListAsync();

        return Ok(citations);
    }


    //Get citation by id
    [HttpGet("{id}")]
    public async Task<ActionResult<Citation>> GetCitation(int id)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        var citation = await _context.Citations
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (citation == null)
            return NotFound();

        return Ok(citation);
    }

    //POST create new citation
    [HttpPost]
    public async Task<ActionResult<Citation>> AddCitation(Citation citation)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        citation.UserId = userId;

        _context.Citations.Add(citation);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCitation", new { id = citation.Id }, citation);
    }

    //PUT update citation
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCitation(int id, Citation updatedCitation)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        var existingcitation = await _context.Citations
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (existingcitation == null)
            return NotFound();

       existingcitation.CitationText = updatedCitation.CitationText;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    //DELETE citation
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCitation(int id)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        var citation = await _context.Citations
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (citation == null)
            return NotFound();

        _context.Citations.Remove(citation);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
