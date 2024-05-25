import { useEffect, useState } from "react";
import userService from "../../services/UserService";
import { useAuth } from "../../hooks/useAuth";
import { UserInfo } from "../../models/User/UserModel";
import { Avatar, Box, Divider, Paper, Typography } from "@mui/material";

import "./ProfileView.css";

export const ProfileView = () => {
  const currentUser = useAuth().user;
  const [profile, setProfile] = useState<UserInfo | null>(null);
  useEffect(() => {
    if (currentUser) {
      userService
        .getUserById(currentUser.token, currentUser.userId)
        .then((data) => {
          setProfile(data);
        });
    } else {
      console.log("Error while getting user profile");
    }
  }, [currentUser]);
  return (
    <Paper
      className="profile"
      sx={{
        padding: "1rem",
        display: "flex",
        flexDirection: "column",
        justifyContent: "flex-start",
        alignContent: "space-evenly",
      }}
    >
      <Box alignContent="center" justifyContent="center" display="flex">
        <Avatar
          src={profile?.userPicture}
          sx={{
            width: 120,
            height: 120,
            marginBottom: "1.5rem",
          }}
        />
      </Box>
      <Box>
        <Typography textAlign="center" variant="h4">
          {profile?.username}
        </Typography>
        <Divider />
        <Typography className="profileLabel" variant="h4">
          Email
        </Typography>
        <Typography className="profileInfo" variant="h6">
          {profile?.email}
        </Typography>
        <Divider />
        <Typography className="profileLabel" variant="h4">
          Fullname
        </Typography>
        <Typography className="profileInfo" variant="h5">
          {profile?.fullname}
        </Typography>
        <Divider />
        <Typography className="profileLabel" variant="h4">
          Address
        </Typography>
        <Typography className="profileInfo" variant="body1">
          {profile?.address}
        </Typography>
        <Divider />
        <Typography className="profileLabel" variant="h4">
          Date of birth
        </Typography>
        <Typography className="profileInfo" variant="h5">
          {profile?.dateOfBirth}
        </Typography>
        <Divider />
      </Box>
    </Paper>
  );
};
