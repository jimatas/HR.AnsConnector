using HR.AnsConnector.Features.Studies;
using HR.AnsConnector.Features.Studies.Commands;
using HR.AnsConnector.Features.Studies.Events;
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
    public class ProcessStudiesTests : IDisposable
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
        public async Task GivenStudyToCreate_CreatesStudy()
        {
            // Arrange
            var study = new StudyRecord
            {
                Action = "c",
                EventId = 1003,
                Name = "Applicatieontwikkeling",
                ExternalId = "AOD",
                DepartmentId = 56789
            };

            var database = new FakeDatabase();
            database.Studies.Enqueue(study);

            var apiResponse = new ApiResponse<Study>
            {
                StatusCode = 201,
                StatusDescription = "Created",
                Data = new Study
                {
                    Id = 78901,
                    Name = "Applicatieontwikkeling",
                    ExternalId = "AOD",
                    DepartmentId = 56789,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            var apiClient = new FakeApiClient();
            apiClient.ExpectedApiResponses.Enqueue(apiResponse);

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<StudyCreated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessStudies(batchSize: 1)).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsStudyCreatedCalled);
            Assert.AreEqual(201, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Studies.Count);
        }

        [TestMethod]
        public async Task GivenStudyToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
            var study = new StudyRecord
            {
                Action = "c",
                EventId = 1003,
                Name = "Applicatieontwikkeling",
                ExternalId = "AOD",
                DepartmentId = 56789
            };

            var database = new FakeDatabase();
            database.Studies.Enqueue(study);

            var apiClient = new FakeApiClient();

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<StudyCreated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessStudies(batchSize: 1, isDeleteContext: true)).WithoutCapturingContext();

            // Assert
            Assert.IsFalse(eventSpy.IsStudyCreatedCalled);
            Assert.AreEqual(1, database.Studies.Count);
        }

        [TestMethod]
        public async Task GivenStudyToDelete_DeletesStudy()
        {
            // Arrange
            var study = new StudyRecord
            {
                Action = "d",
                EventId = 1003,
                Id = 78901,
                Name = "Applicatieontwikkeling",
                ExternalId = "AOD",
                DepartmentId = 56789
            };

            var database = new FakeDatabase();
            database.Studies.Enqueue(study);

            var apiResponse = new ApiResponse<Study>
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Data = new Study
                {
                    Id = 78901,
                    Name = "Applicatieontwikkeling",
                    ExternalId = "AOD",
                    DepartmentId = 56789,
                    CreatedAt = DateTime.Now.AddMinutes(-10),
                    UpdatedAt = DateTime.Now.AddMinutes(-10)
                }
            };

            var apiClient = new FakeApiClient();
            apiClient.ExpectedApiResponses.Enqueue(apiResponse);

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<StudyDeleted>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessStudies(batchSize: 1, isDeleteContext: true)).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsStudyDeletedCalled);
            Assert.AreEqual(200, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Studies.Count);
        }

        [TestMethod]
        public async Task GivenStudyToDeleteInNonDeleteContext_DoesNothing()
        {
            // Arrange
            var study = new StudyRecord
            {
                Action = "d",
                EventId = 1003,
                Id = 78901,
                Name = "Applicatieontwikkeling",
                ExternalId = "AOD",
                DepartmentId = 56789
            };

            var database = new FakeDatabase();
            database.Studies.Enqueue(study);

            var apiClient = new FakeApiClient();

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<StudyDeleted>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessStudies(batchSize: 1, isDeleteContext: false)).WithoutCapturingContext();

            // Assert
            Assert.IsFalse(eventSpy.IsStudyDeletedCalled);
            Assert.AreEqual(1, database.Studies.Count);
        }

        [TestMethod]
        public async Task GivenStudyToUpdate_UpdatesStudy()
        {
            // Arrange
            var study = new StudyRecord
            {
                Action = "u",
                EventId = 1003,
                Id = 78901,
                Name = "Applicatieontwikkeling",
                ExternalId = "AOD",
                DepartmentId = 56789
            };

            var database = new FakeDatabase();
            database.Studies.Enqueue(study);

            var apiResponse = new ApiResponse<Study>
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Data = new Study
                {
                    Id = 78901,
                    Name = "Applicatieontwikkeling",
                    ExternalId = "AOD",
                    CreatedAt = DateTime.Now.AddMinutes(-10),
                    UpdatedAt = DateTime.Now
                }
            };

            var apiClient = new FakeApiClient();
            apiClient.ExpectedApiResponses.Enqueue(apiResponse);

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<StudyUpdated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessStudies(batchSize: 1)).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsStudyUpdatedCalled);
            Assert.AreEqual(200, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Studies.Count);
        }
    }
}
