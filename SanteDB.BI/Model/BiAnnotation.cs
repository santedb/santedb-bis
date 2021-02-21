/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2021-2-9
 */
using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents an annotation for the object
    /// </summary>
    [XmlType(nameof(BiAnnotation), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiAnnotation 
    {
        /// <summary>
        /// Represents the body of the annotation
        /// </summary>
        [XmlElement("div", Namespace = BiConstants.HtmlNamespace), JsonIgnore]
        public XElement Body { get; set; }

        /// <summary>
        /// Gets the body in JSON format
        /// </summary>
        [XmlIgnore, JsonProperty("doc")]
        public string JsonBody
        {
            get => this.Body?.ToString();
            set {
                if (value != null)
                {
                    using (var sr = new StringReader(value))
                        this.Body = XElement.Load(sr);
                }
                else
                    this.Body = null;
            }
        }

        /// <summary>
        /// Gets or sets the language
        /// </summary>
        [XmlAttribute("lang"), JsonProperty("lang")]
        public string Language { get; set; }
    }
}