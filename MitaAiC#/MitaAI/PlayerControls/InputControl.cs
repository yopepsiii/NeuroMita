﻿using Il2Cpp;
using MelonLoader;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;


namespace MitaAI.PlayerControls
{
    public static class InputControl
    {
        public static bool isInputBlocked = false; // Флаг для блокировки
        public static bool isInputActive = false; // Флаг для отслеживания активности ввода текста
        public static bool isInputLocked = false; // Флаг для блокировки закрытия поля ввода

        static GameObject InputFieldComponent;
        static InputField inputField; // Ссылка на компонент InputField

        public static void UpdateInput(string userInput)
        {
            if (inputField == null || string.IsNullOrEmpty(userInput)) return;

            // Сохраняем текущие позиции
            int caretPos = inputField.caretPosition;
            int selectionAnchor = inputField.selectionAnchorPosition;
            int selectionFocus = inputField.selectionFocusPosition;

            // Вставляем текст в позицию курсора
            inputField.text = inputField.text.Insert(caretPos, $" {userInput}");

            // Обновляем позиции курсора и выделения
            int newCaretPos = caretPos + userInput.Length;
            inputField.caretPosition = newCaretPos;
            inputField.selectionAnchorPosition = newCaretPos;
            inputField.selectionFocusPosition = newCaretPos;
        }

        // Метод для блокировки/разблокировки поля ввода
        public static void TurnBlockInputField(bool blocked)
        {
            isInputBlocked = blocked; // Устанавливаем блокировку
            if (InputFieldComponent != null)
            {
                InputFieldComponent.SetActive(!blocked); // Отключаем поле ввода, если оно активно
            }
        }

        public static void processInpute()
        {
            // Обработка блокировки движения при активном вводе
            if (isInputActive)
            {
                if (PlayerAnimationModded.playerMove != null)
                {
                    PlayerAnimationModded.playerMove.speedPlayer = 0f;
                    PlayerAnimationModded.UpdateSpeedAnimation(0f);
                    PlayerAnimationModded.StopPlayerAnimation();
                    PlayerAnimationModded.playerMove.dontMove = true;
                    Animator playerAnimator = PlayerAnimationModded.playerMove.GetComponent<Animator>();
                    if (playerAnimator != null) playerAnimator.SetFloat("Speed", 0f);
                }
            }
            else
            {
                if (PlayerAnimationModded.playerMove != null)
                {
                    PlayerAnimationModded.playerMove.speedPlayer = 1f;
                    PlayerAnimationModded.UpdateSpeedAnimation(1f);
                    PlayerAnimationModded.playerMove.dontMove = false;
                    Animator playerAnimator = PlayerAnimationModded.playerMove.GetComponent<Animator>();
                    if (playerAnimator != null) playerAnimator.SetFloat("Speed", 1f);
                    PlayerAnimationModded.currentPlayerMovement = PlayerAnimationModded.PlayerMovement.normal;
                }
            }

            // Обработка нажатия Enter для открытия/закрытия чата и отправки текста
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (InputFieldComponent == null)
                {
                    try
                    {
                        CreateInputComponent();
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Msg("CreateInputComponent ex:" + ex);
                        return; // Прекращаем выполнение, если создание компонента не удалось
                    }
                }

                if (isInputBlocked) return; // Если поле ввода заблокировано, ничего не делаем

                // Если поле ввода активно и текст не пустой, отправляем текст и скрываем поле
                if (isInputActive && inputField != null && !string.IsNullOrEmpty(inputField.text))
                {
                    ProcessInput(inputField.text); // Обрабатываем введенный текст
                    inputField.text = "";
                    InputFieldComponent.SetActive(false);
                    isInputActive = false;  // Ввод завершен, восстанавливаем движение
                    isInputLocked = false; // Разблокируем поле ввода
                }
                else
                {
                    // Переключаем видимость InputField
                    bool isActive = InputFieldComponent != null && InputFieldComponent.activeSelf;
                    if (InputFieldComponent != null)
                    {
                        InputFieldComponent.SetActive(!isActive);

                        // Если объект стал активным, активируем InputField
                        if (InputFieldComponent.activeSelf)
                        {
                            inputField.Select();
                            inputField.ActivateInputField();
                            isInputActive = true;  // Ввод активен
                            isInputLocked = true;  // Блокируем закрытие поля ввода
                        }
                        else
                        {
                            isInputActive = false;  // Ввод не активен
                            isInputLocked = false; // Разблокируем поле ввода
                        }
                    }
                }
            }

            // Дополнительная обработка нажатия клавиш
            if (Input.GetKeyDown(KeyCode.C) && !checkInput())
            {
                if (PlayerAnimationModded.playerMove != null)
                {
                    PlayerAnimationModded.playerMove.canSit = true;
                }
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                if (PlayerAnimationModded.playerMove != null)
                {
                    PlayerAnimationModded.playerMove.canSit = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !checkInput())
            {
                try
                {
                    MelonLogger.Msg("Space pressed");
                    if (PlayerAnimationModded.playerMove != null)
                    {
                        PlayerAnimationModded.currentPlayerMovement = PlayerAnimationModded.PlayerMovement.normal;
                    }
                }
                catch (Exception e)
                {
                    MelonLogger.Msg(e);
                }
            }
            else if (Input.GetKeyDown(KeyCode.J) )
            {
                MelonCoroutines.Start(changeMitaButtons());
            }




            // Постоянно возвращаем фокус на поле ввода, если оно активно
            if (isInputActive && inputField != null)
            {
                if (!inputField.isFocused)
                {
                    inputField.Select();
                    inputField.ActivateInputField();
                }
            }
        }
        private static DateTime _lastChangeTime = DateTime.MinValue; // Время последнего изменения
        private static readonly TimeSpan _cooldown = TimeSpan.FromSeconds(4); // Задержка в 5 секунд

        static IEnumerator changeMitaButtons()
        {
            MelonLogger.Msg("Try change Mita");

            // Проверяем нажатие клавиш
            if (Input.GetKeyDown(KeyCode.I))
            {
                MelonLogger.Msg("Try change to Kind");
                MitaCore.Instance.changeMita(MitaCore.KindObject, MitaCore.character.Kind);
                _lastChangeTime = DateTime.Now; // Обновляем время последнего изменения

                yield return new WaitForSeconds(0.25f);
                MitaCore.Instance.sendSystemMessage("Тебя только что заменили");

            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                MelonLogger.Msg("Try change to Cappy");
                MitaCore.Instance.changeMita(MitaCore.CappyObject, MitaCore.character.Cappy);
                _lastChangeTime = DateTime.Now;

                yield return new WaitForSeconds(0.25f);
                MitaCore.Instance.sendSystemMessage("Тебя только что заменили");

            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                MelonLogger.Msg("Try change to Crazy");
                MitaCore.Instance.changeMita(MitaCore.CrazyObject, MitaCore.character.Mita);
                _lastChangeTime = DateTime.Now;

                yield return new WaitForSeconds(0.25f);
                MitaCore.Instance.sendSystemMessage("Тебя только что заменили");

            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                MelonLogger.Msg("Try change to ShortHair");
                MitaCore.Instance.changeMita(MitaCore.ShortHairObject, MitaCore.character.ShortHair);
                _lastChangeTime = DateTime.Now;

                yield return new WaitForSeconds(0.25f);
                MitaCore.Instance.sendSystemMessage("Тебя только что заменили");

            }


        }

        static bool checkInput()
        {
            return InputFieldComponent != null && InputFieldComponent.activeSelf;
        }

        private static void CreateInputComponent()
        {
            // Создаем объект InputField
            InputFieldComponent = new GameObject("InputFieldComponent");

            inputField = InputFieldComponent.AddComponent<InputField>();
            var _interface = GameObject.Find("Interface");
            if (_interface == null)
            {
                MelonLogger.Msg("Interface not found!");
                return;
            }

            InputFieldComponent.transform.parent = _interface.transform;

            var rect = InputFieldComponent.AddComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;

            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);

            var image = InputFieldComponent.AddComponent<UnityEngine.UI.Image>();
            Sprite blackSprite = CreateBlackSprite(100, 100);
            image.sprite = blackSprite;

            image.color = new Color(0f, 0f, 0f, 0.7f);
            inputField.image = image;

            var TextLegacy = new GameObject("TextLegacy");
            var textComponent = TextLegacy.AddComponent<Text>();
            TextLegacy.transform.parent = InputFieldComponent.transform;
            var rectText = TextLegacy.GetComponent<RectTransform>();
            rectText.sizeDelta = new Vector2(500, 100);
            rectText.anchoredPosition = Vector2.zero;
            var texts = GameObject.FindObjectsOfType<Text>();

            foreach (var text in texts)
            {
                textComponent.font = text.font;
                textComponent.fontStyle = text.fontStyle;
                textComponent.fontSize = 35;
                if (textComponent.font != null) break;
            }

            inputField.textComponent = TextLegacy.GetComponent<Text>();
            inputField.text = "Введи текст";
            inputField.textComponent.color = Color.yellow;
            inputField.textComponent.alignment = TextAnchor.MiddleCenter;

            RectTransform parentRect = _interface.GetComponent<RectTransform>();
            float parentWidth = parentRect.rect.width;
            rect.sizeDelta = new Vector2(parentWidth * 0.7f, rect.sizeDelta.y);
            rectText.sizeDelta = rect.sizeDelta;
            inputField.Select();
            inputField.ActivateInputField();
        }

        // Пустышка для обработки ввода
        private static void ProcessInput(string inputText)
        {
            MelonLogger.Msg("Input received: " + inputText);
            MelonCoroutines.Start(MitaCore.Instance.PlayerTalk(inputText));
            MitaCore.Instance.playerMessage += $"{inputText}\n";
        }

        public static Sprite CreateBlackSprite(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            Color darkColor = new Color(0f, 0f, 0f, 0f);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    texture.SetPixel(x, y, darkColor);
                }
            }

            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }
    }
}