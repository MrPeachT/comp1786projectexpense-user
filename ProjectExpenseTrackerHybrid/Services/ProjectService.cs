using System;
using System.Collections.Generic;
using System.Text;

using ProjectExpenseTrackerHybrid.Config;
using ProjectExpenseTrackerHybrid.Models;
using System.Text.Json;

using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectExpenseTrackerHybrid.Services
{
    public class ProjectService
    {
        private readonly HttpClient _httpClient;

        public ProjectService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<ProjectItem>> GetProjectsAsync()
        {
            var projects = new List<ProjectItem>();

            string url =
                $"https://firestore.googleapis.com/v1/projects/{AppConfig.FirebaseProjectId}/databases/(default)/documents/projects?key={AppConfig.FirebaseApiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to load projects from Firestore. {errorText}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using JsonDocument document = JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty("documents", out JsonElement documentsElement))
            {
                return projects;
            }

            foreach (var doc in documentsElement.EnumerateArray())
            {
                if (!doc.TryGetProperty("fields", out JsonElement fields))
                {
                    continue;
                }

                var project = new ProjectItem
                {
                    ProjectCode = GetString(fields, "projectCode"),
                    ProjectName = GetString(fields, "projectName"),
                    ProjectDescription = GetString(fields, "projectDescription"),
                    StartDate = GetString(fields, "startDate"),
                    EndDate = GetString(fields, "endDate"),
                    ProjectManager = GetString(fields, "projectManager"),
                    ProjectStatus = GetString(fields, "projectStatus"),
                    ProjectBudget = GetDouble(fields, "projectBudget"),
                    SpecialRequirements = GetString(fields, "specialRequirements"),
                    ClientInfo = GetString(fields, "clientInfo"),
                    ExactLocation = GetString(fields, "exactLocation"),
                    ImagePath = GetString(fields, "imagePath"),
                    LastModified = GetLong(fields, "lastModified"),
                    Synced = (int)GetLong(fields, "synced")
                };

                projects.Add(project);
            }

            return projects;
        }

        private static string GetString(JsonElement fields, string propertyName)
        {
            if (!fields.TryGetProperty(propertyName, out JsonElement property))
            {
                return string.Empty;
            }

            if (property.TryGetProperty("stringValue", out JsonElement stringValue))
            {
                return stringValue.GetString() ?? string.Empty;
            }

            return string.Empty;
        }

        private static double GetDouble(JsonElement fields, string propertyName)
        {
            if (!fields.TryGetProperty(propertyName, out JsonElement property))
            {
                return 0;
            }

            if (property.TryGetProperty("doubleValue", out JsonElement doubleValue))
            {
                return doubleValue.GetDouble();
            }

            if (property.TryGetProperty("integerValue", out JsonElement integerValue))
            {
                var raw = integerValue.GetString();
                return double.TryParse(raw, out double result) ? result : 0;
            }

            return 0;
        }

        private static long GetLong(JsonElement fields, string propertyName)
        {
            if (!fields.TryGetProperty(propertyName, out JsonElement property))
            {
                return 0;
            }

            if (property.TryGetProperty("integerValue", out JsonElement integerValue))
            {
                var raw = integerValue.GetString();
                return long.TryParse(raw, out long result) ? result : 0;
            }

            return 0;
        }
    }
}
