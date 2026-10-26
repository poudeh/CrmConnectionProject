
using RestSharp;
using System.Net;
using WebApplication1.DTOs.WarehouseLocation;

namespace WebApplication1.Infrastructure.Apis;

public interface ICrmApiWarehouseLocation
{
    Task CreateAsync(string name, string code);
    Task UpdateAsync(UpdateWarehouseLocationDto command);
    Task DeleteByCodeAsync(string code);
    Task<string> GetAllAsync();
    Task<UpdateWarehouseLocationDto> GetWarehouseLocationByCode(string code);


}

public class CrmApiWarehouseLocation : ICrmApiWarehouseLocation
{
    private readonly RestClient _client;

    public CrmApiWarehouseLocation(IConfiguration configuration)
    {
        var options = new RestClientOptions(configuration["CrmLinks:OuterCompany"] ?? string.Empty)
        {
            Credentials = new NetworkCredential(
                "k.poudeh",
                "swamiprem",
                "SADADTSP"),
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

    public async Task UpdateAsync(UpdateWarehouseLocationDto command)
    {
        // 1. پیدا کردن WarehouseLocation با کد فعلی
        var searchRequest = new RestRequest("poudeh_warehouselocations", Method.Get);
        searchRequest.AddQueryParameter("$select", "poudeh_warehouselocationid,poudeh_code");
        searchRequest.AddQueryParameter("$filter", $"poudeh_code eq '{command.Code.Replace("'", "''")}'");

        var searchResponse = await _client.ExecuteAsync(searchRequest);

        if (!searchResponse.IsSuccessful)
            throw new Exception($"CRM Search Error: {(int)searchResponse.StatusCode} - " + $"{searchResponse.Content}");
        

        using var document =
            System.Text.Json.JsonDocument.Parse(searchResponse.Content!);

        var records = document.RootElement.GetProperty("value");

        if (records.GetArrayLength() == 0)
            throw new Exception("انبار موردنظر در CRM پیدا نشد.");

        var crmId = records[0]
            .GetProperty("poudeh_warehouselocationid")
            .GetString();

        if (string.IsNullOrWhiteSpace(crmId))
            throw new Exception("شناسه رکورد CRM معتبر نیست.");

        // 2. پیدا کردن System User بر اساس نام
        var ownerRequest = new RestRequest("systemusers", Method.Get);

        ownerRequest.AddQueryParameter("$select", "systemuserid,fullname");

        ownerRequest.AddQueryParameter("$filter", $"fullname eq '{command.Owner.Replace("'", "''")}'");

        var ownerResponse = await _client.ExecuteAsync(ownerRequest);

        if (!ownerResponse.IsSuccessful)
        {
            throw new Exception(
                $"CRM Owner Search Error: {(int)ownerResponse.StatusCode} - " +
                $"{ownerResponse.Content}");
        }

        using var ownerDocument =
            System.Text.Json.JsonDocument.Parse(ownerResponse.Content!);

        var owners = ownerDocument.RootElement.GetProperty("value");

        if (owners.GetArrayLength() == 0)
            throw new Exception($"کاربر CRM با نام '{command.Owner}' پیدا نشد.");

        if (owners.GetArrayLength() > 1)
            throw new Exception($"چند کاربر با نام '{command.Owner}' پیدا شدند.");

        var ownerId = owners[0]
            .GetProperty("systemuserid")
            .GetString();

        if (string.IsNullOrWhiteSpace(ownerId))
            throw new Exception("شناسه کاربر CRM معتبر نیست.");

        // 3. به‌روزرسانی فیلدها و Lookup مربوط به Owner
        var updateData = new Dictionary<string, object>
        {
            ["poudeh_name"] = command.Name,
            ["poudeh_code"] = command.Code,
            ["ownerid@odata.bind"] = $"/systemusers({ownerId})"
        };

        var updateRequest = new RestRequest(
            $"poudeh_warehouselocations({crmId})",
            Method.Patch);

        updateRequest.AddHeader("OData-Version", "4.0");
        updateRequest.AddHeader("OData-MaxVersion", "4.0");

        updateRequest.AddJsonBody(updateData);

        var updateResponse = await _client.ExecuteAsync(updateRequest);

        if (!updateResponse.IsSuccessful)
        {
            throw new Exception(
                $"CRM Update Error: {(int)updateResponse.StatusCode} - " +
                $"{updateResponse.ErrorMessage} - {updateResponse.Content}");
        }
    }


    public async Task DeleteByCodeAsync(string code)
    {
        // پیدا کردن رکورد بر اساس کد
        var request = new RestRequest(
            "poudeh_warehouselocations",
            Method.Get);
        //just return these 2 fields
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

    public async Task<string> GetAllAsync()
    {
        var request = new RestRequest(
            "poudeh_warehouselocations",
            Method.Get);

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new Exception(
                $"CRM GetAll Error: {(int)response.StatusCode} - " +
                $"{response.ErrorMessage} - {response.Content}");
        }
        return response.Content ?? string.Empty;
    }

    public async Task<UpdateWarehouseLocationDto> GetWarehouseLocationByCode(string code)
    {
        var request = new RestRequest(
            "poudeh_warehouselocations",
            Method.Get);

        // فقط فیلدهای موردنیاز رو دریافت می‌کنیم
        request.AddQueryParameter(
            "$select",
            "poudeh_warehouselocationid,poudeh_name,poudeh_code,_ownerid_value");

        // جستجو بر اساس کد انبار
        request.AddQueryParameter(
            "$filter",
            $"poudeh_code eq '{code.Replace("'", "''")}'");

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new Exception(
                $"CRM Search Error: {(int)response.StatusCode} - " +
                $"{response.ErrorMessage} - {response.Content}");
        }

        using var document =
            System.Text.Json.JsonDocument.Parse(response.Content!);

        var records = document.RootElement.GetProperty("value");

        if (records.GetArrayLength() == 0)
            return null;

        if (records.GetArrayLength() > 1)
            throw new Exception($"چند انبار با کد '{code}' پیدا شدند.");

        var record = records[0];

        var crmId = record
            .GetProperty("poudeh_warehouselocationid")
            .GetString();

        var name = record
            .GetProperty("poudeh_name")
            .GetString();

        var warehouseCode = record
            .GetProperty("poudeh_code")
            .GetString();

        // شناسه Owner در CRM
        var ownerId = record.TryGetProperty("_ownerid_value", out var ownerProperty)
            && ownerProperty.ValueKind != System.Text.Json.JsonValueKind.Null
                ? ownerProperty.GetString()
                : null;

        string ownerName = string.Empty;

        // دریافت نام Owner از روی شناسه
        if (!string.IsNullOrWhiteSpace(ownerId))
        {
            var ownerRequest = new RestRequest(
                $"systemusers({ownerId})",
                Method.Get);

            ownerRequest.AddQueryParameter("$select", "fullname");

            var ownerResponse = await _client.ExecuteAsync(ownerRequest);

            if (ownerResponse.IsSuccessful &&
                !string.IsNullOrWhiteSpace(ownerResponse.Content))
            {
                using var ownerDocument =
                    System.Text.Json.JsonDocument.Parse(ownerResponse.Content);

                if (ownerDocument.RootElement.TryGetProperty("fullname", out var fullname))
                    ownerName = fullname.GetString() ?? string.Empty;
            }
        }

        return new UpdateWarehouseLocationDto
        {
            // اگر Id در DTO شما int باشد، شناسه GUID مربوط به CRM
            // را نمی‌توان در آن ذخیره کرد؛ برای این کار CRM ID جداگانه لازم است.
            Owner = ownerName,
            Name = name ?? string.Empty,
            Code = warehouseCode ?? string.Empty
        };
    }
}

