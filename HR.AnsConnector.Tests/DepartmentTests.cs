﻿using HR.AnsConnector.Infrastructure;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class DepartmentTests
    {
        #region Setup
        private static ApiSettings CreateApiSettings() => new()
        {
            BaseUri = "https://stage.ans.app/api/v2/",
            BearerToken = Environment.GetEnvironmentVariable("ApiSettings:BearerToken", EnvironmentVariableTarget.User),
            TenantId = int.TryParse(Environment.GetEnvironmentVariable("ApiSettings:TenantId", EnvironmentVariableTarget.User), out var tenantId) ? tenantId : null,
        };

        private static JsonSerializerOptions CreateJsonOptions()
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

        private static HttpClient CreateHttpClient(ApiSettings apiSettings)
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
        #endregion

        [TestMethod]
        public async Task ListDepartmentsAsync_ByDefault_ListsDepartments()
        {
            // Arrange
            var apiSettings = CreateApiSettings();
            using var httpClient = CreateHttpClient(apiSettings);
            var jsonOptions = CreateJsonOptions();

            IApiClient apiClient = new ApiClient(httpClient, Options.Create(apiSettings), Options.Create(jsonOptions));

            // Act
            var apiResponse = await apiClient.ListDepartmentsAsync().WithoutCapturingContext();

            // Assert
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());
            Assert.IsNotNull(apiResponse.Data);
            Assert.IsTrue(apiResponse.Data.Any());
        }
    }
}
