import { Divider, Stack, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useAuth } from "../../hooks/useAuth";

export const RideTimer: React.FC<{
  driverETA: number;
  rideDuration: number;
  closeParent: () => void;
  openFinishDialog: () => void;
}> = ({ driverETA, rideDuration, closeParent, openFinishDialog }) => {
  const [driverTimer, setDriverTimer] = useState<number>(driverETA);
  const [rideTimer, setRideTimer] = useState<number>(rideDuration);
  const userRole = useAuth().user?.userRole;
  const timeout = 700;

  useEffect(() => {
    if (rideTimer > 0) {
      if (driverTimer > 0) {
        setTimeout(() => setDriverTimer(driverTimer - 1), timeout);
      } else {
        setTimeout(() => setRideTimer(rideTimer - 1), timeout);
      }
    } else {
      if (userRole === "User") {
        openFinishDialog();
      } else {
        closeParent();
      }
    }
  }, [driverTimer, rideTimer, userRole, closeParent, openFinishDialog]);

  return (
    <>
      {driverTimer !== 0 && (
        <Stack>
          <Typography variant="h3" textAlign="center">
            Time until driver arrives
          </Typography>
          <Divider variant="middle" />
          <Typography variant="h3" textAlign="center">
            {driverTimer}
          </Typography>
        </Stack>
      )}
      {driverTimer === 0 && rideTimer !== 0 && (
        <Stack>
          <Typography variant="h3" textAlign="center">
            Time until ride ends
          </Typography>
          <Divider variant="middle" />
          <Typography variant="h3" textAlign="center">
            {rideTimer}
          </Typography>
        </Stack>
      )}
    </>
  );
};
