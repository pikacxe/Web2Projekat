using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Common.DTO
{
    [DataContract]
    public class UserLoginRequest
    {
        [DataMember]
        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required!")]
        public string? Email { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required!")]
        public string? Password { get; set; }
    }

    [DataContract]
    public class RegisterUserRequest
    {
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required!")]
        public string? Username { get; set; }
        [DataMember]
        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required!")]
        public string? Email { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required!")]
        public string? Password { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fullname is required!")]
        public string? Fullname { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Picture is required!")]
        public string? UserPicture { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Date of birth is required!")]
        public DateTimeOffset? DateOfBirth { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Address is required!")]
        public string? Address { get; set; }
        [DataMember]
        [Range(0,1, ErrorMessage ="User type must be 'User' or 'Driver'")]
        public UserType UserType { get; set; }
    }

    [DataContract]
    public class UpdateUserReques
    {
        [DataMember]
        [Required]
        public Guid Id { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required!")]
        public string? Username { get; set; }
        [DataMember]
        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required!")]
        public string? Email { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fullname is required!")]
        public string? Fullname { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Picture is required!")]
        public string? UserPicture { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Date of birth is required!")]
        public DateTimeOffset? DateOfBirth { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Address is required!")]
        public string? Address { get; set; }
    }

    [DataContract]
    public record UserStateResponse
    {
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public UserState UserState { get; set; }
    }

    [DataContract]
    public class UserPasswordChangeRequest
    {
        [DataMember]
        [Required(ErrorMessage = "User Id is required!")]
        public Guid UserId { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Old password is required")]
        public string? OldPassword { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "New password is required")]
        public string? NewPassword { get; set; }
    }

    [DataContract]
    public record UserInfo
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string? Username { get; set; }
        [DataMember]
        public string? Email { get; set; }
        [DataMember]
        public string? Fullname { get; set; }
        [DataMember]
        public DateTimeOffset? DateOfBirth { get; set; }
        [DataMember]
        public string? Address { get; set; }
        [DataMember]
        public UserType UserType { get; set; }
        [DataMember]
        public string? UserPicture { get; set; }
        [DataMember]
        public UserState UserState { get; set; }
    }

    [DataContract]
    public class AuthResponse
    {
        [DataMember]
        public string UserID { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string UserRole { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string ProfileImage { get; set; }
    }

}
