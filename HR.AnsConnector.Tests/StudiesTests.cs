using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Studies;
using HR.AnsConnector.Infrastructure;
using HR.Common.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
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

            ApiResponse apiResponse = await apiClient.CreateDepartmentAsync(department).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());
            department = ((ApiResponse<Department>)apiResponse)!;

            var study = new Study
            {
                Name = "Applicatieontwikkeling",
                ExternalId = "AOD",
                DepartmentId = department.Id,
            };
            apiResponse = await apiClient.CreateStudyAsync(study).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            apiResponse = await apiClient.ListStudiesAsync((int)department.Id!).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            var studies = ((ApiResponse<IEnumerable<Study>>)apiResponse).Data!;
            Assert.AreEqual(1, studies.Count());
            study = studies.Single();
            Assert.AreEqual("Applicatieontwikkeling", study.Name);

            apiResponse = await apiClient.DeleteStudyAsync(study).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            await apiClient.DeleteDepartmentAsync(department).WithoutCapturingContext();
        }
    }
}
