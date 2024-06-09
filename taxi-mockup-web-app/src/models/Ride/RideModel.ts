export interface RideStateRequest {
  id: string;
  rideState: string;
}

export interface ProposedRideRequest {
  passengerId: string;
  startDestination: string;
  endDestination: string;
  price: number;
  estRideDuration: number;
  driverETA: number;
  passengerName: string;
  connectionId: string;
}

export interface AvailableRideRespone {
  rideId: string;
  passengerId: string;
  startDestination: string;
  endDestination: string;
  passengerName: string;
}

export interface RideInProgressInfo {
  rideId: string;
  passengerId: string;
  driverETA: number;
  rideDuration: number;
}

export interface CompletedRideResponse {
  rideId: string;
  passengerId: string;
  driverId: string;
  startDestination: string;
  endDestination: string;
  price: string;
  rideDuration: string;
  rating: string;
}

export interface AcceptRideRequest {
  rideId: string;
  driverId: string;
  driverETA: number;
  connectionId: string;
}

export interface FinishRideRequest {
  rideId: string;
  passengerId: string;
  rating: number;
}
