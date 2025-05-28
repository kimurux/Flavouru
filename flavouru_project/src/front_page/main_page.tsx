import './fp.css'
import logofull from '../assets/img/logofull.svg';

function MainPage() {
    return (
        <div className='main'>
            <div className='body__container'>
                <div className='nav__container'>
                    <nav className='nav'>
                        <div className='nav__logo'>
                            <img src={logofull} alt="Logo" style={{width: '8%'}}/>
                        </div>
                        <div className='catalogue__container'>
                            <div className='catalogue'>
                                <button>
                                    Главное
                                </button>
                                <button>
                                    Рецепты
                                </button>
                                <button>
                                    Форум
                                </button>
                            </div>
                        </div>
                    </nav>
                </div>
            </div>
        </div>
    )
}

export default MainPage