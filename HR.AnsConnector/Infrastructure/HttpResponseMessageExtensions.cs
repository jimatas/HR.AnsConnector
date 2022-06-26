using HR.Common.Utilities;

using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;

namespace HR.AnsConnector.Infrastructure
{
    internal static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Parses the server response encapsulated in the <see cref="HttpResponseMessage"/> object and returns it as an <see cref="ApiResponse{T}"/> object. 
        /// </summary>
        /// <typeparam name="T">The type of the JSON payload.</typeparam>
        /// <param name="httpResponse"></param>
        /// <param name="jsonOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<ApiResponse<T>> ToApiResponseAsync<T>(this HttpResponseMessage httpResponse, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default)
        {
            var apiResponse = new ApiResponse<T>
            {
                StatusCode = (int)httpResponse.StatusCode,
                StatusDescription = httpResponse.ReasonPhrase,
            };

            if (httpResponse.Content.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    apiResponse.Data = await httpResponse.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken).WithoutCapturingContext();
                }
                else if (httpResponse.StatusCode == HttpStatusCode.UnprocessableEntity && httpResponse.RequestMessage?.Method != HttpMethod.Get)
                {
                    var validationErrors = await httpResponse.Content.ReadFromJsonAsync<IDictionary<string, IEnumerable<object>>>(jsonOptions, cancellationToken).WithoutCapturingContext();
                    if (validationErrors is not null && validationErrors.Any())
                    {
                        apiResponse.ValidationErrors = validationErrors;
                    }
                }
                else
                {
                    var validationErrors = await httpResponse.Content.ReadFromJsonAsync<IDictionary<string, string>>(jsonOptions, cancellationToken).WithoutCapturingContext();
                    if (validationErrors is not null && validationErrors.Count == 1)
                    {
                        apiResponse.ValidationErrors = new Dictionary<string, IEnumerable<object>> { { validationErrors.Single().Key, new[] { validationErrors.Single().Value } } };
                    }
                }
            }

            return apiResponse;
        }
    }
}
