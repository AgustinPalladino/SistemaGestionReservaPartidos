import { Typography } from '@mui/material';

export default function ReservationsPage() {
  return (
    <>
      {/* TODO: Calendario o listado de reservas, crear reserva con validación de solapamiento */}
      <Typography variant="h4" gutterBottom>
        Reservations
      </Typography>
      <Typography color="text.secondary">
        Reservas de canchas por fecha y franja horaria.
      </Typography>
    </>
  );
}
