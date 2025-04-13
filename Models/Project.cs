using System;
using System.Collections.Generic;

namespace ProjectAPI.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public decimal Price { get; set; }
        public string CryptoCurrency { get; set; } // BTC, ETH, etc.
        public string ThumbnailUrl { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Tags for searching/filtering
        public string Tags { get; set; }
        
        // Navigation properties
        public List<Purchase> Purchases { get; set; }
        public List<ProjectScreenshot> Screenshots { get; set; }
    }
}
