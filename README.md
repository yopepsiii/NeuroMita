Мита на питоне (юзает доступ к нейронкам по api) + Мелон Мод на юнити, пока не очень работающий.

UPD: Пока ничего не сработало на тестах с другими, ждите.


Гитхаб засветился слишком рано, щас я в темпе пытаюсь предоставить временную демку, но пока тестов не было на других устройствах.

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

2) Мод в плане генерации текста можно запустить в двух форматах.
   
   а) Бесплатно, используя открытые ключи https://openrouter.ai/settings/keys
   б) Платно, оплачивая клюс тут https://console.proxyapi.ru/billing (рф вариант)
   в) Если у вас есть ключи напрямую (вы не в рф), то как бы тоже варик)
   
   Сейчас пойдут настройки, которые нужно прописать в запущенном приложении exe
   Варианты model,url и кнопки реквест:
   
   Бесплатно:
      https://openrouter.ai/api/v1 google/gemini-2.0-flash-lite-preview-02-05:free open api
   Платно: 
      gpt-4o-mini https://api.proxyapi.ru/openai/v1 open api
      gemini-1.5-flash https://api.proxyapi.ru/google/v1/models/gemini-1.5-flash:generateContent request
   
В общем, смысл вы поняли, можно еще поискать варианты. Потом улучшу, просто гемени там отдельная струтура + request.

Для тех кто знает, можете попробовать ввести что надо свое, но не тестировал.
Сохраняется все в системные переменные. 

4) Генерация голоса идет от тг бота силеро @silero_voice_bot, чтобы был голос Миты нужен скорее всего какой-то прем, посмотрите там что доступно по HD голосам и кол-ву символов (моих 66к хватает за глаза)
Там вроде 600 символов ежедневно есть, потестите сначала там фразу вручную, сделайте /mp3 файлы ответы миты (через меню настройте), и тогда мб что-то будет. Прем покпуать лучше я думаю будет, когда все стабильно будет со стороны мода.

6) Для юзания силеро используется telegram api, то есть ваш акк (желательно не основной) превращается в бота, в том плане что им можно управлять из кода. Сделал так со своим, но оцените риски сами)
Нужны api_id and api_hash, тут гайд как их получить: https://core.telegram.org/api/obtaining_api_id
Код исходный есть, риски я сказал. 
UPD: У чела с облачным паролем ну получилось его ввести при подтверждении. Походу с ним нельзя по api работать, увы(
