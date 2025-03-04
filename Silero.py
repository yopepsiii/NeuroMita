from telethon import TelegramClient, events
import os
import sys
import time
import random
import pygame
import asyncio
from telethon.tl.types import MessageMediaDocument, DocumentAttributeAudio

import ffmpeg
import platform

from AudioConverter import AudioConverter
from utils import SH


# Пример использования:
class TelegramBotHandler:

    def __init__(self, gui, api_id, api_hash, phone, tg_bot, message_limit_per_minute=20):
        # Получение параметров из окружения
        self.api_id = api_id
        self.api_hash = api_hash
        self.phone = phone
        self.tg_bot = tg_bot  # Юзернейм Silero бота
        self.gui = gui
        self.patch_to_sound_file = ""
        self.last_speaker_command = ""

        self.silero_time_limit = 8

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
            print("Проблема в ините тг")
            print(SH(self.api_id))
            print(SH(self.api_hash))

    def reset_message_count(self):
        """Сбрасывает счетчик сообщений каждую минуту."""
        if time.time() - self.start_time > 60:
            self.message_count = 0
            self.start_time = time.time()

    async def play_audio(self, file_path):
        """Проигрывает аудиофайл (MP3/OGG)."""

        def play():
            pygame.mixer.init()
            pygame.mixer.music.load(file_path)  # Pygame поддерживает MP3 и OGG
            pygame.mixer.music.play()
            while pygame.mixer.music.get_busy():  # Ждем завершения воспроизведения
                pygame.time.Clock().tick(10)
            pygame.mixer.music.stop()
            pygame.mixer.quit()

        await asyncio.to_thread(play)  # Запуск в отдельном потоке

    async def handle_voice_file(self, file_path):
        """Проигрывает звуковой файл (MP3 или OGG)."""
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

    async def send_and_receive(self, input_message, speaker_command):
        """Отправляет сообщение боту и обрабатывает ответ."""
        global message_count

        if not input_message or not speaker_command:
            return

        if self.last_speaker_command != speaker_command:
            await self.client.send_message(self.tg_bot, speaker_command)
            self.last_speaker_command = speaker_command
            await asyncio.sleep(0.25)

            if self.gui.silero_turn_off_video:
                await self.client.send_message(self.tg_bot, "/videonotes")

                await asyncio.sleep(0.65)

                # Получаем последнее сообщение от бота
                messages = await self.client.get_messages(self.tg_bot, limit=1)
                last_message = messages[0] if messages else None

                # Проверяем содержимое последнего сообщения
                if last_message and last_message.text == "Кружки включены!":
                    # Если условие выполнено, отправляем команду еще раз
                    await self.client.send_message(self.tg_bot, "/videonotes")

        self.last_speaker_command = speaker_command

        self.reset_message_count()

        if self.message_count >= self.message_limit_per_minute:
            print("Превышен лимит сообщений. Ожидаем...")
            await asyncio.sleep(random.uniform(10, 15))
            return

        # Отправка сообщения боту
        if self.tg_bot == "@CrazyMitaAIbot":
            input_message = f"/voice {input_message}"
        await self.client.send_message(self.tg_bot, input_message)
        self.message_count += 1

        # Ожидание ответа от бота
        print("Ожидание ответа от бота...")
        response = None
        attempts = 0
        attempts_per_second = 4
        attempts_max = self.silero_time_limit * attempts_per_second
        await asyncio.sleep(0.7)
        while attempts <= attempts_max:  # Попытки получения ответа

            async for message in self.client.iter_messages(self.tg_bot, limit=1):
                if message.media and isinstance(message.media, MessageMediaDocument):
                    doc = message.media.document

                    # Проверяем MP3-файл
                    if 'audio/mpeg' in doc.mime_type:
                        response = message
                        break

                    # Проверяем голосовое сообщение (OGG, voice)
                    if 'audio/ogg' in doc.mime_type:
                        for attr in doc.attributes:
                            if isinstance(attr, DocumentAttributeAudio) and attr.voice:
                                response = message
                                break
            if response:  # Если ответ найден, выходим из цикла
                break
            print(f"Попытка {attempts + 1}/{attempts_max}. Ответ от бота не найден.")
            attempts += 1
            await asyncio.sleep(1 / attempts_per_second)  # Немного подождем

        if not response:
            print(f"Ответ от бота не получен после {attempts_max} попыток.")
            return

        print("Ответ получен")
        # Обработка полученного сообщения
        if response.media and isinstance(response.media, MessageMediaDocument):
            if response and response.media:
                file_path = await self.client.download_media(response.media)

                print(f"Файл загружен: {file_path}")
                sound_absolute_path = os.path.abspath(file_path)
                if self.gui.ConnectedToGame:
                    print("Подключен к игре, нужна конвертация")

                    # Генерируем путь для WAV-файла на основе имени исходного MP3
                    base_name = os.path.splitext(os.path.basename(file_path))[0]  # Получаем имя файла без расширения
                    wav_path = os.path.join(os.path.dirname(file_path), f"{base_name}.wav")  # Создаем новый путь

                    # Получаем абсолютный путь

                    absolute_wav_path = os.path.abspath(wav_path)
                    # Конвертируем MP3 в WAV
                    await  AudioConverter.convert_mp3_to_wav(sound_absolute_path, absolute_wav_path)
                    #await AudioConverter.convert_to_wav(sound_absolute_path, absolute_wav_path)

                    try:
                        print(f"Удаляю файл: {sound_absolute_path}")
                        os.remove(sound_absolute_path)
                        print(f"Файл {sound_absolute_path} удалён.")
                    except OSError as remove_error:
                        print(f"Ошибка при удалении файла {sound_absolute_path}: {remove_error}")

                    #.BnmRvcModel.process(absolute_wav_path, absolute_wav_path+"_RVC_.wav")

                    self.gui.patch_to_sound_file = absolute_wav_path
                    print(f"Файл wav загружен: {absolute_wav_path}")



                else:
                    print(f"Отправлен воспроизводится: {sound_absolute_path}")
                    await self.handle_voice_file(file_path)
        elif response.text:  # Если сообщение текстовое
            print(f"Ответ от бота: {response.text}")

    async def start(self):

        print("Запуск коннектора ТГ!")
        try:
            pass
            await self.client.start(phone=self.phone)  # тут на macOS проблема: какая-то фигня с ткинтером

            self.gui.silero_connected.set(True)
            print("Успешно авторизован!")

            await self.client.send_message(self.tg_bot, "/start")
            await asyncio.sleep(0.35)
            if self.tg_bot == "@silero_voice_bot":
                await self.client.send_message(self.tg_bot, "/speaker mita")
                self.last_speaker_command = "/speaker mita"
                await asyncio.sleep(0.35)
                await self.client.send_message(self.tg_bot, "/mp3")
                await asyncio.sleep(0.35)
                await self.client.send_message(self.tg_bot, "/hd")
                await asyncio.sleep(0.35)
                await self.client.send_message(self.tg_bot, "/videonotes")
            print("Включено все в ТГ для сообщений миты")
        except Exception as e:
            self.gui.silero_connected.set(False)
            print(f"Ошибка авторизации: {e}")
