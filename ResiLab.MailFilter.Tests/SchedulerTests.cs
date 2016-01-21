using System;
using System.Threading;
using NUnit.Framework;
using ResiLab.MailFilter.Infrastructure;

namespace ResiLab.MailFilter.Tests {
    [TestFixture]
    public class SchedulerTests {
        [Test]
        public void Should_Run_Every_Second() {
            var i = 0;

            var cancel = Scheduler.Interval(TimeSpan.FromSeconds(1), () => {
                i++;
            });

            Thread.Sleep(5000);

            cancel.Cancel();
            Assert.AreEqual(5, i);
        }
    }
}