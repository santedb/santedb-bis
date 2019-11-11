using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents an enumerator for a BiPackage
    /// </summary>
    public class BiPackageEnumerator : IEnumerable<BiDefinition>
    {
        // The package
        private BiPackage m_package;

        /// <summary>
        /// Creates a new BI Package
        /// </summary>
        public BiPackageEnumerator(BiPackage package)
        {
            this.m_package = package;
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        public IEnumerator<BiDefinition> GetEnumerator()
        {
            return this.m_package.DataSources.OfType<BiDefinition>()
                .Union(this.m_package.Formats.OfType<BiDefinition>())
                .Union(this.m_package.Parameters.OfType<BiDefinition>())
                .Union(this.m_package.Queries.OfType<BiDefinition>())
                .Union(this.m_package.Reports.OfType<BiDefinition>())
                .Union(this.m_package.Views.OfType<BiDefinition>()).GetEnumerator();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
