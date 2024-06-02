import { CompletedRideResponse } from "../../models/Ride/RideModel";
import { DataGrid } from "@mui/x-data-grid";
import { GridColDef, GridToolbar } from "@mui/x-data-grid";

export const RideInfo: React.FC<{ rides: Array<CompletedRideResponse> }> = ({
  rides,
}) => {
  const rows = rides.map((ride) => ({
    ...ride,
    id: ride.rideId,
  }));
  const columns: GridColDef<(typeof rows)[number]>[] = [
    { field: "rideId", headerName: "Ride Id", flex: 1.5 },
    {
      field: "driverId",
      headerName: "Driver Id",
      editable: false,
      flex: 1.5,
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
      field: "price",
      headerName: "Price",
      editable: false,
    },
    {
      field: "rideDuration",
      headerName: "Ride duration",
      editable: false,
    },
    {
      field: "rating",
      headerName: "Rating",
      editable: false,
    },
  ];
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
    </>
  );
};
