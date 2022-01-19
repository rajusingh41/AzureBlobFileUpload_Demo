using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string ContainerName { get; }
        protected CloudStorageAccount StorageAccount { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            StorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobStorageConnectionString"]);
            ContainerName = ConfigurationManager.AppSettings["BlobContainer"];
        }

        private void btn_browsefile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            var result = openFileDlg.ShowDialog();
            if (result == true)
            {
                txt_fileName.Text = openFileDlg.FileName;
                var fileName = openFileDlg.SafeFileName;
                var fs = (System.IO.FileStream)openFileDlg.OpenFile();
                 var savefile = Save(fileName, fs, "dgn5");
                txt_filepath.Text = savefile.ToString();
               
            }
        }

        private Uri Save(string fileName, Stream fileStream, string domainCode)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            try
            {
                var directoryName = @"Temp/test/";
                // Create a blob client for interacting with the blob service.
                var blobClient = StorageAccount.CreateCloudBlobClient();
                // Create a container for organizing blobs within the storage account.
                var container = blobClient.GetContainerReference(ContainerName);
                container.CreateIfNotExists();
                var directory = container.GetDirectoryReference(domainCode);
                var blob = directory.GetBlockBlobReference(directoryName + fileName);
                blob.UploadFromStream(fileStream);
                return FileVirtualPath(@$"{directoryName}/{fileName}", domainCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Uri FileVirtualPath(string uploadedFileName, string domainCode)
        {
            try
            {
                // Create a blob client for interacting with the blob service.
                var blobClient = StorageAccount.CreateCloudBlobClient();
                // Create a container for organizing blobs within the storage account.
                var container = blobClient.GetContainerReference(ContainerName);
                container.CreateIfNotExists();
                var directory = container.GetDirectoryReference(domainCode);
                var blob = directory.GetBlockBlobReference(uploadedFileName);
                var token = BlobSharedAccessSignatures.SharedAccessToken(DateTime.MaxValue);
                return new Uri(blob.Uri, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
