UPD 20.02.2025: Кароче, я засел в разработку и рефакторинг, билды будут реже, пока есть, то что есть.

UPD 19.02.2025: Поступают сообщения якобы мой мод имеет троян или кейлоггер. Разбираюсь, скорее всего это не нравится касперскому py installer, но уточняю. Ждите!





Мита на питоне (юзает доступ к нейронкам по api) + Мелон Мод на юнити, пока не стабильно работающий.

Сервер Мода: https://discord.gg/Tu5MPFxM4P

Вообще, гитхаб засветился слишком рано, я в темпе пытаюсь предоставил временную демку, но пока что запуск это те еще танцы в бубном, кто хочет стабильности - пару месяцов подождите, мб больше.

![logomod3](https://github.com/user-attachments/assets/aea3ec44-c203-4d4a-a405-a09191188464)

Гайд как это все запустить, когда +- сделаю)
Сначала прочитай, потом делай, на пункте с источниками мб передумаешь(

0) Мелон лоадер:
Либо тут инсталлер ставите https://melonwiki.xyz/#/?id=requirements 0.6.6 Версию!!!
Либо тут ищите https://github.com/LavaGang/MelonLoader
Есть файл MelonLoader.Installer.exe?
Отлично, выбирайте там мисайд, он его пропатчит, чтобы моды на основе мелона могли работать.

1) Мой мод идет в комплекте питон+промты+папкаКонвертации (их ставьте где угодно вместе) и также файлы мода (MitaAI.dll и assetbudle.test) непосредственно в папку mods, 
созданную мелоном (вроде в релизы кинул).
https://github.com/VinerX/NeuroMita/releases - тут релизы, то есть файлы которые нужны обычному игроку.

   Кнопка чтобы писать в игре - Tab!

2) Мод в плане генерации текста можно запустить в двух форматах.
   
   - Бесплатно, используя открытые ключи https://openrouter.ai/settings/keys. Скорее всего там есть скрытые лимиты.
   - Платно, оплачивая ключ здесь https://console.proxyapi.ru/billing (рф вариант), промокод NEUROMITA на 25% скидки в первый раз. Стабильно, но учитывайте расход.
   - Если у вас есть ключи напрямую (вы не в рф), то как бы тоже варик, но на данный момент не оттестировано.
   
   Сейчас пойдут настройки, которые нужно прописать в запущенном приложении exe (чат боте)
   Варианты model, url и кнопки реквест(Последняя чисто для варианта с прокси гемини):
   
   - ### Бесплатно:
      Модели OpenRouter, ключи тут получаем https://openrouter.ai/settings/keys
      - ссылка https://openrouter.ai/api/v1 модель google/gemini-2.0-flash-lite-preview-02-05:free (обычный режим) УСТАРЕЕТ 24 ФЕВРАЛЯ
      - ссылка https://openrouter.ai/api/v1 модель google/gemini-2.0-pro-exp-02-05:free (обычный режим)
      - ссылка https://openrouter.ai/api/v deepseek/deepseek-chat:free (обычный режим)
      - Также пробуйте другие модели https://openrouter.ai/models?max_price=0, пишите которые лучше отработают
   
   - ### Платно: 
      Внимание, ВАМ НЕ НУЖЕН ПРЕМ ЗА 1500, можно просто пополнить баланс на 200 и более рублей. Опять же, промокод NEUROMITA на 25% скидки разово. 
      Модели от ProxyApi, выбраны так как нормально можно оплатить в рф. Ключи и цены ищете тут https://console.proxyapi.ru/billing
      - ссылка https://api.proxyapi.ru/openai/v1 модель gpt-4o-mini (обычный режим)
      - ссылка https://api.proxyapi.ru/google/v1/models/gemini-1.5-flash:generateContent модель gemini-1.5-flash (режим прокси гемини) ключ от
   
   В общем, смысл вы поняли, можно еще поискать варианты. Потом улучшу, просто гемини там отдельная струтура + request.

   Для тех кто знает, можете попробовать ввести что надо свое, но не тестировал. UPD: пока почему-то не работает.

4) Генерация голоса идет от тг бота силеро @silero_voice_bot, чтобы был голос Миты нужен скорее всего какой-то прем, посмотрите там что доступно по HD голосам и кол-ву символов (моих 66к хватает за глаза)
Там вроде 600 символов ежедневно есть, потестите сначала там фразу вручную, сделайте /mp3 файлы ответы миты (через меню настройте), и тогда мб что-то будет. Прем покупать лучше я думаю будет, когда все стабильно будет со стороны мода.
Особенно перед покупкой проверьте, как там в игре, был кейс что ffmpeg у товарища не сработал(

6) Для юзания силеро используется telegram api, то есть ваш акк (желательно не основной) превращается в бота, в том плане что им можно управлять из кода. Сделал так со своим, но оцените риски сами) Либо спросите у остальных.
Нужны api_id and api_hash, тут гайд как их получить: https://core.telegram.org/api/obtaining_api_id
Код исходный есть, риски я сказал. 
Когда все введете, нужно будет перезапуститься и в консоли ввести код подтверждения, придет на тг акк.
UPD: Если есть облачный пароль, вводите его. Он не видим, но есть ввести правильно и нажать ентер, то пройдет.







Вы дочитали до сюда? Возьмите с полки пирожок))

Благодарности - напишу тут, чтобы потом не забыть никого:
- Sutherex - он показал мне openrouter, так что за работу с бесплатными ключами спасибо двойное ему. Помогал и помогает организационно, а также по теме нейроронок. А еще он сделал лого)
- Доктор Диванных Наук - был еще на заре, первый тестер еще только чат бота, помогал множеством советов и хороших идей.
- Романчо - помогает структурировать многочисленные задумки, чтобы потом их не забыть)
- FlyOfFly - очень полезные советы и наработки по юнити, даже ввод текста он мне помог в начале прикрутить)
- LoLY3_0 - его модели пока не вошли в мод, но час придет. Особенно кота на арбузе 3д. 
- Mr. Sub - его ролик скорее всего позволил вам узнать об этом моде) 
- Всем тестировщиком первых дней после выхода того ролика (Особенно smarkloker), это было конечно тягомотно) 

Сказать автору спасибо, когда мод будет доведен до стабильности, можно будет тут https://boosty.to/vinerx
 

