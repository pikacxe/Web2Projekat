import axios, { AxiosError } from "axios";
import {
  UpdateUserRequest,
  UserPasswordChangeRequest,
  UserStateResponse,
} from "../models/User/UserModel";
import { UserInfo } from "../models/User/UserModel";

const apiUrl = process.env.REACT_APP_API_URL;

const client = axios.create({
  baseURL: apiUrl + "users/",
});

// GET /users/
const getAllUsers = async (token: string): Promise<Array<UserInfo>> => {
  const headers = { Authorization: `Bearer ${token}` };
  const res = await client.get("", { headers });
  if (res.status === 200) {
    return res.data as Array<UserInfo>;
  } else {
    return [];
  }
};

// GET /user/:id
const getUserById = async (token: string, id: string): Promise<UserInfo> => {
  const headers = { Authorization: `Bearer ${token}` };
  const res = await client.get(id, { headers });
  if (res.status === 200) {
    return res.data as UserInfo;
  }
  throw new AxiosError(res.statusText, "500");
};

// GET /users/unverified
const getUnverifiedUsers = async (token: string): Promise<Array<UserInfo>> => {
  const headers = { Authorization: `Bearer ${token}` };
  const res = await client.get("unverified", { headers });
  if (res.status === 200) {
    return res.data as Array<UserInfo>;
  } else {
    return [];
  }
};

// GET /users/:id/state
const getUserState = async (
  token: string,
  id: string
): Promise<UserStateResponse> => {
  const headers = { Authorization: `Bearer ${token}` };
  const res = await client.get(`${id}/state`, { headers });
  if (res.status === 200) {
    return res.data as UserStateResponse;
  }
  throw new AxiosError(res.statusText, "500");
};

// PATCH /users/:id/update
const updateUser = async (
  token: string,
  id: string,
  payload: UpdateUserRequest
): Promise<boolean> => {
  const headers = { Authorization: `Bearer ${token}` };
  const res = await client.patch(`${id}/update`, payload, { headers });
  return res.status === 204;
};

// PATCH /users/:id/change-password
const changeUserPassword = async (
  token: string,
  id: string,
  payload: UserPasswordChangeRequest
): Promise<boolean> => {
  const headers = { Authorization: `Bearer ${token}` };
  const res = await client.patch(`${id}/change-password`, payload, {
    headers,
  });
  return res.status === 204;
};

// PATCH /users/:id/verify
const verifyDriver = async (token: string, id: string): Promise<boolean> => {
  const headers = { Authorization: `Bearer ${token}` };
  const res = await client.patch(`${id}/verify`, {}, { headers });
  return res.status === 204;
};

// PATCH /users/:id/verify
const banDriver = async (token: string, id: string): Promise<boolean> => {
  const headers = { Authorization: `Bearer ${token}` };
  const res = await client.patch(`${id}/ban`, {}, { headers });
  return res.status === 204;
};

// DELETE /users/:id
const deleteUser = async (token: string, id: string): Promise<boolean> => {
  const headers = { Authorization: `Bearer ${token}` };
  const res = await client.delete(id, { headers });
  return res.status === 204;
};

const userService = {
  getAllUsers,
  getUnverifiedUsers,
  getUserById,
  getUserState,
  updateUser,
  changeUserPassword,
  verifyDriver,
  banDriver,
  deleteUser,
};
export default userService;
