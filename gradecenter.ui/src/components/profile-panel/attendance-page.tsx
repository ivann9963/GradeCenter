import { Box } from "@mui/material";
import { AspNetUser } from "../../models/aspNetUser";
import {useEffect, useState} from "react";
import { GridColDef, GridValueGetterParams, DataGrid } from "@mui/x-data-grid";
import axios from "axios";
import { Attendance } from "../../models/attendance";

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
  
export default function Attendances(params: Profile){
    
    useEffect(() => {
        getAllGrades();
    });

    const [attendances, setAttendances] = useState<Attendance[] | null>(null);

    const getAllGrades = () => {
        // TODO: Fetch data based on profile which can be obtained from 
        // the params object and update the attendances state object.
    }

    return(
        <Box sx={{ height: 400, width: '100%' }}>
        <DataGrid
          rows={[]} // TODO: update the rows property with the updated attendances state object.
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