import { Alert, Box, Button, Card, CardContent, Chip, CircularProgress, Divider, Paper, TextField, Typography } from '@mui/material';
import { useEffect, useMemo, useState } from 'react';
import { reservationService } from '../../../services/api';

const FIELD_ID = 1;
const DEFAULT_USER_ID = '11111111-1111-1111-1111-111111111111';

function formatDateKey(date: Date) {
  const year = date.getFullYear();
  const month = `${date.getMonth() + 1}`.padStart(2, '0');
  const day = `${date.getDate()}`.padStart(2, '0');
  return `${year}-${month}-${day}`;
}

function addDays(date: Date, amount: number) {
  const next = new Date(date);
  next.setDate(next.getDate() + amount);
  return next;
}

export default function ReservationsPage() {
  const [currentDate, setCurrentDate] = useState(new Date(2026, 6, 1));
  const [selectedDate, setSelectedDate] = useState(formatDateKey(new Date(2026, 6, 1)));
  const [events, setEvents] = useState<any[]>([]);
  const [hours, setHours] = useState<any[]>([]);
  const [hoursLoading, setHoursLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [userId, setUserId] = useState(DEFAULT_USER_ID);

  const viewMonth = useMemo(() => currentDate.getMonth() + 1, [currentDate]);
  const viewYear = useMemo(() => currentDate.getFullYear(), [currentDate]);

  useEffect(() => {
    const loadEvents = async () => {
      try {
        const data = await reservationService.getCalendarEvents(FIELD_ID, viewYear, viewMonth);
        setEvents(data);
      } catch {
        setMessage('No se pudieron cargar los eventos del calendario.');
      }
    };

    void loadEvents();
  }, [viewMonth, viewYear]);

  useEffect(() => {
    const loadHours = async () => {
      setHoursLoading(true);
      try {
        const data = await reservationService.getAvailableHours(FIELD_ID, selectedDate);
        setHours(data);
      } catch {
        setMessage('No se pudieron cargar las horas disponibles.');
      } finally {
        setHoursLoading(false);
      }
    };

    void loadHours();
  }, [selectedDate]);

  const calendarDays = useMemo(() => {
    const start = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
    const end = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
    const days: Array<{ date: Date; currentMonth: boolean }> = [];

    const leadingDays = start.getDay() === 0 ? 6 : start.getDay() - 1;
    for (let index = leadingDays; index > 0; index -= 1) {
      days.push({ date: addDays(start, -index), currentMonth: false });
    }

    for (let day = 1; day <= end.getDate(); day += 1) {
      days.push({ date: new Date(currentDate.getFullYear(), currentDate.getMonth(), day), currentMonth: true });
    }

    while (days.length % 7 !== 0) {
      const lastDate = days[days.length - 1].date;
      days.push({ date: addDays(lastDate, 1), currentMonth: false });
    }

    return days;
  }, [currentDate]);

  const reserveHour = async (startTime: string, endTime: string) => {
    setSaving(true);
    setMessage(null);

    try {
      await reservationService.create({
        userId,
        fieldId: FIELD_ID,
        date: selectedDate,
        startTime,
        endTime,
        totalAmount: 100,
      });
      setMessage(`Reserva creada para ${selectedDate} de ${startTime} a ${endTime}.`);
      const refreshed = await reservationService.getAvailableHours(FIELD_ID, selectedDate);
      setHours(refreshed);
    } catch {
      setMessage('No se pudo crear la reserva. Verificá el usuario y la disponibilidad.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box sx={{ display: 'grid', gap: 3 }}>
      <Box>
        <Typography variant="h4" gutterBottom>
          Calendario de reservas
        </Typography>
        <Typography color="text.secondary">
          Elegí un día para ver las horas disponibles y reservar la cancha.
        </Typography>
      </Box>

      {message && (
        <Alert severity="info" onClose={() => setMessage(null)}>
          {message}
        </Alert>
      )}

      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' } }}>
        <Card sx={{ flex: 1 }}>
          <CardContent>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
              <Typography variant="h6">
                {currentDate.toLocaleDateString('es-AR', { month: 'long', year: 'numeric' })}
              </Typography>
              <Box sx={{ display: 'flex', gap: 1 }}>
                <Button size="small" variant="outlined" onClick={() => setCurrentDate(new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1))}>
                  ←
                </Button>
                <Button size="small" variant="outlined" onClick={() => setCurrentDate(new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 1))}>
                  →
                </Button>
              </Box>
            </Box>

            <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(7, 1fr)', gap: 1 }}>
              {['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom'].map((day) => (
                <Typography key={day} variant="caption" color="text.secondary" sx={{ textAlign: 'center' }}>
                  {day}
                </Typography>
              ))}

              {calendarDays.map((item, index) => {
                const key = formatDateKey(item.date);
                const isSelected = key === selectedDate;
                const hasEvent = events.some((event) => event.date === key);

                return (
                  <Button
                    key={`${key}-${index}`}
                    variant={isSelected ? 'contained' : 'outlined'}
                    size="small"
                    onClick={() => setSelectedDate(key)}
                    sx={{ minHeight: 56, opacity: item.currentMonth ? 1 : 0.45 }}
                  >
                    <Box sx={{ textAlign: 'center' }}>
                      <Typography variant="body2">{item.date.getDate()}</Typography>
                      {hasEvent && <Chip label="●" size="small" color="primary" sx={{ height: 12, minWidth: 0 }} />}
                    </Box>
                  </Button>
                );
              })}
            </Box>
          </CardContent>
        </Card>

        <Card sx={{ flex: 1 }}>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Horas disponibles · {selectedDate}
            </Typography>
            <TextField
              fullWidth
              label="ID de usuario"
              value={userId}
              onChange={(event) => setUserId(event.target.value)}
              margin="normal"
            />

            {hoursLoading ? (
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <CircularProgress size={20} />
                <Typography color="text.secondary">Cargando horarios...</Typography>
              </Box>
            ) : (
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5, mt: 2 }}>
                {hours.length === 0 ? (
                  <Typography color="text.secondary">No hay horas disponibles para este día.</Typography>
                ) : (
                  hours.map((hour) => (
                    <Paper key={`${hour.startTime}-${hour.endTime}`} variant="outlined" sx={{ p: 1.5 }}>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                        <Box>
                          <Typography sx={{ fontWeight: 600 }}>
                            {hour.startTime} - {hour.endTime}
                          </Typography>
                          <Typography variant="body2" color="text.secondary">
                            {hour.isAvailable ? 'Disponible' : 'Ocupada'}
                          </Typography>
                        </Box>
                        <Button
                          variant="contained"
                          size="small"
                          disabled={!hour.isAvailable || saving}
                          onClick={() => reserveHour(hour.startTime, hour.endTime)}
                        >
                          Reservar
                        </Button>
                      </Box>
                    </Paper>
                  ))
                )}
              </Box>
            )}
          </CardContent>
        </Card>
      </Box>

      <Divider />

      <Typography variant="body2" color="text.secondary">
        Modo MVP: una cancha fija y franjas de 1 hora desde las 06:00 hasta las 22:00.
      </Typography>
    </Box>
  );
}
