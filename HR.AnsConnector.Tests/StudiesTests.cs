using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Infrastructure;
using HR.Common.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class StudiesTests : IntegrationTestsBase
    {
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

            //var departments = await apiClient.ListDepartmentsAsync().WithoutCapturingContext();

            await apiClient.ListStudiesAsync((int)department.Id!).WithoutCapturingContext();

            await apiClient.DeleteDepartmentAsync(department).WithoutCapturingContext();
        }
    }
}
