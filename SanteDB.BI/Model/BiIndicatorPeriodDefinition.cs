using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SanteDB.Core.Model.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// A single period as defined by a <see cref="BiIndicatorPeriodDefinition"/>
    /// </summary>
    public struct BiIndicatorPeriod
    {

        /// <summary>
        /// Empty period
        /// </summary>
        public static readonly BiIndicatorPeriod Empty = new BiIndicatorPeriod(default(DateTime), default(DateTime), default(long));

        /// <summary>
        /// Create new period 
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        public BiIndicatorPeriod(DateTime start, DateTime end) : this(start, end, default(long))
        {
        }

        /// <summary>
        /// Indicator period constructor
        /// </summary>
        internal BiIndicatorPeriod(DateTime start, DateTime end, long index)
        {
            this.Start = start;
            this.End = end;
            this.Index = index;
        }

        /// <summary>
        /// Gets the start of the period
        /// </summary>
        public DateTime Start { get; }

        /// <summary>
        /// Gets the end of the period
        /// </summary>
        public DateTime End { get; }

        /// <summary>
        /// Gets the period index
        /// </summary>
        public long Index { get; }

        /// <summary>
        /// Convert to string
        /// </summary>
        public override string ToString() => $"{this.Index} - {this.Start:o} to {this.End:o}";
    }

    /// <summary>
    /// Represents a period definition
    /// </summary>
    [XmlType(nameof(BiIndicatorPeriodDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiIndicatorPeriodDefinition), Namespace = BiConstants.XmlNamespace)]
    public class BiIndicatorPeriodDefinition : BiDefinition
    {

        /// <summary>
        /// The time where this period is active
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public DateTime? NotBefore { get; set; }

        /// <summary>
        /// XML serialization for <see cref="NotBefore"/>
        /// </summary>
        [XmlElement("notBefore"), JsonProperty("notBefore")]
        public String NotBeforeXml
        {
            get => this.NotBefore?.ToString("o");
            set
            {
                if(!String.IsNullOrEmpty(value))
                {
                    // Try to parse ISO date
                    if (DateTime.TryParseExact(value, new String[] { "o", "yyyy-MM-dd", "yyyy-MM", "yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime dt))
                    {
                        this.NotBefore = dt;
                    }
                    else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                    {
                        this.NotBefore = dt.Date;
                    }
                    else
                    {
                        throw new FormatException($"Cannot parse {value} as a date");
                    }
                }
            }
        }

        /// <summary>
        /// Time that this period is no longer active
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public DateTime? NotAfter { get; set; }

        /// <summary>
        /// XML serialization helper for <see cref="NotAfter"/>
        /// </summary>
        [XmlElement("notAfter"), JsonProperty("notAfter")]
        public String NotAfterXml
        {
            get => this.NotAfter?.ToString("o");
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    // Try to parse ISO date
                    if (DateTime.TryParseExact(value, new String[] { "o", "yyyy-MM-dd", "yyyy-MM", "yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime dt))
                    {
                        this.NotAfter = dt;
                    }
                    else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                    {
                        this.NotAfter = dt.Date;
                    }
                    else
                    {
                        throw new FormatException($"Cannot parse {value} as a date");
                    }
                }
            }
        }

        /// <summary>
        /// Represents a reference date which is the anchor point for the <see cref="Period"/>. 
        /// </summary>
        /// <remarks>
        /// For example: Every first day of the month at midnight would be Period=P1M and ReferenceDate=2020-01-01T00:00:00
        /// For example: Every other Friday at 23:59:59 would be Period=P2W and ReferenceDate=2025-01-10T23:59:59
        /// </remarks>
        [XmlElement("spec"), JsonProperty("spec")]
        public DateTime DateTimeSpec { get; set; }

        /// <summary>
        /// XML serialization helper
        /// </summary>
        [XmlElement("period"), JsonProperty("period")]
        public string PeriodXml {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter which this period will pass to a query as the starting period
        /// </summary>
        [XmlElement("startParameter"), JsonProperty("startParameter")]
        public string PeriodStartParameter { get; set; }

        /// <summary>
        /// Gets or sets the ending parameter this period will pass to a query
        /// </summary>
        [XmlElement("endParameter"), JsonProperty("endParameter")]
        public string PeriodEndParameter { get; set; }

        /// <summary>
        /// Get a period from this BI indicator period definition that is during the specified time
        /// </summary>
        /// <param name="atTime">The time point to get the period</param>
        /// <param name="period">The resulting period</param>
        /// <returns>True if the period <paramref name="atTime"/> is valid</returns>
        public bool TryGetPeriod(DateTime atTime, out BiIndicatorPeriod period)
        {

            if(this.NotAfter.HasValue && atTime > this.NotAfter || 
                this.NotBefore.HasValue && atTime < this.NotBefore)
            {
                period = BiIndicatorPeriod.Empty;
                return false;
            }
            else
            {
                var periodTs = XmlConvert.ToTimeSpan(this.PeriodXml);
                // HACK: Months presents a interesting issue since we want P1M to be every month we need to use add months
                if (periodTs.TotalDays % 30 == 0 && this.PeriodXml.Contains("M")) // Timespan uses 30D as P1M
                {
                    var periodMonth = (int)(periodTs.TotalDays / 30);
                    var atMonth = atTime.Year * 12 + (atTime.Month - 1);
                    var specMonth = this.DateTimeSpec.Year * 12 + (this.DateTimeSpec.Month - 1);
                    var periodIndex = (int)((atMonth - specMonth) / periodMonth);
                    var periodStart = this.DateTimeSpec.AddMonths(periodIndex);
                    var periodEnd = this.DateTimeSpec.AddMonths(periodIndex + 1).AddSeconds(-1);
                    period = new BiIndicatorPeriod(periodStart, periodEnd, periodIndex);
                }
                else if(periodTs.TotalDays % 365 == 0 && this.PeriodXml.Contains("Y"))
                {
                    var periodYear = (int)(periodTs.TotalDays / 365);
                    var atYear = atTime.Year;
                    var specYear = this.DateTimeSpec.Year;
                    var periodIndex = (int)((atYear - specYear) / periodYear);
                    var periodStart = this.DateTimeSpec.AddYears(periodIndex);
                    var periodEnd = this.DateTimeSpec.AddYears(periodIndex + 1).AddSeconds(-1);
                    period = new BiIndicatorPeriod(periodStart, periodEnd, periodIndex);
                }
                else
                {
                    var dateDiff = atTime.Subtract(this.DateTimeSpec);
                    var periodIndex = (int)(dateDiff.TotalSeconds / periodTs.TotalSeconds);
                    var periodStart = this.DateTimeSpec.AddSeconds(periodIndex * periodTs.TotalSeconds);
                    var periodEnd = periodStart.Add(periodTs).AddSeconds(-1);
                    period = new BiIndicatorPeriod(periodStart, periodEnd, periodIndex);
                }
                return true;
            }
        }

        /// <summary>
        /// Get all periods since the specified period
        /// </summary>
        public IEnumerable<BiIndicatorPeriod> GetPeriods(DateTime start, DateTime end)
        {
            while(start < end && this.TryGetPeriod(start, out var period))
            {
                yield return period;
                start = period.End.AddSeconds(1);
            }
        }

        /// <summary>
        /// Get all periods for which this data is valid
        /// </summary>
        /// <remarks>This will return all periods - it is highly recommended the caller use <see cref="Enumerable.Take{TSource}(IEnumerable{TSource}, int)"/> to restrict the calls</remarks>
        public IEnumerable<BiIndicatorPeriod> GetPeriods() => this.GetPeriods(this.NotBefore ?? this.DateTimeSpec, short.MaxValue);

        /// <summary>
        /// Get number of periods
        /// </summary>
        public IEnumerable<BiIndicatorPeriod> GetPeriods(DateTime start, int count)
        {
            while(count != 0 && this.TryGetPeriod(start, out var period))
            {
                yield return period;
                if(count > 0) {  // forwards
                    start = period.End.AddSeconds(1);
                    count--;
                }
                else // backwards
                {
                    start = period.Start.AddSeconds(-1);
                    count++;
                }
            }
        }

    }
}