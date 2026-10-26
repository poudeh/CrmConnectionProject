using System.Text.Json.Serialization;

namespace WebApplication1.DTOs.BaseDto;

public class CrmResponse<T>
{
    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = new();
}