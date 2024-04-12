# Project architecture

# Tech stack
- Frontend - **React**
- Backend - **ASP.NET CORE**
- Service Fabric
- MS SQL

## Services
- DB
- Api gateway
- Frontend React
- CDN service (User images)
- Auth service/Identity provider
- Mail service/Notifcation service
- Google maps
- User service
- Ride service
- Admin service

# Entities
- User
    - UserId
    - Username
    - Email
    - Password
    - Full name
    - DoB
    - Address
    - UserType
        - Administrator
        - User
        - Driver
    - UserPicture
    - State
        - Created
        - Activated
        - Requested
        - Verified
        - Denied
    - _Created
    - _Verified
    - _Updated

- Ride
    - PassangerId
    - DriverId
    - Start Destination
    - End Destination
    - Price
    - Estimated ride duration
    - Esitmated driver arrival
    - State
        - Pending
        - Confirmed
        - InProgress
        - Finished
    - Rating
    - _Created
    - _Updated


> Client app uses GoogleMaps for ride estimations. (**Distance Matrix API**) </br>
> CDN service for user images


