# Project architecture

Mock uber like app with microservice architecture where users can request a ride and wait for designated drivers.


> - Database is hosted inside docker container
> - Services use Microsoft Service Fabric distributed systems platform
> - React app is hosted inside docker container

## Tech stack
- Frontend - **React**
- Backend - **.NET 8.0**
- Service Fabric
- MongoDB

## Services
- API gateway - web stateless
- User data service - stateless
- Ride data service - stateful
- CDN service (User images) - web statless

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
        - Unverified
        - Verified
        - Denied
    - _CreatedAt
    - _VerifiedAt
    - _UpdatedAt
```
```
- Ride
    - RideId
    - PassengerId
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
    - [X] /unverified
    - [X] /login
    - [X] /register
        - [ ] /oauth
    - [X] /:id/state
    - [X] /update
    - [X] /delete
    - [X] /:id/verify
    - [X] /:id/ban
- [X] /rides
    - [X] /
    - [X] /pending
    - [X] /:id/history
    - [X] /:id/finished
    - [X] /request
    - [X] /accept
    - [X] /finish

## TODO
- [ ] add JWT auth / Identity service
    - [ ] add RBAC
- [X] implement CDN service
- [X] implement Ride data service
    - [ ] implement data validation
- [X] implement User data service
    - [ ] implement data validation
- [X] add dto validation
- [ ] secure database connection
- [ ] add retry policy, rate limiters and circuit breaker


