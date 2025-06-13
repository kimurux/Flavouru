import './rp.css'
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

function Reset_password() {
    const [method, setMethod] = useState('');
    const navigate = useNavigate();

    const handleNext = (e: React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        if (!method) return alert('Выберите метод сброса');

        navigate('/Confirm-reset', { state: { method } });
    };

    return (
        <div className='main'>
            <div className='reset__container'>
                <div className='reset__desc'>
                    <h2> Выберите метод сброса пароля </h2>
                </div>
                <div className='reset__choice'>
                    <form>
                        <input
                            type="radio"
                            id="reset__phone"
                            name="reset"
                            className="hidden-radio"
                            value="phone"
                            onChange={(e) => setMethod(e.target.value)}
                        />
                        <label htmlFor="reset__phone" className='reset_choice__button'>Номер телефона</label>

                        <input
                            type="radio"
                            id="reset__email"
                            name="reset"
                            className="hidden-radio"
                            value="email"
                            onChange={(e) => setMethod(e.target.value)}
                        />
                        <label htmlFor="reset__email" className='reset_choice__button'>Почта</label>
                    </form>
                    <div className='reset__button'>
                        <button onClick={handleNext}> Дальше </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Reset_password;
