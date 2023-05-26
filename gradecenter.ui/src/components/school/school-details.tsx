import { Container, Typography, Box, Tab, Tabs, Paper } from "@mui/material";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import requests from "../../requests";
import { School } from "../../models/school";
import AllClassessGrid from "../admin-panel/alllClassessGrid";
import AllUsersGrid from "../admin-panel/allUsersGrid";
import { AspNetUser } from "../../models/aspNetUser";
import { SchoolClass } from "../../models/schoolClass";

export default function SchoolDetails() {
  let { schoolId } = useParams();
  const [school, setSchool] = useState<School | null>(null);
  const [tabValue, setTabValue] = useState(0);
  const [schoolPeople, setSchoolPeople] = useState<AspNetUser[] | null>([]);
  const [schoolClasses, setSchoolClasses] = useState<SchoolClass[] | null>([]);

  useEffect(() => {
    if (schoolId) {
      requests
        .getSchoolById(schoolId)
        .then((response) => {
          setSchool(response.data);
        })
        .catch((error) => {
          console.log(error);
        });

      requests
        .getPeopleInSchool(schoolId)
        .then((response) => {
          setSchoolPeople(response.data);
          console.log(schoolPeople);
        })
        .catch((error) => {
          console.log(error);
        });

      requests.getClassessInSchool(schoolId)
      .then((response) => {
        setSchoolClasses(response.data);
      })
      .catch((error) => {});
    }
  }, [schoolId]);

  const handleTabChange = (event: React.ChangeEvent<{}>, newValue: number) => {
    setTabValue(newValue);
  };

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
                <Typography><AllUsersGrid allUsers={schoolPeople} /></Typography>
              </Box>
            )}
            {tabValue === 1 && (
              <Box p={3}>
                <Typography><AllClassessGrid allClassess={schoolClasses} /></Typography>
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
