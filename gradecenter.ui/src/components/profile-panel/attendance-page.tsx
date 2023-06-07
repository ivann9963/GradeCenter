import { Box, Button } from "@mui/material";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import { useEffect, useState } from "react";
import { GridColDef, GridValueGetterParams, DataGrid, GridRenderCellParams } from "@mui/x-data-grid";
import axios from "axios";
import DeleteIcon from '@material-ui/icons/Delete';
import EditIcon from '@material-ui/icons/Edit';
import { Attendance } from "../../models/attendance";
import requests from "../../requests";
import Discipline from "../../models/discipline";

interface Profile {
  profile: AspNetUser | null;
}

const columns: GridColDef[] = [
  {
    field: 'date',
    headerName: 'Date',
    width: 150,
    editable: false,
  },
  {
    field: 'discipline',
    headerName: 'Discipline',
    width: 150,
    editable: false,
    valueGetter: (params: GridValueGetterParams) =>
      `${params.row.discipline.name}`
  },
  {
    field: 'student',
    headerName: 'Student',
    width: 110,
    editable: true,
    valueGetter: (params: GridValueGetterParams) =>
      `${params.row.student.firstName} - ${params.row.student.lastName}`
  },
  {
    field: 'hasAttended',
    headerName: 'Has Attended',
    width: 110,
    editable: true
  }
];

export default function Attendances(params: Profile) {

  useEffect(() => {
    getAllAttendances();
    getLoggedUser();
  }, []);


  const [attendances, setAttendances] = useState<Attendance[] | null>(null);
  const [loggedUser, setLoggedUser] = useState<AspNetUser | null>(null);
  const [discipline, setDiscipline] = useState<Discipline | null>(null);
  const [isOpened, setIsOpened] = useState(false);
  const [rate, setRate] = useState<string>("");
  const [selectedRowData, setSelectedRowData] = useState<any | null>(null);

  const getAllAttendances = () => {
    requests.getAllAttendances()
      .then((res) => {
        let attendances = res.data;
        attendances = attendances.filter(function (attendance: Attendance) {
          return attendance.student?.firstName === params.profile?.firstName &&
            attendance.student?.lastName === params.profile?.lastName;
        });
        setAttendances(attendances);
      })
      .catch((error) => {
        console.error("Error fetching attendances: ", error);
      });
  }

  const getLoggedUser = () => {
    requests.getLoggedUser()
      .then((res) => {
        var user = res.data;
        setLoggedUser(user);
        if (UserRoles[loggedUser?.userRole as number] === "Teacher") {
          requests.getDisciplineByTeacherId(loggedUser?.id as string)
            .then((res) => {
              var discipline = res.data;
              setDiscipline(discipline);
            })
        }
      })
  }
  
  return (
    <Box sx={{ height: 400, width: '100%' }}>
      <DataGrid
        rows={attendances || []} // Update the rows property with the updated attendances state object // TODO: update the rows property with the updated attendances state object.
        columns={columns}
        initialState={{
          pagination: {
            paginationModel: {
              pageSize: 5,
            },
          },
        }}
        pageSizeOptions={[5]}
        disableRowSelectionOnClick
      />
    </Box>
  );
}