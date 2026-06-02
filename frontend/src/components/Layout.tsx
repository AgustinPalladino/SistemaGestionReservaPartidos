import { Box, Toolbar } from '@mui/material';
import { Outlet, useLocation } from 'react-router-dom';
import { Navbar } from './Navbar';
import { SIDEBAR_WIDTH, Sidebar } from './Sidebar';

const pageTitles: Record<string, string> = {
  '/': 'Dashboard',
  '/fields': 'Fields',
  '/reservations': 'Reservations',
  '/tournaments': 'Tournaments',
  '/teams': 'Teams',
};

function resolveTitle(pathname: string): string {
  return pageTitles[pathname] ?? 'SGR Partidos';
}

export function Layout() {
  const { pathname } = useLocation();
  const title = resolveTitle(pathname);

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <Sidebar />
      <Box
        component="section"
        sx={{
          flexGrow: 1,
          display: 'flex',
          flexDirection: 'column',
          width: { sm: `calc(100% - ${SIDEBAR_WIDTH}px)` },
        }}
      >
        <Toolbar sx={{ display: { sm: 'none' } }} />
        <Navbar title={title} />
        <Box component="main" sx={{ flexGrow: 1, p: 3, bgcolor: 'background.default' }}>
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
}
