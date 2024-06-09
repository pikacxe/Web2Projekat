import {
  Button,
  Divider,
  FormControl,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { ChangeEvent, FormEvent, useState } from "react";
import userService from "../../services/UserService";
import { useAuth } from "../../hooks/useAuth";
import { useNavigate } from "react-router-dom";
import {
  AuthResponse,
  UserPasswordChangeRequest,
} from "../../models/User/UserModel";
import { useAlert } from "../../hooks/useAlert";
import { AxiosError } from "axios";

import "./ChangePasswordForm.css";

export const ChangePasswordForm = () => {
  const [oldPassword, setOldPassword] = useState<string>("");
  const [newPassword, setNewPassword] = useState<string>("");
  const user = useAuth().user as AuthResponse;
  const alert = useAlert();
  const navigate = useNavigate();

  function handleSubmit(event: FormEvent<HTMLFormElement>): void {
    event.preventDefault();
    if (oldPassword !== "" && newPassword !== "") {
      const payload: UserPasswordChangeRequest = {
        userId: user.userId,
        newPassword: newPassword,
        oldPassword: oldPassword,
      };
      userService
        .changeUserPassword(user.token, user.userId, payload)
        .then((res) => {
          if (res) {
            alert.showAlert("Password changed successfully", "success");
            navigate("..", { relative: "path" });
          } else {
            alert.showAlert("Error while changing password.", "error");
          }
        })
        .catch((err) => {
          if (err instanceof AxiosError) {
            alert.showAlert(err.response?.data, "error");
          } else {
            alert.showAlert(err.message, "error");
          }
        });
    } else {
      alert.showAlert(
        "Please provide old/new password before procceding",
        "warning"
      );
    }
  }

  function handleOldPassword(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void {
    setOldPassword(event.target.value);
  }
  function handleNewPassword(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void {
    setNewPassword(event.target.value);
  }

  function handleBack(event: any): void {
    switch (user.userRole) {
      case "Driver":
        navigate("/driver-dashboard");
        break;
      case "User":
        navigate("/dashboard");
        break;
      case "Admin":
        navigate("/admin-dashboard");
        break;
    }
  }

  return (
    <form onSubmit={handleSubmit} className="changePasswordForm">
      <Typography variant="h3">Change password</Typography>
      <Divider />
      <FormControl>
        <TextField
          sx={{
            marginY: "1rem",
          }}
          value={oldPassword}
          placeholder="Old password"
          onChange={handleOldPassword}
        />
      </FormControl>
      <FormControl>
        <TextField
          sx={{
            marginBottom: "1rem",
          }}
          value={newPassword}
          placeholder="New password"
          onChange={handleNewPassword}
        />
      </FormControl>
      <Stack direction="row" gap={2} justifyContent="space-between">
        <Button variant="contained" onClick={handleBack}>
          Cancel
        </Button>
        <Button variant="contained" type="submit" color="success">
          Confirm
        </Button>
      </Stack>
    </form>
  );
};
