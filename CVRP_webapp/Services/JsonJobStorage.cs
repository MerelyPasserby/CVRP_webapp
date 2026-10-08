using CVRP_webapp.Interfaces;
using CVRPlib.Models;
using System.Text.Json;

namespace CVRP_webapp.Services
{
    public class JsonJobStorage : IJobStorage
    {
        readonly string _directory;
        public JsonJobStorage()
        {
            _directory = Path.Combine(
            AppContext.BaseDirectory,
            "Jobs");

            Directory.CreateDirectory(_directory);
        }
        public async Task SaveAsync(CVRPJob job)
        {
            string filePath = Path.Combine(
            _directory,
            $"{job.Id}.json");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(job, options);

            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<CVRPJob?> GetAsync(Guid id)
        {
            string filePath = Path.Combine(
                _directory,
                $"{id}.json");

            if (!File.Exists(filePath))
            {
                return null;
            }

            string json = await File.ReadAllTextAsync(filePath);

            return JsonSerializer.Deserialize<CVRPJob>(json);
        }
    }
}
