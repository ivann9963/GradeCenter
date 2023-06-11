import { MutableRefObject, useEffect, useRef, useState } from "react";
import Box from '@mui/material/Box';
import { DataGrid, GridColDef, GridRenderCellParams, GridValueGetterParams } from '@mui/x-data-grid';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs'
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import { SchoolClass } from "../../models/schoolClass";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, InputLabel, MenuItem, TextField, Typography } from "@mui/material";
import { Link } from "react-router-dom";
import requests from "../../requests";
import Discipline from "../../models/discipline";
import { DatePicker } from "@mui/x-date-pickers";
import Select from "@material-ui/core/Select";
import React from "react";


export default function MyClass({children} : any) {
  let columns: GridColDef[] | null = null;
 
  const [selectedRowData, setSelectedRowData] = useState<any | null>(null);
  const [discipline, setDiscipline] = useState<Discipline | null>(null);
  const [isTeacher, setIsTeacher] = useState(false);
  const [isGradeDialogOpened, setGradeDialogOpened] = useState(false);
  const [isAttendanceDialogOpened, setAttendanceDialogOpened] = useState(false);
  const [user, setUser] = useState<AspNetUser | null>(null);
  const [schoolClass, setSchoolClass] = useState<SchoolClass | null>(null);
  const [students, setStudents] = useState<AspNetUser[] | null>(null);

  const rateRef = React.useRef<HTMLInputElement | null>(null);
  const attendanceDateRef = React.useRef<HTMLInputElement | null>(null);
  const hasAttendedRef = React.useRef<HTMLSelectElement | null>(null);
   
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
        field: "addGrade",
        headerName: "",
        width: 150,
        renderCell: (params: GridRenderCellParams) => {
          const handleOpen = () => {
            setSelectedRowData(params.row);
            setGradeDialogOpened(true);
          };
          return (
            <Button
            variant="contained"
            onClick={handleOpen}
            sx={{ marginBottom: 2, marginTop: 2, marginLeft: 1 }}>
            + Grade
            </Button>
          );
        }
    })

    columns.push({
      field: "addAttendance",
      headerName: "",
      width: 150,
      renderCell: (params: GridRenderCellParams) => {
        const handleOpen = () => {
          setSelectedRowData(params.row);
          setAttendanceDialogOpened(true);
        };
        return (
          <Button
          variant="contained"
          onClick={handleOpen}
          sx={{ marginBottom: 2, marginTop: 2, marginLeft: 1 }}>
          + Attendance
          </Button>
        );
      }
  })

  }
  function handleClose(){
        setGradeDialogOpened(false)
        setAttendanceDialogOpened(false);
  }
  

  function handleGradeSave(){
        var studentUsername = selectedRowData["userName"];
        debugger;
        var studentRate = rateRef.current?.value; 
        var studentDiscipline = discipline?.name;

        requests.createGrade(studentUsername, studentRate, studentDiscipline);
  }
  
  function handleAttendanceSave(){
        var studentUsername = selectedRowData["userName"];
        debugger;
        var date = attendanceDateRef.current?.value;
        var studentDiscipline = discipline?.name;
        var attended = Boolean(hasAttendedRef.current?.value);

        requests.createAttendance(studentUsername, date, attended, studentDiscipline);
  }

  const AddGradeDialog = () => (
    <Dialog open={isGradeDialogOpened} onClose={handleClose}>
      <DialogTitle>Add Grade</DialogTitle>
      <DialogContent>
      <TextField value={discipline?.name} placeholder="Discipline" 
        InputProps={{
          readOnly: true
        }}
      />
      <br />
      <br />
      <TextField inputRef={rateRef} 
                 onChange={(e) => {
                   if(Number.parseInt(e.target.value) > 6){
                      e.target.value = "6";
                   }
                   else if(Number.parseInt(e.target.value) < 2){
                      e.target.value = "2";
                   }
                 }}
                 placeholder="Rate Student" type="number"
      />
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleGradeSave}>Save</Button>
      </DialogActions>
    </Dialog>
  );

  const AddAttendance = () => (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
    <Dialog open={isAttendanceDialogOpened} onClose={handleClose}>
      <DialogTitle>Add Attendance</DialogTitle>
      <DialogContent>
      <TextField value={discipline?.name} placeholder="Discipline" 
        InputProps={{
          readOnly: true
        }}
      />
      <br />
      <br />
      <DatePicker inputRef={attendanceDateRef}/>
      <br/>
      <br/>
      <InputLabel id="hasAttendedLabel">Has Attended</InputLabel>
      <Select labelId="hasAttendedLabel"
              fullWidth={true}
              label="Has Attended"
              inputRef={hasAttendedRef}>
        <MenuItem value={1}>Yes</MenuItem>
        <MenuItem value={0}>No</MenuItem>
      </Select>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button onClick={handleAttendanceSave}>Save</Button>
      </DialogActions>
    </Dialog>
    </LocalizationProvider>
  );


  return (

    <Box sx={{ height: 520, width: "50%", marginLeft:"30%", marginTop:"7%" }}>
      <AddGradeDialog />
      <AddAttendance />
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