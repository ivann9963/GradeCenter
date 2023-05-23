import { Container, Typography, Box, Tab, Tabs, Paper } from "@mui/material";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import requests from "../../requests";
import { School } from "../../models/school";

export default function SchoolDetails() {
  let { schoolId } = useParams();
  const [school, setSchool] = useState<School | null>(null);
  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    if (schoolId) {
      requests.getSchoolById(schoolId)
        .then((response) => {
          setSchool(response.data);
        })
        .catch((error) => {});
    }
  }, [schoolId]);

  const handleTabChange = (event: React.ChangeEvent<{}>, newValue: number) => {
    setTabValue(newValue);
  };

  // Render the school data
  return (
    <Container style={{ padding: "4em 6em", marginLeft: 270 }}>
      {school ? (
        <Box>
          <Typography variant="h4">{school.name}</Typography>
          <Typography variant="h6">{school.address}</Typography>
          <Paper elevation={10}>
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label="People" />
              <Tab label="School Classes" />
            </Tabs>
            {tabValue === 0 && (
              <Box p={3}>
                {/* Render People data here */}
                <Typography>People data goes here...</Typography>
              </Box>
            )}
            {tabValue === 1 && (
              <Box p={3}>
                {/* Render School Classes data here */}
                <Typography>School Classes data goes here...</Typography>
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
