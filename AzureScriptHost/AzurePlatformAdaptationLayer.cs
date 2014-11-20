using Microsoft.Scripting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureScriptHost
{
    /// <summary>
    /// This class allows the IronPython engine to load files from Azure as if the files were stored on the local file system
    /// </summary>
    public class AzurePlatformAdaptationLayer : PlatformAdaptationLayer
    {
        
        #region Instance Variables
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private CloudBlobContainer container;
        #endregion Instance Variables

        #region Constructor
        private AzurePlatformAdaptationLayer()
            : base()
        {
            //Hide empty contstructor
        }

        public AzurePlatformAdaptationLayer(string storageConnectionString, string containerName)
            : base()
        {
            #region validation
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new Exception("AzurePlatformAdaptationLayer storageConnectionString must be set");
            }
            #endregion validation

            // Retrieve storage account from connection string.
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            blobClient = storageAccount.CreateCloudBlobClient();            
            container = blobClient.GetContainerReference(containerName);
            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();
        }
        #endregion Constructor

        #region Methods
        private bool CloudFileExists(string path)
        {
            string cloudPath = ScrubPathForCloudBlobName(path);
            bool foundIt = false;
            if (!string.IsNullOrEmpty(cloudPath))
            {
                try
                {
                    foundIt = container.GetBlobReferenceFromServer(cloudPath).Exists();
                }
                catch(Exception e)
                {
                    //Eat it cause we didn't find it
                }
            }
            return foundIt;
        }

        private static string ScrubPathForCloudBlobName(string path)
        {
            string cloudPath = path.Replace("\\", "");
            cloudPath = cloudPath.Replace(".py", "");
            cloudPath = cloudPath.Replace(".", "");
            cloudPath = cloudPath.Replace(":", "");
            return cloudPath;
        }

        private bool CloudDirectoryExists(string fullPath)
        {
            return true;
        }

        public override bool FileExists(string path)
        {
            return CloudFileExists(path);
        }

        private System.IO.Stream OpenCloudInputStream(string path)
        {
            string cloudPath = ScrubPathForCloudBlobName(path);
            string script = GetBlobFile(cloudPath);
            byte[] byteArray = Encoding.UTF8.GetBytes(script);
            return new MemoryStream(byteArray);
        }

        private string GetBlobFile(string cloudPath)
        {
            
            string script = null;
            if (!string.IsNullOrEmpty(cloudPath))
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(cloudPath);
                script = blob.DownloadText();
                //m_BlobDataStore.DownloadBlob<string>(m_GroupId, ruleSet.Container, ruleSet.Name);
            }

            return script;
        }

        public override string[] GetFileSystemEntries(string path, string searchPattern, bool includeFiles, bool includeDirectories)
        {
            //string fullPath = Path.Combine(path, searchPattern);
            if (includeFiles && CloudFileExists(searchPattern))
            {
                return new[] { searchPattern };
            }

            //if (includeFiles && (CloudDirectoryExists(path)))
            //{
            //    return new[] { path };
            //}

            return new string[0];
        }

        public override bool DirectoryExists(string path)
        {
            return CloudDirectoryExists(path);
        }

        public override Stream OpenInputFileStream(string path)
        {
            return OpenCloudInputStream(path);
        }

        public override Stream OpenInputFileStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return OpenCloudInputStream(path);
        }

        public override Stream OpenInputFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
        {
            return OpenCloudInputStream(path);
        }
        #endregion Methods
    }
}

