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
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Purchases
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetAllPurchases()
        {
            return await _context.Purchases
                .Include(p => p.Project)
                .Include(p => p.User)
                .ToListAsync();
        }

        // GET: api/Purchases/user
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetUserPurchases()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            
            return await _context.Purchases
                .Where(p => p.UserId == userId)
                .Include(p => p.Project)
                .ToListAsync();
        }

        // GET: api/Purchases/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var isAdmin = User.IsInRole("Admin");
            
            var purchase = await _context.Purchases
                .Include(p => p.Project)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            // Only allow users to view their own purchases unless they're an admin
            if (purchase.UserId != userId && !isAdmin)
            {
                return Forbid();
            }

            return purchase;
        }

        // POST: api/Purchases
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Purchase>> CreatePurchase(PurchaseRequest request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            
            var project = await _context.Projects.FindAsync(request.ProjectId);
            if (project == null)
            {
                return BadRequest(new { message = "Project not found" });
            }

            // Check if user already purchased this project
            var existingPurchase = await _context.Purchases
                .FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId && p.UserId == userId && p.IsCompleted);
                
            if (existingPurchase != null)
            {
                return BadRequest(new { message = "You already own this project" });
            }

            var purchase = new Purchase
            {
                ProjectId = request.ProjectId,
                UserId = userId,
                Amount = project.Price,
                CryptoCurrency = project.CryptoCurrency,
                TransactionId = request.TransactionId,
                PurchaseDate = DateTime.UtcNow,
                IsCompleted = request.IsCompleted
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPurchase), new { id = purchase.Id }, purchase);
        }

        // PUT: api/Purchases/5/complete
        [HttpPut("{id}/complete")]
        [Authorize]
        public async Task<IActionResult> CompletePurchase(int id, CompletePurchaseRequest request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var isAdmin = User.IsInRole("Admin");
            
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            // Only allow users to complete their own purchases unless they're an admin
            if (purchase.UserId != userId && !isAdmin)
            {
                return Forbid();
            }

            purchase.TransactionId = request.TransactionId;
            purchase.IsCompleted = true;
            
            _context.Entry(purchase).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class PurchaseRequest
    {
        public int ProjectId { get; set; }
        public string TransactionId { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CompletePurchaseRequest
    {
        public string TransactionId { get; set; }
    }
}
