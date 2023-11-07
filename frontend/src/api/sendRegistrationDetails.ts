import { config } from '@/config'

// Устанавливаем значения для имени пользователя и пароля
const username: string = 'example_username';
const password: string = 'example_password';

// Создаем объект с данными для авторизации
const credentials: object =
{
    username: username,
    password: password
};

// Функция для отправки данных о регистрации
export const sendRegistrationDetails = async (label1: string, label2: string): Promise<boolean> => {

    // Получаем данные с сервера
    const aw = await fetch(`${config.YC_API_URL}/shit`);
    const dt: string = await aw.text();
    console.warn(dt);

    console.warn(label1);
    console.warn(label2);

    let isLogin = false;

    // Опции для запроса
    const requestOptions =
    { 
        method: 'POST',
        headers: {
            'x-api-key': '46BB6D176C0A4B56BA67B6A65CEBDA75',
            "Content-Type": "application/json",
        },
        body: JSON.stringify(credentials),
    };

    // Отправляем POST-запрос на авторизацию
    const response: void = await fetch(`${config.API_URL}/auth/login`, requestOptions)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('API response:', data);
            isLogin = data.isAuth;
        })
        .catch(error => {
            console.error('There was an error:', error);
        });
    
    // Сохраняем данные в Session Storage
    sessionStorage.setItem('isLogin', isLogin.toString());

    // Получаем данные из Session Storage
    const value = sessionStorage.getItem('isLogin');
    console.log(value);
    
    return isLogin;
}