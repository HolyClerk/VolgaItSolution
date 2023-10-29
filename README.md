# Решение задания на полуфинал по дисицплине "Backend разработка Web API"
## Инструкция
### Swagger URL: http://localhost:5000/swagger/index.html
Рекомендую воспроизводить запуск через Visual Studio, в случае возникновения проблем с открытием через экзешник.

1. Перейдите в каталог Simbir.RestApi\bin\Release\net7.0\publish
2. Откройте приложение и перейдите по ссылке указанной выше.
3. В случае, если сайт недоступен - откройте окно приложения и перейдите по указанной в нём ссылке.


## Решение возникших проблем
- В AccountController нет конечной точки "SignOut" т.к. его реализация в рамках АПИ невозможна. JWT токены не должны сохраняться в каком-либо месте, т.к. это считается небезопасной практикой. Без этого SignOut реализовать невозможно. Вместо этого стоит удалять токен в локальном хранилище пользователя (например, кэш).
- RentController и AdminRentController: в задании не было указано ничего про установку времени начала и конца, а так же логику оплаты за аренду. По этой причине я решил не добавлять в сущность Rent такие поля как TimeStart, TimeEnd, PriceOfUnit, FinalPrice - они нигде, кроме админского контроллера, не фигурируют.
- Создать админский аккаунт внутри приложения исходя из задания невозможно. По этой причине в AdminAccountController в конечной точке (POST) api/Admin/Account я убрал ограничение аутентификации и создать админку можно и без наличия уже созданного аккаунта. Иной способ выхода - зарегистрировать обычный аккаунт, и в записи внутри таблицы с пользователями поменять флаг "IsAdministrator" на "true"
