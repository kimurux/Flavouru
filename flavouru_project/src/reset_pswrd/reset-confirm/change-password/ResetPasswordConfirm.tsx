import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './change-pswrd.css';
import axios from 'axios';

function PasswordReset() {
  const location = useLocation();
  const user = location.state?.user;
  const navigate = useNavigate();
  const [isModalOpen, setModalOpen] = useState(false);
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const searchParams = new URLSearchParams(location.search);
  const key = searchParams.get('key'); // Получаем ключ из URL

  const isValidPassword = password.length >= 6 && password.length <= 12;
  const returnToMain = () => navigate('/')
  const handleSubmit = async () => {
    if (!isValidPassword) {
      setError('Пароль должен быть от 6 до 12 символов');
      return;
    }

    try {
        await axios.post('http://92.248.255.123:5000/auth/set-password', {
          key: key,
          new_password: password
      });

      setError('');
      setModalOpen(true);
    } catch (err) {
      console.error('Ошибка при обновлении пароля:', err);
      setError('Ошибка при обновлении пароля');
    }
  };

  return (
    <div className='main'>
      <div className='change-password__container'>
        <h2>Сброс пароля для {user?.email || 'пользователя'}</h2>

        <input
          type="password"
          placeholder="Новый пароль"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        {error && <p style={{ color: 'red' }}>{error}</p>}

        <button
          onClick={handleSubmit}
          disabled={!isValidPassword}
          style={{ opacity: isValidPassword ? 1 : 0.5, cursor: isValidPassword ? 'pointer' : 'not-allowed' }}
        >
          Подтвердить
        </button>
      </div>
        {isModalOpen && (
        <div className="modal">
          <div className="modal__content">
            <h2>Ваш пароль был успешно сброшен! </h2>
            <p> Нажмите, чтобы вернуться на главное меню </p>
            <div className='modal__buttons'>
              <button onClick={returnToMain}>Вернуться</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default PasswordReset;
