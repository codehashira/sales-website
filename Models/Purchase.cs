using System;

namespace ProjectAPI.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string CryptoCurrency { get; set; }
        public string TransactionId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsCompleted { get; set; }
        
        // Navigation properties
        public Project Project { get; set; }
        public ApplicationUser User { get; set; }
    }
}
