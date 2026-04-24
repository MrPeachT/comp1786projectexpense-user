using ProjectExpenseTrackerHybrid.Config;
using ProjectExpenseTrackerHybrid.Models;
using System.Text.Json;

namespace ProjectExpenseTrackerHybrid.Services
{
    public class ExpenseService
    {
        private readonly HttpClient _httpClient;

        public ExpenseService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<ExpenseItem>> GetExpensesByProjectCodeAsync(string projectCode)
        {
            var expenses = new List<ExpenseItem>();

            string url =
                $"https://firestore.googleapis.com/v1/projects/{AppConfig.FirebaseProjectId}/databases/(default)/documents/projects/{projectCode}/expenses?key={AppConfig.FirebaseApiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to load expenses from Firestore. {errorText}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using JsonDocument document = JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty("documents", out JsonElement documentsElement))
            {
                return expenses;
            }

            foreach (var doc in documentsElement.EnumerateArray())
            {
                if (!doc.TryGetProperty("fields", out JsonElement fields))
                {
                    continue;
                }

                var expense = new ExpenseItem
                {
                    ExpenseId = GetString(fields, "expenseId"),
                    ProjectCode = projectCode,
                    DateOfExpense = GetString(fields, "dateOfExpense"),
                    Amount = GetDouble(fields, "amount"),
                    Currency = GetString(fields, "currency"),
                    ExpenseType = GetString(fields, "expenseType"),
                    PaymentMethod = GetString(fields, "paymentMethod"),
                    Claimant = GetString(fields, "claimant"),
                    PaymentStatus = GetString(fields, "paymentStatus"),
                    Description = GetString(fields, "description"),
                    Location = GetString(fields, "location"),
                    ImagePath = GetString(fields, "imagePath"),
                    LastModified = GetLong(fields, "lastModified"),
                    Synced = (int)GetLong(fields, "synced")
                };

                expenses.Add(expense);
            }

            return expenses;
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

        public async Task AddExpenseAsync(string projectCode, ExpenseItem expense)
        {
            string url =
                $"https://firestore.googleapis.com/v1/projects/{AppConfig.FirebaseProjectId}/databases/(default)/documents/projects/{projectCode}/expenses/{expense.ExpenseId}?key={AppConfig.FirebaseApiKey}";

            var payload = new
            {
                fields = new
                {
                    expenseId = new { stringValue = expense.ExpenseId },
                    dateOfExpense = new { stringValue = expense.DateOfExpense },
                    amount = new { doubleValue = expense.Amount },
                    currency = new { stringValue = expense.Currency },
                    expenseType = new { stringValue = expense.ExpenseType },
                    paymentMethod = new { stringValue = expense.PaymentMethod },
                    claimant = new { stringValue = expense.Claimant },
                    paymentStatus = new { stringValue = expense.PaymentStatus },
                    description = new { stringValue = expense.Description ?? string.Empty },
                    location = new { stringValue = expense.Location ?? string.Empty },
                    imagePath = new { stringValue = expense.ImagePath ?? string.Empty },
                    lastModified = new { integerValue = expense.LastModified.ToString() },
                    synced = new { integerValue = expense.Synced.ToString() }
                }
            };

            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await PatchAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to add expense to Firestore. {errorText}");
            }
        }

        private async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            };

            return await _httpClient.SendAsync(request);
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