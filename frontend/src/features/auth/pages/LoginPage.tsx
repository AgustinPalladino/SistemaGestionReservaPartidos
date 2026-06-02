import { Box, Button, Container, Paper, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';

export default function LoginPage() {
  const navigate = useNavigate();

  const handleLoginPlaceholder = () => {
    // TODO: Integrar autenticación JWT real contra el backend
    localStorage.setItem('token', 'placeholder-token');
    navigate('/');
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={2} sx={{ p: 4 }}>
        <Typography variant="h4" gutterBottom>
          Login
        </Typography>
        {/* TODO: Formulario email/password + manejo de errores */}
        <Typography color="text.secondary" sx={{ mb: 3 }}>
          Pantalla de acceso. Pendiente de conectar con el endpoint de autenticación.
        </Typography>
        <Box>
          <Button variant="contained" onClick={handleLoginPlaceholder}>
            Entrar (demo)
          </Button>
        </Box>
      </Paper>
    </Container>
  );
}
