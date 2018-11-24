using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganiser
{
    public class S3ObjectUploader : IObjectUploader
    {
        private readonly IAmazonS3 _s3Client;

        public S3ObjectUploader(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task UploadObjectAsync(string bucketName, string sourceKey, string destinationKey)
        {
            await UploadFileAsync(bucketName, sourceKey, destinationKey);
        }

        private async Task UploadFileAsync(string bucketName, string sourceKey, string destinationKey)
        {
            try
            {
                CopyObjectRequest request = new CopyObjectRequest
                {
                    SourceBucket = bucketName,
                    SourceKey = sourceKey,
                    DestinationBucket = bucketName,
                    DestinationKey = destinationKey
                };
                CopyObjectResponse response = await _s3Client.CopyObjectAsync(request);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

        }

        public async Task<bool> Exists(string key, string bucketName)
        {
            try
            {
                var request = new GetObjectMetadataRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };
                var response = await _s3Client.GetObjectMetadataAsync(request);

                Console.WriteLine($"key {key} already exists in bucket {bucketName}");
                return true;
            }

            catch (Amazon.S3.AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                return false;
            }
        }

        private string GetPathWithoutDrive(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }
            var pathWithoutDriveOrNetworkShare = "";
            if ((path.IndexOf(":") == 1) || (path.IndexOf("\\\\") == 0))
            {
                pathWithoutDriveOrNetworkShare = path.Substring(3);
            }

            return ReplaceSlashes(pathWithoutDriveOrNetworkShare);
        }

        public string ReplaceSlashes(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value?.Replace(@"\\", "/")?.Replace(@"\", "/");
        }
    }
}
