import { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';
import logo from "./assets/img/logo.svg";
import { useNavigate, Link } from 'react-router-dom';

function App() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  // Проверка здоровья API
  useEffect(() => {
    const fetchHealth = async () => {
      try {
        const response = await axios.get('http://92.248.255.123:5000/health');
        console.log(response)
      } catch (err) {
        console.error('API недоступен:', err);
      }
    };
    fetchHealth();
  }, []);

  const ERROR_MESSAGES: Record<number, string> = {
    400: 'Неверный формат данных',
    401: 'Неверный логин или пароль',
    403: 'Доступ запрещён',
    500: 'Ошибка сервера'
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      const response = await axios.post(
        'http://92.248.255.123:5000/auth/login',
        { username, password },
        { 
          headers: { 'Content-Type': 'application/json' },
          timeout: 5000
        }
      );

      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user_id', response.data.user.id);
      axios.defaults.headers.common['Authorization'] = `Bearer ${response.data.token}`;
      navigate('/Main-page');
    } catch (err: any) {
      const status = err.response?.status;
      setError(ERROR_MESSAGES[status] || 'Ошибка соединения');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className='main'>
      <div className='login__container'>
        <div className='login__logo'>
          <img src={logo} alt='Логотип' className='login__logo-img'/>
          <div className='sign__choice'>
            <div className='sign-in__block'>
              <Link to="/">Логин</Link>
            </div>
            <div className='divider'></div>
            <div className='sign-up__block'>
              <Link to="/Registration">Регистрация</Link>
            </div>
          </div>
        </div>

        <form className='login__validate' onSubmit={handleLogin}>
          <input
            type="text"
            className='login__validate-block'
            placeholder='Логин'
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
          <input
            type="password"
            className='login__validate-block'
            placeholder='Пароль'
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          
          <button 
            type="submit" 
            className='login__send_validate'
            disabled={isLoading}
          >
            {isLoading ? 'Загрузка...' : 'Войти'}
          </button>

          {error && <div className="alert__error">{error}</div>}
        </form>

        <div className='login__extra'>
          <Link to="/Reset-password">Забыли пароль?</Link>
        </div>
      </div>
    </div>
  );
}

export default App;