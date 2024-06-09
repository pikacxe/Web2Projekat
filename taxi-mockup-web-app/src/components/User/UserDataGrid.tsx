import { UserInfo } from "../../models/User/UserModel";
import { DataGrid } from "@mui/x-data-grid/DataGrid";
import {
  GridActionsCellItem,
  GridActionsCellItemProps,
  GridColDef,
  GridRowParams,
  GridToolbar,
} from "@mui/x-data-grid";
import {
  JSXElementConstructor,
  ReactElement,
  useCallback,
  useState,
} from "react";
import { useAuth } from "../../hooks/useAuth";
import userService from "../../services/UserService";

import DeleteIcon from "@mui/icons-material/Delete";
import BlockIcon from "@mui/icons-material/Block";
import VerifiedIcon from "@mui/icons-material/Verified";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
} from "@mui/material";
import { AxiosError } from "axios";
import { useAlert } from "../../hooks/useAlert";

interface UserGridProps {
  users: Array<UserInfo>;
  actionsType: "delete" | "verify|ban";
}

export const UserDataGrid = ({ users, actionsType }: UserGridProps) => {
  const [rows, setRows] = useState(users);
  const [openDialog, setOpenDialog] = useState(false);
  const token = useAuth().user?.token as string;
  const [idToDelete, setIdToDelete] = useState("");
  const alert = useAlert();

  const deleteUserRow = (id: string) => {
    userService
      .deleteUser(token, id)
      .then((success) => {
        if (success) {
          setRows((rows) => rows.filter((row) => row.id !== id));
          setOpenDialog(false);
        }
      })
      .catch((err) => {
        console.log(err);
      });
  };
  const verifyUserRow = useCallback(
    (id: string) => () => {
      userService
        .verifyDriver(token, id)
        .then((success) => {
          if (success) {
            setRows((rows) =>
              rows.map((row) =>
                row.id === id ? { ...row, userState: "Verified" } : row
              )
            );
          }
        })
        .catch((err) => {
          console.log(err);
          if (err instanceof AxiosError) {
            alert.showAlert(err.response?.data, "warning");
          } else {
            alert.showAlert(err.message, "error");
          }
        });
    },
    [token]
  );
  const banUserRow = useCallback(
    (id: string) => () => {
      userService
        .banDriver(token, id)
        .then((success) => {
          if (success) {
            setRows((rows) =>
              rows.map((row) =>
                row.id === id ? { ...row, userState: "Denied" } : row
              )
            );
          }
        })
        .catch((err) => {
          console.log(err);
        });
    },
    [token]
  );

  const openDialogBox = useCallback(
    (id: string) => () => {
      setIdToDelete(id);
      setOpenDialog(true);
    },
    []
  );

  function handleClose() {
    setOpenDialog(false);
  }
  function getGridActions(
    params: GridRowParams<UserInfo>
  ): ReactElement<
    GridActionsCellItemProps,
    string | JSXElementConstructor<any>
  >[] {
    if (actionsType === "delete") {
      return [
        <GridActionsCellItem
          icon={<DeleteIcon />}
          label="Delete"
          color="error"
          onClick={openDialogBox(params.row.id)}
        />,
      ];
    } else {
      return [
        <GridActionsCellItem
          icon={<VerifiedIcon />}
          label="Verify"
          color="success"
          onClick={verifyUserRow(params.row.id)}
          disabled={params.row.userState === "Verified"}
        />,
        <GridActionsCellItem
          icon={<BlockIcon />}
          label="Ban"
          color="error"
          onClick={banUserRow(params.row.id)}
          disabled={params.row.userState === "Denied"}
        />,
      ];
    }
  }

  function getActionsHeader(): string | undefined {
    return actionsType === "delete" ? "Delete" : "Verify / Ban";
  }

  const columns: GridColDef<(typeof users)[number]>[] = [
    { field: "id", headerName: "User Id", flex: 1.5 },
    {
      field: "username",
      headerName: "Username",
      editable: false,
      flex: 1,
    },
    {
      field: "email",
      headerName: "Email",
      editable: false,
      flex: 2,
    },
    {
      field: "fullname",
      headerName: "Fullname",
      editable: false,
      flex: 1,
    },
    {
      field: "userType",
      headerName: "User type",
      editable: false,
    },
    {
      field: "userState",
      headerName: "User state",
      editable: false,
    },
    {
      field: "dateOfBirth",
      type: "date",
      headerName: "Date of birth",
      editable: false,
      valueGetter(params) {
        return new Date(params.row.dateOfBirth);
      },
    },
    {
      field: "address",
      headerName: "Address",
      editable: false,
      flex: 2,
    },
    {
      field: "actions",
      headerName: getActionsHeader(),
      type: "actions",
      headerAlign: "center",
      getActions: getGridActions,
    },
  ];
  function handleConfirm(): void {
    deleteUserRow(idToDelete);
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
        <DialogTitle id="alert-dialog-title">{"Delete user?"}</DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            Are you sure you want to delete user? User data will be lost
            forever;
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} autoFocus>
            Cancel
          </Button>
          <Button onClick={handleConfirm}>Confirm</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
