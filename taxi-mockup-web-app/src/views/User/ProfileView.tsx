import { MouseEvent, useEffect, useState } from "react";
import userService from "../../services/UserService";
import { useAuth } from "../../hooks/useAuth";
import { UserInfo } from "../../models/User/UserModel";
import { Avatar, Box, Divider, Button, Paper, Typography } from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";

import "./ProfileView.css";
import { useLocation, useNavigate } from "react-router-dom";

export const ProfileView = () => {
  const currentUser = useAuth().user;
  const [profile, setProfile] = useState<UserInfo | null>(null);
  const location = useLocation();
  const navigate = useNavigate();

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

  function handleEditProfile(event: MouseEvent<HTMLButtonElement>): void {
    navigate(`${location.pathname}/${currentUser?.userId}`);
  }

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
      <Box
        alignContent="center"
        justifyContent="center"
        alignItems="center"
        display="flex"
        flexDirection="column"
        marginBottom="1rem"
      >
        <Avatar
          src={profile?.userPicture}
          sx={{
            width: 120,
            height: 120,
            border: "1px solid black",
          }}
        />
      </Box>
      <Box>
        <Typography textAlign="center" variant="h5">
          {profile?.username}
        </Typography>
        <Divider />
        <Typography className="profileLabel" variant="h5">
          Email
        </Typography>
        <Typography className="profileInfo" variant="h6">
          {profile?.email}
        </Typography>
        <Divider />
        <Typography className="profileLabel" variant="h5">
          Fullname
        </Typography>
        <Typography className="profileInfo" variant="h5">
          {profile?.fullname}
        </Typography>
        <Divider />
        <Typography className="profileLabel" variant="h5">
          Address
        </Typography>
        <Typography className="profileInfo" variant="body1">
          {profile?.address}
        </Typography>
        <Divider />
        <Typography className="profileLabel" variant="h5">
          Date of birth
        </Typography>
        <Typography className="profileInfo" variant="h5">
          {profile?.dateOfBirth}
        </Typography>
        <Divider />
        <Button
          sx={{
            marginY: "1rem",
          }}
          variant="contained"
          startIcon={<EditIcon />}
          onClick={handleEditProfile}
        >
          Edit profile
        </Button>
      </Box>
    </Paper>
  );
};
