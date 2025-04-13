using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Models;

namespace ProjectAPI.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Look for existing data
            if (context.Projects.Any() || context.Users.Any())
            {
                return; // DB has been seeded
            }

            // Add admin user
            var adminUser = new ApplicationUser
            {
                Email = "admin@example.com",
                PasswordHash = HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                IsAdmin = true
            };

            // Add regular user
            var regularUser = new ApplicationUser
            {
                Email = "user@example.com",
                PasswordHash = HashPassword("User123!"),
                FirstName = "Regular",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                IsAdmin = false
            };

            context.Users.AddRange(adminUser, regularUser);
            context.SaveChanges();

            // Add sample projects
            var projects = new List<Project>
            {
                new Project
                {
                    Title = "Crypto Trading Bot",
                    ShortDescription = "Automated trading bot for cryptocurrency markets",
                    FullDescription = "This advanced trading bot uses machine learning algorithms to analyze market trends and execute trades automatically. It supports multiple exchanges including Binance, Coinbase Pro, and Kraken. Features include customizable trading strategies, real-time market analysis, and detailed performance reporting.",
                    Price = 0.005m,
                    CryptoCurrency = "BTC",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1621761191319-c6fb62004040",
                    DownloadUrl = "https://example.com/downloads/trading-bot.zip",
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    Tags = "trading,crypto,bot,automation"
                },
                new Project
                {
                    Title = "Portfolio Tracker",
                    ShortDescription = "Track and analyze your investment portfolio",
                    FullDescription = "Keep track of all your investments in one place with this comprehensive portfolio tracker. Monitor stocks, cryptocurrencies, ETFs, and more. Features include performance analytics, dividend tracking, tax reporting, and integration with major brokerages and exchanges.",
                    Price = 0.003m,
                    CryptoCurrency = "BTC",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71",
                    DownloadUrl = "https://example.com/downloads/portfolio-tracker.zip",
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    Tags = "portfolio,investment,tracking,finance"
                },
                new Project
                {
                    Title = "Market Scanner",
                    ShortDescription = "Scan markets for trading opportunities",
                    FullDescription = "This powerful market scanner helps you identify trading opportunities across multiple markets. Set custom alerts for price movements, volume spikes, and technical indicators. Scan stocks, options, futures, and cryptocurrencies all from one interface.",
                    Price = 0.08m,
                    CryptoCurrency = "ETH",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1611974789855-9c2a0a7236a3",
                    DownloadUrl = "https://example.com/downloads/market-scanner.zip",
                    IsFeatured = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    Tags = "scanner,trading,analysis,alerts"
                },
                new Project
                {
                    Title = "Backtesting Framework",
                    ShortDescription = "Test trading strategies against historical data",
                    FullDescription = "Validate your trading strategies with this comprehensive backtesting framework. Test against years of historical data across multiple markets. Features include detailed performance metrics, optimization tools, and Monte Carlo simulations to assess strategy robustness.",
                    Price = 0.1m,
                    CryptoCurrency = "ETH",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1642543348745-775d77351ac2",
                    DownloadUrl = "https://example.com/downloads/backtesting-framework.zip",
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    Tags = "backtesting,strategy,trading,historical"
                },
                new Project
                {
                    Title = "Options Calculator",
                    ShortDescription = "Calculate options prices and Greeks",
                    FullDescription = "Advanced options pricing calculator using Black-Scholes and binomial models. Calculate fair value, implied volatility, and all Greeks (Delta, Gamma, Theta, Vega, Rho). Visualize risk profiles for complex options strategies and analyze the impact of changing market conditions.",
                    Price = 0.002m,
                    CryptoCurrency = "BTC",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1535320903710-d993d3d77d29",
                    DownloadUrl = "https://example.com/downloads/options-calculator.zip",
                    IsFeatured = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    Tags = "options,calculator,trading,finance"
                }
            };

            context.Projects.AddRange(projects);
            context.SaveChanges();

            // Add screenshots for projects
            var screenshots = new List<ProjectScreenshot>
            {
                // Crypto Trading Bot screenshots
                new ProjectScreenshot
                {
                    ProjectId = projects[0].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1642543348745-775d77351ac2",
                    Caption = "Dashboard view showing active trades",
                    DisplayOrder = 1
                },
                new ProjectScreenshot
                {
                    ProjectId = projects[0].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1642543348745-775d77351ac2",
                    Caption = "Strategy configuration screen",
                    DisplayOrder = 2
                },
                
                // Portfolio Tracker screenshots
                new ProjectScreenshot
                {
                    ProjectId = projects[1].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71",
                    Caption = "Portfolio overview dashboard",
                    DisplayOrder = 1
                },
                new ProjectScreenshot
                {
                    ProjectId = projects[1].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71",
                    Caption = "Performance analytics view",
                    DisplayOrder = 2
                },
                
                // Backtesting Framework screenshots
                new ProjectScreenshot
                {
                    ProjectId = projects[3].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1642543348745-775d77351ac2",
                    Caption = "Backtesting results dashboard",
                    DisplayOrder = 1
                },
                new ProjectScreenshot
                {
                    ProjectId = projects[3].Id,
                    ImageUrl = "https://images.unsplash.com/photo-1642543348745-775d77351ac2",
                    Caption = "Strategy optimization interface",
                    DisplayOrder = 2
                }
            };

            context.ProjectScreenshots.AddRange(screenshots);
            
            // Add sample purchases
            var purchases = new List<Purchase>
            {
                new Purchase
                {
                    ProjectId = projects[0].Id,
                    UserId = regularUser.Id,
                    Amount = projects[0].Price,
                    CryptoCurrency = projects[0].CryptoCurrency,
                    TransactionId = "tx_" + Guid.NewGuid().ToString("N"),
                    PurchaseDate = DateTime.UtcNow.AddDays(-15),
                    IsCompleted = true
                },
                new Purchase
                {
                    ProjectId = projects[1].Id,
                    UserId = regularUser.Id,
                    Amount = projects[1].Price,
                    CryptoCurrency = projects[1].CryptoCurrency,
                    TransactionId = "tx_" + Guid.NewGuid().ToString("N"),
                    PurchaseDate = DateTime.UtcNow.AddDays(-5),
                    IsCompleted = true
                }
            };

            context.Purchases.AddRange(purchases);
            context.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
