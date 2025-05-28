import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Reset_password from './reset_pswrd/reset-password.tsx';
import Registration from './registration/registration.tsx';
import MainPage from './front_page/main_page.tsx';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <Router>
        <Routes>
          <Route path="/" element={<App />} />
          <Route path='/Reset-password' element={<Reset_password />} />
          <Route path='/Registration' element={<Registration />} />
          <Route path='/Main-page' element={<MainPage />} />
        </Routes>
      </Router>
  </StrictMode>,
)
