using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Commons.Constants;
using Roomify.Commons.Services;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBooking
{
    public class CreateBookingRequestHandler : IRequestHandler<CreateBookingRequestModel, CreateBookingResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public CreateBookingRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<CreateBookingResponseModel> Handle(CreateBookingRequestModel request, CancellationToken cancellationToken)
        {
            // Fetch the user's role
            var userRole = await _db.UserRoles
                .Where(ur => ur.UserId == request.UserId)
                .Select(ur => ur.RoleId)
                .FirstOrDefaultAsync(cancellationToken);
            
            var roleName = await _db.Roles
                .Where(r => r.Id == userRole)
                .Select(r => r.NormalizedName)
                .FirstOrDefaultAsync(cancellationToken);

            // Fetch room type ID from the Rooms table
            var roomTypeId = await _db.Rooms
                .Where(r => r.RoomId == request.RoomId)
                .Select(r => r.RoomType)
                .FirstOrDefaultAsync(cancellationToken);

            var room = await _db.Rooms
                .Where(r => r.RoomId == request.RoomId)
                .FirstOrDefaultAsync(cancellationToken);

            if (room == null)
            {
                // Handle case where the Room is not found
                // For example, you could throw an exception or return null
                throw new Exception("Room not found");
            }

            // Now, use the GroupId to query the RoomGroups table
            var roomGroup = await _db.RoomGroups
                .Where(rg => rg.RoomGroupId == room.RoomGroupId)
                .FirstOrDefaultAsync(cancellationToken);

            if (roomGroup == null)
            {
                // Handle case where the RoomGroup is not found
                // For example, you could throw an exception or return null
                throw new Exception("RoomGroup not found");
            }

            if (roleName == "STUDENT" && roomTypeId == 1)
            {
                return await HandleStudentCase(request, roomGroup, userRole);
            }

            if (roleName == "STUDENTORGANIZATION")
            {
                if (roomTypeId == 1)
                {
                    return await HandleStudentOrganizationCase1(request, roomGroup, userRole);
                }
                else if (roomTypeId == 2)
                {
                    return await HandleStudentOrganizationCase2(request, roomGroup, userRole);
                }
                else if (roomTypeId == 3)
                {
                    return await HandleStudentOrganizationCase3(request, roomGroup, userRole);
                }
            }

            if (roleName == "LECTURER" || roleName == "STAFF")
            {
                return await HandleLecturerStaffCase(request, roomTypeId, roomGroup, userRole);
            }

            return new CreateBookingResponseModel
            {
                Success = "false",
                Message = "Invalid role or room type."
            };
        }

        private async Task AddNotificationToDb(string approverUserId, string subject, string message)
        {
            
        var notification = new Notification
        {
            UserId = approverUserId,
            Subject = subject,
            Message = message,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = "SYSTEM"
        };

        await _db.Notifications.AddAsync(notification);
        await _db.SaveChangesAsync();
        }


        private async Task<CreateBookingResponseModel> HandleStudentCase(CreateBookingRequestModel request, RoomGroup roomGroup, string userRole)
        {
            var room = await _db.Rooms
                .Where(r => r.RoomId == request.RoomId)
                .FirstOrDefaultAsync();

            var booking = new Booking
            {
                UserId = request.UserId,
                RoomId = request.RoomId,
                RoleId = userRole,
                BookingDate = request.BookingDate,
                BookingDescription = request.BookingDescription,
                ApprovalCount = 1,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId,
                StatusId = 1,
                IsCanceled = false,
                FullName = request.FullName,
                InstitutionalId = request.InstitutionalId
            };
            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            var approverDetail = new ApproverDetail
            {
                BookingId = booking.Id,
                AppproverUserId = roomGroup.ApproverSLCUserId,
                ApprovalOrder = 1,
                IsApproved = false,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId
            };
            await _db.ApproverDetails.AddAsync(approverDetail);
            await _db.SaveChangesAsync();
            await AddNotificationToDb(roomGroup.ApproverSLCUserId, "Approval Needed", $"Your approval is needed for for room {room?.Name}. Please review it.");

            foreach (var sessionId in request.SessionBookedList)
            {
                var sessionBooked = new SessionBooked
                {
                    BookingId = booking.Id,
                    SessionId = sessionId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };
                await _db.SessionBookeds.AddAsync(sessionBooked);
            }
            await _db.SaveChangesAsync();
            foreach (var equipmentId in request.EquipmentBookedList)
            {
                var equipmentBooked = new EquipmentBooked
                {
                    BookingId = booking.Id,
                    EquipmentId = equipmentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };
                await _db.EquipmentBookeds.AddAsync(equipmentBooked);
            }
            await _db.SaveChangesAsync();

            return new CreateBookingResponseModel
            {
                Success = "true",
                Message = "Booking created successfully."
            };
        }

        private async Task<CreateBookingResponseModel> HandleStudentOrganizationCase1(CreateBookingRequestModel request, RoomGroup roomGroup, string userRole)
        {
            var room = await _db.Rooms
                .Where(r => r.RoomId == request.RoomId)
                .FirstOrDefaultAsync();

            var blob = new Blob
            {
                Id = Guid.NewGuid(),
                FileName = request.Evidence.FileName,
                FilePath = $"{BlobPath.File}/{request.Evidence.FileName}",
                ContentType = request.Evidence.ContentType,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };

            using (var stream = request.Evidence.OpenReadStream())
            {
                await _storageService.UploadFileAsync(blob.FilePath, stream);
            }

            _db.Blobs.Add(blob);
            await _db.SaveChangesAsync();

            var booking = new Booking
            {
                UserId = request.UserId,
                RoomId = request.RoomId,
                RoleId = userRole,
                BookingDate = request.BookingDate,
                BookingDescription = request.BookingDescription,
                ApprovalCount = 2,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId,
                StatusId = 1,
                BlobId = blob.Id,
                IsCanceled = false,
                FullName = request.FullName,
                OrganizationName = request.OrganizationName,
                InstitutionalId = request.InstitutionalId
            };
            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            var approverDetail1 = new ApproverDetail
            {
                BookingId = booking.Id,
                AppproverUserId = roomGroup.ApproverSSOUserId,
                ApprovalOrder = 1,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId
            };
            var approverDetail2 = new ApproverDetail
            {
                BookingId = booking.Id,
                AppproverUserId = roomGroup.ApproverSLCUserId,
                ApprovalOrder = 2,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId
            };
            await _db.ApproverDetails.AddRangeAsync(approverDetail1, approverDetail2);
            await AddNotificationToDb(roomGroup.ApproverSSOUserId, "Approval Needed", $"Your approval is needed for for room {room?.Name}. Please review it.");

            await _db.SaveChangesAsync();

            foreach (var sessionId in request.SessionBookedList)
            {
                var sessionBooked = new SessionBooked
                {
                    BookingId = booking.Id,
                    SessionId = sessionId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };
                await _db.SessionBookeds.AddAsync(sessionBooked);
            }
            await _db.SaveChangesAsync();
            foreach (var equipmentId in request.EquipmentBookedList)
            {
                var equipmentBooked = new EquipmentBooked
                {
                    BookingId = booking.Id,
                    EquipmentId = equipmentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };
                await _db.EquipmentBookeds.AddAsync(equipmentBooked);
            }
            await _db.SaveChangesAsync();

            return new CreateBookingResponseModel
            {
                Success = "true",
                Message = "Booking created successfully."
            };
        }

        private async Task<CreateBookingResponseModel> HandleStudentOrganizationCase2(CreateBookingRequestModel request, RoomGroup roomGroup,  string userRole)
        {
            var room = await _db.Rooms
                .Where(r => r.RoomId == request.RoomId)
                .FirstOrDefaultAsync();
            var blob = new Blob
            {
                Id = Guid.NewGuid(),
                FileName = request.Evidence.FileName,
                FilePath = $"{BlobPath.File}/{request.Evidence.FileName}",
                ContentType = request.Evidence.ContentType,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };

            using (var stream = request.Evidence.OpenReadStream())
            {
                await _storageService.UploadFileAsync(blob.FilePath, stream);
            }

            _db.Blobs.Add(blob);
            await _db.SaveChangesAsync();
            var booking = new Booking
            {
                UserId = request.UserId,
                RoomId = request.RoomId,
                RoleId = userRole,
                BookingDate = request.BookingDate,
                BookingDescription = request.BookingDescription,
                ApprovalCount = 3,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId,
                StatusId = 1,
                BlobId = blob.Id,
                IsCanceled = false,
                FullName = request.FullName,
                OrganizationName = request.OrganizationName,
                InstitutionalId = request.InstitutionalId
            };
            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            var approverDetail1 = new ApproverDetail
            {
                BookingId = booking.Id,
                AppproverUserId = roomGroup.ApproverSSOUserId,
                ApprovalOrder = 1,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId
            };
            var approverDetail2 = new ApproverDetail
            {
                BookingId = booking.Id,
                AppproverUserId = roomGroup.ApproverLSCUserId,
                ApprovalOrder = 2,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId
            };
            var approverDetail3 = new ApproverDetail
            {
                BookingId = booking.Id,
                AppproverUserId = roomGroup.ApproverBMUserId,
                ApprovalOrder = 3,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId
            };
            await _db.ApproverDetails.AddRangeAsync(approverDetail1, approverDetail2, approverDetail3);
            await AddNotificationToDb(roomGroup.ApproverSSOUserId, "Approval Needed", $"Your approval is needed for for room {room?.Name}. Please review it.");

            await _db.SaveChangesAsync();

            foreach (var sessionId in request.SessionBookedList)
            {
                var sessionBooked = new SessionBooked
                {
                    BookingId = booking.Id,
                    SessionId = sessionId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };
                await _db.SessionBookeds.AddAsync(sessionBooked);
            }
            await _db.SaveChangesAsync();
            foreach (var equipmentId in request.EquipmentBookedList)
            {
                var equipmentBooked = new EquipmentBooked
                {
                    BookingId = booking.Id,
                    EquipmentId = equipmentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };
                await _db.EquipmentBookeds.AddAsync(equipmentBooked);
            }
            await _db.SaveChangesAsync();

            return new CreateBookingResponseModel
            {
                Success = "true",
                Message = "Booking created successfully."
            };
        }
        private async Task<CreateBookingResponseModel> HandleStudentOrganizationCase3(CreateBookingRequestModel request, RoomGroup roomGroup,  string userRole)
        {
            var room = await _db.Rooms
                .Where(r => r.RoomId == request.RoomId)
                .FirstOrDefaultAsync();

            var blob = new Blob
            {
                Id = Guid.NewGuid(),
                FileName = request.Evidence.FileName,
                FilePath = $"{BlobPath.File}/{request.Evidence.FileName}",
                ContentType = request.Evidence.ContentType,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };

            using (var stream = request.Evidence.OpenReadStream())
            {
                await _storageService.UploadFileAsync(blob.FilePath, stream);
            }

            _db.Blobs.Add(blob);
            await _db.SaveChangesAsync();
            var booking = new Booking
            {
                UserId = request.UserId,
                RoomId = request.RoomId,
                RoleId = userRole,
                BookingDate = request.BookingDate,
                BookingDescription = request.BookingDescription,
                ApprovalCount = 2,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId,
                StatusId = 1,
                BlobId = blob.Id,
                IsCanceled = false,
                FullName = request.FullName,
                OrganizationName = request.OrganizationName,
                InstitutionalId = request.InstitutionalId

            };
            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            var approverDetail1 = new ApproverDetail
            {
                BookingId = booking.Id,
                AppproverUserId = roomGroup.ApproverSSOUserId,
                ApprovalOrder = 1,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId
            };
            var approverDetail2 = new ApproverDetail
            {
                BookingId = booking.Id,
                AppproverUserId = roomGroup.ApproverBMUserId,
                ApprovalOrder = 2,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId
            };
            await _db.ApproverDetails.AddRangeAsync(approverDetail1, approverDetail2);
            await AddNotificationToDb(roomGroup.ApproverSSOUserId, "Approval Needed", $"Your approval is needed for for room {room?.Name}. Please review it.");

            await _db.SaveChangesAsync();

            foreach (var sessionId in request.SessionBookedList)
            {
                var sessionBooked = new SessionBooked
                {
                    BookingId = booking.Id,
                    SessionId = sessionId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };
                await _db.SessionBookeds.AddAsync(sessionBooked);
            }
            await _db.SaveChangesAsync();
            foreach (var equipmentId in request.EquipmentBookedList)
            {
                var equipmentBooked = new EquipmentBooked
                {
                    BookingId = booking.Id,
                    EquipmentId = equipmentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };
                await _db.EquipmentBookeds.AddAsync(equipmentBooked);
            }
            await _db.SaveChangesAsync();

            return new CreateBookingResponseModel
            {
                Success = "true",
                Message = "Booking created successfully."
            };
        }

        private async Task<CreateBookingResponseModel> HandleLecturerStaffCase(CreateBookingRequestModel request, int roomTypeId, RoomGroup roomGroup,  string userRole)
        {
            var room = await _db.Rooms
                .Where(r => r.RoomId == request.RoomId)
                .FirstOrDefaultAsync();

            var booking = new Booking
            {
                UserId = request.UserId,
                RoomId = request.RoomId,
                RoleId = userRole,
                BookingDate = request.BookingDate,
                BookingDescription = request.BookingDescription,
                ApprovalCount = 1,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = request.UserId,
                StatusId = 1,
                IsCanceled = false,
                FullName = request.FullName,
                InstitutionalId = request.InstitutionalId,
                OrganizationName =  request.OrganizationName
            };
            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            ApproverDetail approverDetail = roomTypeId switch
            {
                1 => new ApproverDetail
                {
                    BookingId = booking.Id,
                    AppproverUserId = roomGroup.ApproverSLCUserId,
                    ApprovalOrder = 1,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                },
                2 => new ApproverDetail
                {
                    BookingId = booking.Id,
                    AppproverUserId = roomGroup.ApproverLSCUserId,
                    ApprovalOrder = 1,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                },
                3 => new ApproverDetail
                {
                    BookingId = booking.Id,
                    AppproverUserId = roomGroup.ApproverBMUserId,
                    ApprovalOrder = 1,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                },
                _ => null
            };

            if (approverDetail != null)
            {
                await _db.ApproverDetails.AddAsync(approverDetail);
                await _db.SaveChangesAsync();
                await AddNotificationToDb(approverDetail.AppproverUserId, "Approval Needed", $"Your approval is needed for for room {room?.Name}. Please review it.");


                foreach (var sessionId in request.SessionBookedList)
                {
                    var sessionBooked = new SessionBooked
                    {
                        BookingId = booking.Id,
                        SessionId = sessionId,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = request.UserId
                    };
                    await _db.SessionBookeds.AddAsync(sessionBooked);
                }
                await _db.SaveChangesAsync();
                foreach (var equipmentId in request.EquipmentBookedList)
            {
                var equipmentBooked = new EquipmentBooked
                {
                    BookingId = booking.Id,
                    EquipmentId = equipmentId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };
                await _db.EquipmentBookeds.AddAsync(equipmentBooked);
            }
            await _db.SaveChangesAsync();

                return new CreateBookingResponseModel
                {
                    Success = "true",
                    Message = "Booking created successfully."
                };
            }

            return new CreateBookingResponseModel
            {
                Success = "false",
                Message = "Invalid room type for Lecturer/Staff."
            };
        }
    }
}
