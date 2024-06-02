export interface UserInfo {
  id: string;
  username: string;
  email: string;
  fullname: string;
  dateOfBirth: string;
  address: string;
  userType: string;
  userPicture: string;
  userState: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  userId: string;
  username: string;
  userRole: string;
  token: string;
  profileImage: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  fullname: string;
  dateOfBirth: string;
  address: string;
  userType: string;
  userPicture: string;
}

export interface UpdateUserRequest {
  username?: string;
  email?: string;
  fullname?: string;
  dateOfBirth?: string;
  address?: string;
  userPicture?: string;
}

export interface UserStateResponse {
  id: string;
  userState: string;
}

export interface UserPasswordChangeRequest {
  id: string;
  oldPassword: string;
  newPassword: string;
}
