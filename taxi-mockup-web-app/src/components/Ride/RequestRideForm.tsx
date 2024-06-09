import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
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
import { useSignalR } from "../../hooks/useSignalR";
import { MouseEvent } from "react";

export const RequestRideForm = () => {
  const user = useAuth().user;
  const { connection } = useSignalR();
  const [isOpen, setIsOpen] = useState<boolean>(false);
  let defaultRequest: ProposedRideRequest = {
    passengerId: user?.userId as string,
    driverETA: 0,
    endDestination: "",
    startDestination: "",
    price: 0,
    estRideDuration: 0,
    passengerName: user?.username as string,
    connectionId: connection?.connectionId ?? "",
  };
  const alert = useAlert();
  const [rideRequest, setRideRequest] =
    useState<ProposedRideRequest>(defaultRequest);

  function handleSubmit(event: FormEvent<HTMLFormElement>): void {
    event.preventDefault();
    rideRequest.driverETA = 0;
    rideRequest.price = generateRandomNumber(1, 300);
    rideRequest.estRideDuration = generateRandomNumber(1, 60);
    rideRequest.connectionId = connection?.connectionId as string;
    setIsOpen(true);
  }

  function generateRandomNumber(min: number, max: number): number {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.round(Math.random() * (max - min + 1) + min);
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

  function handleClose() {
    setIsOpen(false);
  }

  function requestRide(event: MouseEvent<HTMLButtonElement>): void {
    rideService.requestRide(user?.token as string, rideRequest).then((res) => {
      if (res) {
        alert.showAlert("Ride request sent", "success");
        setIsOpen(false);
        setRideRequest(defaultRequest);
      } else {
        alert.showAlert(
          "Unable to request a ride. Please try again later",
          "warning"
        );
      }
    });
  }

  return (
    <>
      <form onSubmit={handleSubmit}>
        <Typography variant="h4">Request new ride</Typography>
        <Divider />
        <Stack direction="row" alignContent="center" gap={2} padding="1rem">
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
      <Dialog open={isOpen} onClose={handleClose}>
        <DialogTitle> Confirm ride request</DialogTitle>
        <DialogContent>
          <Typography variant="h6">Request info</Typography>
          <Typography variant="body1">Price: {rideRequest.price}</Typography>
          <Typography variant="body1">
            Estimated ride duration: {rideRequest.estRideDuration} minutes
          </Typography>
          <Typography variant="body1">
            Driver arival time will be determined when request is accepted
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} autoFocus>
            Cancel
          </Button>
          <Button onClick={requestRide}>Confirm</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
