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
        - User
        - Driver
        - Administrator
    - UserPicture
    - State
        - Default
        - Verified
        - Denied
    - _CreatedAt
    - _VerifiedAt
    - _UpdatedAt
```
```
- Ride
    - RideId
    - PassangerId
    - DriverId
    - Start Destination
    - End Destination
    - Price
    - Ride duration
    - Driver ETA
    - State
        - Requested
        - Confirmed
        - Pending
        - InProgress
        - Finished
    - Rating
    - _CreatedAt
    - _UpdatedAt
    - _FinishedAt
```


## Endpoints

- [ ] /users
    - [X] /
    - [X] /:id
    - [X] /:id/verify
    - [ ] /login
    - [X] /register
        - [ ] /register/oauth
    - [X] /unverified
    - [X] /state
- [X] /rides
    - [X] /
    - [X] /request
    - [X] /confirm
    - [X] /pending
    - [X] /history
    - [X] /finished

### TODO
- [ ] add JWT auth
- [X] implement Rides service
- [ ] implement CDN service
- [ ] add auth to mongoDb
- [ ] implement CDN service
- [ ] add model and dto validation

