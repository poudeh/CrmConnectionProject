
using RestSharp;
using System.Net;

namespace WebApplication1.Services
{
    public interface ICrmApiService
    {
        Task CreateAsync(string name, string code);
        Task DeleteByCodeAsync(string code);

    }

    public class CrmApiService : ICrmApiService
    {
        private readonly RestClient _client;

        public CrmApiService()
        {
            var options = new RestClientOptions("https://crm-dynamics/CRM/api/data/v9.1/")
            {
                Credentials = CredentialCache.DefaultCredentials,
                RemoteCertificateValidationCallback =
                    (_, _, _, _) => true
            };
            _client = new RestClient(options);
        }

        public async Task CreateAsync(string name, string code)
        {
            var data = new Dictionary<string, object>
            {
                ["poudeh_name"] = name,
               
                ["poudeh_code"] = code
            };

            // همان Entity که در کد کنسولی موفق بود
            var request = new RestRequest(
                "poudeh_warehouselocations",
                Method.Post);

            request.AddHeader("OData-Version", "4.0");
            request.AddHeader("OData-MaxVersion", "4.0");
            request.AddHeader("Prefer", "return=representation");


            request.AddJsonBody(data);

            var response = await _client.ExecuteAsync(request);
            Console.WriteLine(response);

            if (!response.IsSuccessful)
            {
                throw new Exception(
                    $"CRM Error: {(int)response.StatusCode} - " +
                    $"{response.ErrorMessage} - {response.Content}");
            }
        }

        public async Task DeleteByCodeAsync(string code)
        {
            // پیدا کردن رکورد بر اساس کد
            var request = new RestRequest(
                "poudeh_warehouselocations",
                Method.Get);

            request.AddQueryParameter(
                "$select",
                "poudeh_warehouselocationid,poudeh_code");

            request.AddQueryParameter(
                "$filter",
                $"poudeh_code eq '{code.Replace("'", "''")}'");

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception(
                    $"CRM Search Error: {(int)response.StatusCode} - {response.Content}");
            }

            using var document =
                System.Text.Json.JsonDocument.Parse(response.Content!);

            var records = document.RootElement.GetProperty("value");

            if (records.GetArrayLength() == 0)
                return; // رکوردی با این کد پیدا نشد

            foreach (var record in records.EnumerateArray())
            {
                var crmId = record
                    .GetProperty("poudeh_warehouselocationid")
                    .GetString();

                if (string.IsNullOrWhiteSpace(crmId))
                    continue;

                var deleteRequest = new RestRequest(
                    $"poudeh_warehouselocations({crmId})",
                    Method.Delete);

                var deleteResponse =
                    await _client.ExecuteAsync(deleteRequest);

                if (!deleteResponse.IsSuccessful)
                {
                    throw new Exception(
                        $"CRM Delete Error: {(int)deleteResponse.StatusCode} - " +
                        $"{deleteResponse.Content}");
                }
            }
        }
    }
}
