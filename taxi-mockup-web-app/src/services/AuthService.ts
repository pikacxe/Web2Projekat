import axios from "axios";
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
} from "../models/User/UserModel";

const apiUrl = process.env.REACT_APP_API_URL;

const client = axios.create({
  baseURL: apiUrl + "users/",
});

// POST users/register
const register = async (payload: RegisterRequest): Promise<boolean> => {
  try {
    const res = await client.post("register", payload);
    return res.status === 204;
  } catch (err) {
    console.log(err);
    return false;
  }
};

// POST users/login
const login = async (payload: LoginRequest): Promise<AuthResponse | null> => {
  try {
    const res = await client.post("login", payload);
    if (res.status === 200) {
      return res.data as AuthResponse;
    } else {
      // handle other status codes if necessary
      console.log(`Unexpected status code: ${res.status}`);
      return null;
    }
  } catch (err) {
    console.log(err);
    return null;
  }
};

const authService = { register, login };
export default authService;
