import {
  AcceptRideRequest,
  AvailableRideRespone,
} from "../../models/Ride/RideModel";
import {
  DataGrid,
  GridActionsCellItem,
  GridActionsCellItemProps,
  GridRowParams,
} from "@mui/x-data-grid";
import { GridColDef, GridToolbar } from "@mui/x-data-grid";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  TextField,
} from "@mui/material";
import {
  JSXElementConstructor,
  ReactElement,
  useCallback,
  useState,
  MouseEvent,
  ChangeEvent,
  useEffect,
} from "react";
import { useAlert } from "../../hooks/useAlert";
import { useAuth } from "../../hooks/useAuth";
import { AuthResponse } from "../../models/User/UserModel";
import rideService from "../../services/RideService";
import NoCrashIcon from "@mui/icons-material/NoCrash";
import { useSignalR } from "../../hooks/useSignalR";

interface RideDataRow {
  id: string;
  startDestination: string;
  endDestination: string;
}

export const AvailableRidesList: React.FC<{
  rides: Array<AvailableRideRespone>;
}> = ({ rides }) => {
  const { token, userId } = useAuth().user as AuthResponse;
  const alert = useAlert();
  const { connection } = useSignalR();
  const [rows, setRows] = useState<RideDataRow[]>([]);
  const [rideToAccept, setRideToAccept] = useState<string>();
  const [openDialog, setOpenDialog] = useState<boolean>(false);
  const [acceptRideRequest, setAcceptRideRequest] = useState<AcceptRideRequest>(
    {
      driverId: userId,
      rideId: "",
      driverETA: 0,
      connectionId: connection?.connectionId ?? "",
    }
  );
  useEffect(() => {
    setRows(
      rides.map((ride) => ({
        ...ride,
        id: ride.rideId,
      })) as RideDataRow[]
    );
  }, [rides]);

  const openDialogBox = useCallback(
    (id: string) => () => {
      setRideToAccept(id);
      console.log(rideToAccept);
      setOpenDialog(true);
    },
    [rideToAccept]
  );

  const acceptRide = () => {
    acceptRideRequest.rideId = rideToAccept as string;
    acceptRideRequest.connectionId = connection?.connectionId as string;
    rideService
      .acceptPendingRide(token, acceptRideRequest)
      .then((res) => {
        if (res) {
          alert.showAlert("Ride accepted successfully", "success");
          setOpenDialog(false);
          setRows((rows) =>
            rows.filter((row) => row.id !== (rideToAccept as string))
          );
          acceptRideRequest.driverETA = 0;
          acceptRideRequest.rideId = "";
        } else {
          alert.showAlert("Error while accepting proposed ride", "warning");
        }
      })
      .catch((err) => {
        alert.showAlert(err.message, "error");
        console.log(err);
      });
  };
  function getGridActions(
    params: GridRowParams<RideDataRow>
  ): ReactElement<
    GridActionsCellItemProps,
    string | JSXElementConstructor<any>
  >[] {
    return [
      <GridActionsCellItem
        icon={<NoCrashIcon />}
        label="Accept ride"
        color="success"
        onClick={openDialogBox(params.row.id)}
      />,
    ];
  }

  const columns: GridColDef<RideDataRow[][number]>[] = [
    { field: "id", headerName: "Ride Id", flex: 1.5 },
    {
      field: "passengerName",
      headerName: "Passenger name",
      editable: false,
      flex: 1,
    },
    {
      field: "startDestination",
      headerName: "Start destination",
      editable: false,
      flex: 2,
    },
    {
      field: "endDestination",
      headerName: "End destination",
      editable: false,
      flex: 2,
    },
    {
      field: "actions",
      headerName: "Accept ride",
      type: "actions",
      headerAlign: "center",
      flex: 1.5,
      getActions: getGridActions,
    },
  ];
  function handleClose(event: MouseEvent<HTMLButtonElement>): void {
    setOpenDialog(false);
  }

  function handleDriverETA(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ): void {
    if (+event.target.value > 0) {
      setAcceptRideRequest((prevState) => ({
        ...prevState,
        driverETA: +event.target.value,
      }));
    }
  }

  return (
    <>
      <DataGrid
        rows={rows}
        columns={columns}
        rowSelection={false}
        initialState={{
          pagination: {
            paginationModel: { page: 0, pageSize: 5 },
          },
        }}
        pageSizeOptions={[5, 10]}
        slots={{ toolbar: GridToolbar }}
      />
      <Dialog
        open={openDialog}
        onClose={handleClose}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description"
      >
        <DialogTitle id="alert-dialog-title">
          {"Accept proposed ride"}
        </DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            Please provide your estimated time to proposed starting destination.
            Try as best as you can to provide correct time estimation.
          </DialogContentText>
          <TextField
            sx={{
              marginY: "1rem",
            }}
            type="number"
            value={acceptRideRequest.driverETA}
            onChange={handleDriverETA}
            label="Your estimated arival time"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} autoFocus>
            Cancel
          </Button>
          <Button
            onClick={acceptRide}
            disabled={acceptRideRequest.driverETA === 0}
          >
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
