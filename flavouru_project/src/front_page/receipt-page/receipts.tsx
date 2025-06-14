import '../fp.css';
import logofull from '../../assets/img/logofull.svg';
import { Link } from 'react-router-dom';
import { useState, useEffect } from 'react';
import './receipts.css';
import axios from 'axios';

interface Recipe {
  id: string;
  title: string;
  description: string;
  image?: string;
  instructions?: string;
  prep_time?: number;
  cook_time?: number;
  servings?: number;
}

function ReceiptPage() {
    const [searchQuery, setSearchQuery] = useState('');
    const [viewMode, setViewMode] = useState<'cards' | 'list'>('cards');
    const [recipes, setRecipes] = useState<Recipe[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState('');

    // Загрузка рецептов с API
    useEffect(() => {
        const fetchRecipes = async () => {
            try {
                const response = await axios.get<Recipe[]>('http://92.248.255.123:5000/recipes');
                const recipesWithImages = response.data.map(recipe => ({
                    ...recipe,
                    image: "http://92.248.255.123:5000/static/images/206139f3-d60b-4766-a575-e23019c3f8ac.png"
                }));
                setRecipes(recipesWithImages);
            } catch (err) {
                setError('Не удалось загрузить рецепты');
                console.error('Ошибка загрузки:', err);
            } finally {
                setIsLoading(false);
            }
        };

        fetchRecipes();
    }, []);

    // Фильтрация рецептов
    const filteredRecipes = recipes.filter(recipe =>
        recipe.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
        recipe.description.toLowerCase().includes(searchQuery.toLowerCase())
    );


    return (
        <div className='main'>
            <div className='body__container'>
                <div className='nav__container'>
                    <nav className='nav'>
                        <div className='nav__logo'>
                            <img src={logofull} alt="Logo" style={{ height: '100%' }} />
                        </div>
                        <div className='catalogue__container'>
                            <div className='catalogue'>
                                <Link to="/Main-page"><button>Главное</button></Link>
                                <Link to="/Forum"><button>Форум</button></Link>
                                <Link to="/Information"><button>Инфо</button></Link>
                                <Link to="/Receipts"><button>Рецепты</button></Link>
                                <Link to="/Messages"><button>Сообщения</button></Link>
                                <Link to="/Profile"><button>Профиль</button></Link>
                            </div>
                        </div>
                    </nav>
                </div>
                
                <div className='main-body__container'>
                    {/* Поиск рецептов */}
                    <div className='receipt__search-up'>
                        <input
                            type="text"
                            placeholder="Найти рецепт..."
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                        />
                        <button>🔍</button>
                    </div>

                    {/* Переключение вида */}
                    <div className='receipt__preference'>
                        <button 
                            onClick={() => setViewMode('cards')}
                            style={{ background: viewMode === 'cards' ? '#ddd' : '' }}
                        >
                            🗂️ Карточки
                        </button>
                        <button
                            onClick={() => setViewMode('list')}
                            style={{ background: viewMode === 'list' ? '#ddd' : '' }}
                        >
                            📜 Список
                        </button>
                    </div>

                    {/* Отображение рецептов */}
                    <div className='receipts'>
                        <div className={`receipts__container ${viewMode}`}>
                            {isLoading ? (
                                <p>Загрузка рецептов...</p>
                            ) : error ? (
                                <p>{error}</p>
                            ) : filteredRecipes.length > 0 ? (
                                filteredRecipes.map(recipe => (
                                    <div key={recipe.id} className={`receipt-item ${viewMode}`}>
                                        <img 
                                            src={recipe.image} 
                                            className='recipe__image' 
                                            alt={recipe.title}
                                        />
                                        <div className="recipe-content">
                                            <h3>{recipe.title}</h3>
                                            <p>{recipe.description}</p>
                                        </div>
                                    </div>
                                ))
                            ) : (
                                <p>Рецептов не найдено 😢</p>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ReceiptPage;