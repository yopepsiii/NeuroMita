
Мита на питоне (юзает доступ к нейронкам по api) + Мелон Мод на юнити, пока не очень работающий.

UPD: Пока сработало у единиц, если что спрашивайте вот тут:
https://discord.com/channels/508309955686957057/1338859139102670890
сам серв https://discord.gg/aihasto
Гитхаб засветился слишком рано, щас я в темпе пытаюсь предоставить временную демку, но пока запуск это те еще танцы в бубном, кто хочет стабильности - месяцов подождите, мб больше.

![image](https://github.com/user-attachments/assets/1baad23d-d58a-484c-ba83-25a9f3dcbc03)



Гайд как это все запустить, когла +- сделаю)
Да, это танцы с бубном...
Сначала прочитай, потом делай, на пункте с источниками мб передумаешь(

0) Мелон лоадер:
Либо тут инсталлер ставите https://melonwiki.xyz/#/?id=requirements
Либо тут ищите https://github.com/LavaGang/MelonLoader
Есть файл MelonLoader.Installer.exe?
Отлично, выбирайте там мисайд, он его пропатчит, чтобы моды на основе мелона могли работать.

1) Мой мод идет в комплекте питон+промты+папкаКонвертации (их ставьте где угодно вместе) и также файлы мода (MitaAI.dll и assetbudle.test) непосредственно в папку mods, 
созданную мелоном (вроде в релизы кинул).
https://github.com/VinerX/NeuroMita/releases - тут релизы, то есть файлы которые нужны обычному игроку.

2) Мод в плане генерации текста можно запустить в двух форматах.
   
   а) Бесплатно, используя открытые ключи https://openrouter.ai/settings/keys
   б) Платно, оплачивая клюс тут https://console.proxyapi.ru/billing (рф вариант)
   в) Если у вас есть ключи напрямую (вы не в рф), то как бы тоже варик, но не оттестировано.
   
   Сейчас пойдут настройки, которые нужно прописать в запущенном приложении exe
   Варианты model,url и кнопки реквест(Последняя чисто для варианта с прокси гемини):
   
   Бесплатно:
      ссылка https://openrouter.ai/api/v1 модель google/gemini-2.0-flash-lite-preview-02-05:free open api ключ от https://openrouter.ai/settings/keys
   
   Платно: 
      ссылка https://api.proxyapi.ru/openai/v1 модель gpt-4o-mini  open api
      ссылка https://api.proxyapi.ru/google/v1/models/gemini-1.5-flash:generateContent модель gemini-1.5-flash request ключ от https://console.proxyapi.ru/billing
   
   В общем, смысл вы поняли, можно еще поискать варианты. Потом улучшу, просто гемени там отдельная струтура + request.

   Для тех кто знает, можете попробовать ввести что надо свое, но не тестировал. UPD: пока почему-то не работает.

4) Генерация голоса идет от тг бота силеро @silero_voice_bot, чтобы был голос Миты нужен скорее всего какой-то прем, посмотрите там что доступно по HD голосам и кол-ву символов (моих 66к хватает за глаза)
Там вроде 600 символов ежедневно есть, потестите сначала там фразу вручную, сделайте /mp3 файлы ответы миты (через меню настройте), и тогда мб что-то будет. Прем покупать лучше я думаю будет, когда все стабильно будет со стороны мода.
Особенно перед покупкой проверьте, как там в игре, был кейс что ffmpeg у товарища не сработал(

6) Для юзания силеро используется telegram api, то есть ваш акк (желательно не основной) превращается в бота, в том плане что им можно управлять из кода. Сделал так со своим, но оцените риски сами) Либо спросите у остальных.
Нужны api_id and api_hash, тут гайд как их получить: https://core.telegram.org/api/obtaining_api_id
Код исходный есть, риски я сказал. 
UPD: Если есть облачный пароль, вводите его. Он не видим, но есть ввести правильно и нажать ентер, то пройдет.


Вы дочитали до сюда? Возьмите с полки пирожок))


Благодарности - напишу тут, чтобы потом не забыть никого:
Sutherex - он показал мне openrouter, так что за работу с бесплатными ключами спасибо двойное ему. Помогал и помогает организационно, а также по теме нейроронок.
Доктор Диванных Наук - был еще на заре, первый тестер еще только чат бота, помогал множеством советов и хороших идей.
Романчо - помогает структурировать многочисленные задумки, чтобы потом их не забыть)
FlyOfFly - очень полезные советы и наработки по юнити, даже ввод текста он мне помог в начале прикрутить)
LoLY3_0 - его модели пока не вошли в мод, но час придет. Особенно кота на арбузе 3д. 
Всем тестировщиком первых дней после выхода того ролика (Особенно smarkloker), это было конечно тягомотно) 
 

