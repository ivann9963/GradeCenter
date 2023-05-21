import * as React from 'react';
import Box from '@mui/material/Box';
import { DataGrid, GridColDef, GridValueGetterParams } from '@mui/x-data-grid';
import { useState, useEffect } from 'react';
import axios from 'axios';
import { parse } from 'path';

const userRoles = ["Admin", "Principal", "Teacher", "Parent", "Student"];
const url = 'https://localhost:7273/api/Account/GetAllUsers';

const columns: GridColDef[] = [
    {
        field: 'firstName',
        headerName: 'First name',
        width: 300,
        editable: true,
    },
    {
        field: 'lastName',
        headerName: 'Last name',
        width: 300,
        editable: true,
    },
    {
        field: 'userRole',
        headerName: 'Role',
        width: 150,
        editable: true,
    },

];

export default function Users() {

    const [users, setUsers] = useState([]);

    useEffect(() => {
        getAllUsers();
    }, []);

    const getAllUsers = () => {

        axios({
            method: "get",
            url: url,
            headers: {
                "Content-Type": "text/plain;charset=utf-8",
            },
        }).then((res) => {
            var parsedUsers = res.data.map((user: { [x: string]: any; }) => {
            return {
                "id": user["id"],
                "firstName": user["firstName"],
                "lastName": user["lastName"],
                "userRole": userRoles[user["userRole"]]
            }
           })
           console.log(parsedUsers);
            setUsers(parsedUsers);
        });
    };

    return (
        <Box sx={{ height: 400, width: '50%' }} marginLeft={50} marginTop={10}>
            <DataGrid
                rows={users}
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