using HR.AnsConnector.Features.Users;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Text.Json;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class UserSearchCriteriaTests
    {
        [TestMethod]
        public void ToQueryString_WithNoPropertiesSet_ReturnsEmptyString()
        {
            // Arrange
            var searchCriteria = new UserSearchCriteria();

            // Act
            var result = searchCriteria.ToQueryString();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void ToQueryString_WithEmailPropertySetAndPropertyNamingPolicyNotProvided_ReturnsExpectedPropertyName()
        {
            // Arrange
            var searchCriteria = new UserSearchCriteria
            {
                Email = "testgebruiker@hr.nl"
            };

            // Act
            var result = searchCriteria.ToQueryString();

            // Assert
            Assert.IsTrue(result.StartsWith("Email:"));
        }

        [TestMethod]
        public void ToQueryString_WithEmailPropertySetAndPropertyNamingPolicyProvided_ReturnsExpectedPropertyName()
        {
            // Arrange
            var searchCriteria = new UserSearchCriteria
            {
                Email = "testgebruiker@hr.nl"
            };

            // Act
            var result = searchCriteria.ToQueryString(JsonNamingPolicy.CamelCase);

            // Assert
            Assert.IsTrue(result.StartsWith("email:"));
        }

        [TestMethod]
        public void ToQueryString_WithExternalIdPropertySetAndPropertyNamingPolicyNotProvided_ReturnsExpectedPropertyName()
        {
            // Arrange
            var searchCriteria = new UserSearchCriteria
            {
                ExternalId = "testgebruiker"
            };

            // Act
            var result = searchCriteria.ToQueryString();

            // Assert
            Assert.AreEqual("external_id:testgebruiker", result);
        }

        [TestMethod]
        public void ToQueryString_WithEmailPropertySet_EscapesAtSign()
        {
            // Arrange
            var searchCriteria = new UserSearchCriteria
            {
                Email = "testgebruiker@hr.nl"
            };

            // Act
            var result = searchCriteria.ToQueryString();

            // Assert
            Assert.AreEqual("Email:testgebruiker%40hr.nl", result);
        }
    }
}
