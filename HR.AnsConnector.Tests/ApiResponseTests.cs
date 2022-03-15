using HR.AnsConnector.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class ApiResponseTests
    {
        [TestMethod]
        public void GetStatusMessage_ByDefault_ReturnsExpectedString()
        {
            // Arrange
            var apiResponse = new ApiResponse();

            // Act
            var result = apiResponse.GetStatusMessage();

            // Assert
            Assert.AreEqual("HTTP  - ", result);
        }

        [TestMethod]
        public void GetStatusMessage_WithStatusCodeAndDescriptionSet_ReturnsExpectedString()
        {
            // Arrange
            var apiResponse = new ApiResponse { StatusCode = 200, StatusDescription = "OK" };

            // Act
            var result = apiResponse.GetStatusMessage();

            // Assert
            Assert.AreEqual("HTTP 200 - OK", result);
        }
    }
}
