import Grid from "@mui/material/Unstable_Grid2";
import { ProfileView } from "../User/ProfileView";

export const AdminDashboard = () => {
  return (
    <Grid container width="100%" height="800px">
      <Grid xs={5} sm={5} md={4} lg={3} xl={2}>
        <ProfileView />
      </Grid>
      <Grid xs={9}>
        <h1>Hello from admin dashboard</h1>
      </Grid>
    </Grid>
  );
};
