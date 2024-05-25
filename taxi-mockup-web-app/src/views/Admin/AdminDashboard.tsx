import Grid from "@mui/material/Unstable_Grid2";
import { ProfileView } from "../User/ProfileView";
import { Box, Typography, Paper } from "@mui/material";
import { UserDataGrid } from "../../components/User/UserDataGrid";
import { useEffect, useState } from "react";
import { UserInfo } from "../../models/User/UserModel";
import userService from "../../services/UserService";
import { useAuth } from "../../hooks/useAuth";
import { useAlert } from "../../hooks/useAlert";

export const AdminDashboard = () => {
  const alert = useAlert();
  const [users, setUsers] = useState<UserInfo[]>([]);
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
        alert.showAlert("Error occured while getting users");
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
      });
  }, [token]);

  return (
    <Box sx={{ height: "96vh", width: "100%" }}>
      <Grid container height="100%" flexGrow={1}>
        <Grid xs={12} sm={12} md={12} lg={4} xl={3}>
          <ProfileView />
        </Grid>
        <Grid xs={12} sm={12} md={12} lg={8} xl={9}>
          <Box>
            <Paper variant="outlined" sx={{ padding: "1rem" }}>
              <Typography variant="h3">
                Drivers currently awaiting verification
              </Typography>
              {usersUnverified.length > 0 ? (
                <UserDataGrid
                  users={usersUnverified}
                  actionsType="verify|ban"
                />
              ) : (
                <Typography>
                  No unverified drivers curently in system
                </Typography>
              )}
            </Paper>
            <Paper variant="outlined" sx={{ padding: "1rem" }}>
              <Typography variant="h3">Users currently in system</Typography>
              {users.length > 0 ? (
                <UserDataGrid users={users} actionsType="delete" />
              ) : (
                <Typography>"No users curently in system</Typography>
              )}
            </Paper>
          </Box>
        </Grid>
      </Grid>
    </Box>
  );
};
