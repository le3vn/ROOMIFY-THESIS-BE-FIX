using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Roomify.Entities.EntityDescriptors;

namespace Roomify.Entities
{
    public class User : IdentityUser, IHaveCreateAndUpdateAudit
    {
        [StringLength(64)]
        public string GivenName { set; get; } = "";

        [StringLength(64)]
        public string FamilyName { set; get; } = "";

        [StringLength(2000)]
        public string Picture { set; get; } = "";

        /// <summary>
        /// "Male" or "Female"
        /// </summary>
        [StringLength(8)]
        public string Gender { set; get; } = "";

        /// <summary>
        /// "YYYY-MM-DD" [ISO8601‑2004] The year MAY be 0000, indicating that it is omitted.
        /// </summary>
        [StringLength(10)]
        public string Birthdate { set; get; } = "";

        [StringLength(255)]
        public string StreetAddress { set; get; } = "";

        [StringLength(64)]
        public string City { set; get; } = "";

        [StringLength(64)]
        public string Province { set; get; } = "";

        [StringLength(16)]
        public string PostalCode { set; get; } = "";

        [StringLength(64)]
        public string Country { set; get; } = "";

        public bool IsEnabled { set; get; }

        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(256)]
        public string? CreatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(256)]
        public string? UpdatedBy { get; set; }
        public Guid? BlobId { get; set; }
        public Blob Blob { get; set; } = null!;
        public Guid DefaultRoleId { get; set; }
    }
}