import React from "react";
import { RideInfo } from "../../models/Ride/RideModel";
import { DataGrid, GridColDef, GridToolbar } from "@mui/x-data-grid";

export const FullRideInfoList: React.FC<{ rides: RideInfo[] }> = ({
  rides,
}) => {
  const columns: GridColDef<RideInfo[][number]>[] = [
    { field: "id", headerName: "Ride Id", flex: 1.5 },
    {
      field: "passengerId",
      headerName: "Passenger Id",
      editable: false,
      flex: 1,
    },
    {
      field: "driverId",
      headerName: "Driver Id",
      editable: false,
      flex: 1,
    },
    {
      field: "startDestination",
      headerName: "Start destination",
      editable: false,
      flex: 1.5,
    },
    {
      field: "endDestination",
      headerName: "End destination",
      editable: false,
      flex: 1.5,
    },
    {
      field: "price",
      type: "number",
      headerName: "Price",
      editable: false,
    },
    {
      field: "driverETA",
      type: "number",
      headerName: "Driver ETA",
      editable: false,
    },
    {
      field: "rideDuration",
      type: "number",
      headerName: "Ride duration",
      editable: false,
    },
    {
      field: "rideState",
      headerName: "State",
      editable: false,
    },
    {
      field: "rating",
      type: "number",
      headerName: "Rating",
      editable: false,
    },
    {
      field: "createdAT",
      type: "number",
      headerName: "Requested At",
      editable: false,
    },
    {
      field: "updatedAt",
      type: "number",
      headerName: "Updated At",
      editable: false,
    },
    {
      field: "finishedAt",
      type: "number",
      headerName: "Finished At",
      editable: false,
    },
  ];
  return (
    <DataGrid
      rows={rides}
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
  );
};
