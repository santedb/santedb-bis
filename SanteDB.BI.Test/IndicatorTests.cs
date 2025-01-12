using NUnit.Framework;
using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Test
{
    [TestFixture]
    public class IndicatorTests
    {

        [Test]
        public void TestCreateWeeklyPeriod()
        {

            var periodDefinition = new BiIndicatorPeriodDefinition()
            {
                DateTimeSpec = DateTime.Parse("2015-01-05T00:00:00"),
                PeriodXml = "P7D"
            };

            Assert.IsTrue(periodDefinition.TryGetPeriod(DateTime.Now, out var period));
            Assert.AreEqual(DayOfWeek.Monday, period.Start.DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, period.End.DayOfWeek);
            // Try period before spec
            Assert.IsTrue(periodDefinition.TryGetPeriod(DateTime.Parse("2014-01-01"), out period));
            Assert.AreEqual(DayOfWeek.Monday, period.Start.DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, period.End.DayOfWeek);
            Assert.AreEqual(-52, period.Index);

            var periods = periodDefinition.GetPeriods(DateTime.Parse("2025-01-01"), DateTime.Parse("2025-12-31"));
            Assert.AreEqual(53, periods.Count());
        }

        [Test]
        public void TestCreateMonthlyPeriod()
        {

            var periodDefinition = new BiIndicatorPeriodDefinition()
            {
                DateTimeSpec = DateTime.Parse("2015-01-01T00:00:00"),
                PeriodXml = "P1M"
            };

            var lastDayOfMonth = DateTime.Now.AddDays(-DateTime.Now.Day + 1).Date.AddMonths(1).AddSeconds(-1);
            Assert.IsTrue(periodDefinition.TryGetPeriod(DateTime.Now, out var period));
            Assert.AreEqual(1, period.Start.Day);
            Assert.AreEqual(lastDayOfMonth.Day, period.End.Day);

            Assert.IsTrue(periodDefinition.TryGetPeriod(DateTime.Parse("2025-02-04"), out period));
            Assert.AreEqual(1, period.Start.Day);
            Assert.AreEqual(28, period.End.Day);

            var periods = periodDefinition.GetPeriods(DateTime.Parse("2025-01-01"), DateTime.Parse("2025-12-31"));
            Assert.AreEqual(12, periods.Count());
        }

        [Test]
        public void TestCreateYearlyPeriod()
        {

            var periodDefinition = new BiIndicatorPeriodDefinition()
            {
                DateTimeSpec = DateTime.Parse("2015-01-01T00:00:00"),
                PeriodXml = "P1Y"
            };

            Assert.IsTrue(periodDefinition.TryGetPeriod(DateTime.Now, out var period));
            Assert.AreEqual(1, period.Start.Day);
            Assert.AreEqual(1, period.Start.Month);
            Assert.AreEqual(DateTime.Now.Year, period.Start.Year);
            Assert.AreEqual(31, period.End.Day);
            Assert.AreEqual(12, period.End.Month);
            Assert.AreEqual(DateTime.Now.Year, period.End.Year);

            var periods = periodDefinition.GetPeriods(DateTime.Parse("2025-01-01"), DateTime.Parse("2030-12-31"));
            Assert.AreEqual(6, periods.Count());
        }
    }
}
