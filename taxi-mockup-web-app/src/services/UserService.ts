import axios from "axios";
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
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.get("", { headers });
    if (res.status === 200) {
      return res.data as Array<UserInfo>;
    } else {
      return [];
    }
  } catch (err) {
    console.log(err);
    return [];
  }
};

// GET /user/:id
const getUserById = async (
  token: string,
  id: string
): Promise<UserInfo | null> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.get(id, { headers });
    if (res.status === 200) {
      return res.data as UserInfo;
    }
    return null;
  } catch (err) {
    console.log(err);
    return null;
  }
};

// GET /users/unverified
const getUnverifiedUsers = async (token: string): Promise<Array<UserInfo>> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.get("unverified", { headers });
    if (res.status === 200) {
      return res.data as Array<UserInfo>;
    } else {
      return [];
    }
  } catch (err) {
    console.log(err);
    return [];
  }
};

// GET /users/:id/state
const getUserState = async (
  token: string,
  id: string
): Promise<UserStateResponse | null> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.get(`${id}/state`, { headers });
    if (res.status === 200) {
      return res.data as UserStateResponse;
    } else {
      return null;
    }
  } catch (err) {
    console.log(err);
    return null;
  }
};

// PUT /users/:id/update
const updateUser = async (
  token: string,
  id: string,
  payload: UpdateUserRequest
): Promise<boolean> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.put(`${id}/update`, payload, { headers });
    return res.status === 204;
  } catch (err) {
    console.log(err);
    return false;
  }
};

// PATCH /users/:id/change-password
const changeUserPassword = async (
  token: string,
  id: string,
  payload: UserPasswordChangeRequest
): Promise<boolean> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.patch(`${id}/change-password`, payload, {
      headers,
    });
    return res.status === 204;
  } catch (err) {
    console.log(err);
    return false;
  }
};

// PATCH /users/:id/verify
const verifyDriver = async (token: string, id: string): Promise<boolean> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.patch(`${id}/verify`, { headers });
    return res.status === 204;
  } catch (err) {
    console.log(err);
    return false;
  }
};

// PATCH /users/:id/verify
const banDriver = async (token: string, id: string): Promise<boolean> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.patch(`${id}/ban`, { headers });
    return res.status === 204;
  } catch (err) {
    console.log(err);
    return false;
  }
};

// DELETE /users/:id
const deleteUser = async (token: string, id: string): Promise<boolean> => {
  try {
    const headers = { Authorization: `Bearer ${token}` };
    const res = await client.delete(id, { headers });
    return res.status === 204;
  } catch (err) {
    console.log(err);
    return false;
  }
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
