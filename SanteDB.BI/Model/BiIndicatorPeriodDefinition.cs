using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
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
        /// Gets or sets the timespan interval 
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public TimeSpan Period { get; set; }

        /// <summary>
        /// XML serialization field helper for <see cref="Period"/>
        /// </summary>
        [XmlElement("period"), JsonProperty("period")]
        public string PeriodXml {
            get => XmlConvert.ToString(this.Period);
            set {
                this.Period = XmlConvert.ToTimeSpan(value);
            }
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

    }
}