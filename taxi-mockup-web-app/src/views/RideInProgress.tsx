import Grid from "@mui/material/Unstable_Grid2/Grid2";
import { Chat } from "../components/Chat/Chat";
import { Box } from "@mui/material";
import { RideTimer } from "../components/Ride/RideTime";
import { RideInProgressInfo } from "../models/Ride/RideModel";

export const RideInProgress: React.FC<{
  ride: RideInProgressInfo;
  closeParent: () => void;
  openFinishDialog: () => void;
}> = ({ ride, closeParent, openFinishDialog }) => {
  return (
    <Box sx={{ height: "96vh", width: "100%" }}>
      <Grid
        container
        columns={12}
        sx={{
          height: "100%",
          flexGrow: 1,
        }}
      >
        <Grid sm={6} alignContent="center">
          <RideTimer
            driverETA={ride.driverETA}
            rideDuration={ride.rideDuration}
            closeParent={closeParent}
            openFinishDialog={openFinishDialog}
          />
        </Grid>
        <Grid sm={6}>
          <Chat recipients={ride.rideId} />
        </Grid>
      </Grid>
    </Box>
  );
};
