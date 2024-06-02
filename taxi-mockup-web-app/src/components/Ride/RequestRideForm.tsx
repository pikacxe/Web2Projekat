import {
  Button,
  Divider,
  FormControl,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { ChangeEvent, FormEvent, useState } from "react";
import rideService from "../../services/RideService";
import { useAuth } from "../../hooks/useAuth";
import { ProposedRideRequest } from "../../models/Ride/RideModel";
import { useAlert } from "../../hooks/useAlert";

export const RequestRideForm = () => {
  const user = useAuth().user;
  let defaultRequest: ProposedRideRequest = {
    passengerId: user?.userId as string,
    driverETA: 0,
    endDestination: "",
    startDestination: "",
    price: 0,
    estRideDuraiton: 0,
  };
  const alert = useAlert();
  const [rideRequest, setRideRequest] =
    useState<ProposedRideRequest>(defaultRequest);

  function handleSubmit(event: FormEvent<HTMLFormElement>): void {
    event.preventDefault();
    rideRequest.driverETA = generateRandomNumber(1, 40);
    rideRequest.price = generateRandomNumber(1, 300);
    rideRequest.estRideDuraiton = generateRandomNumber(1, 20);
    rideService.requestRide(user?.token as string, rideRequest).then((res) => {
      if (res) {
        alert.showAlert("Ride request sent", "success");
        setRideRequest(defaultRequest);
      } else {
        alert.showAlert(
          "Unable to request a ride. Please try again later",
          "warning"
        );
      }
    });
  }

  function generateRandomNumber(min: number, max: number): number {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.round((Math.random() * (max - min + 1) + min) * 100) / 100;
  }

  function handleChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void {
    const { value, name } = event.target;
    updateRequestProperty(name, value);
  }

  function updateRequestProperty(name: string, value: string) {
    setRideRequest((prevState) => ({
      ...prevState,
      [name]: value,
    }));
  }

  return (
    <form onSubmit={handleSubmit}>
      <Typography variant="h4">Request new ride</Typography>
      <Divider />
      <Stack direction="row" alignContent="center" gap={2} padding="2rem 1rem">
        <FormControl>
          <TextField
            name="startDestination"
            value={rideRequest.startDestination}
            label="Start destination"
            InputLabelProps={{
              shrink: true,
            }}
            onChange={handleChange}
            required
          />
        </FormControl>
        <Typography variant="h3"> ➡️ </Typography>
        <FormControl>
          <TextField
            name="endDestination"
            value={rideRequest.endDestination}
            label="End destination"
            InputLabelProps={{
              shrink: true,
            }}
            required
            onChange={handleChange}
          />
        </FormControl>
        <Button type="submit" variant="contained" color="success">
          Request new ride
        </Button>
      </Stack>
    </form>
  );
};
