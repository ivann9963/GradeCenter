import * as React from "react";
import Box from "@mui/material/Box";
import { DataGrid, GridColDef, GridRenderCellParams } from "@mui/x-data-grid"; // Import DataGrid instead of DataGridPro
import { School } from "../../models/school";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import { SchoolClass } from "../../models/schoolClass";
import { MenuItem, Select, SelectChangeEvent } from "@mui/material";
import requests from "../../requests";

interface Collection {
  allSchools: School[] | null;
  allUsers: AspNetUser[] | null;
  allClassess: SchoolClass[] | null;
}

export default function PeopleGrid(collection: Collection | null) {
  let data: School[] | AspNetUser[] | SchoolClass[] | null = null;
  let columns: GridColDef[] | null = null;
  const [userRoles, setUserRoles] = React.useState<Record<number, UserRoles>>({});

  const handleUserRoleChange = (userId: string, event: SelectChangeEvent<UserRoles>) => {
    const selectedRole = UserRoles[event.target.value as keyof typeof UserRoles];
    setUserRoles({
      ...userRoles,
      [userId]: selectedRole,
    });

    requests.updateUser(userId, undefined, selectedRole, undefined);
    

    console.log(`Changed role of user ${userId} to ${event.target.value}`);
  };
  if (collection!.allUsers!.length > 0) {
    data = collection!.allUsers!.map((user) => ({
      ...user,
      schoolName: user.school?.name,
    }));

    columns = [
      { field: "firstName", headerName: "First name", width: 130 },
      { field: "lastName", headerName: "Last name", width: 130 },
      { field: "schoolName", headerName: "School", width: 90 },
      { field: "isActive", headerName: "Active", width: 90 },
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
              {Object.values(UserRoles).filter(value => typeof value === 'string').map((role) => (
                <MenuItem key={role} value={role}>
                  {role}
                </MenuItem>
              ))}
            </Select>
          );
        },
      },
    ];
  }

  if (collection!.allSchools!.length > 0) {
    data = collection!.allSchools;
    columns = [
      { field: "name", headerName: "Name", width: 100 },
      { field: "address", headerName: "Address", width: 150 },
      { field: "isActive", headerName: "Active", width: 100 },
    ];
  }

  if (collection!.allClassess!.length > 0) {
    data = collection!.allClassess!.map((user) => ({
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

  console.log(collection?.allClassess);

  return (
    <Box sx={{ height: 520, width: "100%" }}>
      <DataGrid
        columns={columns!}
        rows={data || []}
        loading={data!.length === 0}
        rowHeight={38}
        checkboxSelection // disableSelectionOnClick
      />
    </Box>
  );
}
