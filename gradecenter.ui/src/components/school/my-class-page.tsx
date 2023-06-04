import { useEffect, useState } from "react";
import Box from '@mui/material/Box';
import { DataGrid, GridColDef, GridRenderCellParams, GridValueGetterParams } from '@mui/x-data-grid';
import axios from "axios";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import { SchoolClass } from "../../models/schoolClass";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Typography } from "@mui/material";
import { Link } from "react-router-dom";
import requests from "../../requests";
import Discipline from "../../models/discipline";


export default function MyClass() {
  let columns: GridColDef[] | null = null;
 
  const [selectedRowData, setSelectedRowData] = useState<any | null>(null);
  const [discipline, setDiscipline] = useState<Discipline | null>(null);
  const [rate, setRate] = useState<string>("");
  const [isTeacher, setIsTeacher] = useState(false);
  const [isOpened, setIsOpened] = useState(false);
  const [user, setUser] = useState<AspNetUser | null>(null);
  const [schoolClass, setSchoolClass] = useState<SchoolClass | null>(null);
  const [students, setStudents] = useState<AspNetUser[] | null>(null);
   
  useEffect(() => {
    getLoggedUser();

    setTimeout(function(){
        getStudents();
    },150)

  }, [])

  const getLoggedUser = () => {
     requests.getLoggedUser()
        .then((res) => {
        const user = res.data;
        setUser(user);
        switch (UserRoles[user?.userRole as number]) {
            case "Teacher":
            case "Student":
                requests.getUserById(user?.id)
                .then((res) => {
                    const schoolClass = res.data["schoolClass"];
                    console.log(res.data);
                    setSchoolClass(schoolClass);
                })
                break;
            default:
                break;
        }

        if (UserRoles[user?.userRole as number] == "Teacher") {
            setIsTeacher(true);
            requests.getDisciplineByTeacherId(user?.id)
            .then((res) => {
               var discipline = res.data;
               setDiscipline(discipline);
               console.log(discipline);
            })
        }

     });
  };
  
  const getStudents = () => {
    requests.getAllUsers()
       .then((res) => {
         const students = res.data.filter(function(student: AspNetUser){
             return UserRoles[student.userRole as number] == "Student" 
                    && student.schoolClass != null 
                    && schoolClass?.id == student.schoolClassId;
        });
        
        setStudents(students);
    })
  }

  columns = [  
  { field: "firstName", 
    headerName: "First name", 
    width: 200,
    valueGetter: (params) => params.row.firstName + " " + params.row.lastName,
    renderCell: (params: GridRenderCellParams) => {
        return (
           <Link to={`/profile/${params.row.id}`} >
            <Typography variant="h5" fontSize={"15px"}>
              {params.row.firstName + " "+ params.row.lastName}
            </Typography>
          </Link>
        );
      }
  },
  {
    field: "schoolClass.year",
    headerName: "Class",
    width: 90,
    valueGetter: (params) => params.row.schoolClass?.year + params.row.schoolClass?.department ?? '-',
  }]

  if (isTeacher) {
    columns.push({
        field: "-",
        headerName: "",
        width: 150,
        renderCell: (params: GridRenderCellParams) => {
        setSelectedRowData(params.row);
        const handleOpen = () => {
            setIsOpened(true);
          };
          return (
            <Button
            variant="outlined"
            onClick={handleOpen}
            sx={{ marginBottom: 2, marginTop: 2, marginLeft: 1 }}>
            + Add Grade
            </Button>
          );
        }
    })

  }
  function handleClose(){
        setIsOpened(false);
  }
  function handleSubmit(){
        var studentUsername = selectedRowData["userName"];
        var studentRate = rate;
        var studentDiscipline = discipline?.name;

        requests.createGrade(studentUsername, studentRate, studentDiscipline);
  }

  const handleChangeRate = (rate : string) => {
     if(rate <= "1"){
       setRate("2");
     }
     else if (rate >= "6"){
       setRate("6");
     }
     else{
       setRate(rate);
     }
  }

  const AddDialog = () => (
    <Dialog open={isOpened} onClose={handleClose}>
      <DialogTitle>Add Grade</DialogTitle>
      <DialogContent>
      <TextField value={discipline?.name} placeholder="Discipline" 
        InputProps={{
          readOnly: true
        }}
      />
      <br />
      <br />
      <TextField value={rate} placeholder="Rate Student" onChange={(e) => { handleChangeRate(e.target.value) }} type="number"/>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleSubmit}>Save</Button>
      </DialogActions>
    </Dialog>
  );

  return (

    <Box sx={{ height: 520, width: "50%", marginLeft:"30%", marginTop:"7%" }}>
      <AddDialog />
      <Typography variant="h3" align="center">
        {schoolClass != undefined ? `${schoolClass?.year}-${schoolClass?.department}`
            : "Loading..."
        }
      </Typography>
      <DataGrid
        rows={students as AspNetUser [] || []}
        columns={columns!}
        rowHeight={48}
        checkboxSelection={false}
      />
    </Box>
  );
}