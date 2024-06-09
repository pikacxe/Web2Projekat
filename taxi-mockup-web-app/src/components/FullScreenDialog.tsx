import Dialog from "@mui/material/Dialog";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import IconButton from "@mui/material/IconButton";
import CloseIcon from "@mui/icons-material/Close";
import Slide from "@mui/material/Slide";
import { TransitionProps } from "@mui/material/transitions";
import { RideInProgress } from "../views/RideInProgress";
import {
  FinishRideRequest,
  RideInProgressInfo,
} from "../models/Ride/RideModel";
import React, { ChangeEvent, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import {
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
  TextField,
} from "@mui/material";
import rideService from "../services/RideService";
import { useAuth } from "../hooks/useAuth";
import { useAlert } from "../hooks/useAlert";
import { AxiosError } from "axios";

const Transition = React.forwardRef(function Transition(
  props: TransitionProps & {
    children: React.ReactElement;
  },
  ref: React.Ref<unknown>
) {
  return <Slide direction="up" ref={ref} {...props} />;
});

export const FullScreenDialog = () => {
  const [open, setOpen] = React.useState(true);
  const location = useLocation();
  const token = useAuth().user?.token as string;
  const alert = useAlert();
  const navigate = useNavigate();
  const ride = location.state as RideInProgressInfo;
  const [finishRideData, setFinishRideData] = useState<FinishRideRequest>({
    rideId: ride.rideId,
    passengerId: ride.passengerId,
    rating: 0,
  });
  const [openDialog, setOpenDialog] = useState<boolean>(false);
  const handleClose = () => {
    setOpen(false);
    navigate("..", { relative: "path" });
  };

  const openFinishDialog = () => {
    setOpenDialog(true);
  };

  const handleDialogClose = () => {
    finishRide();
    setOpenDialog(false);
  };

  const handleDialogConfirm = () => {
    finishRide();
    setOpenDialog(false);
  };

  function finishRide() {
    console.log(finishRide);
    rideService
      .finishActiveRide(token, finishRideData)
      .then((res) => {
        if (res) {
          alert.showAlert("Thank you for your feedback", "success");
          handleClose();
        } else {
          alert.showAlert("Could not proccess your feedback", "warning");
        }
      })
      .catch((err) => {
        if (err instanceof AxiosError) {
          alert.showAlert(err.response?.data, "error");
        } else {
          alert.showAlert(err.message, "error");
        }
      });
  }

  function handleRatingChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void {
    if (+event.target.value > 0) {
      setFinishRideData((prevState) => ({
        ...prevState,
        rating: +event.target.value,
      }));
    }
  }

  return (
    <React.Fragment>
      <Dialog
        fullScreen
        open={open}
        onClose={handleClose}
        TransitionComponent={Transition}
      >
        <AppBar sx={{ position: "relative" }}>
          <Toolbar>
            <IconButton
              edge="start"
              color="inherit"
              onClick={handleClose}
              aria-label="close"
            >
              <CloseIcon />
            </IconButton>
          </Toolbar>
        </AppBar>
        <RideInProgress
          ride={ride}
          closeParent={handleClose}
          openFinishDialog={openFinishDialog}
        />
      </Dialog>
      <Dialog
        open={openDialog}
        TransitionComponent={Transition}
        keepMounted
        onClose={handleClose}
        aria-describedby="alert-dialog-slide-description"
      >
        <DialogTitle>Please rate your ride expirience</DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-slide-description">
            Please provide your feedback infomation for current ride.
          </DialogContentText>
          <TextField
            sx={{
              marginY: "1rem",
            }}
            type="number"
            placeholder="Your driver rating"
            onChange={handleRatingChange}
            value={finishRideData.rating}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleDialogClose}>Cancel</Button>
          <Button onClick={handleDialogConfirm}>Submit</Button>
        </DialogActions>
      </Dialog>
    </React.Fragment>
  );
};
