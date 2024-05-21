# Project architecture

---

## Intruduction

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
- User data service - stateful
- Ride data service - stateful
- CDN service (User images) - statless

<summary>

## Entities

<details>

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

</details>
</summary>

<summary>

## Endpoints

<details>

- [ ] /users
  - [x] /
  - [x] /:id
  - [x] /unverified
  - [x] /login
  - [x] /register
    - [ ] /oauth
  - [x] /:id/state
  - [x] /update
  - [x] /delete
  - [x] /:id/verify
  - [x] /:id/ban
- [x] /rides

  - [x] /
  - [x] /pending
  - [x] /:id/history
  - [x] /:id/finished
  - [x] /request
  - [x] /accept
  - [x] /finish

    </details>
  </summary>

## TODO

- [x] add JWT auth
  - [x] add RBAC
- [ ] implement OAuth signin 
- [x] implement CDN service
    - [x] update cdn service to regular stateless from web stateless
- [x] implement Ride data service
  - [x] implement data validation
- [x] implement User data service
  - [x] implement data validation
- [x] add dto validation
- [x] improve error message propagation 
    - [x] better handling of AggregateException
- [x] add communication between TaxiUserData and TaxiRideData
- [x] add partition key resolver for stateful service
- [x] implement database backup for stateful service's state
- [x] move service proxy creation to Common
    - [ ] add service partition resolver 
- [x] add mailing upon user verification
- [x] update database connection to be more secure
- [ ] add password hashing (currently disabled for easier debugging)
- [ ] add CancelationTokens to repository methods
- [ ] add app settings validation
- [ ] update data service to check for edge cases
    - [ ] only users of type driver can be verified/banned
    - [ ] only users of type user can request/finish rides
    - [ ] only users of type driver can accept rides
    - [ ] check for id consistency when updating user or changing password (users can only change own data)
