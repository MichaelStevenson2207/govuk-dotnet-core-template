using Newtonsoft.Json;

namespace govuk_dotnet_core_template.Models;

public sealed class GovUkPay
{
    [JsonProperty("amount")]
    public int Amount { get; set; }

    [JsonProperty("reference")]
    public string? Reference { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("return_url")]
    public string? ReturnUrl { get; set; }
}