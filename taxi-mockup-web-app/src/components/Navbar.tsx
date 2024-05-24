import { Link } from "react-router-dom";
import AppBar from "@mui/material/AppBar";
import CssBaseline from "@mui/material/CssBaseline";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";

import "./Navbar.css";
import { useAuth } from "../hooks/useAuth";
import Button from "@mui/material/Button";
import { useEffect, useState } from "react";

export const Navbar = () => {
  const [loggedIn, setLoggedIn] = useState(false);
  const context = useAuth();
  useEffect(() => {
    if (context.user) {
      setLoggedIn(true);
    } else {
      setLoggedIn(false);
    }
  }, [context?.user]);

  function handleLogout(_event: any): void {
    context?.logout();
  }

  return (
    <AppBar position="static">
      <CssBaseline />
      <Box className="nav">
        <Typography
          variant="h4"
          sx={{
            padding: "0.5rem",
          }}
        >
          Taxi mockup
        </Typography>
        {!loggedIn && (
          <Toolbar>
            <Typography variant="h5">
              <Link to="/" className="nav-link">
                Login
              </Link>
            </Typography>
            <Typography variant="h5">
              <Link to="/register" className="nav-link">
                Register
              </Link>
            </Typography>
          </Toolbar>
        )}
        {loggedIn && (
          <Button
            variant="contained"
            color="success"
            sx={{
              margin: "0.5rem",
              alignSelf: "center",
            }}
            onClick={handleLogout}
          >
            Logout
          </Button>
        )}
      </Box>
    </AppBar>
  );
};
