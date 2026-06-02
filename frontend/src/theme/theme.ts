import { createTheme } from '@mui/material/styles';

export const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1b5e20',
      light: '#4c8c4a',
      dark: '#003300',
      contrastText: '#ffffff',
    },
    secondary: {
      main: '#0d47a1',
      light: '#5472d3',
      dark: '#002171',
      contrastText: '#ffffff',
    },
    background: {
      default: '#f5f5f5',
      paper: '#ffffff',
    },
  },
  typography: {
    fontFamily: 'Roboto, sans-serif',
    h4: { fontWeight: 700 },
    h6: { fontWeight: 600 },
  },
  shape: {
    borderRadius: 8,
  },
});
