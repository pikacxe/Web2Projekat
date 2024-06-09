using Common.Entities;

namespace Common.DTO
{
    public static class Extensions
    {
        public static CompletedRideInfoResponse AsInfoDTO(this Ride ride) => new CompletedRideInfoResponse
        {
            RideId = ride.Id,
            PassengerId = ride.PassengerId,
            DriverId = ride.DriverId,
            StartDestination = ride.StartDestination,
            EndDestination = ride.EndDestination,
            Price = ride.Price,
            RideDuration = ride.RideDuration,
            Rating = ride.Rating
        };

        public static AvailableRideResponse AsAvailableRideDTO(this Ride ride) => new AvailableRideResponse
        {
            RideId = ride.Id,
            StartDestination = ride.StartDestination,
            EndDestination = ride.EndDestination,
            PassengerName = ride.PassengerName
        };

        public static UserInfo AsInfoDTO(this User user) => new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Address = user.Address,
            DateOfBirth = user.DateOfBirth?.Date.ToShortDateString(),
            Fullname = user.Fullname,
            UserPicture = user.UserPicture,
            UserState = user.UserState,
            UserType = user.UserType

        };

        public static UserStateResponse AsStateDTO(this User user) => new UserStateResponse
        {
            UserId = user.Id,
            UserState = user.UserState
        };

    }
}
