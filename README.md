# Project architecture

Mock uber like app with microservice architecture where users can request a ride and wait for designated drivers.


> - Database is hosted inside docker container
> - Services use Microsoft Service Fabric distributed systems platform
> - React app is hosted inside docker containe

## Tech stack
- Frontend - **React**
- Backend - **ASP.NET CORE**
- Service Fabric
- MS SQL

## Services
- DB
- Frontend React
- CDN service (User images)
- Auth service / Identity provider
- Mail service / Notifcation service
- Google maps / OpenRoutesService
- User service
- Ride service
- Admin service

## Entities
```
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
```
```
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
```


## Endpoints

- [ ] /login
- [ ] /register
    - [ ] /register/oauth
- [ ] /admin
    - [ ] /verify
    - [ ] /unverified
- [ ] /users
    - [ ] /
    - [ ] /:id
    - [ ] /state
- [ ] /ride
    - [ ] /
    - [ ] /request
    - [ ] /confirm
    - [ ] /pending
    - [ ] /history
    - [ ] /finished



