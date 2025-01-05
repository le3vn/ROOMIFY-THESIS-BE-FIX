using System;

namespace Roomify.Commons.Services;

public interface IStorageService
    {
        Task<string> GetPresignedUrlReadAsync(string fileName);
        Task<string> GetPresignedUrlWriteAsync(string fileName);
        Task UploadFileAsync(string fileName, Stream data);
        Task DeleteFileAsync(string fileName);
        Task MoveFileAsync(string sourcePath, string destPath);
    }
