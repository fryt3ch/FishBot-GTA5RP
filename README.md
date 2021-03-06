# Бот на рыбалку для GTA5 RP (FishBot)
## Умный многофункциональный бот, способный решать Google reCaptcha v2

[English README](README_ENG.md)

## Попробовать / приобрести бота
+ https://vk.com/fishbot_gta5rp

## Оглавление
+ [Используемые технологии](#tools)  
    + [Клиентская часть](#tools-client)  
    + [Серверная часть (антикапча)](#tools-server)  
    + [Веб часть](#tools-web)  
+ [Изображения](#photos) 
+ [Видео демонстрации работы бота](#video-demos)  
+ [Функционал бота](#functions)  
+ [Рыбалка на проекте](#about-fishing) 
    + [Составляющие рыбалки](#about-fishing-1) 
    + [Принцип рыбалки](#about-fishing-2) 
+ [Как все работает?](#about-bot) 
    + [Сам бот](#about-bot-1) 
    + [Антикапча](#about-bot-2)
+ [Возможности системы подписок](#about-subscribtions) 


<a name="tools"/>  

## Используемые технологии  

<a name="tools-client"/> 

### Клиентская часть
+ **C# + .NET Framework v4.7.2**  
Вся логика приложения
+ **Tesseract OCR (wrapper for .NET Framework: https://github.com/charlesw/tesseract)**  
Получение текста из изображения  
+ **TCPWrapper by frytech (самописный wrapper для Sockets: https://github.com/fryt3ch/TCPWrapper-C#)**  
Подключение и взаимодействие с сервером анти-капчи  
+ **XML**  
Хранение пользовательских настроек

<a name="tools-server"/> 

### Серверная часть (антикапча):  
+ **C# + .NET Framework v4.7.2**  
Вся логика приложения
+ **TCPWrapper by frytech (самописный wrapper для Sockets: https://github.com/fryt3ch/TCPWrapper-C#)**  
Обработка запросов, поступающих от бота, и отправка ответа   
+ **Tesseract OCR (wrapper for .NET Framework: https://github.com/charlesw/tesseract)**  
Получение текста из изображения  
+ **Yolo v3 (wrapper for .NET Framework: https://github.com/AlturosDestinations/Alturos.Yolo)**   
Обнаружение объектов на изображении 

<a name="tools-web"/> 

### Веб часть:  
+ PHP, MySQL 
    + Хранение пользователей в базе данных, прием автоматических запросов и выдача ответов для аутентификации пользователей  
+ Использование инструментария VK API и сообщений сообщества ВКонтакте (https://vk.com/fishbot_gta5rp)
    + Для клиентов: обработка запросов (получение бесплатного ключа, просмотр текущего ключа и периода его действия)
    + Для администратора: выдача ключей пользователю по ID страницы VK (как для бота, так и для анти-капчи), просмотр информации о пользователе из базы данных, аннулирование подписок у пользователя, сброс HWID пользователя

<a name="photos"/>  

## Изображения  
![Screenshot_3](https://user-images.githubusercontent.com/37770139/143047460-00bc2495-92ee-4187-b003-be2a32378d38.png "Панель авторизации")  
![Screenshot_4](https://user-images.githubusercontent.com/37770139/143047470-cc7a5e04-061d-40f6-97a1-1a537c00ab56.png "Меню бота")  

<a name="video-demos"/> 

## Видео демонстрации работы бота:
+ Запуск бота: https://youtu.be/yFmKOM2YR1w
+ Процесс начала работы и вылавливания рыбы: https://youtu.be/-wWIcPnoBJo
+ Появление и прохождение капчи: https://youtu.be/VvW-CAsrNGU
+ Автоперекладывание рыбы в сумку при перегрузе инвентаре: https://youtu.be/BPtA45r-52A
+ Обзор бота на YouTube (старая версия, без антикапчи): https://www.youtube.com/watch?v=zO8e55dFQ0c

<a name="functions"/> 

## Функционал бота:
+ Работа в свернутом режиме. Бот полностью функционален в таком режиме, вы можете запустить его, свернуть игру и заниматься своими делами, все оповещения о его работе вы увидите в меню  
+ Автодетект сумки  
+ Автоматическое перекладывание рыбы из инвентаря в сумку при заполнении инвентаря  
+ Автодетект удочки и приманки, вам не нужно класть их в определенные слоты, бот найдет их сам  
+ Автоматическое восстановление голода (при наличии еды в инвентаре, в список можно добавить любую еду, которая у вас есть)  
+ Автоматическое восстановление настроения (бот сам определит, сколько сигарет вам нужно выкурить для того, чтобы удочка не сломалась из-за плохого настроения)  
+ Звуковые оповещения при появлении капчи или при перегрузе сумки/инвентаря  
+ Уведомления в ВК при появлении капчи или при перегрузе сумки/инвентаря 
+ Автоматическое выбрасывание любой рыбы (список указывается в конфиге, по умолчанию - Стерлядь) при перегрузе инвентаря  
+ Счетчик прибыли: бот определяет, какую рыбу вы поймали и увеличивает вашу прибыль за вычетом стоимости приманки. Цены на рыбу можно поменять в файле настроек, как минимальную, так и максимальную. При нажатии на счетчик прибыли бот пересчитает ваш доход как в максимум, так и в минимум. Прибыль можно обнулить, если хотите считать заново  
+ Останавливаться, если рыба сорвалась. Функция не особо нужна, за время тестов рыба не срывалась, но если такое и произойдет, то бот остановит свою работу  
+ Пауза между подходами: после каждого вылова рыбы бот будет останавливаться на время (стандарт: 1 секунда, можно поменять в файле настроек или выключить вообще)  
+ Задержка между кликами при вылове рыбы. Вы можете изменить это значение, если у вас проблемы с пингом, хотя за время тестов нужды в этом не было  
+ Меню. Вы можете изменить прозрачность меню, либо же свернуть его, чтобы оно вам не мешало  

<a name="about-fishing"/> 

## О рыбалке на проекте

<a name="about-fishing-1"/> 

### Составляющие механики рыбалки на данном проекте:  
+ У нас есть удочка (их 3 вида, все содержат в своем названии слово **Удочка**)  
![изображение](https://user-images.githubusercontent.com/37770139/142511014-a07bde9c-8358-4d6f-bdc3-eb5bf9b55ff9.png)
+ У нас есть приманка (цена - **35$**)  
![изображение](https://user-images.githubusercontent.com/37770139/142510955-fa0946e7-3ae7-411a-bb27-38231627e97e.png "Приманка / Bait")
+ На сервере есть лишь несколько мест, где разрешено рыбачить, при входе в нужную нам зону появляется соответствующий индикатор желтого цвета в левом нижнем углу окна игры  
![изображение](https://user-images.githubusercontent.com/37770139/142510420-20219889-6e99-4888-ac8b-faec971ce74f.png "Место для рыбалки / Fish place")
+ На сервере присутствует механика голода и настроения. Соответствующие индикаторы о плохом настроении и голоде располагаются в том же месте, что и индикатор места для рыбалки 
    + Плохое настроение (появление индикатора **фиолетового** цвета) добавляет шанс на то, что во время рыбалки удочка сломается  
    + Голод (появление индикатора **оранжевого** цвета) постепенно уменьшает кол-во здоровья персонажа
+ Есть множество видов рыб, у каждой свой шанс вылова и своя цена  
+ Каждая рыба имеет вес, поэтому инвентарь (максимум 10 кг) заполняется, на проекте так же существуют сумки и рюкзаки, при наличии которых, наш бот будет перекладывать всю рыбу в них при перегрузе инвентаря

<a name="about-fishing-2"/> 

### Принцип самого процесса рыбалки на данном проекте:  
+ Открываем инвентарь в игре и закидываем удочку (ПКМ по ней)  
+ Ждем появления соответсвующего индикатора красного цвета о том, что настало время быстро нажимать на ПКМ до появления индикатора остановки белого цвета (и так циклично, пока не выловим)   
![изображение](https://user-images.githubusercontent.com/37770139/142511626-afc6eea3-0716-486d-8b8a-8a3e6ccb541e.png "Начинаем кликать")
![изображение](https://user-images.githubusercontent.com/37770139/142511669-71ba42a8-7212-4d9e-a313-60f877af07af.png "Останавливаемся")  
+ Когда мы успешно вылавливаем рыбу, внизу экрана появляется сообщение с названием данной рыбы  
![изображение](https://user-images.githubusercontent.com/37770139/142511882-cbb06210-1e58-47f9-af68-8c7443c9bcd9.png "Пойманная рыба")  
+ Выловленная рыба оказывается у нас в инвентаре и хранится в следующем виде  
![изображение](https://user-images.githubusercontent.com/37770139/142512745-136384e8-bfe9-42c1-b9de-e58a9e3f17fc.png "Хранение рыбы в инвентаре")  
+ Раз в 20 выловленных рыб появляется сообщение о необходимости пройти капчу (**Google reCaptcha v2**), если мы откажемся и нажмем на кнопку **Я робот**, то мы не сможем рыбачить 10-20-30-40 и т.д. минут  
![изображение](https://user-images.githubusercontent.com/37770139/142514773-0d6a22cc-e2e9-43fb-bb47-b7b70ad0e227.png "Необходимо пройти капчу")

<a name="about-bot"/> 

## Как все работает?  

<a name="about-bot-1"/> 

### Принцип работы нашего бота:  
+ Аутентификация пользователя (схематично, без подробностей)  
![изображение](https://user-images.githubusercontent.com/37770139/142931364-09e1cae1-9ec1-4d7f-904d-7ca8789bf929.png "Процесс аутентификации")
+ В отдельном потоке детектим процесс с игрой и получаем IntPtr ее окна
+ При нажатии на кнопку F4 бот ищет цвет индикатора места, разрешенного для рыбалки, в определенной области окна игры, и, если он найден, бот начинает свою работу  
    + То же самое выполняется для индикаторов настроения и голода, если соответствующие настройки включены в меню бота  
![1](https://user-images.githubusercontent.com/37770139/142738597-81da58be-1f21-4f1a-9135-df86d98d6292.png "Красным выделена сканируемая область индикаторов настроения, голода и места для рыбалки")  
+ Бот отправляет нажатие клавиши **I** в окно с игрой, открывается инвентарь
+ Бот считывает вес инвентаря с определенной области и определяет, есть на вас сумка или же нет. В соответствии с этим выводом, он сканирует область инвентаря (если есть сумка, то инвентарь располагается выше, чем если ее нет) и получает координаты найденых вещей, которые он может использовать  
    + Мы разбиваем сетку инвентаря (или сумки) на одинаковые квадраты, циклично проходимся по всем квадратам и обрабатываем их через **Tesseract**, который возвращает текст, найденный в каждом из квадратиков. Местонахождение предмета - точка середины квадрата, в котором был найден текст, идентифицирующий данный предмет  
 ![ezgif-1-2731e7283ccc](https://user-images.githubusercontent.com/37770139/142765626-a28fafbb-b658-4aab-92f2-3912954f4b89.gif "Как происходит сканирование инвентаря")  
+ Если ваш инвентарь забит (n >= 9.6, т.к. средний вес рыбы - 0.4 кг.) и при этом у вас есть сумка, то бот сам переложит имеющеюся рыбу в сумку. Если ее нет, то бот выдаст ошибку и отправит уведомление о перегрузе в ВКонтакте
+ При наличии удочки/приманки бот просто нажмет ЛКМ по удочке и начнет рыбачить, в противном случае бот сообщит об ошибке (и уведомит в ВКонтакте)  
+ После начала рыбалки бот сканирует потенциальную область расположения кнопки "Я робот" (чтобы обнаружить капчу, если она появилась)  
+ Далее, каждые 100 мсек. бот ищет в области появления индикатора о начале вылова рыбы пикселя красного цвета  
+ Как только пиксель был обнаружен, бот начинает отправлять в окно с игрой нажатия ЛКМ с заданной пользователем скоростью, одновременно ища белый пиксель в той же области, в которой был найден красный. И так циклично, пока не появится сообщение об успешном/провальном вылове рыбы
+ Бот считывает текст снизу экрана, определяет, что за рыба была поймана, и, при включенной функции счетчика прибыли, учтет эту рыбу в статистике и прибавит ее стоимость за вычетом цены приманки в счетчик прибыли (рыбы и их цены можно изменять в файле конфигурации)  

<a name="about-bot-2"/> 

### Принцип работы антикапчи:  
+ Образная схема взаимодействия с клиентом (без подробностей)   
![изображение](https://user-images.githubusercontent.com/37770139/143093596-1f252f1d-06c5-406d-8210-7bc1ca5c0a96.png "Схема")  
+ Антикапча запускается отдельным приложением и представляет из себя TCP сервер, ожидающий подключения клиентов  
+ Как только у одного из пользователей появляется капча, бот подключается к TCP серверу антикапчи, происходит аутентификация (бот высылает HWID пользователя, антикапча проверяет через POST/GET запрос к нашему **captcha.php**, есть ли у данного пользователя доступ к антикапче)  
+ Если аутентификация прошла успешна, пользователь добавляется в очередь на решение капчи и пока он находится в очереди, каждые 10 сек. ему высылается его номер в очереди, который бот выводит в статус, чтобы пользователь мог понять, сколько ему еще осталось ждать
+ При запуске нашего сервера мы можем ввести число - кол-во сущностей, которые будут работать с пользователями (1 сущность - одновременно обрабатывается 1 пользователь). Когда очередь не пуста, каждая сущность пытается взять из очереди клиента, который успешно прошел аутентификацию и был допущен к решению капчи. Как только одной (или нескольким сразу) сущности это удалось, начинается обработка клиента
+ Все картинки, полученные от разных пользователей обрабатывает единый объект **YoloWrapper**. Мы используем **Mutex**, чтобы предотвратить одновременный доступ к **YoloWrapper** со стороны нескольких сущностей
    + Для чего нам вводить понятие **сущностей** в наш проект? Дело в том, что пока **YoloWrapper** работает с сущностью **N1**, другие (**N2**, **N3**, и т.д.) за это время уже начнут обрабатывать капчу (примут массив с байтами со стороны клиента, выяснят, что надо искать в капче, какое задание выполнять), а когда обработка **YoloWrapper**, запущенная со стороны сущности **N1**, закончится, тогда уже **N2** запустит свою обработку **YoloWrapper**. Таким образом вы увеличиваем скорость прохождения капчи в случае, если у нас много пользователей
+ Как только сущность антикапчи закончила обработку полученного изображения, она высылает своему клиенту сериализованный словарь, ключ которого - точка (куда нажимать боту на капче), а значение - число (пауза между текущим и следующим нажатием)  
+ Бот получает этот словарь и выполняет указанные сервером действия. Таким образом и проходится капча  

<a name="about-subscribtions"/> 

## Возможности системы подписок на приложение:  
+ Привязка пользователей по принципу ID VK - уникальный ключ - HWID (уникальный ключ формата FISHBOT_[0-9A-Z]{16}, пример: FISHBOT_Y1NZVSY18W6EJUFA)
    + Пользователь получает ключ в группе ВК в автоматическом режиме, используя комманду "бесплатный ключ", сгенерированный ключ привязывается к пользователю по VK ID. Как только пользователь введет этот ключ при запуске бота, ключ привяжется к HWID пользователя (уникальный идентификатор компьютера, только на котором будет работать данный ключ)  
+ Выдача подписок (как на бота, так и на антикапчу) администратором в автоматическом режиме через ту же группу ВК. Достаточно использовать команду "выдать бота (или капчу) [VK ID] [Период]. Допустимый диапазон поля [Период] - 1 неделя, 2 недели, 1 месяц, Навсегда  
+ Просмотр всей информации о пользователе администратором, используя команду "подписки [VK ID]"  
+ Аннулирование подписки администратором, используя команду "аннулировать [VK ID]"  
+ Сброс HWID у пользователя администратором, используя команду "сбросить [VK ID]"  
