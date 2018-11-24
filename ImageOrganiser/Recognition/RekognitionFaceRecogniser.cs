using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageOrganiser
{
    public class RekognitionFaceRecogniser : IFaceRecogniser
    {
        private readonly IConfiguration _configuration;

        private IAmazonS3 _s3Client;
        private int _ageBuffer = 2;
        private int _confidenceRequired = 80;
        public RekognitionFaceRecogniser(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _configuration = configuration;
        }

        public async Task<RecognitionResult> Recognise(string bucketName, string key, RecognitionRequest request)
        {
            var client = new AmazonRekognitionClient(_configuration.GetSection("AWS:AccessKey").Value.ToString(), _configuration.GetSection("AWS:SecretKey").Value.ToString(), Amazon.RegionEndpoint.APSoutheast2);
            var faceResponse = await client.DetectFacesAsync(new DetectFacesRequest
            {
                Image = new Image
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object
                    {
                        Bucket = bucketName,
                        Name = key
                    }
                },
                Attributes = new List<string> { "ALL" }
            });

            List<FaceDetail> faceDetails = faceResponse?.FaceDetails;
            
            if (faceDetails != null)
            {
                var results = FilterFaces(faceDetails, request);
                if (results.Any())
                {
                    Console.WriteLine($"Found at least one person in {key.GetFileFromKey()} meeting the criteria.");
                    return new RecognitionResult()
                    {
                        IsFound = true,
                        IsAlone = results.Count() == 1 && faceDetails.Count() == 1
                    };
                }
            }

            return new RecognitionResult()
            {
                IsFound = false,
                IsAlone = false

            };

        }

        private List<FaceDetail> FilterFaces(List<FaceDetail> faceDetails, RecognitionRequest request)
        {
            if (faceDetails == null)
            {
                return faceDetails;
            }
            var results = faceDetails;
            if (request.Age.HasValue)
            {
                results = results.Where(f => 
                f.AgeRange?.High >= (request.Age.Value + _ageBuffer) && 
                f.AgeRange?.Low <= (Math.Max(0, request.Age.Value + _ageBuffer))).ToList();
            }

            if (request.IsMale.HasValue)
            {
                var genderType = request.IsMale.Value ? GenderType.Male : GenderType.Female;
                results = results.Where(f =>
                f?.Gender?.Value == genderType && f?.Gender?.Confidence >= _confidenceRequired).ToList();
            }

            if (request.IsSmiling.HasValue)
            {
                results = results.Where(f =>
                f?.Smile?.Value == request.IsSmiling.Value && f?.Smile?.Confidence >= _confidenceRequired).ToList();
            }

            return results;
        }
    }
}
