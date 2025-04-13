using System;
using System.Collections.Generic;

namespace ProjectAPI.Models;

public class ApplicationUser
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsAdmin { get; set; }
        
    // Navigation properties
    public List<Purchase> Purchases { get; set; }
}