using HR.AnsConnector.Features.Courses;
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

        [TestMethod]
        public async Task CompleteLifecycleIntegrationTest()
        {
            IApiClient apiClient = CreateApiClient();

            var course = new Course
            {
                Name = "Programmeren in .NET (C#)",
                CourseCode = "NETPROG-01",
                ExternalId = "FIT.AB.NETPROG-01",
                Year = 2022,
                SelfEnroll = true,
            };

            var apiResponse = await apiClient.CreateCourseAsync(course).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            course = apiResponse!;
            Assert.IsNotNull(course.Id);

            course.Year = 2023;
            course.SelfEnroll = false;
            apiResponse = await apiClient.UpdateCourseAsync(course).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            course = apiResponse!;
            Assert.AreNotEqual(course.CreatedAt, course.UpdatedAt);
            Assert.AreEqual(2023, course.Year);
            Assert.IsFalse(course.SelfEnroll);

            apiResponse = await apiClient.DeleteCourseAsync(course).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            course = apiResponse!;
            Assert.IsTrue(course.IsDeleted);
        }
    }
}
