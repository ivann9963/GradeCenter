import axios from "axios";
import { AspNetUser } from "../../models/aspNetUser";
import { useEffect, useState } from "react";
import { Box } from "@mui/material";
import { Grade } from "../../models/grade";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import requests from "../../requests";

interface Profile {
    profile: AspNetUser | null;
}

export default function Grades (params: Profile){
    let columns: GridColDef[] | null = null;

    const token = sessionStorage["jwt"];

     useEffect(() => {
        getAllGrades();
    }, [])

    const [grades, setGrades] = useState<Grade[] | null>(null);
    const getAllGrades = () => {
        requests.getAllGrades().then((res) => {
           let grades = res.data;
           grades = grades.filter(function(grade: Grade){
               return grade.student?.firstName === params.profile?.firstName 
                        && 
                      grade.student?.lastName === params.profile?.lastName; 
           });
           setGrades(grades);
        });
    }

    columns = [
        { field: "rate", 
          headerName: "Rate", 
          width: 130 
        },
        { field: "discipline.name", 
          headerName: "Discipline", 
          width: 130,
          valueGetter: (params) => params.row.discipline?.name, 
        },
        { field: "discipline.teacher", 
          headerName: "Teacher", 
          width: 130,
          valueGetter: (params) => params.row.discipline?.teacher.firstName + "-" + params.row.discipline?.teacher.lastName
        },
        {
          field: "discipline.schoolClass.school", 
          headerName: "School", 
          width: 130,
          valueGetter: (params) => params.row.discipline?.schoolClass.school.name, 
        }
    ];

    return(
      <Box sx={{ height: 520, width: "100%" }}>
        <DataGrid
          columns={columns!}
          rows={grades || []}
          rowHeight={48}
          checkboxSelection={false}
        />
      </Box>
    )
}