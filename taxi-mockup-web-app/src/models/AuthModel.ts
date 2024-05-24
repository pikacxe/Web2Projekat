import { AuthResponse } from "./User/UserModel";

export enum ROLE {
  User = "User",
  Driver = "Driver",
  Admin = "Admin",
}

export type AuthContext = {
  user?: AuthResponse;
  login: (user: AuthResponse) => void;
  logout: () => void;
};
