using System.ComponentModel.DataAnnotations;
using Roomify.Entities.EntityDescriptors;

namespace Roomify.Entities
{
    public class Blob : IHaveCreateOnlyAudit
    {
        public Guid Id { get; set; }

        [StringLength(255)]
        public string FileName { get; set; } = "";

        [StringLength(255)]
        public string FilePath { get; set; } = "";

        [StringLength(255)]
        public string ContentType { get; set; } = "";
        
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [StringLength(256)]
        public string? CreatedBy { get; set; }
        public List<User>? Users { get; set; } = new List<User>();
        public List<Building>? Buildings { get; set; } = new List<Building>();
    }
}
