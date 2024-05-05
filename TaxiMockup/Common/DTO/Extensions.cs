using Common.Entities;

namespace Common.DTO
{
    public static class Extensions
    {
        public static CompletedRideInfoDTO AsInfoDTO(this Ride ride) => new CompletedRideInfoDTO
        {
            RideId = ride.Id,
            UserId = ride.PassengerId,
            DriverId = ride.DriverId,
            StartDestination = ride.StartDestination,
            EndDestination = ride.EndDestination,
            Price = ride.Price,
            RideDuration = ride.RideDuration,
            Rating = ride.Rating
        };

        public static AvailableRideDTO AsAvailableRideDTO(this Ride ride) => new AvailableRideDTO
        {
            RideId = ride.Id,
            StartDestination = ride.StartDestination,
            EndDestination = ride.EndDestination
        };

        public static UserInfoDTO AsInfoDTO(this User user) => new UserInfoDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Address = user.Address,
            DateOfBirth = user.DateOfBirth,
            Fullname = user.Fullname,
            UserPicture = user.UserPicture,
            UserState = user.UserState,
            UserType = user.UserType

        };

    }
}
