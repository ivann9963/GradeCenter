import { DataGrid, GridColDef, GridValueGetterParams, GridValueSetterParams } from "@mui/x-data-grid";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import { useEffect, useState } from "react";
import { UserRelation } from "../../models/userRelation";
import requests from "../../requests";
import { Box } from "@mui/material";

interface Profile {
    profile: AspNetUser | null;
}

const columns: GridColDef[] = [
    {
      field: 'child',
      headerName: 'Child',
      width: 150,
      editable: false,
      valueGetter: (params: GridValueGetterParams) =>
        `${params.row.child.firstName}`
    },
    {
      field: 'parent',
      headerName: 'Parent',
      width: 110,
      editable: true,
      valueGetter: (params: GridValueGetterParams) =>
        `${params.row.parent.firstName}`
    }
];

export default function Family(params : Profile){
    const [userRelations, setUserRelations] = useState<UserRelation[] | null>(null);
    
    console.log(params.profile);
    useEffect(() => {
        getAllUserRelationsById();
    }, [])

    const getAllUserRelationsById = () => {
            requests.getUserById(params.profile?.id)
                .then(res => {
                    switch(UserRoles[params.profile?.userRole as number])
                    {
                        case "Student":
                            var userRelations = res.data.parentRelations
                                .map((record: any) => {
                                    return {child: params.profile, parent: record.parent};
                                });
                            setUserRelations(userRelations);
                            break;
                        case "Parent":
                            var userRelations = res.data.childrenRelations
                            .map((record: any) => {
                                return {parent: params.profile, child: record.child};
                            });
                            setUserRelations(userRelations)
                            break;
                        default:
                            break;
                    }
                })
    }

    function generateRandom() {
        var length = 8,
            charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            retVal = "";
        for (var i = 0, n = charset.length; i < length; ++i) {
            retVal += charset.charAt(Math.floor(Math.random() * n));
        }
        return retVal;
    }

    return (
        <Box sx={{ height: 400, width: '100%' }}>
          <DataGrid
            getRowId={(row: any) =>  generateRandom()}
            rows={userRelations || []}
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