import * as React from "react";
import Box from "@mui/material/Box";
import { DataGrid, GridColDef, GridRenderCellParams } from "@mui/x-data-grid"; // Import DataGrid instead of DataGridPro
import { School } from "../../models/school";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import { SchoolClass } from "../../models/schoolClass";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, MenuItem, Select, SelectChangeEvent, TextField } from "@mui/material";
import requests from "../../requests";


interface AllUsersGridParams {
    allUsers: AspNetUser[] | null
}

export default function AllUsersGrid(params: AllUsersGridParams) {
  let data: School[] | AspNetUser[] | SchoolClass[] | null = null;
  let columns: GridColDef[] | null = null;
  const [userRoles, setUserRoles] = React.useState<Record<number, UserRoles>>({});
  const [open, setOpen] = React.useState(false);
  const [currentParentId, setCurrentParentId] = React.useState<string | null>(null);
  const [firstName, setFirstName] = React.useState('');
  const [lastName, setLastName] = React.useState('');
  
  const handleUserRoleChange = (userId: string, event: SelectChangeEvent<UserRoles>) => {
    const selectedRole = UserRoles[event.target.value as keyof typeof UserRoles];
    setUserRoles({
      ...userRoles,
      [userId]: selectedRole,
    });

    requests.updateUser(userId, undefined, selectedRole, undefined, undefined);
  };

  const handleSubmit = async () => {
    // Send your API request here using firstName and lastName
    await requests.AddChild(currentParentId!, firstName, lastName);
    // Clear the form fields and close the dialog
    setFirstName('');
    setLastName('');
    setCurrentParentId(null);
    setOpen(false);
  };
  
  const ChildAddDialog = () => (
    <Dialog open={open} onClose={() => setOpen(false)}>
      <DialogTitle>Add Child</DialogTitle>
      <DialogContent>
        <form>
          <TextField
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            label="First Name"
            fullWidth
          />
          <TextField
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            label="Last Name"
            fullWidth
          />
        </form>
      </DialogContent>
      <DialogActions>
        <Button onClick={() => setOpen(false)}>Cancel</Button>
        <Button onClick={handleSubmit}>Save</Button>
      </DialogActions>
    </Dialog>
  );
  
  if (params && params.allUsers && params!.allUsers!.length > 0) {
    data = params!.allUsers!.map((user) => ({
      ...user,
      schoolName: user.school?.name,
    }));

    columns = [
      { field: "firstName", headerName: "First name", width: 130 },
      { field: "lastName", headerName: "Last name", width: 130 },
      { field: "schoolName", headerName: "School", width: 90 },
      {
        field: "isActive",
        headerName: "Status",
        width: 90,
        renderCell: (params: GridRenderCellParams) => {
          const toggleStatus = () => {
            const userId = params.id as string;
            const newStatus = !params.value;

            requests.updateUser(userId, undefined, undefined, newStatus, undefined);
          };

          return (
            <Button size="small" variant="contained" sx={{ borderRadius: '12%', height: 40, fontSize: 12 }} color={params.value ? "success" : "error"} onClick={toggleStatus}>
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
        field: "",
        headerName: "Actions",
        renderCell: (params: GridRenderCellParams) => {
          const userRoleKey = UserRoles[params.row.userRole as keyof typeof UserRoles];
          console.log(userRoleKey);
          if (userRoleKey.toLocaleString() !== 'Parent') {
            return null;
          }
          return (
            <Button size="small" variant="contained" sx={{ borderRadius: '10%', height: 40, fontSize: 13 }} color={"primary"} 
            onClick={() => {
              setOpen(true);
              setCurrentParentId(params.id as string);
            }}>
              <h5>Add child</h5>
            </Button>
          );
        },
      },
    ];
  }

  return (
    <Box sx={{ height: 520, width: "100%" }}>
      <DataGrid
        columns={columns!}
        rows={data || []}
        loading={data!.length === 0}
        rowHeight={48}
        checkboxSelection={false}
      />
     <ChildAddDialog />
    </Box>
  );
}
