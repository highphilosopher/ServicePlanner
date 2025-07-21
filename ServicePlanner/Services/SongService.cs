using Microsoft.EntityFrameworkCore;
using ServicePlanner.Data;
using ServicePlanner.Models;

namespace ServicePlanner.Services
{
    public class SongService
    {
        private readonly ServicePlannerContext _context;

        public SongService(ServicePlannerContext context)
        {
            _context = context;
        }

        public async Task<List<Song>> GetAllSongsAsync()
        {
            return await _context.Songs
                .Where(s => !s.Disabled)
                .OrderBy(s => s.SongName)
                .ToListAsync();
        }

        public async Task<Song?> GetSongByIdAsync(int id)
        {
            return await _context.Songs.FindAsync(id);
        }

        public async Task<List<Song>> SearchSongsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllSongsAsync();
            }

            return await _context.Songs
                .Where(s => !s.Disabled && (
                    s.SongName.Contains(searchTerm) ||
                    (s.Artist != null && s.Artist.Contains(searchTerm)) ||
                    (s.Category != null && s.Category.Contains(searchTerm)) ||
                    (s.Publisher != null && s.Publisher.Contains(searchTerm))
                ))
                .OrderBy(s => s.SongName)
                .ToListAsync();
        }

        public async Task<List<Song>> GetSongsByCategoryAsync(string category)
        {
            return await _context.Songs
                .Where(s => !s.Disabled && s.Category != null && s.Category.Equals(category))
                .OrderBy(s => s.SongName)
                .ToListAsync();
        }

        public async Task<Song> CreateSongAsync(Song song)
        {
            song.CreatedDate = DateTime.Now;
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
            return song;
        }

        public async Task<Song> UpdateSongAsync(Song song)
        {
            _context.Songs.Update(song);
            await _context.SaveChangesAsync();
            return song;
        }

        public async Task<bool> DeleteSongAsync(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _context.Songs
                .Where(s => !s.Disabled && !string.IsNullOrEmpty(s.Category))
                .Select(s => s.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<List<Song>> GetSeasonalSongsAsync()
        {
            return await _context.Songs
                .Where(s => !s.Disabled && s.Seasonal)
                .OrderBy(s => s.SongName)
                .ToListAsync();
        }

        public async Task<DateTime?> GetLastPlayedDateAsync(string songName)
        {
            var lastService = await _context.ServiceEventInstances
                .Where(sei => sei.SongTitle == songName)
                .Join(_context.Services, sei => sei.ServiceId, s => s.Id, (sei, s) => s)
                .OrderByDescending(s => s.ServiceDate)
                .FirstOrDefaultAsync();

            return lastService?.ServiceDate;
        }

        public async Task<Dictionary<string, DateTime?>> GetLastPlayedDatesAsync()
        {
            var songPlayDates = await _context.ServiceEventInstances
                .Where(sei => !string.IsNullOrEmpty(sei.SongTitle))
                .Join(_context.Services, sei => sei.ServiceId, s => s.Id, (sei, s) => new { sei.SongTitle, s.ServiceDate })
                .GroupBy(x => x.SongTitle)
                .Select(g => new { SongTitle = g.Key, LastPlayed = g.Max(x => x.ServiceDate) })
                .ToDictionaryAsync(x => x.SongTitle!, x => (DateTime?)x.LastPlayed);

            return songPlayDates;
        }

        public async Task<List<Song>> GetAllSongsWithLastPlayedAsync()
        {
            var songs = await GetAllSongsAsync();
            var lastPlayedDates = await GetLastPlayedDatesAsync();

            // Order songs by last played date (oldest first), with never-played songs at the top
            var orderedSongs = songs
                .OrderBy(song => 
                    lastPlayedDates.ContainsKey(song.SongName) 
                        ? lastPlayedDates[song.SongName] 
                        : DateTime.MinValue)
                .ThenBy(song => song.SongName)
                .ToList();

            return orderedSongs;
        }

        public async Task<List<Song>> ImportSongsFromCsvAsync(Stream csvStream)
        {
            var importedSongs = new List<Song>();
            
            // Read the entire stream content asynchronously first
            using var reader = new StreamReader(csvStream);
            var content = await reader.ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(content))
                return importedSongs;

            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (lines.Length == 0)
                return importedSongs;

            var headers = lines[0].Split(',').Select(h => h.Trim().Trim('"')).ToArray();
            
            for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = ParseCsvLine(line);
                if (values.Length != headers.Length)
                    continue;

                var song = new Song
                {
                    CreatedDate = DateTime.Now,
                    // Set default values for required fields
                    SongName = string.Empty,
                    Name = string.Empty,
                    Speed = "Medium",
                    Publisher = "Unknown",
                    Artist = "Unknown"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    var header = headers[i].ToLowerInvariant();
                    var value = values[i].Trim().Trim('"');

                    switch (header)
                    {
                        case "songname":
                        case "song name":
                        case "name":
                            song.SongName = value;
                            song.Name = value; // Set both properties
                            break;
                        case "key":
                            song.Key = value;
                            break;
                        case "songselectid":
                        case "songselect id":
                        case "ccli":
                            if (int.TryParse(value, out var songSelectId))
                            {
                                song.SongSelectId = songSelectId;
                            }
                            break;
                        case "seasonal":
                            song.Seasonal = bool.TryParse(value, out var seasonal) && seasonal;
                            break;
                        case "speed":
                        case "tempo":
                            if (!string.IsNullOrWhiteSpace(value))
                                song.Speed = value;
                            break;
                        case "publisher":
                            if (!string.IsNullOrWhiteSpace(value))
                                song.Publisher = value;
                            break;
                        case "artist":
                        case "author":
                            if (!string.IsNullOrWhiteSpace(value))
                                song.Artist = value;
                            break;
                        case "category":
                        case "genre":
                            song.Category = value;
                            break;
                        case "notes":
                            song.Notes = value;
                            break;
                        case "disabled":
                            song.Disabled = bool.TryParse(value, out var disabled) && disabled;
                            break;
                    }
                }

                // Ensure we have at least a song name
                if (!string.IsNullOrWhiteSpace(song.SongName))
                {
                    _context.Songs.Add(song);
                    importedSongs.Add(song);
                }
            }

            if (importedSongs.Any())
            {
                await _context.SaveChangesAsync();
            }

            return importedSongs;
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }
    }
}
