# ImageOrganiser
Uses AWS Rekognition to identify age, gender, and whether person is smiling to organise images

Something I quickly hacked together to help extract all my photos of my son place them in a separate s3 prefix.

This app expects to be looking at files in s3, when it finds images that match the criteria, it copies it to your specified bucket prefix.

Pre-requisites

.NET core

Existing s3 bucket

Existing s3 profile on machine with read/write access to bucket

Access key and secret key specified in appsettings (tried to avoid this but AmazonRekognitionClient forces you to)

Specify source prefix in app settings
