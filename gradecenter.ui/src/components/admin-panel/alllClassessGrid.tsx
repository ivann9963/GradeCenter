import * as React from "react";
import Box from "@mui/material/Box";
import { DataGrid, GridColDef, GridRenderCellParams } from "@mui/x-data-grid"; // Import DataGrid instead of DataGridPro
import { School } from "../../models/school";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import { SchoolClass } from "../../models/schoolClass";
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, MenuItem, Select, SelectChangeEvent, TextField } from "@mui/material";
import requests from "../../requests";

interface AllClassessGridParams {
  allClassess: SchoolClass[] | null;
}

export default function AllClassessGrid(params: AllClassessGridParams | null) {
  let data: School[] | AspNetUser[] | SchoolClass[] | null = null;
  let columns: GridColDef[] | null = null;

  if (params && params.allClassess && params!.allClassess!.length > 0) {
    data = params!.allClassess!.map((user) => ({
      ...user,
      schoolName: user.school?.name,
    }));

    columns = [
      { field: "year", headerName: "Year", width: 100 },
      { field: "department", headerName: "Department", width: 150 },
      {
        field: "headTeacher",
        headerName: "Head Teacher",
        width: 200,
        valueGetter: (params) => `${params.row.headTeacher.firstName} ${params.row.headTeacher.lastName}`,
      },
      {
        field: "schoolName",
        headerName: "School",
        width: 150,
        valueGetter: (params) => params.row.schoolName,
      },
      {
        field: "students",
        headerName: "Number of Students",
        width: 150,
        valueGetter: (params) => params.row.students.length,
      },
      {
        field: "curriculum",
        headerName: "Number of Disciplines",
        width: 200,
        valueGetter: (params) => params.row.curriculum.length,
      },
    ];
  }

  console.log(params?.allClassess);

  return (
    <Box sx={{ height: 520, width: "100%" }}>
      <DataGrid
        columns={columns!}
        rows={data || []}
        loading={data!.length === 0}
        rowHeight={48}
        checkboxSelection={false}
      />
    </Box>
  );
}
