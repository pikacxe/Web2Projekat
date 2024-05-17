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
- CDN service (User images) - web statless
- Mail service - stateless (TBD)

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

- [x] add JWT auth / Identity service
  - [x] add RBAC
- [x] implement CDN service
- [x] implement Ride data service
  - [ ] implement data validation
- [x] implement User data service
  - [ ] implement data validation
- [x] add dto validation
- [x] improve error message propagation
- [ ] refactor service proxy creation
- [ ] add partition key resolver for stateful service
- [ ] implement database backup for stateful service's state
- [ ] update cdn service to regular stateless from web stateless ?? (user mail service as regular stateless)
- [ ] add mail service
