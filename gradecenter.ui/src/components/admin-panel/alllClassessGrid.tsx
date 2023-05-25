import * as React from "react";
import Box from "@mui/material/Box";
import { DataGrid, GridColDef, GridRenderCellParams } from "@mui/x-data-grid"; // Import DataGrid instead of DataGridPro
import { School } from "../../models/school";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import { SchoolClass } from "../../models/schoolClass";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  MenuItem,
  Select,
  SelectChangeEvent,
  TextField,
} from "@mui/material";
import requests from "../../requests";

interface AllClassessGridParams {
  allClassess: SchoolClass[] | null;
}

export default function AllClassessGrid(params: AllClassessGridParams | null) {
  let data: School[] | AspNetUser[] | SchoolClass[] | null = null;
  let columns: GridColDef[] | null = null;
  const [createSchoolClassOpen, setCreateSchoolClassOpen] = React.useState(false);
  const [newClass, setNewClass] = React.useState("");
  const [schoolName, setSchoolName] = React.useState("");
  const [teacherNames, setTeacherNames] = React.useState("");

  const CreateSchoolClassButton = () => (
    <Button variant="outlined" sx={{ marginBottom: 2, marginTop: -2, marginLeft: 1}}  onClick={() => setCreateSchoolClassOpen(true)}>+ New</Button>
  );

  const submitCreateNewClass = () => {
    var year = newClass[0] as unknown as number;
    var department = newClass[1];

    requests.createSchoolClass(year, department, schoolName, teacherNames)
  }

  const CreateSchoolClassDialog = () => (
    <Dialog open={createSchoolClassOpen} onClose={() => setCreateSchoolClassOpen(false)}>
      <DialogTitle>Create School Class</DialogTitle>
      <DialogContent>
          <TextField value={newClass} onChange={(e) => setNewClass(e.target.value)} label="Ex: '8A'.." fullWidth />
          <br />
          <br />
          <TextField value={schoolName} onChange={(e) => setSchoolName(e.target.value)} label="School Name.." fullWidth />
          <br />
          <br />
          <TextField value={teacherNames} onChange={(e) => setTeacherNames(e.target.value)} label="Taecher First and Last names.." fullWidth />
      </DialogContent>
      <DialogActions>
        <Button onClick={() => setCreateSchoolClassOpen(false)}>Cancel</Button>
        <Button onClick={() => submitCreateNewClass()}>Save</Button>
      </DialogActions>
    </Dialog>
  );

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
      <CreateSchoolClassButton />

      <DataGrid
        columns={columns!}
        rows={data || []}
        loading={data!.length === 0}
        rowHeight={48}
        checkboxSelection={false}
      />
      <CreateSchoolClassDialog />
    </Box>
  );
}
