import { Box, Divider, Paper, Typography } from "@mui/material";
import { ProfileView } from "../User/ProfileView";
import Grid from "@mui/material/Unstable_Grid2";
import { useEffect, useState } from "react";
import {
  AvailableRideRespone,
  CompletedRideResponse,
} from "../../models/Ride/RideModel";
import { useAuth } from "../../hooks/useAuth";
import { AuthResponse } from "../../models/User/UserModel";
import { useAlert } from "../../hooks/useAlert";
import rideService from "../../services/RideService";
import { RideInfo } from "../../components/Ride/RideInfo";
import { AvailableRidesList } from "../../components/Ride/AvailableRidesList";

export const DriverDashboard = () => {
  const [availableRides, setAvailableRides] = useState<AvailableRideRespone[]>(
    []
  );
  const [completedRides, setCompletedRides] = useState<CompletedRideResponse[]>(
    []
  );
  const { token, userId } = useAuth().user as AuthResponse;
  const alert = useAlert();

  useEffect(() => {
    rideService
      .getAllPending(token)
      .then((res) => {
        setAvailableRides(res);
      })
      .catch((err) => {
        alert.showAlert(err.message, "error");
        console.log(err);
      });
  }, [token, userId, alert]);
  useEffect(() => {
    rideService
      .getCompletedForDriver(token, userId)
      .then((res) => {
        setCompletedRides(res);
      })
      .catch((err) => {
        alert.showAlert(err.message, "error");
        console.log(err);
      });
  }, [token, userId, alert]);
  return (
    <Box sx={{ height: "96vh", width: "100%" }}>
      <Grid container height="100%" flexGrow={1}>
        <Grid xs={12} sm={12} md={12} lg={4} xl={3}>
          <ProfileView />
        </Grid>
        <Grid xs={12} sm={12} md={12} lg={8} xl={9}>
          <Paper variant="outlined" sx={{ padding: "2rem", height: "50%" }}>
            <Typography variant="h4">Available rides</Typography>
            <Divider />
            {availableRides.length > 0 ? (
              <AvailableRidesList rides={availableRides} />
            ) : (
              <Typography variant="h6" marginY="1rem">
                Currently no pending rides in system
              </Typography>
            )}
          </Paper>
          <Paper variant="outlined" sx={{ padding: "2rem", height: "50%" }}>
            <Typography variant="h4">Completed rides</Typography>
            <Divider />
            {completedRides.length > 0 ? (
              <RideInfo rides={completedRides} />
            ) : (
              <Typography variant="h6" marginY="1rem">
                You have no completed rides
              </Typography>
            )}
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};
