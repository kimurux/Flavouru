import { useState } from 'react';
import axios from 'axios';
import './App.css';
import logo from "./assets/img/logo.svg";
import { useNavigate, Link } from 'react-router-dom';

function App() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const ERROR_MESSAGES: Record<number, string> = {
  400: 'Неверный формат данных.',
  401: 'Неверный логин или пароль.',
  403: 'Доступ запрещён.',
  404: 'Данные не найдены.',
  408: 'Истекло время ожидания.',
  429: 'Слишком много запросов. Попробуйте позже.',
  500: 'Проблемы с сервером, повторите попытку позже.',
  503: 'Сервер временно недоступен.',
};

function getErrorMessage(status?: number): string {
  if (!status) return 'Нет соединения с сервером.';
  return ERROR_MESSAGES[status] || `Неизвестная ошибка (${status})`;
}

const handleLogin = async (e: React.FormEvent) => {
  e.preventDefault();

  try {
    const response = await axios.post(
      'http://92.248.255.123:5000/api/Auth/login',
      { username, password },
      { headers: { 'Content-Type': 'application/json' } }
    );

    navigate('/Main-page');
  } catch (err: any) {
      const status = err.response?.status;
      const message = getErrorMessage(status);
      setError(message);
      console.error(err);
    }
};

  return (
    <div className='main'>
      <div className='login__container'>
        <div className='login__logo'>
          <img src={logo} alt='svg' className='login__logo-img'/>
          <h2 style={{color: 'coral'}}>Flavouru</h2>
        </div>

        <form className='login__validate' onSubmit={handleLogin}>
          <div className='login__validate-upper'>
            <input
              type="text"
              className='login__validate-block'
              placeholder='Логин'
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </div>
          <div className='login__validate-lower'>
            <input
              type="password"
              className='login__validate-block'
              placeholder='Пароль'
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          {error && <div className="alert__error">{error}</div>}

          <button type="submit" className='login__send_validate' style={{ padding: '8px', margin: '5px' }}>
            Войти
          </button>
        </form>

        <div className='login__extra'>
          <div className='login__extra-block'>
            <Link to="/Reset-password">Забыли пароль?</Link>
          </div>
          <div className='login__extra-block'>
            <Link to="/Registration">Зарегистрироваться</Link>
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;
