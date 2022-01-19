using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Configuration;

namespace WpfApp1
{
    /// <summary>
    /// Get BlobSharedAccessSignatures
    /// </summary>
    public static class BlobSharedAccessSignatures
    {
        /// <summary>
        /// BlobSharedAccessSignatures
        /// </summary>
        /// <returns></returns>
        public static string Token
        {
            get
            {
                return GetSharedAccessToken(DateTime.UtcNow.AddHours(8));
            }
        }

        /// <summary>
        /// GetSharedAccessToken
        /// </summary>
        /// <param name="tokenExpiryTime"></param>
        /// <returns></returns>
        private static string GetSharedAccessToken(DateTimeOffset tokenExpiryTime)
        {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                                                       ConfigurationManager.AppSettings["BlobStorageConnectionString"]);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["BlobContainer"]);
                container.CreateIfNotExists();
                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
                containerPermissions.SharedAccessPolicies.Add("HrOne", new SharedAccessBlobPolicy
                {
                    SharedAccessExpiryTime = tokenExpiryTime,
                    Permissions = SharedAccessBlobPermissions.Read
                });

                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Off;
                container.SetPermissions(containerPermissions);
                return container.GetSharedAccessSignature(new SharedAccessBlobPolicy(), "HrOne");
        }

        /// <summary>
        /// SharedAccessToken
        /// </summary>
        /// <param name="tokenExpiryTime"></param>
        /// <returns></returns>
        public static string SharedAccessToken(DateTimeOffset tokenExpiryTime)
        {
            return GetSharedAccessToken(tokenExpiryTime);
        }
    }
}
