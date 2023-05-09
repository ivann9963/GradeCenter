import { Button, TextField, Paper, Grid, Container, Typography } from "@mui/material";
import axios from "axios";
import React from "react";

export default function Register() {
  const [userName, setUserName] = React.useState("");
  const [email, setEmail] = React.useState("");
  const [password, setPassword] = React.useState("");

  const onChangeUserName = (e: any) => {
    setUserName(e.target.value);
  };

  const onChangeEmail = (e: any) => {
    setEmail(e.target.value);
  };

  const onChangePassword = (e: any) => {
    setPassword(e.target.value);
  };

  const onRegister = () => {
    const url = `https://localhost:7273/api/Account/Register`;

    axios({
      method: "post",
      url: url,
      params: {
        userName: userName,
        email: email,
        password: password,
      },
      headers: {
        "Content-Type": "text/plain;charset=utf-8",
      },
    }).then((res) => {
      window.location.href = "/login";
    })
  };

  return (
    <Container maxWidth="sm">
      <Grid container justifyContent="center" alignItems="center" style={{ minHeight: "70vh" }}>
        <Grid item xs={12} sm={8} md={6} lg={10}>
          <Paper style={{ padding: 24 }} elevation={20}>
            <Typography variant="h4" component="h1" align="center">
              Register
            </Typography>
            <TextField
              onChange={onChangeUserName}
              fullWidth
              margin="normal"
              label="User Name"
              variant="outlined"
            />
            <TextField
              onChange={onChangeEmail}
              fullWidth
              margin="normal"
              label="Email"
              variant="outlined"
            />
            <TextField
              onChange={onChangePassword}
              fullWidth
              margin="normal"
              label="Password"
              type="password"
              variant="outlined"
            />
            <Grid container justifyContent="flex-end" spacing={2} style={{ marginTop: 24 }}>
              <Grid item>
                <Button variant="outlined" href="/login">Cancel</Button>
              </Grid>
              <Grid item>
                <Button variant="contained" color="primary" onClick={onRegister}>
                  Confirm
                </Button>
              </Grid>
            </Grid>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}