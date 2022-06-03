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
                EventId = 1002,
                Name = "Applicatieontwikkeling",
                ExternalId = "AOD",
                DepartmentId = 2345
            };

            var database = new FakeDatabase();
            database.Studies.Enqueue(study);

            var apiResponse = new ApiResponse<Study>
            {
                StatusCode = 201,
                StatusDescription = "Created",
                Data = new Study
                {
                    Id = 56789,
                    Name = "Applicatieontwikkeling",
                    ExternalId = "AOD",
                    DepartmentId = 2345,
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
    }
}
