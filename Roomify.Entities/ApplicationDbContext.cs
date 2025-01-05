using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Roomify.Entities
{
    public class ApplicationDbContext : IdentityDbContext<User>, IDataProtectionKeyContext
    {
        /// <summary>
        /// https://www.postgresql.org/docs/current/pgtrgm.html
        /// </summary>
        private const string PgTrigramExtension = "pg_trgm";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresExtension(PgTrigramExtension);

            // Guid max length
            builder.Entity<User>().Property(Q => Q.Id).HasMaxLength(36);
            builder.Entity<User>().Property(Q => Q.PhoneNumber).HasMaxLength(16);
            builder.Entity<OpenIddictEntityFrameworkCoreApplication>().Property(Q => Q.Id).HasMaxLength(36);
            builder.Entity<OpenIddictEntityFrameworkCoreAuthorization>().Property(Q => Q.Id).HasMaxLength(36);
            builder.Entity<OpenIddictEntityFrameworkCoreScope>().Property(Q => Q.Id).HasMaxLength(36);
            builder.Entity<OpenIddictEntityFrameworkCoreToken>().Property(Q => Q.Id).HasMaxLength(36);

            // Use: Roomify/RequestHandlers/ListUserRequestHandler.cs
            builder.Entity<User>().HasIndex(Q => new { Q.GivenName, Q.Id });
            builder.Entity<User>().HasTrigramIndex(Q => Q.GivenName);
            builder.Entity<User>().HasTrigramIndex(Q => Q.FamilyName);
            builder.Entity<User>().HasTrigramIndex(Q => Q.Email);
        }

        public DbSet<Blob> Blobs => Set<Blob>();
        public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();
        public DbSet<ApproverDetail> ApproverDetails => Set<ApproverDetail>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Building> Buildings => Set<Building>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomGroup> RoomGroups => Set<RoomGroup>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<SessionBooked> SessionBookeds => Set<SessionBooked>();
        public DbSet<Status> Statuses => Set<Status>();
        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<RejectMessage> RejectMessages => Set<RejectMessage>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<ManageRole> ManageRoles => Set<ManageRole>();
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Equipment> Equipments => Set<Equipment>();
        public DbSet<EquipmentBooked> EquipmentBookeds => Set<EquipmentBooked>();
        public DbSet<InstitutionalNumber> InstitutionalNumbers => Set<InstitutionalNumber>();
        public DbSet<QRCode> QRCodes => Set<QRCode>();
        public DbSet<ApproverHistory> ApproverHistories => Set<ApproverHistory>();
        public DbSet<Blocker> Blockers=> Set<Blocker>();
        public DbSet<Notification> Notifications=> Set<Notification>();
    }
}