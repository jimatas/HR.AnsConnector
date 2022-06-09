using HR.AnsConnector;
using HR.AnsConnector.Infrastructure;
using HR.AnsConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Polly;
using Polly.Extensions.Http;

using System.ComponentModel.DataAnnotations;
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
        services.AddScoped<IDatabase, Database>();
        services.AddDbContext<AnsDbContext>(dbContext => dbContext.UseSqlServer(
            connectionString: Configuration.GetConnectionString(nameof(AnsDbContext)),
            sqlOptions => sqlOptions.CommandTimeout(60)));

        services.AddDispatcher().AddHandlersFromAssembly(GetType().Assembly);

        services.Configure<RecoverySettings>(RecoverySettings.Names.CommandTimeoutExpired, Configuration.GetSection($"{nameof(RecoverySettings)}:{RecoverySettings.Names.CommandTimeoutExpired}"))
            .PostConfigure<RecoverySettings>(RecoverySettings.Names.CommandTimeoutExpired, recoverySettings =>
        {
            Validator.ValidateObject(recoverySettings, new ValidationContext(recoverySettings), validateAllProperties: true);
        });
        services.Configure<RecoverySettings>(RecoverySettings.Names.TransientHttpFault, Configuration.GetSection($"{nameof(RecoverySettings)}:{RecoverySettings.Names.TransientHttpFault}"))
            .PostConfigure<RecoverySettings>(RecoverySettings.Names.TransientHttpFault, recoverySettings =>
        {
            Validator.ValidateObject(recoverySettings, new ValidationContext(recoverySettings), validateAllProperties: true);
        });
        services.Configure<ApiSettings>(Configuration.GetSection(nameof(ApiSettings))).PostConfigure<ApiSettings>(apiSettings =>
        {
            Validator.ValidateObject(apiSettings, new ValidationContext(apiSettings), validateAllProperties: true);
        });
        services.Configure<BatchSettings>(Configuration.GetSection(nameof(BatchSettings))).PostConfigure<BatchSettings>(batchSettings =>
        {
            Validator.ValidateObject(batchSettings, new ValidationContext(batchSettings), validateAllProperties: true);
        });

        services.Configure<JsonSerializerOptions>(jsonOptions =>
        {
            jsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            jsonOptions.AllowTrailingCommas = true;
            jsonOptions.PropertyNameCaseInsensitive = true; // Web default
            jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Web default
            jsonOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            jsonOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString; // Web default
            jsonOptions.Converters.Add(new ObjectJsonConverter());
        });

        services.AddHttpClient<IApiClient, ApiClient>((IServiceProvider serviceProvider, HttpClient httpClient) =>
        {
            var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
            httpClient.BaseAddress = new Uri($"{apiSettings.BaseUri.TrimEnd('/')}/"); // https://stackoverflow.com/a/23438417
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiSettings.BearerToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HR.AnsConnector.Infrastructure.ApiClient", "1.0"));
        }).AddPolicyHandler((serviceProvider, httpRequest) =>
        {
            var recoverySettings = serviceProvider.GetRequiredService<IOptionsSnapshot<RecoverySettings>>().Get(RecoverySettings.Names.TransientHttpFault);
            return HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(recoverySettings.RetryAttempts, attempt => recoverySettings.CalculateRetryDelay(attempt));
        });

        services.AddLogging(logging => logging.AddFile(Configuration.GetSection("Serilog:FileLogging")));
    }
}
