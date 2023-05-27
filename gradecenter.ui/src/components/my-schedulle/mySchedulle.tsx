import * as React from "react";
import { Card, CardContent, Container, Typography } from "@mui/material";
import { useEffect } from "react";
import requests from "../../requests";
import Discipline from "../../models/discipline";

export enum Days {
  Monday = 1,
  Tuesday = 2,
  Wednesday = 3,
  Thursday = 4,
  Friday = 5,
}

export default function MySchedulle() {
  const [mySchedulle, setMySchedulle] = React.useState<Discipline[] | null>(null);

  useEffect(() => {
    requests
      .getLoggedUserCurricullum()
      .then((response) => {
        setMySchedulle(response.data);
      })
      .catch((error) => {
        // console.log(error)
      });
  }, []);

  // Create an array for each day of the week
  const dayColumns: { [day in Days]: { name: string, time: string }[] } = {
    [Days.Monday]: [],
    [Days.Tuesday]: [],
    [Days.Wednesday]: [],
    [Days.Thursday]: [],
    [Days.Friday]: [],
  };

  // Sort classes into the correct day list
  mySchedulle?.forEach((discipline) => {
    const day: Days = discipline.occuranceDay as unknown as number as Days;

    dayColumns[day].push({ name: discipline.name, time: discipline.occuranceTime as string });
  });

  // Sort the discipline times within each day
  for (const day in dayColumns) {
    dayColumns[day as unknown as Days].sort((a, b) => a.time.localeCompare(b.time));
  }

  return (
    <Container style={{ padding: "4em 6em", marginLeft: 270 }}>
      <div style={{ display: "flex", justifyContent: "space-between", padding: "1em", gap: "1em" }}>
        {Object.entries(dayColumns).map(([day, disciplines]) => (
          <Card key={day} style={{ 
            width: "19%", 
            backgroundColor: "#f3f3f3", 
            borderRadius: "15px", 
            boxShadow: "0 4px 8px 0 rgba(0,0,0,0.2)"
          }}>
            <CardContent>
              <Typography color="textSecondary" gutterBottom variant="h5" style={{ fontWeight: "bold" }}>
                {Days[day as unknown as number]}
              </Typography>
              {disciplines.map((discipline, index) => (
                <Typography variant="body2" component="p" key={index} style={{ marginBottom: "0.5em" }}>
                  {discipline.name} - {discipline.time}
                </Typography>
              ))}
            </CardContent>
          </Card>
        ))}
      </div>
    </Container>
  );
}
