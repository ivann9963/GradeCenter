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
  DialogTitle,
  MenuItem,
  Select,
  SelectChangeEvent,
  TextField,
} from "@mui/material";
import requests from "../../requests";

interface AllUsersGridParams {
  allUsers: AspNetUser[] | null;
}

export default function AllUsersGrid(params: AllUsersGridParams) {
  let data: School[] | AspNetUser[] | SchoolClass[] | null = null;
  let columns: GridColDef[] | null = null;
  const [userRoles, setUserRoles] = React.useState<Record<number, UserRoles>>({});
  const [childOpen, setChildOpen] = React.useState(false);
  const [currentParentId, setCurrentParentId] = React.useState<string | null>(null);
  const [schoolOpen, setSchoolOpen] = React.useState(false);
  const [enrollOpen, setEnrollOpen] = React.useState(false);

  const firstNameRef = React.useRef<HTMLInputElement | null>(null);
  const lastNameRef = React.useRef<HTMLInputElement | null>(null);
  const schoolNameRef = React.useRef<HTMLInputElement | null>(null);
  const newClassNameRef = React.useRef<HTMLInputElement | null>(null);

  const handleUserRoleChange = (userId: string, event: SelectChangeEvent<UserRoles>) => {
    const selectedRole = UserRoles[event.target.value as keyof typeof UserRoles];
    setUserRoles({
      ...userRoles,
      [userId]: selectedRole,
    });

    requests.updateUser(userId, undefined, selectedRole, undefined, undefined);
  };

  const SubmitAddChild = async () => {
    // Send your API request here using firstName and lastName
    var firstName = firstNameRef.current?.value;
    var lastName = lastNameRef.current?.value;

    await requests.addChild(currentParentId!, firstName, lastName);
    // Clear the form fields and close the dialog
    setCurrentParentId(null);
    setChildOpen(false);
  };

  const ChildAddDialog = () => (
    <Dialog open={childOpen} onClose={() => setChildOpen(false)}>
      <DialogTitle>Add Child</DialogTitle>
      <DialogContent>
        <form>
          <TextField inputRef={firstNameRef} label="First Name" fullWidth />
          <br />
          <br />
          <TextField inputRef={lastNameRef} label="Last Name" fullWidth />
        </form>
      </DialogContent>
      <DialogActions>
        <Button onClick={() => setChildOpen(false)}>Cancel</Button>
        <Button onClick={SubmitAddChild}>Save</Button>
      </DialogActions>
    </Dialog>
  );

  const SubmitChangeSchool = async () => {
    // Send your API request here using firstName and lastName
    await requests.changeSchool(schoolNameRef.current?.value, currentParentId!);
    // Clear the form fields and close the dialog
    setCurrentParentId(null);
    setSchoolOpen(false);
  };

  const SubmitEnrollForClass = () => {
    requests.enroll(currentParentId!, newClassNameRef.current?.value);

    setCurrentParentId(null);
    setEnrollOpen(false);
  };

  const SubmitWithdrawFromClass = (userId: string) => {
    requests.withdraw(userId!);

    setCurrentParentId(null);
  };

  const ChangeSchoolDialog = () => (
    <Dialog open={schoolOpen} onClose={() => setSchoolOpen(false)}>
      <DialogTitle>Change School</DialogTitle>
      <DialogContent>
        <form>
          <TextField inputRef={schoolNameRef} label="New School.." fullWidth />
        </form>
      </DialogContent>
      <DialogActions>
        <Button onClick={() => setSchoolOpen(false)}>Cancel</Button>
        <Button onClick={SubmitChangeSchool}>Save</Button>
      </DialogActions>
    </Dialog>
  );

  const EnrollDialog = () => (
    <Dialog open={enrollOpen} onClose={() => setEnrollOpen(false)}>
      <DialogTitle>Enroll for Class</DialogTitle>
      <DialogContent>
        <form>
          <TextField inputRef={newClassNameRef} label="New Class.." fullWidth />
        </form>
      </DialogContent>
      <DialogActions>
        <Button onClick={() => setEnrollOpen(false)}>Cancel</Button>
        <Button onClick={SubmitEnrollForClass}>Save</Button>
      </DialogActions>
    </Dialog>
  );

  columns = [
    { field: "firstName", headerName: "First name", width: 130 },
    { field: "lastName", headerName: "Last name", width: 130 },
    { field: "schoolName", headerName: "School", width: 90 },
    {
      field: "schoolClass.year",
      headerName: "Class",
      width: 90,
      valueGetter: (params) => params.row.schoolClass?.year + params.row.schoolClass?.department ?? '-',
    },
    {
      field: "isActive",
      headerName: "Status",
      width: 90,
      renderCell: (params: GridRenderCellParams) => {
        const toggleStatus = () => {
          const userId = params.id as string;
          const newStatus = !params.value;

          requests.updateUser(userId, null, null, newStatus, null);
        };

        return (
          <Button
            size="small"
            variant="contained"
            sx={{ borderRadius: "12%", height: 40, fontSize: 12 }}
            color={params.value ? "success" : "error"}
            onClick={toggleStatus}
          >
            <h4>{params.value ? "Active" : "Inactive"}</h4>
          </Button>
        );
      },
    },
    {
      field: "userRole",
      headerName: "User Role",
      width: 130,
      renderCell: (params: GridRenderCellParams) => {
        const userRoleKey = UserRoles[params.value as keyof typeof UserRoles];
        return (
          <Select
            value={userRoles[params.id as number] || userRoleKey}
            onChange={(event) => handleUserRoleChange(params.id as string, event)}
          >
            {Object.values(UserRoles)
              .filter((value) => typeof value === "string")
              .map((role) => (
                <MenuItem key={role} value={role}>
                  {role}
                </MenuItem>
              ))}
          </Select>
        );
      },
    },
    {
      field: "-",
      headerName: "-",
      renderCell: (params: GridRenderCellParams) => {
        const userRoleKey = UserRoles[params.row.userRole as keyof typeof UserRoles];

        if (userRoleKey.toLocaleString() !== "Parent") {
          return null;
        }

        return (
          <Button
            size="small"
            variant="contained"
            sx={{ borderRadius: "10%", height: 40, fontSize: 13 }}
            color={"info"}
            onClick={() => {
              setChildOpen(true);
              setCurrentParentId(params.id as string);
            }}
          >
            <h5>Add child</h5>
          </Button>
        );
      },
    },
    {
      field: "",
      headerName: "-",
      renderCell: (params: GridRenderCellParams) => {
        return (
          <Button
            size="small"
            variant="contained"
            sx={{ borderRadius: "10%", height: 40, fontSize: 10 }}
            color={"primary"}
            onClick={() => {
              setSchoolOpen(true);
              setCurrentParentId(params.row.id as string);
            }}
          >
            <h5>Change School</h5>
          </Button>
        );
      },
    },
    {
      field: "~",
      headerName: "-",
      renderCell: (params: GridRenderCellParams) => {
        return (
          <Button
            size="small"
            variant="contained"
            sx={{ borderRadius: "10%", height: 40, fontSize: 10 }}
            color={params.row.schoolClass ? "warning" : "secondary"}
            onClick={() => {
              if (params.row.schoolClass) {
                SubmitWithdrawFromClass(params.row.id as string);
              } else {
                setCurrentParentId(params.row.id as string);
                setEnrollOpen(true);
              }
            }}
          >
            <h4>{params.row.schoolClass ? "Withdraw" : "Enroll"}</h4>
          </Button>
        );
      },
    },
  ];

  if (params && params.allUsers && params!.allUsers!.length > 0) {
    data = params!.allUsers!.map((user) => ({
      ...user,
      schoolName: user.school?.name,
    }));
  }

  return (
    <Box sx={{ height: 520, width: "100%" }}>
      <DataGrid
        columns={columns!}
        rows={data || []}
        rowHeight={48}
        checkboxSelection={false}
      />
      <ChildAddDialog />
      <ChangeSchoolDialog />
      <EnrollDialog />
    </Box>
  );
}
