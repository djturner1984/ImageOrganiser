using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using ImageOrganiser.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganiser
{
    public class FileTraverser : IFileTraverser
    {
        private readonly IObjectUploader _objectUploader;
        private readonly IFaceRecogniser _faceRecogniser;

        private IAmazonS3 _s3Client { get; }
        private List<string> _acceptedExtensions = new List<string>
        {
            "png",
            "jpg",
            "jpeg",
            "gif"
        };

        public FileTraverser(IObjectUploader objectUploader, IAmazonS3 s3Client, IFaceRecogniser faceRecogniser)
        {
            _objectUploader = objectUploader;
            _s3Client = s3Client;
            _faceRecogniser = faceRecogniser;
        }

        public async Task TraverseFor(Settings settings)
        {
            var request = new ListObjectsRequest()
            {
                BucketName = settings.BucketName,
                Prefix = settings.SourcePrefix
            };
            var objects = new List<Amazon.S3.Model.S3Object>();
            var response = await _s3Client.ListObjectsAsync(request);
            objects.AddRange(response.S3Objects);
            while (response.IsTruncated)
            {
                request.Marker = response.NextMarker;
                response = await _s3Client.ListObjectsAsync(request);
                objects.AddRange(response.S3Objects);
            }
            foreach(var s3Object in objects)
            {
                var extension = s3Object.Key.Substring(s3Object.Key.LastIndexOf(".") + 1);
                if (!_acceptedExtensions.Any(x => x.Equals(extension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }
                var recognitionRequest = new RecognitionRequest()
                {
                    Age = settings.Age,
                    IsSmiling = settings.IsSmiling,
                    IsMale = settings.IsMale
                };
                var result = await _faceRecogniser.Recognise(settings.BucketName, s3Object.Key, recognitionRequest);
                if (result.IsFound)
                {
                    await _objectUploader.UploadObjectAsync(settings.BucketName, s3Object.Key, $"{settings.DestinationPrefix}/{s3Object.Key.GetFileFromKey()}");
                }
            }
        }
    }
}
