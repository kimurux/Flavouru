import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Reset_password from './reset_pswrd/reset-password.tsx';
import Registration from './registration/registration.tsx';
import MainPage from './front_page/main_page.tsx';
import ConfirmReset from './reset_pswrd/reset-confirm/confirm-reset.tsx';
import PasswordReset from './reset_pswrd/reset-confirm/change-password/ResetPasswordConfirm.tsx';
import ForumPage from './front_page/forum-page/forum.tsx';
import InfoPage from './front_page/info-page/fyi.tsx';
import MsgPage from './front_page/msg-page/messages.tsx';
import ProfilePage from './front_page/profile-page/profile.tsx';
import ReceiptPage from './front_page/receipt-page/receipts.tsx';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <Router>
        <Routes>
          <Route path="/" element={<App />} />
          <Route path='/Reset-password' element={<Reset_password />} />
          <Route path='/Registration' element={<Registration />} />
          <Route path='/Main-page' element={<MainPage />} />
          <Route path='/Confirm-reset' element={<ConfirmReset />} />
          <Route path='/Forum' element={<ForumPage />} />
          <Route path='/Messages' element={<MsgPage/>} />
          <Route path='/Information' element={<InfoPage />} />
          <Route path='/Profile' element={<ProfilePage />} />
          <Route path='/Receipts' element={<ReceiptPage />} />
          <Route path='/ResetPasswordConfirm' element={<PasswordReset />} />
        </Routes>
      </Router>
  </StrictMode>,
)
