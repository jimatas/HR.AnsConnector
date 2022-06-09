using HR.AnsConnector.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace HR.AnsConnector.Tests
{
    [TestClass]
    public class RecoverySettingsTests
    {
        [TestMethod]
        public void NewInstance_HasExpectedDefaultValues()
        {
            RecoverySettings recovery = new();

            Assert.AreEqual(4, recovery.RetryAttempts);
            Assert.AreEqual(new TimeSpan(hours: 0, minutes: 0, seconds: 15), recovery.RetryDelay);
            Assert.AreEqual(1.0, recovery.BackOffFactor);
        }

        [TestMethod]
        public void CalculateRetryDelay_GivenAttemptZero_ReturnsDelayOfZero()
        {
            var recovery = new RecoverySettings
            {
                RetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(5),
                BackOffFactor = 1.2
            };

            var delay0 = recovery.CalculateRetryDelay(0);

            Assert.AreEqual(TimeSpan.Zero, delay0);
        }

        [TestMethod]
        public void CalculateRetryDelay_GivenAttemptGreaterThanMaxAttempt_ReturnsMaxDelay()
        {
            var recovery = new RecoverySettings
            {
                RetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(5),
                BackOffFactor = 1.2
            };

            var delay4 = recovery.CalculateRetryDelay(4);
            var delay5 = recovery.CalculateRetryDelay(5); // Max attempt.
            var delay6 = recovery.CalculateRetryDelay(6);

            Assert.IsTrue(delay4 < delay5);
            Assert.AreEqual(delay5, delay6);
        }

        [TestMethod]
        public void CalculateRetryDelay_WithBackOffFactoryOfOne_ReturnsSameDelay()
        {
            var recovery = new RecoverySettings
            {
                RetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(5),
                BackOffFactor = 1.0
            };

            var delay1 = recovery.CalculateRetryDelay(1);
            var delay2 = recovery.CalculateRetryDelay(2);
            var delay3 = recovery.CalculateRetryDelay(3);
            var delay4 = recovery.CalculateRetryDelay(4);
            var delay5 = recovery.CalculateRetryDelay(5);

            Assert.AreEqual(delay1, delay2);
            Assert.AreEqual(delay2, delay3);
            Assert.AreEqual(delay3, delay4);
            Assert.AreEqual(delay4, delay5);
        }

        [TestMethod]
        public void CalculateRetryDelay_WithBackOffFactoryOfGreaterThanOne_ReturnsGraduallyLongerDelays()
        {
            var recovery = new RecoverySettings
            {
                RetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(5),
                BackOffFactor = 1.2
            };

            var delay1 = recovery.CalculateRetryDelay(1);
            var delay2 = recovery.CalculateRetryDelay(2);
            var delay3 = recovery.CalculateRetryDelay(3);
            var delay4 = recovery.CalculateRetryDelay(4);
            var delay5 = recovery.CalculateRetryDelay(5);

            Assert.IsTrue(delay1 < delay2);
            Assert.IsTrue(delay2 < delay3);
            Assert.IsTrue(delay3 < delay4);
            Assert.IsTrue(delay4 < delay5);
        }
    }
}
