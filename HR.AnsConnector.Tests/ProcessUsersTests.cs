using HR.AnsConnector.Features.Users;
using HR.AnsConnector.Features.Users.Commands;
using HR.AnsConnector.Features.Users.Events;
using HR.AnsConnector.Infrastructure;
using HR.AnsConnector.Infrastructure.Persistence;
using HR.AnsConnector.Tests.Fixture;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Cqrs.Infrastructure;
using HR.Common.Utilities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class ProcessUsersTests : IDisposable
    {
        #region Setup & cleanup
        private IServiceProvider? serviceProvider;

        protected IServiceProvider CreateServiceProvider(IDatabase database, IApiClient apiClient)
        {
            var services = new ServiceCollection();
            services.AddSingleton(_ => database);
            services.AddSingleton(_ => apiClient);
            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddConsole();
            });
            services.AddDispatcher(ServiceLifetime.Singleton);
            services.AddHandlersFromAssembly(typeof(Worker).Assembly, ServiceLifetime.Singleton);
            services.AddHandlersFromAssembly(GetType().Assembly, ServiceLifetime.Singleton);
            return serviceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            (serviceProvider as IDisposable)?.Dispose();
            serviceProvider = null;
            GC.SuppressFinalize(this);
        }
        #endregion

        [TestMethod]
        public async Task GivenUserToCreate_CreatesUser()
        {
            // Arrange
            var user = new UserRecord
            {
                Action = "c",
                EventId = 1001,
                FirstName = "Jim",
                LastName = "Atas",
                Email = "atask@hr.nl",
                Role = UserRole.Staff,
                UniqueId = "atask@hro.nl",
                ExternalId = "atask"
            };

            var database = new FakeDatabase();
            database.Users.Enqueue(user);

            var apiResponse = new ApiResponse<User>
            {
                StatusCode = 201,
                StatusDescription = "Created",
                Data = new User
                {
                    Id = 12345,
                    FirstName = "Jim",
                    LastName = "Atas",
                    Email = "atask@hr.nl",
                    Role = UserRole.Staff,
                    UniqueId = "atask@hro.nl",
                    ExternalId = "atask",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                }
            };

            var apiClient = new FakeApiClient();
            apiClient.ExpectedApiResponses.Enqueue(apiResponse);

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<UserCreated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers()).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsUserCreatedCalled);
            Assert.AreEqual(201, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Users.Count);
        }

        [TestMethod]
        public async Task GivenUserToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
            var user = new UserRecord
            {
                Action = "c",
                EventId = 1001,
                FirstName = "Jim",
                LastName = "Atas",
                Email = "atask@hr.nl",
                Role = UserRole.Staff,
                UniqueId = "atask@hro.nl",
                ExternalId = "atask"
            };

            var database = new FakeDatabase();
            database.Users.Enqueue(user);

            var apiClient = new FakeApiClient();

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<UserCreated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(isDeleteContext: true)).WithoutCapturingContext();

            // Assert
            Assert.IsFalse(eventSpy.IsUserCreatedCalled);
            Assert.AreEqual(1, database.Users.Count);
        }

        [TestMethod]
        public async Task GivenUserToDelete_DeletesUser()
        {
            // Arrange
            var user = new UserRecord
            {
                Action = "d",
                EventId = 1001,
                Id = 12345,
                FirstName = "Jim",
                LastName = "Atas",
                Email = "atask@hr.nl",
                Role = UserRole.Staff,
                UniqueId = "atask@hro.nl",
                ExternalId = "atask"
            };

            var database = new FakeDatabase();
            database.Users.Enqueue(user);

            var apiResponse = new ApiResponse<User>
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Data = new User
                {
                    Id = 12345,
                    FirstName = "Jim",
                    LastName = "Atas",
                    Email = "atask@hr.nl",
                    Role = UserRole.Staff,
                    UniqueId = "atask@hro.nl",
                    ExternalId = "atask",
                    CreatedAt = DateTime.Now.AddMinutes(-10),
                    UpdatedAt = DateTime.Now.AddMinutes(-10),
                    IsDeleted = true,
                    DeletedAt = DateTime.Now,
                }
            };

            var apiClient = new FakeApiClient();
            apiClient.ExpectedApiResponses.Enqueue(apiResponse);

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<UserDeleted>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(isDeleteContext: true)).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsUserDeletedCalled);
            Assert.AreEqual(200, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Users.Count);
        }
    }
}
