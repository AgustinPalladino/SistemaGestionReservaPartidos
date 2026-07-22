import { test, expect } from '@playwright/test';

const defaultDate = '2026-07-01';
const selectedDate = '2026-07-15';
const baselineHours = [
  { startTime: '08:00', endTime: '09:00', isAvailable: true },
  { startTime: '09:00', endTime: '10:00', isAvailable: false },
];
const selectedDateHours = [
  { startTime: '10:00', endTime: '11:00', isAvailable: true },
  { startTime: '11:00', endTime: '12:00', isAvailable: true },
];
const selectedDateHoursAfterBooking = [
  { startTime: '10:00', endTime: '11:00', isAvailable: false },
  { startTime: '11:00', endTime: '12:00', isAvailable: true },
];

const createdReservation = {
  id: '00000000-0000-0000-0000-000000000001',
  userId: '11111111-1111-1111-1111-111111111111',
  fieldId: 1,
  date: selectedDate,
  startTime: '10:00',
  endTime: '11:00',
  status: 'Pending',
  totalAmount: 100,
  createdAt: new Date().toISOString(),
};

test('Reserva un horario y verifica la reactividad del calendario y listado', async ({ page }) => {
  await page.route('**/api/reservations/calendar*', (route) =>
    route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify([]),
    }),
  );

  let selectedDateCallCount = 0;
  await page.route('**/api/reservations/available*', (route, request) => {
    const url = new URL(request.url());
    const date = url.searchParams.get('date');

    if (date === defaultDate) {
      return route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(baselineHours),
      });
    }

    if (date === selectedDate) {
      selectedDateCallCount += 1;
      if (selectedDateCallCount === 1) {
        return route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(selectedDateHours),
        });
      }

      return route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(selectedDateHoursAfterBooking),
      });
    }

    return route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify([]),
    });
  });

  await page.route('**/api/reservations', async (route, request) => {
    if (request.method() !== 'POST') {
      return route.continue();
    }

    const body = JSON.parse(request.postData() ?? '{}');
    expect(body).toMatchObject({
      fieldId: 1,
      date: selectedDate,
      startTime: '10:00',
      endTime: '11:00',
    });

    return route.fulfill({
      status: 201,
      contentType: 'application/json',
      body: JSON.stringify(createdReservation),
    });
  });

  await page.goto('/reservations');

  await expect(page.locator('h4', { hasText: 'Calendario de reservas' })).toBeVisible();
  await expect(page.locator('text=Horas disponibles · 2026-07-01')).toBeVisible();
  await expect(page.locator('text=08:00 - 09:00')).toBeVisible();
  await expect(page.locator('text=09:00 - 10:00')).toBeVisible();

  await page.click('button:has-text("15")');

  await expect(page.locator('text=Horas disponibles · 2026-07-15')).toBeVisible();
  await expect(page.locator('text=10:00 - 11:00')).toBeVisible();
  await expect(page.locator('text=11:00 - 12:00')).toBeVisible();

  await page.locator('button:has-text("Reservar")').first().click();

  await expect(page.locator('text=Reserva creada para 2026-07-15 de 10:00 a 11:00.')).toBeVisible();
  await expect(page.locator('text=10:00 - 11:00')).toBeVisible();
  await expect(page.locator('text=10:00 - 11:00').locator('..').locator('text=Ocupada')).toBeVisible();
});
