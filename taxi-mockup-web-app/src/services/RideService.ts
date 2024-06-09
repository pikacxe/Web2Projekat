import axios from "axios";
import {
  AcceptRideRequest,
  AvailableRideRespone,
  CompletedRideResponse,
  FinishRideRequest,
  ProposedRideRequest,
} from "../models/Ride/RideModel";

const apiUrl = process.env.REACT_APP_API_URL;

const client = axios.create({
  baseURL: apiUrl + "rides/",
});

// GET /rides/
const getAllRides = async (
  token: string
): Promise<Array<CompletedRideResponse>> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.get("", { headers });
    if (res.status === 200) {
      return res.data as Array<CompletedRideResponse>;
    } else {
      return [];
    }
  } catch (err) {
    console.log(err);
    return [];
  }
};

// GET /rides/pending
const getAllPending = async (
  token: string
): Promise<Array<AvailableRideRespone>> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.get("pending", { headers });
    if (res.status === 200) {
      return res.data as Array<AvailableRideRespone>;
    } else {
      return [];
    }
  } catch (err) {
    console.log(err);
    return [];
  }
};

// GET /rides/:id/history
const getCompletedRidesForUser = async (
  token: string,
  id: string
): Promise<Array<CompletedRideResponse>> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.get(`${id}/history`, { headers });
    if (res.status === 200) {
      return res.data as Array<CompletedRideResponse>;
    } else {
      return [];
    }
  } catch (err) {
    console.log(err);
    return [];
  }
};
// GET /rides/:id/finished
const getCompletedForDriver = async (
  token: string,
  id: string
): Promise<Array<CompletedRideResponse>> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.get(`${id}/finished`, { headers });
    if (res.status === 200) {
      return res.data as Array<CompletedRideResponse>;
    } else {
      return [];
    }
  } catch (err) {
    console.log(err);
    return [];
  }
};

// POST /rides/request
const requestRide = async (
  token: string,
  payload: ProposedRideRequest
): Promise<boolean> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.post("request", payload, { headers });
    return res.status === 201;
  } catch (err) {
    console.log(err);
    return false;
  }
};

// PATCH /rides/accept
const acceptPendingRide = async (
  token: string,
  payload: AcceptRideRequest
): Promise<boolean> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.patch("accept", payload, { headers });
    return res.status === 204;
  } catch (err) {
    console.log(err);
    return false;
  }
};

// PATCH /rides/finish
const finishActiveRide = async (
  token: string,
  payload: FinishRideRequest
): Promise<boolean> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.patch("finish", payload, { headers });
    return res.status === 204;
  } catch (err) {
    console.log(err);
    return false;
  }
};

const rideService = {
  getAllRides,
  getAllPending,
  getCompletedRidesForUser,
  getCompletedForDriver,
  requestRide,
  acceptPendingRide,
  finishActiveRide,
};
export default rideService;
