import {
  Avatar,
  Box,
  Button,
  Divider,
  FormControl,
  FormLabel,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { ChangeEvent, MouseEvent, FormEvent, useEffect, useState } from "react";
import userService from "../../services/UserService";
import { useAuth } from "../../hooks/useAuth";
import { useNavigate } from "react-router-dom";
import { UpdateUserRequest } from "../../models/User/UserModel";
import { AxiosError } from "axios";
import { useAlert } from "../../hooks/useAlert";

import "./UserForm.css";
import imageService from "../../services/ImageService";

export const UserForm: React.FC<{ userId: string | undefined }> = ({
  userId,
}) => {
  const token = useAuth().user?.token as string;
  const alert = useAlert();
  const navigate = useNavigate();
  const [selectedImage, setSelectedImage] = useState<File | null>(null);
  const [currentUser, setCurrentUser] = useState<UpdateUserRequest>({
    username: undefined,
    email: undefined,
    fullname: undefined,
    dateOfBirth: undefined,
    address: undefined,
    userPicture: undefined,
  });

  useEffect(() => {
    if (!token) {
      navigate("/");
    }
    if (!userId) {
      navigate("..", { relative: "path" });
    }
    userService
      .getUserById(token, userId as string)
      .then((res) => {
        console.log(res);
        setCurrentUser(res as UpdateUserRequest);
      })
      .catch((err) => {
        if (err.type instanceof AxiosError) {
          alert.showAlert(err.response.data, "error");
        } else {
          alert.showAlert(err.message, "error");
        }
        navigate("..", { relative: "path" });
      });
  }, [userId, alert, navigate, token]);

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
    if (currentUser.dateOfBirth) {
      currentUser.dateOfBirth = new Date(currentUser.dateOfBirth).toISOString();
    }
    userService
      .updateUser(token, userId as string, currentUser as UpdateUserRequest)
      .then((res) => {
        if (res) {
          alert.showAlert("Profile updated successfully", "success");
          navigate("..", { relative: "path" });
        } else {
          alert.showAlert("Error while updating your profile", "error");
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
    navigate("..", { relative: "path" });
  }

  return (
    <>
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
      <form onSubmit={handleSubmit} className="userForm">
        <Typography variant="h4" textAlign="center">
          Edit profile
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
                : undefined
            }
            onChange={handleChange}
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
          />
        </FormControl>
        <Stack direction="row" alignItems="end" justifyContent="space-between">
          <FormControl>
            <FormLabel htmlFor="userPicture">User picture</FormLabel>
            <TextField
              id="userPicture"
              sx={{
                marginBottom: "1rem",
              }}
              type="file"
              onChange={handleFileChange}
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
            onClick={handleBack}
          >
            Cancel
          </Button>
          <Button
            type="submit"
            variant="contained"
            size="large"
            color="success"
          >
            Confirm
          </Button>
        </Stack>
      </form>
    </>
  );
};
