using crudApp.Persistence.Contexts;
using HtmlAgilityPack;
using crudApp.Services.AutomationService.DTOs;

namespace crudApp.Services.AutomationService
{
    public class AutomationService : IAutomationService
    {
        private readonly ApplicationDbContext _context;
        public AutomationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> RunAutomation()
        {
            int playersGathered = 0;

            HttpClient httpClient = new();
            
            // Add User-Agent header 
            httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept", 
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

            string url = "https://www.pgatour.com/stats/detail/101";
            string html = await httpClient.GetStringAsync(url);
            
            // 2 second delay to not raise any suspision
            await Task.Delay(2000);
            
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);

            // Look for player names and distances in the raw HTML content
            var playerPattern = @"(Aldrich Potgieter|Rory McIlroy|Jesper Svensson|Niklas Norgaard|Michael Thorbjornsen|Nicolai Højgaard|Kurt Kitayama|Chris Gotterup|Rasmus Højgaard|Will Gordon|Trevor Cone| Keith Mitchell| Alejandro Tosti| Min Woo Lee| Wyndham Clark| Xander Schauffele| J.J Spaun| Sahith Theegala).*?(\d{3}\.\d)";
            var matches = System.Text.RegularExpressions.Regex.Matches(html, playerPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            if (matches.Count == 0)
            {
                // Try a broader pattern to find any name followed by a distance
                var broadPattern = @"([A-Z][a-z]+ [A-Z][a-z]+).*?(\d{3}\.\d)";
                matches = System.Text.RegularExpressions.Regex.Matches(html, broadPattern);
                
                // Filter to only include realistic golf names and distances
                var filteredMatches = matches.Cast<System.Text.RegularExpressions.Match>()
                    .Where(m => {
                        var distance = double.Parse(m.Groups[2].Value);
                        return distance >= 250 && distance <= 400; // Realistic driving distances
                    })
                    .Take(10); 
                
                Console.WriteLine($"Found {filteredMatches.Count()} player-distance pairs");
                
                foreach (var match in filteredMatches)
                {
                    string playerName = match.Groups[1].Value.Trim();
                    string distance = match.Groups[2].Value;
                    Console.WriteLine($"{playerName} | {distance} yards");
                    playersGathered++;
                }
            }
            else
            {
                Console.WriteLine($"Found {matches.Count} known players");
                
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    string playerName = match.Groups[1].Value.Trim();
                    string distance = match.Groups[2].Value;
                    Console.WriteLine($"{playerName} | {distance} yards");
                    playersGathered++;
                }
            }

            return playersGathered;
        }

        public async Task<List<PlayerStatsDTO>> GetPlayerStats()
        {
            var playerStatsList = new List<PlayerStatsDTO>();

            HttpClient httpClient = new();
            
            // Add User-Agent header 
            httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept", 
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

            string url = "https://www.pgatour.com/stats/detail/101";
            string html = await httpClient.GetStringAsync(url);
            
            // 2 second delay to not raise any suspision
            await Task.Delay(2000);
            
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);

            // Look for player names and distances in the raw HTML content
            var playerPattern = @"(Aldrich Potgieter|Rory McIlroy|Jesper Svensson|Niklas Norgaard|Michael Thorbjornsen|Nicolai Højgaard|Kurt Kitayama|Chris Gotterup|Rasmus Højgaard|Will Gordon).*?(\d{3}\.\d)";
            var matches = System.Text.RegularExpressions.Regex.Matches(html, playerPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            if (matches.Count == 0)
            {
                // Try a broader pattern to find any name followed by a distance
                var broadPattern = @"([A-Z][a-z]+ [A-Z][a-z]+).*?(\d{3}\.\d)";
                matches = System.Text.RegularExpressions.Regex.Matches(html, broadPattern);
                
                // Filter to only include realistic golf names and distances
                var filteredMatches = matches.Cast<System.Text.RegularExpressions.Match>()
                    .Where(m => {
                        var distance = double.Parse(m.Groups[2].Value);
                        return distance >= 250 && distance <= 400; 
                    })
                    .Take(50); 
                
                foreach (var match in filteredMatches)
                {
                    string playerName = match.Groups[1].Value.Trim();
                    string distance = match.Groups[2].Value;
                    
                    playerStatsList.Add(new PlayerStatsDTO 
                    { 
                        PlayerName = playerName,
                        AverageDrivingDistance = distance 
                    });
                }
            }
            else
            {
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    string playerName = match.Groups[1].Value.Trim();
                    string distance = match.Groups[2].Value;
                    
                    playerStatsList.Add(new PlayerStatsDTO 
                    { 
                        PlayerName = playerName,
                        AverageDrivingDistance = distance 
                    });
                }
            }

            return playerStatsList;
        }
    }
}
