import { Box, Container, Paper, Tab, Tabs, Typography } from "@mui/material";
import PeopleGrid from "../school/people-grid";
import { useEffect, useState } from "react";
import { School } from "../../models/school";
import requests from "../../requests";
import { AspNetUser } from "../../models/aspNetUser";

export default function AdminPanel() {
  const [schools, setSchool] = useState<School[] | null>(null);
  const [allUsers, setAllusers] = useState<AspNetUser[] | null>(null);
  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    requests
      .getAllSchools()
      .then((response) => {
        setSchool(response.data);
      })
      .catch((error) => {
        console.log(error);
      });

    requests
      .getAllUsers()
      .then((response) => {
        setAllusers(response.data);
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
      {schools || allUsers ? (
        <Box>
          <Typography variant="h4">{"Admin Panel"}</Typography>
          <br />
          <Paper elevation={10}>
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label="All Users" />
              <Tab label="All Schools" />
            </Tabs>
            {tabValue === 0 && (
              <Box p={3}>
                {/* Render People data here */}
                <Typography>
                  <PeopleGrid allUsers={allUsers} allSchools={null} />
                </Typography>
              </Box>
            )}
            {tabValue === 1 && (
              <Box p={3}>
                <Typography>
                  <PeopleGrid allSchools={schools} allUsers={null} />
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
