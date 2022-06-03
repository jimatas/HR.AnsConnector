using HR.AnsConnector.Features.Courses;
using HR.AnsConnector.Features.Courses.Commands;
using HR.AnsConnector.Features.Courses.Events;
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
    public class ProcessCoursesTests : IDisposable
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
        public async Task GivenCourseToCreate_CreatesCourse()
        {
            // Arrange
            var course = new CourseRecord
            {
                Action = "c",
                EventId = 1004,
                Name = "Programmeren in .NET (C#)",
                CourseCode = "NETPROG-101",
                ExternalId = "FIT.AB.NETPROG-01",
                StudyIds = new[] { 78901 }
            };

            var database = new FakeDatabase();
            database.Courses.Enqueue(course);

            var apiResponse = new ApiResponse<Course>
            {
                StatusCode = 201,
                StatusDescription = "Created",
                Data = new Course
                {
                    Id = 54321,
                    Name = "Programmeren in .NET (C#)",
                    CourseCode = "NETPROG-101",
                    ExternalId = "FIT.AB.NETPROG-01",
                    StudyIds = new[] { 78901 },
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            var apiClient = new FakeApiClient();
            apiClient.ExpectedApiResponses.Enqueue(apiResponse);

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<CourseCreated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourses(batchSize: 1)).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsCourseCreatedCalled);
            Assert.AreEqual(201, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Courses.Count);
        }

        [TestMethod]
        public async Task GivenCourseToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
            var course = new CourseRecord
            {
                Action = "c",
                EventId = 1004,
                Name = "Programmeren in .NET (C#)",
                CourseCode = "NETPROG-101",
                ExternalId = "FIT.AB.NETPROG-01",
                StudyIds = new[] { 78901 }
            };

            var database = new FakeDatabase();
            database.Courses.Enqueue(course);

            var apiClient = new FakeApiClient();

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<CourseCreated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourses(batchSize: 1, isDeleteContext: true)).WithoutCapturingContext();

            // Assert
            Assert.IsFalse(eventSpy.IsCourseCreatedCalled);
            Assert.AreEqual(1, database.Courses.Count);
        }

        [TestMethod]
        public async Task GivenCourseToDelete_DeletesCourse()
        {
            // Arrange
            var course = new CourseRecord
            {
                Action = "d",
                EventId = 1004,
                Id = 54321,
                Name = "Programmeren in .NET (C#)",
                CourseCode = "NETPROG-101",
                ExternalId = "FIT.AB.NETPROG-01",
                StudyIds = new[] { 78901 }
            };

            var database = new FakeDatabase();
            database.Courses.Enqueue(course);

            var apiResponse = new ApiResponse<Course>
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Data = new Course
                {
                    Id = 54321,
                    Name = "Programmeren in .NET (C#)",
                    CourseCode = "NETPROG-101",
                    ExternalId = "FIT.AB.NETPROG-01",
                    StudyIds = new[] { 78901 },
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
            var eventSpy = serviceProvider.GetServices<IEventHandler<CourseDeleted>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourses(batchSize: 1, isDeleteContext: true)).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsCourseDeletedCalled);
            Assert.AreEqual(200, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Courses.Count);
        }

        [TestMethod]
        public async Task GivenCourseToDeleteInNonDeleteContext_DoesNothing()
        {
            // Arrange
            var course = new CourseRecord
            {
                Action = "d",
                EventId = 1004,
                Id = 54321,
                Name = "Programmeren in .NET (C#)",
                CourseCode = "NETPROG-101",
                ExternalId = "FIT.AB.NETPROG-01",
                StudyIds = new[] { 78901 }
            };

            var database = new FakeDatabase();
            database.Courses.Enqueue(course);

            var apiClient = new FakeApiClient();

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<CourseDeleted>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourses(batchSize: 1, isDeleteContext: false)).WithoutCapturingContext();

            // Assert
            Assert.IsFalse(eventSpy.IsCourseDeletedCalled);
            Assert.AreEqual(1, database.Courses.Count);
        }

        [TestMethod]
        public async Task GivenCourseToUpdate_UpdatesCourse()
        {
            // Arrange
            var course = new CourseRecord
            {
                Action = "u",
                EventId = 1004,
                Id = 54321,
                Name = "Programmeren in .NET (C#)",
                CourseCode = "NETPROG-101",
                ExternalId = "FIT.AB.NETPROG-01",
                StudyIds = new[] { 78901 }
            };

            var database = new FakeDatabase();
            database.Courses.Enqueue(course);

            var apiResponse = new ApiResponse<Course>
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Data = new Course
                {
                    Id = 54321,
                    Name = "Programmeren in .NET (C#)",
                    CourseCode = "NETPROG-101",
                    ExternalId = "FIT.AB.NETPROG-01",
                    StudyIds = new[] { 78901 },
                    CreatedAt = DateTime.Now.AddMinutes(-10),
                    UpdatedAt = DateTime.Now
                }
            };

            var apiClient = new FakeApiClient();
            apiClient.ExpectedApiResponses.Enqueue(apiResponse);

            var serviceProvider = CreateServiceProvider(database, apiClient);

            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            var eventSpy = serviceProvider.GetServices<IEventHandler<CourseUpdated>>().OfType<EventHandlerSpy>().Single();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourses(batchSize: 1)).WithoutCapturingContext();

            // Assert
            Assert.IsTrue(eventSpy.IsCourseUpdatedCalled);
            Assert.AreEqual(200, apiClient.LastApiResponse?.StatusCode);
            Assert.AreEqual(0, database.Courses.Count);
        }
    }
}
