# FishBot for GTA5 RP
## Pretty intelligent multifunctional fish bot, which is also able to solve Google reCaptcha v2

[Russian README](README.md)

### Client-side tools:
+ **C# + .NET Framework v4.7.2**  
All the program logic
+ **Tesseract OCR (wrapper for .NET Framework)**  
Getting text from image
+ **XML**  
Storing users settings
### Server-side tools (anti-captcha):  
+ **C# + .NET Framework v4.7.2**  
All the program logic
+ **Tesseract OCR (wrapper for .NET Framework)**  
Getting text from image
+ **Yolo v3 (wrapper for .NET Framework)**   
Object detection from input image
### Web:  
+ PHP, MySQL 
    + Storing users, HWID's, subscription periods, VK ID. Processing automatic requests/responses from/to client
+ Using VK API and messages of VKontakte Group (https://vk.com/fishbot_gta5rp)
    + For clients: processing requests (get a free key, get an existing key and it's subscribtion period)
    + For admin: giving new keys with subscribtion (both for fish bot and for anti-captcha), get all the info about user, cancel users subsribtions, reset users HWID

### Constituents of fishing on this project (GTA5 RP):  
+ We have a fishing rod (there are 3 types of it, they all contains the word **Удочка** (**Rod**) in their name)  
![изображение](https://user-images.githubusercontent.com/37770139/142511014-a07bde9c-8358-4d6f-bdc3-eb5bf9b55ff9.png)
+ We have a bait (price - **35$**)  
![изображение](https://user-images.githubusercontent.com/37770139/142510955-fa0946e7-3ae7-411a-bb27-38231627e97e.png "Bait")
+ There are just a certain fishing-allowed places on server, when entering such a place there appears a special yellow indicator  
![изображение](https://user-images.githubusercontent.com/37770139/142510420-20219889-6e99-4888-ac8b-faec971ce74f.png "Fishing place")
+ There are mood and hunger systems on server. Such indicators of bad mood and hunger are placed beside a fishing-allowed place indicator
    + Bad mood (appearance of **purple** indicator) adds an essential chance of breaking our rod while fishing
    + Hunger (appearance of **orange** indicator) gradually decreases our player's health
+ There are a variaty of fish, each of which has it's own chance and price
+ Each fish has it's own weight, that's why our inventory (max. 10 kg) overweights. There are also some bags on this server (with additional free weight to our inventory), so we've got an automatical inventory-to-bag fish shifting system in our bot.

### Principles of fishing on this project (GTA5 RP):  
+ Open the inventory (button I) and press left mouse button on fishing rod
+ Wait some time while a special red (start) indicator isn't appered. Then quckly click right mouse boutton on the screen while white (stop) indicator isn't apperaed. (do it cyclically, while fish wasn't caught)   
![изображение](https://user-images.githubusercontent.com/37770139/142511626-afc6eea3-0716-486d-8b8a-8a3e6ccb541e.png "Start clicking")
![изображение](https://user-images.githubusercontent.com/37770139/142511669-71ba42a8-7212-4d9e-a313-60f877af07af.png "Make a pause")  
+ When we catch our fish successfuly, we've got a special message about it in the buttom of the screen 
![изображение](https://user-images.githubusercontent.com/37770139/142511882-cbb06210-1e58-47f9-af68-8c7443c9bcd9.png)  
+ Caught fish appeares in our inventory and kept there in such a way
![изображение](https://user-images.githubusercontent.com/37770139/142512745-136384e8-bfe9-42c1-b9de-e58a9e3f17fc.png)  
+ Every 20 fishes caught we need to pass **Google reCaptcha v2**, if we reject it and press the button **Я робот** (**I'm a robot**), then fishing will be restricted for us for 10-20-30-40-etc. minutes  
![изображение](https://user-images.githubusercontent.com/37770139/142514773-0d6a22cc-e2e9-43fb-bb47-b7b70ad0e227.png)


