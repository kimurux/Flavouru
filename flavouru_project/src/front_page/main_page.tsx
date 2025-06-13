import './fp.css';
import logofull from '../assets/img/logofull.svg';
import { Link } from 'react-router-dom';

function MainPage() {
  return (
    <div className="main">
      <div className="body__container">
        <div className="nav__container">
          <nav className="nav">
            <div className="nav__logo">
              <img src={logofull} alt="Logo" style={{ height: '100%' }} />
            </div>
            <div className="catalogue__container">
              <Link to="/Main-page"><button>Главное</button></Link>
              <Link to="/Forum"><button>Форум</button></Link>
              <Link to="/Information"><button>Инфо</button></Link>
              <Link to="/Receipts"><button>Рецепты</button></Link>
              <Link to="/Messages"><button>Сообщения</button></Link>
              <Link to="/Profile"><button>Профиль</button></Link>
            </div>
          </nav>
        </div>
        <div className="main-body__container">
          <h1>Блог №1</h1>
          <article>Ты умничка</article>
        </div>
      </div>
    </div>
  );
}

export default MainPage;