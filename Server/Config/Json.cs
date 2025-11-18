using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;

namespace backend.Config;

public static class Json
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };
}

