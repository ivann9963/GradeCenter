import { Box, Container, Paper, Typography } from "@mui/material";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { useEffect, useState } from "react";
import requests from "../../requests";
import Statistic from "../../models/statistic";

interface ClassesStatisticsParams {
    data: Statistic[] | null
}

export default function TeacherStatisticsGrid(params: ClassesStatisticsParams) {
    let columns: GridColDef[] | null = null;
    columns = [
        { field: "name", headerName: "Teacher", width: 100 },
        { field: "disciplineName", headerName: "Discipline", width: 100 },
        { field: "averageRate", headerName: "Average Rate", width: 150 },
        { field: "comparedToLastWeek", headerName: "Since last week (%)", width: 150 },
        { field: "comparedToLastMonth", headerName: "Since last month (%)", width: 150 },
        { field: "comparedToLastYear", headerName: "Since last year (%)", width: 150 },
    ];

    return (
        <Box sx={{ height: 520, width: "100%" }}>
            <DataGrid
                columns={columns!}
                rows={params.data || []}
                loading={false}
                rowHeight={48}
                checkboxSelection={false}
            />
        </Box>
    );
}
