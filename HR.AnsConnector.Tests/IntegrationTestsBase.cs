using HR.AnsConnector.Infrastructure;

using Microsoft.Extensions.Options;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HR.AnsConnector.Tests
{
    public abstract class IntegrationTestsBase : IDisposable
    {
        private ApiSettings? apiSettings;
        private HttpClient? httpClient;
        private JsonSerializerOptions? jsonOptions;

        protected IApiClient CreateApiClient(string baseUri = "https://stage.ans.app/api/v2/")
        {
            apiSettings ??= CreateApiSettings();
            httpClient ??= CreateHttpClient(apiSettings);
            jsonOptions ??= CreateJsonOptions();

            IApiClient apiClient = new ApiClient(httpClient, Options.Create(apiSettings), Options.Create(jsonOptions));
            return apiClient;

            ApiSettings CreateApiSettings() => new()
            {
                BaseUri = baseUri,
                BearerToken = Environment.GetEnvironmentVariable("ApiSettings:BearerToken", EnvironmentVariableTarget.User),
                TenantId = int.TryParse(Environment.GetEnvironmentVariable("ApiSettings:TenantId", EnvironmentVariableTarget.User), out var tenantId) ? tenantId : null,
            };

            static JsonSerializerOptions CreateJsonOptions()
            {
                JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web)
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    AllowTrailingCommas = true,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                };
                jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                return jsonOptions;
            }

            static HttpClient CreateHttpClient(ApiSettings apiSettings)
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri($"{apiSettings.BaseUri.TrimEnd('/')}/") // https://stackoverflow.com/a/23438417
                };
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiSettings.BearerToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HR.AnsConnector.Infrastructure.ApiClient", "1.0"));

                return httpClient;
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
            httpClient = null;
            GC.SuppressFinalize(this);
        }
    }
}
