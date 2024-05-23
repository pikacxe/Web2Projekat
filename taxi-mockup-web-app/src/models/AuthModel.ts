import { AuthResponse } from "./User/UserModel";

export type AuthContext = {
  user: AuthResponse;
  login: (user: AuthResponse) => void;
  logout: () => void;
};
