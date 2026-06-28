using DocumentFormat.OpenXml.Math;
using Entities.Models;
using Repositories.Repository;
using Services.Interface;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        public UserService(UserRepository userRepository) => _userRepository = userRepository;

        public Task<User> GetUserById(int userId)
        {
            return _userRepository.GetUserById(userId);
        }

        public async Task<int> UpdateUser(User user)
        {
            await _userRepository.UpdateAsync(user);
            return 1;
        }

        public Task<MangakaProfile> GetMangakaProfile(int userId)
        {
            return _userRepository.GetMangakaProfile(userId);
        }

        public Task<AssistantProfile> GetAssistantProfile(int userId)
        {
            return _userRepository.GetAssistantProfile(userId);
        }

        public async Task<int> AddMangakaProfile(MangakaProfile profile)
        {
            await _userRepository.AddMangakaProfile(profile);
            return 1;
        }

        public async Task<int> AddAssistantProfile(AssistantProfile profile)
        {
            await _userRepository.AddAssistantProfile(profile);
            return 1;
        }

        public async Task<int> UpdateMangakaProfile(int userId, DTO.UserDto.UpdateMangakaProfile dto, string? avatarUrl)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null) return 0;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                user.Fullname = dto.FullName;
                await _userRepository.UpdateAsync(user);
            }

            var profile = await _userRepository.GetMangakaProfile(userId);
            if (profile == null) return 0;

            profile.PenName = dto.PenName;
            profile.Bio = dto.Bio;
            profile.PhoneNumber = dto.PhoneNumber;
            profile.BankName = dto.BankName;
            profile.BankAccountNumber = dto.BankAccountNumber;
            profile.BankAccountName = dto.BankAccountName;
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                profile.AvatarUrl = avatarUrl;
            }
            profile.Updatedat = DateTime.UtcNow;

            await _userRepository.UpdateMangakaProfile(profile);
            return 1;
        }

        public async Task<int> UpdateAssistantProfile(int userId, DTO.UserDto.UpdateAssistantProfile dto, string? avatarUrl)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null) return 0;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                user.Fullname = dto.FullName;
                await _userRepository.UpdateAsync(user);
            }

            var profile = await _userRepository.GetAssistantProfile(userId);
            if (profile == null) return 0;

            if (!string.IsNullOrEmpty(avatarUrl))
            {
                profile.AvatarUrl = avatarUrl;
            }
            profile.PortfolioUrl = dto.PortfolioUrl;
            profile.PhoneNumber = dto.PhoneNumber;
            profile.IsAvailable = dto.IsAvailable;
            profile.Skills = dto.Skills;
            profile.SoftwareUsed = dto.SoftwareUsed;
            profile.BankName = dto.BankName;
            profile.BankAccountNumber = dto.BankAccountNumber;
            profile.BankAccountName = dto.BankAccountName;
            profile.Updatedat = DateTime.UtcNow;

            await _userRepository.UpdateAssistantProfile(profile);
            return 1;
        }

        public async Task<List<DTO.UserDto.AvailableAssistant>> GetAvailableAssistants()
        {
            var profiles = await _userRepository.GetAvailableAssistantsAsync();
            return profiles.Select(p => new DTO.UserDto.AvailableAssistant(
                p.Userid,
                p.User?.Username ?? string.Empty,
                p.User?.Fullname ?? string.Empty,
                p.User?.Email ?? string.Empty,
                p.AvatarUrl,
                p.PortfolioUrl,
                p.Skills,
                p.SoftwareUsed
            )).ToList();
        }

        public async Task<List<DTO.UserDto.TantouEditorResponse>> GetTantouEditorsAsync()
        {
            var users = await _userRepository.GetUsersAsync(3, "Active");
            return users.Select(u => new DTO.UserDto.TantouEditorResponse(
                u.Userid,
                u.Username,
                u.Fullname,
                u.Email ?? string.Empty,
                null
            )).ToList();
        }

        public async Task<List<DTO.UserDto.EbMemberResponse>> GetEbMembersAsync()
        {
            var users = await _userRepository.GetUsersAsync(2, "Active");
            return users.Select(u => new DTO.UserDto.EbMemberResponse(
                u.Userid,
                u.Username,
                u.Fullname,
                u.Email ?? string.Empty
            )).ToList();
        }

        // Admin User Management
        public async Task<List<DTO.UserDto.AdminUserResponse>> GetUsersAsync(int? roleId, string? status)
        {
            var users = await _userRepository.GetUsersAsync(roleId, status);
            var result = users.Select(u => new DTO.UserDto.AdminUserResponse(
                u.Userid,
                u.Username,
                u.Fullname,
                u.Email,
                u.Roleid,
                u.Role?.Rolename ?? "",
                u.Status,
                u.Createdat
            )).ToList();

            return result;
        }

        public async Task<DTO.UserDto.AdminUserResponse?> GetUserDetailsAsync(int id)
        {
            var u = await _userRepository.GetUserById(id);
            if (u == null || u.Isdeleted == true) return null;

            // Lấy role tay nếu query không include (để an toàn)
            string roleName = u.Role?.Rolename ?? "";
            
            return new DTO.UserDto.AdminUserResponse(
                u.Userid,
                u.Username,
                u.Fullname,
                u.Email,
                u.Roleid,
                roleName,
                u.Status,
                u.Createdat
            );
        }

        public async Task<int> AdminCreateUserAsync(DTO.UserDto.AdminCreateUserRequest request)
        {
            var existing = await _userRepository.GetUserByUsername(request.Username);
            if (existing != null)
            {
                throw new InvalidOperationException("Username đã tồn tại.");
            }

            var user = new User
            {
                Username = request.Username,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Fullname = request.FullName,
                Email = request.Email,
                Roleid = request.RoleId,
                Createdat = DateTime.Now,
                Isdeleted = false,
                Status = "Active"
            };

            await _userRepository.CreateAsync(user);
            return user.Userid;
        }

        public async Task<int> AdminUpdateUserAsync(int id, DTO.UserDto.AdminUpdateUserRequest request)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null || user.Isdeleted == true) return 0;

            user.Fullname = request.FullName;
            user.Email = request.Email;
            user.Roleid = request.RoleId;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _userRepository.UpdateAsync(user);
            return 1;
        }

        public async Task<bool> AdminUpdateStatusAsync(int id, string newStatus)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null || user.Isdeleted == true) return false;

            user.Status = newStatus;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> AdminSoftDeleteUserAsync(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null || user.Isdeleted == true) return false;

            user.Isdeleted = true;
            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}