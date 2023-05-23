import * as React from 'react';
import Box from '@mui/material/Box';
import { DataGridPro, GridColDef } from '@mui/x-data-grid-pro';
import { School } from '../../models/school';
import { AspNetUser, UserRoles } from '../../models/aspNetUser';

interface Collection {
  allSchools: School[] | null
  allUsers: AspNetUser[] | null
}

export default function PeopleGrid(collection: Collection | null) {
  let data: School[] | AspNetUser[] | null = null;
  let columns: GridColDef[] | null = null;
  
  if(collection?.allSchools) {
    data = collection?.allSchools;
    console.log(data);
    
    columns = [
      { field: 'name', headerName: 'Name', width: 100 },
      { field: 'address', headerName: 'Address', width: 150 },
      { field: 'isActive', headerName: 'Active', width: 100 },
    ]
  }  else if (collection?.allUsers) {
    data = collection.allUsers.map(user => ({
      ...user,
      schoolName: user.school?.name,
    }));

    console.log(data);

    columns = [
      { field: 'firstName', headerName: 'First name', width: 130 },
      { field: 'lastName', headerName: 'Last name', width: 130 },
      { field: 'schoolName', headerName: 'School', width: 90 },
      { field: 'isActive', headerName: 'Active', width: 90 },
      { 
        field: 'userRole', 
        headerName: 'User Role', 
        width: 130,
        valueFormatter: ({ value }) => UserRoles[value as UserRoles]
      },
    ]
  }

  return (
    <Box sx={{ height: 520, width: '100%' }}>
      <DataGridPro
      columns={columns!} 
      rows={data || []} 
      loading={collection!.allSchools?.length === 0}
      rowHeight={38}
      checkboxSelection        // disableSelectionOnClick
      />
    </Box>
  );
}
