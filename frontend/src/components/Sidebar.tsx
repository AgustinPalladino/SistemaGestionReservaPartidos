import {
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
} from '@mui/material';
import DashboardIcon from '@mui/icons-material/Dashboard';
import SportsSoccerIcon from '@mui/icons-material/SportsSoccer';
import EventAvailableIcon from '@mui/icons-material/EventAvailable';
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents';
import GroupsIcon from '@mui/icons-material/Groups';
import { NavLink } from 'react-router-dom';

const DRAWER_WIDTH = 240;

const navItems = [
  { to: '/', label: 'Dashboard', icon: <DashboardIcon /> },
  { to: '/fields', label: 'Fields', icon: <SportsSoccerIcon /> },
  { to: '/reservations', label: 'Reservations', icon: <EventAvailableIcon /> },
  { to: '/tournaments', label: 'Tournaments', icon: <EmojiEventsIcon /> },
  { to: '/teams', label: 'Teams', icon: <GroupsIcon /> },
] as const;

export function Sidebar() {
  return (
    <Drawer
      variant="permanent"
      sx={{
        width: DRAWER_WIDTH,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: DRAWER_WIDTH,
          boxSizing: 'border-box',
        },
      }}
    >
      <Toolbar>
        <Typography variant="h6" color="primary" noWrap>
          SGR Partidos
        </Typography>
      </Toolbar>
      <List component="nav">
        {navItems.map(({ to, label, icon }) => (
          <ListItem key={to} disablePadding>
            <ListItemButton
              component={NavLink}
              to={to}
              end={to === '/'}
              sx={{
                '&.active': {
                  bgcolor: 'primary.main',
                  color: 'primary.contrastText',
                  '& .MuiListItemIcon-root': { color: 'inherit' },
                },
              }}
            >
              <ListItemIcon sx={{ color: 'inherit' }}>{icon}</ListItemIcon>
              <ListItemText primary={label} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Drawer>
  );
}

export const SIDEBAR_WIDTH = DRAWER_WIDTH;
