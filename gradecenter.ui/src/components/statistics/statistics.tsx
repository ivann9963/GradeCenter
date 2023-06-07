import { Box, Container, Paper, Tab, Tabs, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import requests from "../../requests";
import Statistic from "../../models/statistic";
import SchoolStatisticsGrid from "./schoolStatisticsGrid";
import ClassesStatisticsGrid from "./classesStatisticsGrid";
import TeacherStatisticsGrid from "./teacherStatisticsGrid";

export default function Statistics() {
    const [schoolStatistics, setSchoolStatistics] = useState<Statistic[] | null>(null);
    const [classesStatistics, setClassesStatistics] = useState<Statistic[] | null>(null);
    const [teacherStatistics, setTeacherStatistics] = useState<Statistic[] | null>(null);

    useEffect(() => {
        requests.getSchoolStatistics().then((response) => {

            const mappedData: Statistic[] = response.data.map((item: any) => {

                const statistic: Statistic = new Statistic(
                    item.id,
                    item.school ? item.school.name : '',
                    item.disciplineName,
                    item.averageRate,
                    item.comparedToLastWeek,
                    item.comparedToLastMonth,
                    item.comparedToLastYear,
                )

                return statistic;
            });

            setSchoolStatistics(mappedData);
        });

        requests.getClassesStatistics().then((response) => {

            const mappedData: Statistic[] = response.data.map((item: any) => {

                const statistic: Statistic = new Statistic(
                    item.id,
                    item.schoolClass ? item.schoolClass.year + item.schoolClass.department : '',
                    item.disciplineName,
                    item.averageRate,
                    item.comparedToLastWeek,
                    item.comparedToLastMonth,
                    item.comparedToLastYear,
                )

                return statistic;
            });

            setClassesStatistics(mappedData);
        });

        requests.getTeachersStatistics().then((response) => {

            const mappedData: Statistic[] = response.data.map((item: any) => {

                const statistic: Statistic = new Statistic(
                    item.id,
                    item.teacher ? `${item.teacher.firstName} ${item.teacher.lastName}` : '',
                    item.disciplineName,
                    item.averageRate,
                    item.comparedToLastWeek,
                    item.comparedToLastMonth,
                    item.comparedToLastYear,
                )

                return statistic;
            });
            console.log(response.data);
            setTeacherStatistics(mappedData);
        });
    }, []);

    const [tabValue, setTabValue] = useState(0);

    const handleTabChange = (event: React.ChangeEvent<{}>, newValue: number) => {
        setTabValue(newValue);
    };

    return (
        <Container style={{ padding: "4em 6em", marginLeft: 270 }}>
            {schoolStatistics || classesStatistics || teacherStatistics ? (
                <Box>
                    <Typography variant="h4">{"Statistics"}</Typography>
                    <br />
                    <Paper elevation={10}>
                        <Tabs value={tabValue} onChange={handleTabChange}>
                            <Tab label="Schools" />
                            <Tab label="Classess" />
                            <Tab label="Teachers" />
                        </Tabs>
                        {tabValue === 0 && (
                            <Box p={3}>
                                <Typography>
                                    <SchoolStatisticsGrid data={schoolStatistics} />
                                </Typography>
                            </Box>
                        )}
                        {tabValue === 1 && (
                            <Box p={3}>
                                <Typography>
                                    <ClassesStatisticsGrid data={classesStatistics} />
                                </Typography>
                            </Box>
                        )}
                        {tabValue === 2 && (
                            <Box p={3}>
                                <Typography>
                                    <TeacherStatisticsGrid data={teacherStatistics} />
                                </Typography>
                            </Box>
                        )}
                    </Paper>
                </Box>
            ) : (
                <Typography variant="h4">Loading...</Typography>
            )}
        </Container>
    );
}
