import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import LoginPage from './features/auth/pages/LoginPage';
import DashboardPage from './features/dashboard/pages/DashboardPage';
import FieldsPage from './features/fields/pages/FieldsPage';
import ReservationsPage from './features/reservations/pages/ReservationsPage';
import TeamsPage from './features/teams/pages/TeamsPage';
import TournamentsPage from './features/tournaments/pages/TournamentsPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route element={<Layout />}>
          <Route index element={<DashboardPage />} />
          <Route path="fields" element={<FieldsPage />} />
          <Route path="reservations" element={<ReservationsPage />} />
          <Route path="tournaments" element={<TournamentsPage />} />
          <Route path="teams" element={<TeamsPage />} />
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
