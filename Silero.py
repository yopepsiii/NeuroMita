from telethon import TelegramClient, events
import os
import sys
import time
import random
import pygame
import asyncio
from telethon.tl.types import MessageMediaDocument

import ffmpeg
import platform

from AudioConverter import AudioConverter
from utils import SH


# Пример использования:
class TelegramBotHandler:
    def __init__(self, gui, api_id, api_hash, phone, message_limit_per_minute=20):
        # Получение параметров из окружения

        self.api_id = api_id
        self.api_hash = api_hash
        self.phone = phone
        self.mita_ai_bot = '@CrazyMitaAIbot'  # Юзернейм бота
        self.gui = gui
        self.patch_to_sound_file = ""

        if getattr(sys, 'frozen', False):
            # Если программа собрана в exe, получаем путь к исполняемому файлу
            base_dir = os.path.dirname(sys.executable)

            # Альтернативный вариант: если ffmpeg всё же упакован в _MEIPASS
            alt_base_dir = sys._MEIPASS
        else:
            # Если программа запускается как скрипт
            base_dir = os.path.dirname(__file__)
            alt_base_dir = base_dir  # Для единообразия

        # Проверяем, где лежит ffmpeg
        ffmpeg_rel_path = os.path.join("ffmpeg-7.1-essentials_build", "bin", "ffmpeg.exe")

        ffmpeg_path = os.path.join(base_dir, ffmpeg_rel_path)
        if not os.path.exists(ffmpeg_path):
            # Если не нашли в base_dir, пробуем _MEIPASS (актуально для PyInstaller)
            ffmpeg_path = os.path.join(alt_base_dir, ffmpeg_rel_path)

        self.ffmpeg_path = ffmpeg_path

        # Системные параметры
        device_model = platform.node()  # Имя устройства
        system_version = f"{platform.system()} {platform.release()}"  # ОС и версия
        app_version = "1.0.0"  # Версия приложения задается вручную

        # Параметры бота
        self.message_limit_per_minute = message_limit_per_minute
        self.max_get_bot_answer_attempts = 50
        self.message_count = 0
        self.start_time = time.time()

        self.client = None
        try:
            # Создание клиента Telegram с системными параметрами
            self.client = TelegramClient(
                'session_name',
                int(self.api_id),
                self.api_hash,
                device_model=device_model,
                system_version=system_version,
                app_version=app_version
            )
        except:
            print(f"Проблема в ините тг. API ID: {self.api_id} - API HASH: {self.api_hash}")


    def reset_message_count(self):
        """Сбрасывает счетчик сообщений каждую минуту."""
        if time.time() - self.start_time > 60:
            self.message_count = 0
            self.start_time = time.time()

    async def play_audio(self, file_path):
        """Проигрывает audio/* файл."""

        def play():
            pygame.mixer.init()
            pygame.mixer.music.load(file_path)
            pygame.mixer.music.play()
            while pygame.mixer.music.get_busy():  # Ожидаем завершения воспроизведения
                pygame.time.Clock().tick(10)
            pygame.mixer.music.stop()  # Останавливаем воспроизведение
            pygame.mixer.quit()  # Закрываем микшер, чтобы освободить ресурсы

        # Выполняем блокирующую функцию в отдельном потоке
        await asyncio.to_thread(play)

    async def handle_voice_file(self, file_path):
        """Проигрывает звуковой файл."""
        try:
            print(f"Проигрываю файл: {file_path}")
            await self.play_audio(file_path)
            if os.path.exists(file_path):
                try:
                    await asyncio.sleep(0.02)
                    os.remove(file_path)
                    print(f"Файл {file_path} удалён.")
                except Exception as e:
                    print(f"Файл {file_path} НЕ удалён. Ошибка: {e}")
        except Exception as e:
            print(f"Ошибка при воспроизведении файла: {e}")
    
    async def send_and_receive(self, input_message):
        """Отправляет сообщение боту и обрабатывает ответ."""
        global message_count

        if not input_message:
            return

        self.reset_message_count()

        if self.message_count >= self.message_limit_per_minute:
            print("Превышен лимит сообщений. Ожидаем...")
            await asyncio.sleep(random.uniform(10, 15))
            return

        # Отправка сообщения боту
        await self.client.send_message(self.mita_ai_bot, f"/voice {input_message}")
        self.message_count += 1

        # Ожидание ответа от бота
        print("Ожидание ответа от бота...")
        response = None
        attempts = 0
        
        await asyncio.sleep(5)
        while attempts < self.max_get_bot_answer_attempts:  # Попытки получения ответа
            async for message in self.client.iter_messages(self.mita_ai_bot, limit=1):
                if message.media and isinstance(message.media, MessageMediaDocument):
                    # Проверяем тип файла и его атрибуты
                    if 'audio/ogg' in message.media.document.mime_type: # Было audio/mpeg
                        response = message
                        break
            if response:  # Если ответ найден, выходим из цикла
                break
            print(f"Попытка {attempts + 1}/{self.max_get_bot_answer_attempts}. Ответ от бота не найден.")
            attempts += 1
            await asyncio.sleep(0.1)  # Немного подождем

        if not response:
            print(f"Ответ от бота не получен после {self.max_get_bot_answer_attempts} попыток.")
            return

        # Обработка полученного сообщения
        if response.media and isinstance(response.media, MessageMediaDocument):
            if 'audio/ogg' in response.media.document.mime_type:  # Проверка OGG файла, было audio/mpeg
                file_path = await self.client.download_media(response.media)

                print(f"Файл загружен: {file_path}")
                absolute_audio_from_bot_path = os.path.abspath(file_path)
                if self.gui.ConnectedToGame:
                    # Генерируем путь для WAV-файла на основе имени исходного MP3
                    base_name = os.path.splitext(os.path.basename(file_path))[0]  # Получаем имя файла без расширения
                    wav_path = os.path.join(os.path.dirname(file_path), f"{base_name}.wav")  # Создаем новый путь

                    # Получаем абсолютный путь

                    absolute_wav_path = os.path.abspath(wav_path)
                    # Конвертируем в WAV
                    await AudioConverter.convert_to_wav(absolute_audio_from_bot_path, absolute_wav_path)

                    try:
                        print(f"Удаляю файл: {absolute_audio_from_bot_path}")
                        os.remove(absolute_audio_from_bot_path)
                        print(f"Файл {absolute_audio_from_bot_path} удалён.")
                    except OSError as remove_error:
                        print(f"Ошибка при удалении файла {absolute_audio_from_bot_path}: {remove_error}")

                    #.BnmRvcModel.process(absolute_wav_path, absolute_wav_path+"_RVC_.wav")

                    self.gui.patch_to_sound_file = absolute_wav_path
                    print(f"Файл wav загружен: {absolute_wav_path}")

                else:
                    print(f"Отправлен воспроизводится: {absolute_audio_from_bot_path}")
                    await self.handle_voice_file(file_path)
        elif response.text:  # Если сообщение текстовое
            print(f"Ответ от бота: {response.text}")

    async def start(self):

        print("Запуск коннектора ТГ!")
        try:    
            await self.client.start(phone=self.phone)
            print("Успешно авторизован!")
            await self.client.send_message(self.mita_ai_bot, "/start")
            self.gui.bot_connected.set(True)
        except AttributeError as e:
            self.gui.bot_connected.set(False)
            print(f"Переданы неверные параметры либо пустые: {str(e)}")    
        except Exception as e:
            self.gui.bot_connected.set(False)
            print(f"Ошибка авторизации: {e}")
        
