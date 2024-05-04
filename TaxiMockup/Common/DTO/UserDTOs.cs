namespace Common.DTO
{
    public record UserLoginDTO(string Username, string Password);
    public record UserStateDTO(Guid UserId, UserState UserState);
}
