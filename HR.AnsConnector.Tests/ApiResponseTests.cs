using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

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

        [TestMethod]
        public void GetValidationErrorsAsSingleMessage_ByDefault_ReturnsEmptyString()
        {
            // Arrange
            var apiResponse = new ApiResponse<User>();

            // Act
            var result = apiResponse.GetValidationErrorsAsSingleMessage();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void GetValidationErrorsAsSingleMessage_GivenSingleError_ReturnsExpectedString()
        {
            // Arrange
            var apiResponse = new ApiResponse<User>
            {
                ValidationErrors = new Dictionary<string, IEnumerable<string>>
                {
                    { "email", new[] { "is required" } }
                }
            };

            // Act
            var result = apiResponse.GetValidationErrorsAsSingleMessage();

            // Assert
            Assert.AreEqual("email: is required", result);
        }

        [TestMethod]
        public void GetValidationErrorsAsSingleMessage_GivenMultipleErrorsForSameField_ReturnsExpectedString()
        {
            // Arrange
            var apiResponse = new ApiResponse<User>
            {
                ValidationErrors = new Dictionary<string, IEnumerable<string>>
                {
                    { "email", new[] { "is required", "Length must be 7 characters" } }
                }
            };

            // Act
            var result = apiResponse.GetValidationErrorsAsSingleMessage();

            // Assert
            Assert.AreEqual("email: is required\r\nemail: Length must be 7 characters", result);
        }

        [TestMethod]
        public void GetValidationErrorsAsSingleMessage_GivenSingleErrorForDifferentFields_ReturnsExpectedString()
        {
            // Arrange
            var apiResponse = new ApiResponse<User>
            {
                ValidationErrors = new Dictionary<string, IEnumerable<string>>
                {
                    { "email", new[] { "is required" } },
                    { "student_number", new[] { "is required" } }
                }
            };

            // Act
            var result = apiResponse.GetValidationErrorsAsSingleMessage();

            // Assert
            Assert.AreEqual("email: is required\r\nstudent_number: is required", result);
        }
    }
}
