import { Box, Container, Paper, Tab, Tabs, Typography } from "@mui/material";
import AllClassessGrid from "./alllClassessGrid";
import { useEffect, useState } from "react";
import { School } from "../../models/school";
import requests from "../../requests";
import { AspNetUser } from "../../models/aspNetUser";
import { SchoolClass } from "../../models/schoolClass";
import AllUsersGrid from "./allUsersGrid";
import AllSchoolsGrid from "./allSchoolsGrid";

export default function AdminPanel() {
  const [schools, setSchool] = useState<School[] | null>(null);
  const [allUsers, setAllusers] = useState<AspNetUser[] | null>(null);
  const [schoolClasses, setSchoolClasses] = useState<SchoolClass[] | null>(null);

  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    requests
      .getAllUsers()
      .then((response) => {
        setAllusers(response.data);
      })
      .catch((error) => {
        console.log(error);
      });

    requests
      .getAllSchools()
      .then((response) => {
        setSchool(response.data);
      })
      .catch((error) => {
        console.log(error);
      });

      requests
      .getAllSchoolsClassess()
      .then((response) => {
        setSchoolClasses(response.data);
      })
      .catch((error) => {
        console.log(error);
      });
  }, []);

  const handleTabChange = (event: React.ChangeEvent<{}>, newValue: number) => {
    setTabValue(newValue);
  };

  return (
    <Container style={{ padding: "4em 6em", marginLeft: 270 }}>
      {schools || allUsers || schoolClasses ? (
        <Box>
          <Typography variant="h4">{"Admin Panel"}</Typography>
          <br />
          <Paper elevation={10}>
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label="All Users" />
              <Tab label="All Schools" />
              <Tab label="All Classess" />
            </Tabs>
            {tabValue === 0 && (
              <Box p={3}>
                {/* Render People data here */}
                <Typography>
                  <AllUsersGrid allUsers={allUsers} />
                </Typography>
              </Box>
            )}
            {tabValue === 1 && (
              <Box p={3}>
                <Typography>
                  <AllSchoolsGrid allSchools={schools} />
                </Typography>
              </Box>
            )}
              {tabValue === 2 && (
              <Box p={3}>
                <Typography>
                  <AllClassessGrid allClassess={schoolClasses} />
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
