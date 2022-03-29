namespace HR.AnsConnector.Infrastructure
{
    public class ApiResponse<T> : ApiResponse
    {
        /// <summary>
        /// Either the deserialized JSON payload that was returned by the server, or <c>null</c> if an error occurred.
        /// </summary>
        public T? Data { get; internal set; }

        /// <summary>
        /// A possibly empty collection of field level validation errors, keyed by field name.
        /// </summary>
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> ValidationErrors { get; internal set; } = new Dictionary<string, IEnumerable<string>>();

        /// <summary>
        /// Supports the implicit conversion from <see cref="ApiResponse{T}"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="apiResponse"></param>
        public static implicit operator T?(ApiResponse<T> apiResponse) => apiResponse.Data;
    }
}
