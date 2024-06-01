import "./LoginForm.css";
import { ChangeEvent, FormEvent, useState } from "react";
import { useAuth } from "../hooks/useAuth";
import { LoginRequest } from "../models/User/UserModel";
import {
  Button,
  Divider,
  FormControl,
  FormLabel,
  TextField,
  Typography,
} from "@mui/material";
import LoginIcon from "@mui/icons-material/Login";
import authService from "../services/AuthService";
import { useAlert } from "../hooks/useAlert";

export const LoginForm = () => {
  const [emailField, setEmailField] = useState("");
  const [passwordField, setPasswordField] = useState("");
  const [emailError, setEmailError] = useState(false);
  const [passwordError, setPasswordError] = useState(false);
  const alert = useAlert();

  const { login } = useAuth();

  function handleSubmit(event: FormEvent<HTMLFormElement>): void {
    event.preventDefault();
    setEmailError(false);
    setPasswordError(false);

    if (emailField === "") {
      setEmailError(true);
    }
    if (passwordField === "") {
      setPasswordError(true);
    }

    if (emailField && passwordField) {
      let loginReq: LoginRequest = {
        email: emailField,
        password: passwordField,
      };
      authService
        .login(loginReq)
        .then((res) => {
          if (res) {
            login(res);
          } else {
            alert.showAlert("Invalid login data. Please try again", "error");
          }
        })
        .catch((err) => {
          console.log(err);
          alert.showAlert(
            "Error occured while proccessing your request",
            "error"
          );
        });
    }
  }

  function handleEmailChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void {
    setEmailField(event.target.value);
  }

  function handlePasswordChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void {
    setPasswordField(event.target.value);
  }

  return (
    <form className="loginForm" onSubmit={handleSubmit}>
      <Typography variant="h2" textAlign="center">
        Login
      </Typography>
      <Divider variant="middle" />
      <br />
      <FormControl>
        <FormLabel>Email</FormLabel>
        <TextField
          type="email"
          variant="outlined"
          placeholder="Email"
          error={emailError}
          value={emailField}
          onChange={handleEmailChange}
        />
      </FormControl>
      <br />
      <FormControl>
        <FormLabel>Password</FormLabel>
        <TextField
          type="password"
          variant="outlined"
          placeholder="Password"
          error={passwordError}
          value={passwordField}
          onChange={handlePasswordChange}
        />
      </FormControl>
      <br />
      <Button
        type="submit"
        variant="contained"
        color="primary"
        size="medium"
        endIcon={<LoginIcon />}
      >
        Login
      </Button>
    </form>
  );
};
