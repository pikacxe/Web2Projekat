import {
  Avatar,
  Box,
  Button,
  Divider,
  FormControl,
  FormLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { ChangeEvent, MouseEvent, FormEvent, useState, ReactNode } from "react";
import authService from "../services/AuthService";
import { useNavigate } from "react-router-dom";
import { RegisterRequest } from "../models/User/UserModel";
import { AxiosError } from "axios";
import { useAlert } from "../hooks/useAlert";

import imageService from "../services/ImageService";

export const RegisterForm = () => {
  const alert = useAlert();
  const navigate = useNavigate();
  const [selectedImage, setSelectedImage] = useState<File | null>(null);
  const [confirmPassword, setConfirmPassword] = useState<string>("");
  const [errorPassword, setErrorPassword] = useState<boolean>(false);
  const [currentUser, setCurrentUser] = useState<RegisterRequest>({
    username: "",
    password: "",
    userType: "",
    email: "",
    fullname: "",
    dateOfBirth: "",
    address: "",
    userPicture: "",
  });

  function handleChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void {
    const { name, value } = event.target;
    updateUserProperty(name, value);
  }

  function updateUserProperty(name: string, value: string) {
    setCurrentUser((prevState) => ({
      ...prevState,
      [name]: value,
    }));
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>): void {
    event.preventDefault();
    if (currentUser.password !== confirmPassword) {
      setErrorPassword(true);
      return;
    }
    currentUser.dateOfBirth = new Date(currentUser.dateOfBirth).toISOString();
    authService
      .register(currentUser)
      .then((res) => {
        if (res) {
          alert.showAlert("Registered successfully", "success");
          navigate("..", { relative: "path" });
        } else {
          alert.showAlert("Error while proccesing your registration", "error");
        }
      })
      .catch((err) => {
        if (err.type instanceof AxiosError) {
          alert.showAlert(err.response.data, "error");
        } else {
          alert.showAlert(err.message, "error");
        }
      });
  }

  function handleFileChange(event: ChangeEvent<HTMLInputElement>): void {
    const file = event.target.files?.[0] || null;
    setSelectedImage(file);
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        alert.showAlert("Profile picture selected", "info");
      };
      reader.readAsDataURL(file);
    } else {
      alert.showAlert("Error while selecting profile picture", "warning");
    }
  }

  const handleImageUpload = (event: MouseEvent<HTMLButtonElement>) => {
    if (selectedImage) {
      const formData = new FormData();
      formData.append("file", selectedImage);
      imageService
        .uploadImage(formData)
        .then((res) => {
          updateUserProperty("userPicture", res);
          alert.showAlert("Picture uploaded successfully", "success");
        })
        .catch((err) => {
          if (err.type instanceof AxiosError) {
            alert.showAlert(err.response.data, "error");
          } else {
            alert.showAlert(err.message, "error");
          }
        });
    }
  };

  function handleBack(event: MouseEvent<HTMLButtonElement>): void {
    navigate("/");
  }

  function handlePasswordCheck(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void {
    setConfirmPassword(event.target.value);
    setErrorPassword(currentUser.password === confirmPassword);
  }

  function handleSelectChange(
    event: SelectChangeEvent<string>,
    child: ReactNode
  ): void {
    console.log(event.target.value);
    updateUserProperty("userType", event.target.value);
  }

  return (
    <form onSubmit={handleSubmit} className="userForm">
      <Typography variant="h4" textAlign="center">
        Register
      </Typography>
      <Divider variant="middle" />
      <FormControl>
        <TextField
          sx={{
            marginY: "1rem",
          }}
          InputLabelProps={{
            shrink: true,
          }}
          label="Username"
          name="username"
          value={currentUser.username}
          onChange={handleChange}
          required
        />
      </FormControl>
      <FormControl>
        <TextField
          sx={{
            marginBottom: "1rem",
          }}
          InputLabelProps={{
            shrink: true,
          }}
          type="password"
          label="Password"
          name="password"
          value={currentUser.password}
          onChange={handleChange}
          error={errorPassword}
          helperText={errorPassword && "Passwords do not match"}
          required
        />
      </FormControl>
      <FormControl>
        <TextField
          sx={{
            marginBottom: "1rem",
          }}
          InputLabelProps={{
            shrink: true,
          }}
          type="password"
          label="Confirm password"
          value={confirmPassword}
          error={errorPassword}
          helperText={errorPassword && "Passwords do not match"}
          onChange={handlePasswordCheck}
          required
        />
      </FormControl>
      <FormControl>
        <TextField
          sx={{
            marginBottom: "1rem",
          }}
          InputLabelProps={{
            shrink: true,
          }}
          label="Email"
          name="email"
          value={currentUser.email}
          type="email"
          onChange={handleChange}
          required
        />
      </FormControl>
      <FormControl>
        <TextField
          sx={{
            marginBottom: "1rem",
          }}
          InputLabelProps={{
            shrink: true,
          }}
          label="Full name"
          name="fullname"
          onChange={handleChange}
          value={currentUser.fullname}
          required
        />
      </FormControl>
      <FormControl>
        <TextField
          sx={{
            marginBottom: "1rem",
          }}
          InputLabelProps={{
            shrink: true,
          }}
          label="Date of birth"
          type="date"
          name="dateOfBirth"
          value={
            currentUser.dateOfBirth
              ? new Date(currentUser.dateOfBirth).toISOString().split("T")[0]
              : new Date(1800, 1, 1)
          }
          onChange={handleChange}
          required
        />
      </FormControl>
      <FormControl>
        <TextField
          sx={{
            marginBottom: "1rem",
          }}
          InputLabelProps={{
            shrink: true,
          }}
          label="Address"
          name="address"
          value={currentUser.address}
          onChange={handleChange}
          required
        />
      </FormControl>
      <FormControl>
        <FormLabel htmlFor="userTypeSelect">User type</FormLabel>
        <Select value={currentUser.userType} onChange={handleSelectChange}>
          <MenuItem key={0} value={"User"}>
            User
          </MenuItem>
          <MenuItem key={1} value={"Driver"}>
            Driver
          </MenuItem>
        </Select>
      </FormControl>
      <Stack
        direction="row"
        alignItems="end"
        gap={2}
        justifyContent="space-between"
      >
        <FormControl>
          <FormLabel htmlFor="userPicture">User picture</FormLabel>
          <TextField
            id="userPicture"
            sx={{
              marginBottom: "1rem",
            }}
            type="file"
            onChange={handleFileChange}
            required
          />
        </FormControl>
        <Button
          sx={{
            height: "55px",
            marginBottom: "1rem",
          }}
          variant="contained"
          onClick={handleImageUpload}
          color="success"
          disabled={!selectedImage}
        >
          Upload image
        </Button>
      </Stack>
      <Stack direction="row" justifyContent="space-between" marginTop="1rem">
        <Button
          variant="contained"
          size="large"
          color="primary"
          sx={{
            maxHeight: "50px",
          }}
          onClick={handleBack}
        >
          Cancel
        </Button>
        {currentUser.userPicture && (
          <Box
            alignContent="center"
            justifyContent="center"
            alignItems="center"
            display="flex"
            flexDirection="column"
            marginY="1rem"
          >
            <Avatar
              src={currentUser.userPicture}
              sx={{
                width: 120,
                height: 120,
                border: "1px solid black",
              }}
            />
          </Box>
        )}
        <Button
          type="submit"
          variant="contained"
          size="large"
          sx={{
            maxHeight: "50px",
          }}
          color="success"
        >
          Register
        </Button>
      </Stack>
    </form>
  );
};
