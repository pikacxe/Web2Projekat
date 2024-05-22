export interface RideStateRequest {
  id: string;
  rideState: string;
}

export interface ProposedRideRequest {
  passengerId: string;
  startDestination: string;
  endDestination: string;
  price: number;
  estRideDuraiton: number;
  driverETA: number;
}

export interface AvailableRideRespone {
  rideId: string;
  startDestination: string;
  endDestination: string;
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
}

export interface FinishRideRequest {
  rideId: string;
  passengerId: string;
  rating: number;
}
