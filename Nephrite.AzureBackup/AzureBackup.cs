using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;

namespace Nephrite.AzureBackup
{
    public class AzureBackup
    {
		string connectionString = "DefaultEndpointsProtocol=https;AccountName=nephritetech;AccountKey=uBs3BRRttHC7Fy7uyTi4mQJuSwnYEIU0THW7TrEfK3XA/G3vkqPVNYBFrFhQ8r5NB93WL+UhsZkxXw5o8/fzHQ==";
		static CloudStorageAccount storageAccount; 

		public AzureBackup(string connectionstring)
		{
			if (!string.IsNullOrEmpty(connectionstring)) connectionString = connectionstring;	
			storageAccount = CloudStorageAccount.Parse(connectionString);
		}

		public void Save(string filefrom, string filename, string containername)
		{
			var blobClient = storageAccount.CreateCloudBlobClient();
			var container = blobClient.GetContainerReference(containername);
			if (container.CreateIfNotExists())
				container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

			if (container.Exists())
			{
				var lastblob = container.GetBlockBlobReference(filename);
				if (!lastblob.Exists())
				using (var fileStream = System.IO.File.OpenRead(filefrom))
				{
					lastblob.UploadFromStream(fileStream);
				}

				var curdate = DateTime.Now.ToUniversalTime();
				var onemonthdate = curdate.AddMonths(-1);
				var sevendaydate = curdate.AddDays(-7);
				var listblob = container.ListBlobs().Cast<CloudBlockBlob>();
				foreach (var blob in listblob.Where(o => onemonthdate > o.Properties.LastModified.Value.DateTime))
				{
						blob.Delete();
				}

				int week2 = 1, week3 = 1, week4 = 1, week5 = 1;
				foreach (var blob in listblob.Where(o => sevendaydate > o.Properties.LastModified.Value.DateTime &&
						onemonthdate <= o.Properties.LastModified.Value.DateTime).OrderByDescending(o => o.Properties.LastModified))
				{
					if (curdate <= blob.Properties.LastModified.Value.DateTime.AddDays(14))
					{
						if (week2 != 1) blob.Delete();
						week2++;
					}
					else
						if (curdate <= blob.Properties.LastModified.Value.DateTime.AddDays(21))
						{
							if (week3 != 1) blob.Delete();
							week3++;
						}
						else
							if (curdate <= blob.Properties.LastModified.Value.DateTime.AddDays(28))
							{
								if (week4 != 1) blob.Delete();
								week4++;
							}
							else
							{
								if (week5 != 1) blob.Delete();
								week5++;
							}

				}

				int day1 = 1, day2 = 1, day3 = 1, day4 = 1, day5 = 1, day6 = 1, day7 = 1;
				foreach (var blob in listblob.Where(o => sevendaydate <= o.Properties.LastModified.Value.DateTime)
											.OrderByDescending(o => o.Properties.LastModified))
				{
					if (curdate <= blob.Properties.LastModified.Value.DateTime.AddDays(1))
					{
						if (day1 != 1) blob.Delete();
						day1++;
					}
					else
					if (curdate <= blob.Properties.LastModified.Value.DateTime.AddDays(2))
					{
						if (day2 != 1) blob.Delete();
						day2++;
					}
					else
					if (curdate <= blob.Properties.LastModified.Value.DateTime.AddDays(3))
					{
						if (day3 != 1) blob.Delete();
						day3++;
					}
					else
					if (curdate <= blob.Properties.LastModified.Value.DateTime.AddDays(4))
					{
						if (day4 != 1) blob.Delete();
						day4++;
					}
					else
					if (curdate <= blob.Properties.LastModified.Value.DateTime.AddDays(5))
					{
						if (day5 != 1) blob.Delete();
						day5++;
					}
					else
					if (curdate <= blob.Properties.LastModified.Value.DateTime.AddDays(6))
					{
						if (day6 != 1) blob.Delete();
						day6++;
					}
					else
					{
						if (day7 != 1) blob.Delete();
						day7++;
					}
				}
			}
		}

		public void Save2(string filefrom, string filename, string containername)
		{
			var fileClient = storageAccount.CreateCloudFileClient();

			var share = fileClient.GetShareReference("backupdb");

			if (share.Exists() || share.CreateIfNotExists())
			{
				var rootDir = share.GetRootDirectoryReference();

				var sampleDir = rootDir.GetDirectoryReference(containername);
				if (sampleDir.Exists() || sampleDir.CreateIfNotExists())
				{
					var file = sampleDir.GetFileReference(filename);

					if (!file.Exists())
					using (var fileStream = System.IO.File.OpenRead(filefrom))
					{
						file.UploadFromStream(fileStream);
					}
				}
			}
		}
	}
}
