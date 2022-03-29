using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Infrastructure;
using HR.Common.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Threading.Tasks;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class DepartmentTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task ListDepartmentsAsync_ByDefault_ListsDepartments()
        {
            // Arrange
            IApiClient apiClient = CreateApiClient();

            // Act
            var apiResponse = await apiClient.ListDepartmentsAsync().WithoutCapturingContext();

            // Assert
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());
            Assert.IsNotNull(apiResponse.Data);
            Assert.IsTrue(apiResponse.Data.Any());
        }

        [TestMethod]
        public async Task CompleteLifecycleIntegrationTest()
        {
            IApiClient apiClient = CreateApiClient();

            var department = new Department
            {
                Name = "FIT_Applicatiebeheer_Webcentrum",
                ExternalId = "FIT.AB.WEB"
            };

            var apiResponse = await apiClient.CreateDepartmentAsync(department).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            department = apiResponse!;
            Assert.IsNotNull(department.Id);
            Assert.IsNotNull(department.CreatedAt);
            Assert.AreEqual(department.CreatedAt, department.UpdatedAt);

            department.Name = "Faciliteiten_en_Informatietechnologie_Applicatiebeheer_Webcentrum";

            apiResponse = await apiClient.UpdateDepartmentAsync(department).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            department = apiResponse!;
            Assert.AreEqual("Faciliteiten_en_Informatietechnologie_Applicatiebeheer_Webcentrum", department.Name);
            Assert.IsNotNull(department.UpdatedAt);
            Assert.AreNotEqual(department.UpdatedAt, department.CreatedAt);

            apiResponse = await apiClient.DeleteDepartmentAsync(department).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());
        }
    }
}
