using HR.AnsConnector.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class ApiResponseTests
    {
        [TestMethod]
        public void GetStatusMessage_ByDefault_ReturnsEmptyString()
        {
            // Arrange
            var apiResponse = new ApiResponse();

            // Act
            var result = apiResponse.GetStatusMessage();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void GetStatusMessage_WithOnlyStatusCodeSet_ReturnsExpectedString()
        {
            // Arrange
            var apiResponse = new ApiResponse { StatusCode = 200 };

            // Act
            var result = apiResponse.GetStatusMessage();

            // Assert
            Assert.AreEqual("HTTP 200", result);
        }

        [TestMethod]
        public void GetStatusMessage_WithOnlyStatusDescriptionSet_ReturnsExpectedString()
        {
            // Arrange
            var apiResponse = new ApiResponse { StatusDescription = "OK" };

            // Act
            var result = apiResponse.GetStatusMessage();

            // Assert
            Assert.AreEqual("OK", result);
        }

        [TestMethod]
        public void GetStatusMessage_WithBothStatusCodeAndDescriptionSet_ReturnsExpectedString()
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
