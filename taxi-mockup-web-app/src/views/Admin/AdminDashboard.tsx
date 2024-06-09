import Grid from "@mui/material/Unstable_Grid2";
import { ProfileView } from "../User/ProfileView";
import { Box, Typography, Paper, Divider } from "@mui/material";
import { UserDataGrid } from "../../components/User/UserDataGrid";
import { useEffect, useState } from "react";
import { UserInfo } from "../../models/User/UserModel";
import userService from "../../services/UserService";
import { useAuth } from "../../hooks/useAuth";
import { useAlert } from "../../hooks/useAlert";
import { RideInfo } from "../../models/Ride/RideModel";
import rideService from "../../services/RideService";
import { FullRideInfoList } from "../../components/Ride/FullRideInfoList";

export const AdminDashboard = () => {
  const alert = useAlert();
  const [users, setUsers] = useState<UserInfo[]>([]);
  const [rides, setRides] = useState<RideInfo[]>([]);
  const [usersUnverified, setUnverifiedUsers] = useState<UserInfo[]>([]);
  const token = useAuth().user?.token as string;
  useEffect(() => {
    userService
      .getAllUsers(token)
      .then((data) => {
        setUsers(data);
        console.log(data);
      })
      .catch((err) => {
        console.log(err);
        alert.showAlert("Error occured while getting users", "error");
      });
  }, [token, alert]);

  useEffect(() => {
    userService
      .getUnverifiedUsers(token)
      .then((data) => {
        setUnverifiedUsers(data);
        console.log(data);
      })
      .catch((err) => {
        console.log(err);
        alert.showAlert(
          "Error occured while getting unverified users",
          "error"
        );
      });
  }, [token, alert]);

  useEffect(() => {
    rideService
      .getAllRides(token)
      .then((res) => {
        setRides(res);
      })
      .catch((err) => {
        console.log(err);
        alert.showAlert("Error occured while getting rides", "error");
      });
  }, [alert, token]);

  return (
    <Box sx={{ height: "96vh", width: "100%" }}>
      <Grid container height="100%" flexGrow={1}>
        <Grid xs={12} sm={12} md={12} lg={4} xl={3}>
          <ProfileView />
        </Grid>
        <Grid xs={12} sm={12} md={12} lg={8} xl={9}>
          <Paper variant="outlined" sx={{ padding: "2rem", height: "50%" }}>
            <Typography variant="h4">
              Drivers currently awaiting verification
            </Typography>
            <Divider />
            {usersUnverified.length > 0 ? (
              <UserDataGrid users={usersUnverified} actionsType="verify|ban" />
            ) : (
              <Typography variant="h6" marginY="1rem">
                No unverified drivers curently in system
              </Typography>
            )}
          </Paper>
          <Paper variant="outlined" sx={{ padding: "2rem", height: "50%" }}>
            <Typography variant="h4">Rides currently in system</Typography>
            <Divider />
            {users.length > 0 ? (
              <FullRideInfoList rides={rides} />
            ) : (
              <Typography variant="h6" marginY="1rem">
                No rides curently in system
              </Typography>
            )}
          </Paper>
          <Paper variant="outlined" sx={{ padding: "2rem", height: "50%" }}>
            <Typography variant="h4">Users currently in system</Typography>
            <Divider />
            {users.length > 0 ? (
              <UserDataGrid users={users} actionsType="delete" />
            ) : (
              <Typography variant="h6" marginY="1rem">
                No users curently in system
              </Typography>
            )}
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};
