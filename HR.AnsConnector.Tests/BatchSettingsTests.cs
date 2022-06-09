using HR.AnsConnector.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class BatchSettingsTests
    {
        [TestMethod]
        public void NewInstance_HasDefaultValues()
        {
            BatchSettings batchSettings = new();

            Assert.AreEqual(10, batchSettings.BatchSize);
            Assert.AreEqual(new TimeSpan(hours: 0, minutes: 0, seconds: 1), batchSettings.TimeDelayBetweenRuns);
        }

        [TestMethod]
        public void TimeDelayBetweenRuns_GivenInvalidValue_ThrowsException()
        {
            BatchSettings batchSettings = new();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => batchSettings.TimeDelayBetweenRuns = TimeSpan.FromMilliseconds(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => batchSettings.TimeDelayBetweenRuns = TimeSpan.Zero.Subtract(new TimeSpan(ticks: 1)));
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void BatchSize_GivenInvalidValue_ThrowsException(int batchSize)
        {
            BatchSettings batchSettings = new();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => batchSettings.BatchSize = batchSize);
        }
    }
}
