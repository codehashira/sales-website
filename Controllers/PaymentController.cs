using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Data;
using ProjectAPI.Models;
using ProjectAPI.Services;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Payment/checkout/5
        [HttpGet("checkout/{projectId}")]
        [Authorize]
        public async Task<ActionResult<PaymentCheckoutResponse>> GetCheckoutInfo(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            // This is a simulated Coinbase Commerce integration
            // In a real implementation, this would create a checkout session with Coinbase Commerce
            var checkoutId = Guid.NewGuid().ToString();
            
            return new PaymentCheckoutResponse
            {
                CheckoutId = checkoutId,
                ProjectId = project.Id,
                ProjectTitle = project.Title,
                Amount = project.Price,
                Currency = project.CryptoCurrency,
                PaymentAddress = GenerateSimulatedAddress(project.CryptoCurrency),
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        // POST: api/Payment/verify
        [HttpPost("verify")]
        [Authorize]
        public async Task<ActionResult<PaymentVerificationResponse>> VerifyPayment(PaymentVerificationRequest request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            
            var project = await _context.Projects.FindAsync(request.ProjectId);
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            // This is a simulated payment verification
            // In a real implementation, this would verify the transaction with Coinbase Commerce
            var isVerified = true; // Simulate successful verification
            
            if (isVerified)
            {
                // Create a completed purchase record
                var purchase = new Purchase
                {
                    ProjectId = request.ProjectId,
                    UserId = userId,
                    Amount = project.Price,
                    CryptoCurrency = project.CryptoCurrency,
                    TransactionId = request.TransactionId,
                    PurchaseDate = DateTime.UtcNow,
                    IsCompleted = true
                };

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                return new PaymentVerificationResponse
                {
                    Success = true,
                    PurchaseId = purchase.Id,
                    DownloadUrl = project.DownloadUrl
                };
            }
            else
            {
                return new PaymentVerificationResponse
                {
                    Success = false,
                    Message = "Payment verification failed"
                };
            }
        }

        private string GenerateSimulatedAddress(string currency)
        {
            // Generate a simulated crypto address based on the currency
            switch (currency.ToUpper())
            {
                case "BTC":
                    return "bc1q" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 38);
                case "ETH":
                    return "0x" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 40);
                default:
                    return "addr_" + Guid.NewGuid().ToString().Replace("-", "");
            }
        }
    }

    public class PaymentCheckoutResponse
    {
        public string CheckoutId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentAddress { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class PaymentVerificationRequest
    {
        public int ProjectId { get; set; }
        public string CheckoutId { get; set; }
        public string TransactionId { get; set; }
    }

    public class PaymentVerificationResponse
    {
        public bool Success { get; set; }
        public int? PurchaseId { get; set; }
        public string DownloadUrl { get; set; }
        public string Message { get; set; }
    }
}
