import '../fp.css'
import './profile.css'
import logofull from '../../assets/img/logofull.svg';
import { useNavigate, Link } from 'react-router-dom';
import DefaultLogo from '../../assets/img/DefaultNahui.jpg'
function ProfilePage() {
    return (
        <div className='main'>
            <div className='body__container'>
                <div className='nav__container'>
                    <nav className='nav'>
                        <div className='nav__logo'>
                            <img src={logofull} alt="Logo" style={{ height: '100%' }}/>
                        </div>
                        <div className='catalogue__container'>
                            <div className='catalogue'>
                                <Link to="/Main-page">
                                    <button>
                                        Главное
                                    </button>
                                </Link>
                                <Link to="/Forum">
                                    <button>
                                        Форум
                                    </button>
                                </Link>
                                <Link to="/Information">
                                    <button>
                                        Инфо
                                    </button>
                                </Link>
                                <Link to="/Receipts">
                                    <button>
                                        Рецепты
                                    </button>
                                </Link>
                                <Link to="/Messages">
                                    <button>
                                        Сообщения
                                    </button>
                                </Link>
                                <Link to="/Profile">
                                    <button>
                                        Профиль
                                    </button>
                                </Link>
                            </div>
                        </div>
                    </nav>
                </div>
                <div className='main-body__container'>
                    <div className='avatar__container'>
                            <img src={DefaultLogo}></img>
                    </div>
                    <div className='profile__container'>
                        <div className='profile__info'>
                            <div className='profile__nickname'>
                                <h2> Филлер </h2>
                            </div>
                            <div className='profile__blog-history'>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default ProfilePage