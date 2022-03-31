using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Infrastructure;
using HR.Common.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class UserTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task CompleteLifecycleIntegrationTest()
        {
            IApiClient apiClient = CreateApiClient();

            var user = new User
            {
                Email = "atask+test1@hr.nl",
                FirstName = "Jim",
                LastName = "Atas",
                UniqueId = "atask",
                ExternalId = "atask@hro.nl",
                Role = UserRole.Staff,
            };

            var apiResponse = await apiClient.CreateUserAsync(user).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            user = apiResponse!;
            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.CreatedAt);
            // The following assertion might or might not hold true, as users are only soft-deleted.
            // Assert.AreEqual(user.CreatedAt, user.UpdatedAt);

            user.FirstName = "Jimbo";

            apiResponse = await apiClient.UpdateUserAsync(user).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            user = apiResponse!;
            Assert.AreEqual("Jimbo", user.FirstName);
            Assert.IsNotNull(user.UpdatedAt);
            Assert.AreNotEqual(user.UpdatedAt, user.CreatedAt);

            apiResponse = await apiClient.DeleteUserAsync(user).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());
        }

        [TestMethod]
        public async Task CreateUserAsync_GivenExistingUser_UpdatesEmptyPropertiesOnly()
        {
            IApiClient apiClient = CreateApiClient();

            var user = new User
            {
                Email = "atask+test2@hr.nl",
                FirstName = "Jim",
                LastName = "Atas",
                UniqueId = "atask@hro.nl",
                ExternalId = "atask",
                Role = UserRole.Staff,
            };

            var apiResponse = await apiClient.CreateUserAsync(user).WithoutCapturingContext();
            Assert.IsTrue(apiResponse.IsSuccessStatusCode());

            user.FirstName = "Jimbo"; // FirstName should not change.
            user.MiddleName = "van der"; // MiddleName should be updated.
            apiResponse = await apiClient.CreateUserAsync(user).WithoutCapturingContext();

            Assert.AreEqual(200, apiResponse.StatusCode);
            Assert.AreEqual("Jim", apiResponse.Data!.FirstName);
            Assert.AreEqual("van der", apiResponse.Data!.MiddleName);

            await apiClient.DeleteUserAsync(user).WithoutCapturingContext();
        }
    }
}
