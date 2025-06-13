import './reg.css'
import { useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';
import logo from "../assets/img/logo.svg";

function Registration() {

    const [username, setUsername] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [passwordRedo, setPasswordRedo] = useState<string>('');
    const [email, setEmail] = useState<string>('');
    const [error, setErrorState] = useState<string>('');
    const [isModalOpen, setModalOpen] = useState<boolean>(false);

    const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      setPassword(e.target.value);
    };

    const handlePasswordRedoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      setPasswordRedo(e.target.value);

      if (e.target.value !== password) {
        setErrorState('Пароли не совпадают');
      } else {
        setErrorState('');
      }
    };
    const ConfirmRegistration = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!username || !email || !password || !passwordRedo) {
        setErrorState('Все поля должны быть заполнены!');
        return;
    }

    if (password !== passwordRedo) {
        setErrorState('Пароли не совпадают');
        return;
    }

    // const userExists = users.some((user) => user.username === username || user.email === email);
    // if (userExists) {
    //   setErrorState('Логин или почта уже заняты');
    //   return;
    // }

    setErrorState('');

    try {
        await axios.post('http://92.248.255.123:5000/auth/register', {
        username,
        email,
        password,
        });
        setModalOpen(true);
    } catch (err: any) {
        setErrorState(err.message || 'Не удалось сохранить пользователя');
    }
    };

    return (
        <div className='main'>
            <div className='reg__container'>
                {error && <div className="alert__error">{error}</div>}
                <div className='login__logo'>
                    <img src={logo} alt='svg' className='login__logo-img'/>
                    <div className='sign__choice'>
                        <div className='sign-in__block'>
                            <Link to="/">
                                Логин
                            </Link>
                        </div>
                        <div className='divider'></div>
                        <div className='sign-up__block'>
                            <Link to="/Registration">
                                Регистрация
                            </Link>
                        </div>
                    </div>
                </div>
                <div className='reg__field'>
                    <form onSubmit={ConfirmRegistration}>
                        <label htmlFor="username"></label>
                        <input 
                        className='reg__field-input'
                        placeholder='Ваш логин'
                        type="text"
                        id="username"
                        name="username"
                        autoComplete='username'
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        />
                        <label htmlFor="email"></label>
                        <input
                        className='reg__field-input'
                        placeholder='Ваша почта' 
                        type="email"
                        id="email"
                        name="email"
                        autoComplete='username'
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        />
                        <label htmlFor="new-password"></label>
                        <input
                        className='reg__field-input'
                        placeholder='Ваш пароль' 
                        type="password"
                        id="new-password"
                        name="new-password"
                        autoComplete='new-password'
                        value={password}
                        onChange={handlePasswordChange}
                        />
                        <label htmlFor="password-redo"></label>
                        <input
                        className='reg__field-input'
                        placeholder='Повторите пароль' 
                        type="password"
                        id="password-redo"
                        name="password-redo"
                        autoComplete='new-password'
                        value={passwordRedo}
                        onChange={handlePasswordRedoChange}
                        />
                        <div className='reg__confirm'>
                            <button type="submit">
                                Создать аккаунт
                            </button>
                        </div>
                    </form>
                </div>
            </div>
            {isModalOpen && (
            <div className="modal">
                <div className="modal__content">
                <h2> Регистрация прошла успешно! </h2>
                <p>Добро пожаловать, {username}!</p>
                <Link
                    to="/"
                    className="test"
                >
                    Вернуться на главное меню
                </Link>
                </div>
            </div>
            )}
        </div>
        
    )
}

export default Registration