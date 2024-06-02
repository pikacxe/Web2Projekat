import { Box } from "@mui/material";
import { ProfileView } from "../User/ProfileView";
import Grid from "@mui/material/Unstable_Grid2";

export const DriverDashboard = () => {
  return (
    <Box sx={{ height: "96vh", width: "100%" }}>
      <Grid container height="100%" flexGrow={1}>
        <Grid xs={12} sm={12} md={12} lg={4} xl={3}>
          <ProfileView />
        </Grid>
        <Grid xs={12} sm={12} md={4} lg={8} xl={9}>
          <h1>Hello from driver dashboard</h1>
        </Grid>
      </Grid>
    </Box>
  );
};
