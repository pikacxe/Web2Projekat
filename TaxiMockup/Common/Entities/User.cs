
using System.ComponentModel.DataAnnotations;

namespace Common.Entities
{
    public class User : IEntity
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string? Username { get; set; }
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }   
        public string? Fullname { get; set; }  
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public UserType UserType { get; set; }
        public string? UserPicture { get; set; }
        public UserState UserState { get; set; }
        public DateTimeOffset? _CreatedAt { get; set; }
        public DateTimeOffset? _UpdatedAt { get; set; }
        public DateTimeOffset? _VerifiedAt { get; set; }

    }
}
