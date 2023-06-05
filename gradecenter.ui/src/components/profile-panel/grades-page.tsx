import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import { useEffect, useState } from "react";
import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from "@mui/material";
import { Grade } from "../../models/grade";
import { DataGrid, GridColDef, GridRenderCellParams } from "@mui/x-data-grid";
import DeleteIcon from '@material-ui/icons/Delete';
import EditIcon from '@material-ui/icons/Edit';
import requests from "../../requests";
import Discipline from "../../models/discipline";

interface Profile {
    profile: AspNetUser | null;
}

export default function Grades (params: Profile){
    let columns: GridColDef[] | null = null;
    
    useEffect(() => {
        getAllGrades();
        getLoggedUser();
    }, [])
    
    const [grades, setGrades] = useState<Grade[] | null>(null);
    const [loggedUser, setLoggedUser] = useState<AspNetUser | null>(null);
    const [discipline, setDiscipline] = useState<Discipline | null>(null);
    const [isOpened, setIsOpened] = useState(false);
    const [rate, setRate] = useState<string>("");
    const [selectedRowData, setSelectedRowData] = useState<any | null>(null);
    
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

    const getLoggedUser = () => {
        requests.getLoggedUser()
          .then((res) => {
             var user = res.data;
             setLoggedUser(user);
             if(UserRoles[loggedUser?.userRole as number] === "Teacher"){
              requests.getDisciplineByTeacherId(loggedUser?.id as string)
                .then((res) => {
                  var discipline = res.data;
                  setDiscipline(discipline);
                })
            }
          })
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

    if(UserRoles[loggedUser?.userRole as number] === "Teacher"){
        columns.push({
            field: "update",
            headerName: "",
            width: 90,
            renderCell: (params: GridRenderCellParams) => {
              const handleOpen = () => {
                setSelectedRowData(params.row);
                requests.getDisciplineByTeacherId(loggedUser?.id as string)
                  .then((res) => {
                    var discipline = res.data;
                    setDiscipline(discipline);
                    if (params.row.discipline.name != discipline.name) {
                        setIsOpened(false);
                        document.getElementsByClassName("updateBtn")[0]?.replaceWith("Teacher does not have permission for update");
                    }
                    else{
                      setIsOpened(true);
                    }
                  })
              };
              
              return (
                <Button
                variant="contained"
                className="updateBtn"
                startIcon={<EditIcon/>}
                onClick={handleOpen}
                sx={{ marginBottom: 2, marginTop: 2, marginLeft: 1 }}>
                </Button>
              );
            }
        })

        columns.push({
          field: "delete",
          headerName: "",
          width: 90,
          renderCell: (params: GridRenderCellParams) => {
            const handleDelete = () => {
              requests.getDisciplineByTeacherId(loggedUser?.id as string)
                .then((res) => {
                  var discipline = res.data;
                  setDiscipline(discipline);
                  if (params.row.discipline.name != discipline.name) {
                      setIsOpened(false);
                      document.getElementsByClassName("deleteBtn")[0]?.replaceWith("Teacher does not have permission for delete");
                  }
                  else{
                      requests.deleteGrade(params.row?.id)
                        .then(() => {
                           requests.getAllGrades()
                            .then((allGrades) => {
                               setGrades(allGrades.data);
                            })
                        });
                  }
                })
            };
            return (
              <Button
              variant="contained"
              color="secondary"
              id="deleteBtn"
              startIcon={<DeleteIcon />}
              onClick={handleDelete}
              sx={{ marginBottom: 2, marginTop: 2, marginLeft: 1 }}>
              </Button>
            );
          }
      })

    }

    function handleClose(){
       setIsOpened(false);
    }
    function handleSubmit(){
          var id = selectedRowData["id"];
          var studentUsername = selectedRowData.student.userName;
          var studentRate = rate;
          var studentDiscipline = discipline?.name;
  
          requests.updateGrade(id, studentUsername, studentRate, studentDiscipline)
            .then(() => {
              requests.getAllGrades()
                .then((allGrades) => {
                  setGrades(allGrades.data);
                })
            });
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
     const UpdateDialog = () => (
       <Dialog open={isOpened} onClose={handleClose}>
         <DialogTitle>Update Grade</DialogTitle>
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
      
    return(
      <Box sx={{ height: 520, width: "100%" }}>
        <UpdateDialog/>
        <DataGrid
          columns={columns!}
          rows={grades || []}
          rowHeight={48}
          checkboxSelection={false}
        />
      </Box>
    )
}