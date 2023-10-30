# Решение задания на полуфинал по дисицплине "Backend разработка Web API"
## Инструкция
### Swagger URL: http://localhost:5000/swagger/index.html
Рекомендую воспроизводить запуск через Visual Studio, в случае возникновения проблем с открытием через экзешник.

1. Перейдите в каталог Simbir.RestApi\bin\Release\net7.0\publish
2. Откройте приложение и перейдите по ссылке указанной выше.
3. В случае, если сайт недоступен - откройте окно приложения и перейдите по указанной в нём ссылке.


## Решение возникших проблем и примечания для проверяющих задание
- Имя пользователя должно быть не меньше 4 символов. Пароль должен иметь как минимум 1 заглавный символ, 1 циферный, 1 non-alphanumeric символ и состоять не менее чем из 6 символов. (напр. "Qwerty123!")
- В некоторых конечных точках указано передавать параметры в строке, а не как тело. Большая часть таких точек покрыта [FromBody] атрибутами в параметрах и к ним написаны дтошки.
- В AccountController нет конечной точки "SignOut" т.к. его реализация в рамках АПИ по моему мнению необязательна. JWT токены не должны сохраняться в каком-либо месте, т.к. это считается небезопасной практикой. Вместо этого стоит удалять токен в локальном хранилище пользователя (например, кэш). 
- RentController и AdminRentController: вместо string для типа аренды (в днях или минутах) используется Enum, который находится в проекте Core/Entities/Rent.cs. 0 - дни, 1 - минуты. По окончанию аренды ставится дата окончания аренды, а так же рассчитывается конечная сумма (если аренда в днях, то округляется вверх). Вся бизнес-логика аренды лежит в сервисе "RentService.cs".
- Создать админский аккаунт внутри приложения исходя из задания невозможно (получается замкнутный круг, ибо админов могут создавать только админы). По этой причине в AdminAccountController в конечной точке (POST) api/Admin/Account я убрал ограничение аутентификации и создать админку можно и без наличия уже созданного аккаунта. Иной способ выхода - зарегистрировать обычный аккаунт, и в записи внутри таблицы с пользователями поменять флаг "IsAdministrator" на "true"
- В случае, если вы просматриваете код - "Force" в начале названий метода обозначает те методы, которые не будут проверять учетную запись на доступность действий, т.к. логика ролей лежит в контролерах. 
