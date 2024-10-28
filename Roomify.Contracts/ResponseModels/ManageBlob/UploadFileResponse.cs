using System;

namespace Roomify.Contracts.ResponseModels.ManageBlob;

public class UploadFileResponse
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string ContentType { get; set; } = "";
    }
