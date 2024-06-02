import { Box, Grid } from "@mui/material";
import { ProfileView } from "./ProfileView";

export const UserDashboard = () => {
  return (
    <Box sx={{ height: "96vh", width: "100%" }}>
      <Grid container height="100%" flexGrow={1}>
        <Grid item xs={12} sm={12} md={12} lg={4} xl={3}>
          <ProfileView />
        </Grid>
        <Grid item xs={12} sm={12} md={12} lg={8} xl={9}>
          <h1>Hello from user dashboard</h1>
        </Grid>
      </Grid>
    </Box>
  );
};
