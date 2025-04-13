using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Data;
using ProjectAPI.Models;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects(string search = null, string tags = null)
        {
            IQueryable<Project> query = _context.Projects
                .Include(p => p.Screenshots);

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => 
                    p.Title.Contains(search) || 
                    p.ShortDescription.Contains(search) || 
                    p.FullDescription.Contains(search) ||
                    p.Tags.Contains(search));
            }

            // Apply tags filter if provided
            if (!string.IsNullOrEmpty(tags))
            {
                var tagList = tags.Split(',').Select(t => t.Trim().ToLower());
                query = query.Where(p => tagList.Any(tag => p.Tags.ToLower().Contains(tag)));
            }

            return await query.ToListAsync();
        }

        // GET: api/Projects/featured
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<Project>>> GetFeaturedProjects()
        {
            return await _context.Projects
                .Where(p => p.IsFeatured)
                .Include(p => p.Screenshots)
                .ToListAsync();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Screenshots)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // POST: api/Projects
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            project.CreatedAt = DateTime.UtcNow;
            
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            project.UpdatedAt = DateTime.UtcNow;
            
            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Projects/5/screenshots
        [HttpPost("{id}/screenshots")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProjectScreenshot>> AddScreenshot(int id, ProjectScreenshot screenshot)
        {
            if (id != screenshot.ProjectId)
            {
                return BadRequest();
            }

            if (!ProjectExists(id))
            {
                return NotFound();
            }

            _context.ProjectScreenshots.Add(screenshot);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = id }, screenshot);
        }

        // DELETE: api/Projects/screenshots/5
        [HttpDelete("screenshots/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteScreenshot(int id)
        {
            var screenshot = await _context.ProjectScreenshots.FindAsync(id);
            if (screenshot == null)
            {
                return NotFound();
            }

            _context.ProjectScreenshots.Remove(screenshot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
