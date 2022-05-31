using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Departments.Commands;
using HR.AnsConnector.Features.Departments.Events;
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
    public class ProcessDepartmentsTests : IDisposable
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
            services.AddDispatcher(ServiceLifetime.Singleton)
                .AddHandlersFromAssembly(typeof(Worker).Assembly, ServiceLifetime.Singleton)
                .AddHandlersFromAssembly(GetType().Assembly, ServiceLifetime.Singleton);

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
        public async Task GivenDepartmentToCreate_CreatesDepartment()
        {
            // Arrange
            var department = new DepartmentRecord
            {
                Action = "c",
                EventId = 1002,
                Name = "Faciliteiten & Informatietechnologie",
                ExternalId = "FIT"
            };

            var database = new FakeDatabase();
            database.Departments.Enqueue(department);

            var apiResponse = new ApiResponse<Department>
            {
                StatusCode = 201,
                StatusDescription = "Created",
                Data = new Department
                {
                    Id = 56789,
                    Name = "Faciliteiten & Informatietechnologie",
                    ExternalId = "FIT",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            var apiClient = new FakeApiClient();
            apiClient.ExpectedApiResponses.Enqueue(apiResponse);

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<DepartmentCreated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessDepartments(batchSize: 1)).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsDepartmentCreatedCalled);
            Assert.AreEqual(201, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Users.Count);
        }

        [TestMethod]
        public async Task GivenDepartmentToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
            var department = new DepartmentRecord
            {
                Action = "c",
                EventId = 1002,
                Name = "Faciliteiten & Informatietechnologie",
                ExternalId = "FIT"
            };

            var database = new FakeDatabase();
            database.Departments.Enqueue(department);

            var apiClient = new FakeApiClient();

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<DepartmentCreated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessDepartments(batchSize: 1, isDeleteContext: true)).WithoutCapturingContext();

            // Assert
            Assert.IsFalse(eventSpy.IsDepartmentCreatedCalled);
            Assert.AreEqual(1, database.Departments.Count);
        }

        [TestMethod]
        public async Task GivenDepartmentToDelete_DeletesDepartment()
        {
            // Arrange
            var department = new DepartmentRecord
            {
                Action = "d",
                EventId = 1002,
                Id = 56789,
                Name = "Faciliteiten & Informatietechnologie",
                ExternalId = "FIT"
            };

            var database = new FakeDatabase();
            database.Departments.Enqueue(department);

            var apiResponse = new ApiResponse<Department>
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Data = new Department
                {
                    Id = 56789,
                    Name = "Faciliteiten & Informatietechnologie",
                    ExternalId = "FIT",
                    CreatedAt = DateTime.Now.AddMinutes(-10),
                    UpdatedAt = DateTime.Now.AddMinutes(-10)
                }
            };

            var apiClient = new FakeApiClient();
            apiClient.ExpectedApiResponses.Enqueue(apiResponse);

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<DepartmentDeleted>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessDepartments(batchSize: 1, isDeleteContext: true)).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsDepartmentDeletedCalled);
            Assert.AreEqual(200, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Departments.Count);
        }

        [TestMethod]
        public async Task GivenDepartmentToDeleteInNonDeleteContext_DoesNothing()
        {
            // Arrange
            var department = new DepartmentRecord
            {
                Action = "d",
                EventId = 1002,
                Id = 56789,
                Name = "Faciliteiten & Informatietechnologie",
                ExternalId = "FIT"
            };

            var database = new FakeDatabase();
            database.Departments.Enqueue(department);

            var apiClient = new FakeApiClient();

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<DepartmentDeleted>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessDepartments(batchSize: 1, isDeleteContext: false)).WithoutCapturingContext();

            // Assert
            Assert.IsFalse(eventSpy.IsDepartmentDeletedCalled);
            Assert.AreEqual(1, database.Departments.Count);
        }
    }
}
