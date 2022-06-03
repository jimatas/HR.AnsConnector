using HR.AnsConnector.Infrastructure;
using HR.Common.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Threading.Tasks;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class CoursesTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task ListCoursesAsync_ByDefault_ListsCourses()
        {
            // Arrange
            var apiClient = CreateApiClient();

            // Act
            var apiResponse = await apiClient.ListCoursesAsync().WithoutCapturingContext();

            // Assert
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());
            Assert.IsNotNull(apiResponse.Data);
            Assert.IsTrue(apiResponse.Data.Any());
        }
    }
}
