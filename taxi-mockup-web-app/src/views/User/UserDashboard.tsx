import { Box, Divider, Paper, Typography } from "@mui/material";
import { ProfileView } from "./ProfileView";
import Grid from "@mui/material/Unstable_Grid2";
import { RequestRideForm } from "../../components/Ride/RequestRideForm";
import { RideInfo } from "../../components/Ride/RideInfo";
import { useAuth } from "../../hooks/useAuth";
import { AuthResponse } from "../../models/User/UserModel";
import { useEffect, useState } from "react";
import rideService from "../../services/RideService";
import { CompletedRideResponse } from "../../models/Ride/RideModel";
import { useAlert } from "../../hooks/useAlert";
import { useSignalR } from "../../hooks/useSignalR";
import { HubConnectionState } from "@microsoft/signalr";
import { useNavigate } from "react-router-dom";

export const UserDashboard = () => {
  const { userId, token } = useAuth().user as AuthResponse;
  const [rides, setRides] = useState<CompletedRideResponse[]>([]);
  const alert = useAlert();
  const { connection } = useSignalR();
  const navigate = useNavigate();

  useEffect(() => {
    rideService
      .getCompletedRidesForUser(token, userId)
      .then((res) => {
        setRides(res);
      })
      .catch((err) => {
        alert.showAlert(err.message, "error");
        console.log(err);
      });
  }, [token, userId, alert]);
  if (connection?.state === HubConnectionState.Disconnected) {
    connection
      ?.start()
      .then(() => {
        console.log("Connected now");
        connection?.on("rideAccepted", (data) => {
          console.log(data);
          navigate("/ride-in-progress", { state: data });
        });
      })
      .catch((err) => {
        alert.showAlert("Could now connect to server", "warning");
      });
  }
  return (
    <Box sx={{ height: "96vh", width: "100%" }}>
      <Grid container height="100%" flexGrow={1}>
        <Grid xs={12} sm={12} md={12} lg={4} xl={3}>
          <ProfileView />
        </Grid>
        <Grid xs={12} sm={12} md={12} lg={8} xl={9} height="100%">
          <Paper variant="outlined" sx={{ padding: "2rem", height: "20%" }}>
            <RequestRideForm />
          </Paper>
          <Paper variant="outlined" sx={{ padding: "2rem", height: "80%" }}>
            <Typography variant="h4">Completed rides</Typography>
            <Divider />
            {rides.length > 0 ? (
              <RideInfo rides={rides} />
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
