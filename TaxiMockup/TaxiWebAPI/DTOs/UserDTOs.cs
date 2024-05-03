using Common;

namespace TaxiWebAPI.DTOs
{
    public record UserLoginDTO(string Username, string Password);
    public record UserStateDTO(Guid UserId, UserState UserState);
}
