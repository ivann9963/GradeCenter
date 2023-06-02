import React from 'react';
import IconButton from '@material-ui/core/IconButton';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import PersonIcon from '@material-ui/icons/Person';
import { makeStyles } from '@material-ui/core/styles';
import { useNavigate } from 'react-router-dom';

const useStyles = makeStyles((theme) => ({
  menu: {
    marginTop: theme.spacing(2),
  },
  menuItem: {
    fontSize: 16,
    color: '#333',
  },
}));

export default function MyProfileMenu() {
  const classes = useStyles();
  const [anchorEl, setAnchorEl] = React.useState(null);
  const navigate = useNavigate();

  const handleClick = (event:any) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = () => {
    sessionStorage.removeItem("jwt");
    window.location.reload();
  };

  const handleProfileClick = () => {
    navigate('/profile');
    handleClose();
  };

  const handleAccountClick = () => {
    navigate('/account');
    handleClose();
  };

  const handleSettingsClick = () => {
    navigate('/settings');
    handleClose();
  };

  const handleAdminPanelClick = () => {
    navigate('/admin-panel');
    handleClose();
  };


  return (
    <div>
      <IconButton
        aria-controls="simple-menu"
        aria-haspopup="true"
        onClick={handleClick}
      >
        <PersonIcon />
      </IconButton>
      <Menu
        id="myprofile-menu"
        anchorEl={anchorEl}
        keepMounted
        open={Boolean(anchorEl)}
        onClose={handleClose}
        getContentAnchorEl={null}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'center',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'center',
        }}
        className={classes.menu}
      >
        <MenuItem onClick={handleProfileClick} className={classes.menuItem}>Profile</MenuItem>
        <MenuItem onClick={handleSettingsClick} className={classes.menuItem}>Personal settings</MenuItem>
        <MenuItem onClick={handleAdminPanelClick} className={classes.menuItem}>Admin panel</MenuItem>
        <MenuItem onClick={handleLogout} className={classes.menuItem}>Logout</MenuItem>
      </Menu>
    </div>
  );
}
