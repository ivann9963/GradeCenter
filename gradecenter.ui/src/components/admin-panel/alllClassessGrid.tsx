import * as React from "react";
import Box from "@mui/material/Box";
import { DataGrid, GridColDef, GridRenderCellParams } from "@mui/x-data-grid"; // Import DataGrid instead of DataGridPro
import { School } from "../../models/school";
import { AspNetUser } from "../../models/aspNetUser";
import { SchoolClass } from "../../models/schoolClass";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from "@mui/material";
import requests from "../../requests";
import TransferList from "./curricullumTransferList";
import Discipline from "../../models/discipline";

interface AllClassessGridParams {
  allClassess: SchoolClass[] | null;
}

export default function AllClassessGrid(params: AllClassessGridParams | null) {
  let data: School[] | AspNetUser[] | SchoolClass[] | null = null;
  let columns: GridColDef[] | null = null;
  const [createSchoolClassOpen, setCreateSchoolClassOpen] = React.useState(false);
  const [openTransferList, setOpenTransferList] = React.useState(false);
  const [selectedRowData, setSelectedRowData] = React.useState(null);
  const [selectedItems, setSelectedItems] = React.useState<readonly string[]>([]);
  const [left, setLeft] = React.useState<readonly string[]>([
    "Math",
    "Science",
    "English",
    "History",
    "Music",
    "Art",
    "Physical Education",
    "Computer Science",
  ]);

  const newClassRef = React.useRef<HTMLInputElement | null>(null);
  const schoolNameRef = React.useRef<HTMLInputElement | null>(null);
  const teacherNamesRef = React.useRef<HTMLInputElement | null>(null);

  const [right, setRight] = React.useState<readonly string[]>([]);


  const TransferListDialog = () => {
    const handleCloseTransferList = () => {
      setOpenTransferList(false);
    };

    const SubmitCurricullum = () => {
      setOpenTransferList(false);

      const disciplines: Discipline[] = [];

      const schoolClassId = (selectedRowData as any).id;
      const headTeacherId = (selectedRowData as any).headTeacher.id;
      
      selectedItems.forEach((i) => {
        const discipline: Discipline = new Discipline(i, schoolClassId, headTeacherId);

        disciplines.push(discipline);
      });

      requests.createCurricullum(disciplines);
    };

    return (
      <Dialog open={openTransferList} onClose={handleCloseTransferList}>
        <DialogTitle>Transfer List</DialogTitle>
        <DialogContent>
          {selectedRowData && (
            <TransferList
              rowData={selectedRowData}
              onClose={handleCloseTransferList}
              setSelectedItems={setSelectedItems}
              left={left}
              setLeft={setLeft}
              right={right}
              setRight={setRight}
            />
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseTransferList}>Cancel</Button>
          <Button onClick={SubmitCurricullum}>Save</Button>
        </DialogActions>
      </Dialog>
    );
  };

  const CreateSchoolClassButton = () => (
    <Button
      variant="outlined"
      sx={{ marginBottom: 2, marginTop: -2, marginLeft: 1 }}
      onClick={() => setCreateSchoolClassOpen(true)}
    >
      + New
    </Button>
  );

  const submitCreateNewClass = () => {
    var year = newClassRef.current?.value.length == 2 ? newClassRef.current.value[0] as unknown as number :
             `${newClassRef.current?.value[0]}${newClassRef.current?.value[1]}` as unknown as number;

    var department = newClassRef.current?.value.length == 2 ? newClassRef.current.value[1] : newClassRef.current?.value[2];
    var schoolName = schoolNameRef.current?.value;

    var teacherNames = teacherNamesRef.current?.value;

    requests.createSchoolClass(year, department, schoolName, teacherNames);
  };

  const CreateSchoolClassDialog = () => (
    <Dialog open={createSchoolClassOpen} onClose={() => setCreateSchoolClassOpen(false)}>
      <DialogTitle>Create School Class</DialogTitle>
      <DialogContent>
        <TextField inputRef={newClassRef} label="Ex: '8A'.." fullWidth />
        <br />
        <br />
        <TextField inputRef={schoolNameRef} label="School Name.." fullWidth />
        <br />
        <br />
        <TextField inputRef={teacherNamesRef} label="Teacher First and Last names.." fullWidth />
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
      {
        field: "year",
        headerName: "Year",
        width: 100,
        valueGetter: (params) => `${params.row.year}${params.row.department}`,
      },
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
      {
        field: "-",
        headerName: "Curricullum",
        width: 90,
        renderCell: (params: GridRenderCellParams) => {
          const handleOpenTransferList = () => {
            setSelectedRowData(params.row);
            setOpenTransferList(true);
          };

          return (
            <Button
              size="small"
              variant="contained"
              sx={{ borderRadius: "12%", height: 40, fontSize: 12 }}
              color={"info"}
              onClick={handleOpenTransferList}
            >
              <h4>Change</h4>
            </Button>
          );
        },
      },
    ];
  }

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
      <TransferListDialog />
    </Box>
  );
}
