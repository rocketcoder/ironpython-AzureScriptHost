using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureScriptHost
{
    
    /// <summary>
    /// This class works with the CloudPlatformAdaptationLayer and enabled the IronPython engine to load files from Azure as if the files were stored on the local file system
    /// </summary>
    public class AzureCloudScriptHost : ScriptHost
    {
        #region Instance Variables
        private PlatformAdaptationLayer azurePlatformAdaptationLayer;
        #endregion Instance Variables

        #region Constructor
        private AzureCloudScriptHost()
            : base()
        {
            //Hides the empty constructor
        }
        public AzureCloudScriptHost(object storageConnectionString, object containerName)
            : base()
        {

            if (string.IsNullOrEmpty(storageConnectionString.ToString()) && string.IsNullOrEmpty(containerName.ToString()))
            {
                throw new Exception("AzureScriptHost requires a storageConnectionString to be set!");
            }

            azurePlatformAdaptationLayer = new AzurePlatformAdaptationLayer(storageConnectionString.ToString(), containerName.ToString());
        }
        #endregion Constructor

        #region Properties
        public override PlatformAdaptationLayer PlatformAdaptationLayer
        {
            get { return azurePlatformAdaptationLayer; }
        }
        #endregion Properties
    }
}