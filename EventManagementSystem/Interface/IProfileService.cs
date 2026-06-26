    using EventManagementSystem.DTO;
    using EventManagementSystem.DTO;

    namespace EventManagementSystem.Interface
    {
        public interface IProfileService
        {
            Task<ProfileDto> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    }
    }

