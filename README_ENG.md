# FishBot for GTA5 RP
## Pretty intelligent multifunctional fish bot, which is also able to solve Google reCaptcha v2

[Russian README](README.md)


## Table of contents
+ [Used tools](#tools)  
    + [Client-side](#tools-client)  
    + [Server-side (anti-captcha)](#tools-server)  
    + [Web](#tools-web)  
+ [Pictures](#photos) 
+ [Video demonstrations](#video-demos)  
+ [Functions](#functions)  
+ [Fishing system on server](#about-fishing) 
    + [Constituents of fishing](#about-fishing-1) 
    + [Fishing process](#about-fishing-2) 
+ [How it works?](#about-bot) 
    + [Bot itself](#about-bot-1) 
    + [Anti-captcha](#about-bot-2)
+ [Subscribtions system abilities](#about-subscribtions) 

<a name="tools"/>  

## Used tools

<a name="tools-client"/> 

### Client-side:
+ **C# + .NET Framework v4.7.2**  
All the program logic
+ **Tesseract OCR (wrapper for .NET Framework: https://github.com/charlesw/tesseract)**  
Getting text from an input image
+ **XML**  
Storing user's settings

<a name="tools-server"/> 

### Server-side tools (anti-captcha):  
+ **C# + .NET Framework v4.7.2**  
All the program logic
+ **Tesseract OCR (wrapper for .NET Framework: https://github.com/charlesw/tesseract)**  
Getting text from an input image
+ **Yolo v3 (wrapper for .NET Framework: https://github.com/AlturosDestinations/Alturos.Yolo)**   
Object detection from an input image

<a name="tools-web"/> 

### Web:  
+ PHP, MySQL 
    + Storing users, their HWIDs, subscription periods, VK IDs. Processing automatic requests/responses from/to client
+ Using VK API and messages of VKontakte Group (https://vk.com/fishbot_gta5rp)
    + For clients: processing requests (get a free key, get an existing key and it's subscribtion period)
    + For admin: giving new keys with subscribtion (both for fish bot and for anti-captcha), get all the info about user, cancel users subsribtions, reset users HWID

<a name="photos"/>  

## Pictures  
![Screenshot_3](https://user-images.githubusercontent.com/37770139/143047460-00bc2495-92ee-4187-b003-be2a32378d38.png "Authorization form")  
![Screenshot_4](https://user-images.githubusercontent.com/37770139/143047470-cc7a5e04-061d-40f6-97a1-1a537c00ab56.png "Menu")  

<a name="video-demos"/> 

## Video demonstrations:
+ Launching: https://youtu.be/yFmKOM2YR1w
+ Starting bot and fishing process: https://youtu.be/-wWIcPnoBJo
+ Captcha appearing and solving it: https://youtu.be/VvW-CAsrNGU
+ Auto-shifting fish to bag when overweight: https://youtu.be/BPtA45r-52A
+ YouTube review in russian (old version of bot, no anti-captcha): https://www.youtube.com/watch?v=zO8e55dFQ0c

<a name="functions"/> 

## Functions:
+ It works when your game is minimized. You'll see what happens in game in always-on-top menu  
+ Bag autodetection  
+ Auto-shifting fish from inventory to bag when overweight  
+ Autodetection of fishing rod and bait, you don't need to place it in certain slots  
+ Automatic hunger reduction (when some food from list in config is available in inventory, you can also add other types of food)  
+ Automatic mood uplifting (bot will determine whether your mood is bad and how many cigarettes it needs to smoke to reduct it)  
+ Sound notifications about some bad things  
+ VKontakte notifications 
+ Auto-drop of fish when overweight (you can manage a special list, by default there is - Стерлядь (Sterlet))
+ Income counter: bot determines, what fish it caught and increases your income excluding price of bait. You can change fish price list in config (both minimal and maximum). Double-click on counter and bot will recount your income in min/max. You can also reset your counter   
+ Stop when fish off the hook. Not really useful function because after a lot of tests there were no such cases. But if it happen, bot stops working
+ Pause: after every fish caught bot will make a break (by default: 1 second, you can change it in config or turn it off at all)  
+ Pause between mouse-clicks when fish on the hook. You can change this value if you have some problems with your ingame ping, but there were no such situatuations in my experience  
+ Menu. You can change its opacity, or minimize it  

<a name="about-fishing"/> 

## Fishing system on server (GTA5 RP)  

<a name="about-fishing-1"/> 

### Constituents of fishing on this project:  
+ We have a fishing rod (there are 3 types of it, they all contain the word **Удочка** (**Rod**) in their name)  
![изображение](https://user-images.githubusercontent.com/37770139/142511014-a07bde9c-8358-4d6f-bdc3-eb5bf9b55ff9.png)
+ We have a bait (price - **35$**)  
![изображение](https://user-images.githubusercontent.com/37770139/142510955-fa0946e7-3ae7-411a-bb27-38231627e97e.png "Bait")
+ There are just a certain fishing-allowed places on server, when entering such a place there appears a special yellow indicator  
![изображение](https://user-images.githubusercontent.com/37770139/142510420-20219889-6e99-4888-ac8b-faec971ce74f.png "Fishing place")
+ There are mood and hunger systems on server. Such indicators of bad mood and hunger are placed beside a fishing-allowed place indicator
    + Bad mood (appearance of **purple** indicator) adds an essential chance of breaking our rod while fishing
    + Hunger (appearance of **orange** indicator) gradually decreases our player's health
+ There is a variaty of fish, each of them has it's own chance and price
+ Each fish has it's own weight, that's why our inventory (max. 10 kg) overweights. There are also some bags on this server (with additional free weight to our inventory), so we've got an automatical inventory-to-bag fish shifting system in our bot.

<a name="about-fishing-2"/> 

### Fishing process:  
+ Open the inventory (button I) and press left mouse button on fishing rod
+ Wait some time while a special red (start) indicator isn't appered. Then quckly click right mouse boutton on the screen while white (stop) indicator isn't apperaed. (do it cyclically, while fish wasn't caught)   
![изображение](https://user-images.githubusercontent.com/37770139/142511626-afc6eea3-0716-486d-8b8a-8a3e6ccb541e.png "Start clicking")
![изображение](https://user-images.githubusercontent.com/37770139/142511669-71ba42a8-7212-4d9e-a313-60f877af07af.png "Make a pause")  
+ When we catch our fish successfuly, we've got a special message with fish name about it in the buttom of the screen 
![изображение](https://user-images.githubusercontent.com/37770139/142511882-cbb06210-1e58-47f9-af68-8c7443c9bcd9.png)  
+ Caught fish appeares in our inventory and kept there in such a way  
![изображение](https://user-images.githubusercontent.com/37770139/142512745-136384e8-bfe9-42c1-b9de-e58a9e3f17fc.png)  
+ Every 20 fishes caught we need to pass **Google reCaptcha v2**, if we reject it and press the button **Я робот** (**I'm a robot**), then fishing will be restricted for us for 10-20-30-40-etc. minutes  
![изображение](https://user-images.githubusercontent.com/37770139/142514773-0d6a22cc-e2e9-43fb-bb47-b7b70ad0e227.png)


![изображение](https://user-images.githubusercontent.com/37770139/143093948-61d26ca7-052f-466b-9a83-aace72b5db3d.png)
![изображение](https://user-images.githubusercontent.com/37770139/143093813-44853416-0cf2-44e9-b557-078207799233.png)




