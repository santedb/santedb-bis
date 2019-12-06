using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// BI Render service
    /// </summary>
    public interface IBiRenderService : IServiceImplementation
    {

        /// <summary>
        /// Render the specified report
        /// </summary>
        Stream Render(String reportId, String viewName, String formatName, IDictionary<String, object> parameters, out string mimeType);

    }
}
