using System;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Roomify.Commons.Extensions;

namespace Roomify.Commons.Services;

public class StorageService : IStorageService
    {
        private readonly MinioClient _minioClient;
        private readonly string _bucketName;
        public StorageService(MinioClient minioClient, IOptions<MinIoOptions> options)
        {
            _minioClient = minioClient;
            _bucketName = options.Value.BucketName;
        }

        public async Task<string> GetPresignedUrlReadAsync(string fileName)
        {
            var expiry = (int)TimeSpan.FromMinutes(15).TotalSeconds;
            var args = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithExpiry(expiry);

            return await _minioClient.PresignedGetObjectAsync(args);
        }
        public async Task<string> GetPresignedUrlWriteAsync(string fileName)
        {
            var expiry = (int)TimeSpan.FromMinutes(15).TotalSeconds;
            var args = new PresignedPutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithExpiry(expiry);

            return await _minioClient.PresignedPutObjectAsync(args);
        }

        public async Task UploadFileAsync(string fileName, Stream data)
        {
            var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            bool isBucketExist = await _minioClient.BucketExistsAsync(bucketExistArgs);

            if (!isBucketExist)
            {
                var makeBucketArgs = new MakeBucketArgs()
                      .WithBucket(_bucketName);

                await _minioClient.MakeBucketAsync(makeBucketArgs);
            }

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithStreamData(data)
                .WithObjectSize(data.Length);

            await _minioClient.PutObjectAsync(putObjectArgs);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs);
        }

        public async Task MoveFileAsync(string sourcePath, string destPath)
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(sourcePath);
            var sourceObject = await _minioClient.StatObjectAsync(statObjectArgs);

            if (sourceObject != null)
            {
                var sourceObjectArgs = new CopySourceObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(sourcePath);

                var destObjectArgs = new CopyObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(destPath)
                    .WithCopyObjectSource(sourceObjectArgs);

                await _minioClient.CopyObjectAsync(destObjectArgs);

                //kalau mau sekalian delete
                await DeleteFileAsync(sourcePath);
            }
        }
    }
