import { useEffect, useState } from "react";
import userService from "../../services/UserService";
import { useAuth } from "../../hooks/useAuth";
import { UserInfo } from "../../models/User/UserModel";
import { Avatar, Divider, Paper, Typography } from "@mui/material";

export const ProfileView = () => {
  const currentUser = useAuth().user;
  const [profile, setProfile] = useState<UserInfo | null>(null);
  useEffect(() => {
    if (currentUser) {
      console.log(currentUser.userId);
      userService
        .getUserById(currentUser.token, currentUser.userId)
        .then((data) => {
          setProfile(data);
        });
    } else {
      console.log("Error while getting user profile");
    }
  });
  return (
    <Paper
      className="profile"
      sx={{
        padding: "1rem",
        height: "100%",
      }}
    >
      <Avatar
        src={profile?.userPicture}
        sx={{
          width: 100,
          height: 100,
        }}
      />
      <Divider />
      <Typography variant="h5">Username: {profile?.username}</Typography>
      <Divider />
      <Typography variant="h5">User type: {profile?.userType}</Typography>
      <Divider />
      <Typography variant="h5">User state: {profile?.userState}</Typography>
      <Divider />
      <Typography variant="h5">Email: {profile?.email}</Typography>
      <Divider />
      <Typography variant="h5">Fullname: {profile?.fullName}</Typography>
      <Divider />
      <Typography variant="h5">Address</Typography>
      <Typography variant="body1">{profile?.address}</Typography>
      <Divider />
      <Typography variant="h5">Date of birth:{profile?.dateOfBirth}</Typography>
      <Divider />
    </Paper>
  );
};
