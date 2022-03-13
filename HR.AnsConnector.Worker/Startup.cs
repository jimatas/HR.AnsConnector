using Developist.Core.Cqrs.DependencyInjection;

using HR.AnsConnector;
using HR.AnsConnector.Infrastructure;

using Microsoft.Extensions.Options;

using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<Worker>();

        services.AddDispatcher();
        services.AddHandlersFromAssembly(GetType().Assembly);

        services.Configure<ApiSettings>(Configuration.GetSection(nameof(ApiSettings)));
        services.Configure<JsonSerializerOptions>(jsonOptions =>
        {
            jsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            jsonOptions.AllowTrailingCommas = true;
            jsonOptions.PropertyNameCaseInsensitive = true; // Web default
            jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Web default
            jsonOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            jsonOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString; // Web default
        });

        services.AddHttpClient<IApiClient, ApiClient>((IServiceProvider serviceProvider, HttpClient httpClient) =>
        {
            var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
            httpClient.BaseAddress = new Uri($"{apiSettings.BaseUri.TrimEnd('/')}/"); // https://stackoverflow.com/a/23438417
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiSettings.BearerToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HR.AnsConnector.Infrastructure.ApiClient", "1.0"));
        });
    }
}
