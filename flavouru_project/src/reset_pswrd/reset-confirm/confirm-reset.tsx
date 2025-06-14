import { useLocation, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import axios from 'axios';
import './confirm.css';

function ConfirmReset() {
  const location = useLocation();
  const method = location.state?.method;
  const navigate = useNavigate();

  const [input, setInput] = useState('');
  const [message, setMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async () => {
    try {
      setIsLoading(true);
      
      // Отправляем запрос на бэкенд для проверки пользователя
      const res = await axios.post('http://92.248.255.123:5000/auth/forgot-password', {
        [method === 'phone' ? 'phone' : 'email']: input
      });

      if (res.data.status === 'ok') {
        // Получаем ключ и сразу переходим на страницу сброса
        navigate(`/ResetPasswordConfirm?key=${res.data.key}`);
      } else {
        setMessage(res.data.message || 'Пользователь не найден');
      }
    } catch (error) {
      console.error(error);
      setMessage('Ошибка сервера. Попробуйте позже.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className='main'>
      <div className='confirm__container'>
        <h2>Сброс пароля через {method === 'phone' ? 'телефон' : 'почту'}</h2>

        <input
          type={method === 'phone' ? 'tel' : 'email'}
          placeholder={method === 'phone' ? '+7 (XXX) XXX-XX-XX' : 'example@mail.com'}
          value={input}
          onChange={(e) => setInput(e.target.value)}
        />

        <button onClick={handleSubmit} disabled={isLoading}>
          {isLoading ? 'Проверка...' : 'Продолжить'}
        </button>
        
        {message && <p className="message">{message}</p>}
      </div>
    </div>
  );
}

export default ConfirmReset;