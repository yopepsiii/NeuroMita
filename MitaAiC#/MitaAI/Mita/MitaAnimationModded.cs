﻿using Il2Cpp;

using MelonLoader;
using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;
namespace MitaAI.Mita
{
    public static class MitaAnimationModded
    {
        static private Queue<string> animationQueue = new Queue<string>();
        static private bool isPlaying = false;
        static private Il2CppAssetBundle bundle;
        static Animator_FunctionsOverride mitaAnimatorFunctions;
        static Location34_Communication location34_Communication;
        static AnimationClip idleAnimation;
        static AnimationClip idleWalkAnimation;
        // Основной метод для добавления анимации в очередь
        static RuntimeAnimatorController runtimeAnimatorController;
        static AnimatorOverrideController overrideController;
        static Animator animator;

        public enum IdleStates
        {
            normal = 0,
            talkWithPlayer = 1,

        }



        static public void init(Animator_FunctionsOverride _mitaAnimatorFunctions, Location34_Communication _location34_Communication)
        {
            // Получаем компонент Animator_FunctionsOverride из текущего объекта
            mitaAnimatorFunctions = _mitaAnimatorFunctions;
            location34_Communication = _location34_Communication;

            bundle = AssetBundleLoader.LoadAssetBundle("assetbundle");
            if (mitaAnimatorFunctions == null)
            {
                MelonLogger.Msg("Animator_FunctionsOverride component not found on this object!");
            }
            idleAnimation = location34_Communication.mitaAnimationIdle;
            idleWalkAnimation = location34_Communication.mitaAnimationWalk;

            try
            {
                runtimeAnimatorController = AssetBundleLoader.LoadAnimatorControllerByName(bundle, "Mita_1.controller");
                MelonLogger.Msg("a");
                animator = MitaCore.Instance.MitaPersonObject.GetComponent<Animator>();
                animator.runtimeAnimatorController = runtimeAnimatorController;
                foreach (var item in runtimeAnimatorController.animationClips)
                {
                    item.events = Array.Empty<AnimationEvent>();
                }
                //setIdleWalk("Mita Walk_1");
                MelonLogger.Msg("b!");
            }
            catch (Exception ex)
            {

                MelonLogger.Msg("Error custom controller"+ex);
            }
            
        }

        public static string setAnimation(string response)
        {
            // Регулярное выражение для извлечения эмоций
            string pattern = @"<a>(.*?)</a>";
            Match match = Regex.Match(response, pattern);

            string animName = string.Empty;
            string cleanedResponse = Regex.Replace(response, @"<a>.*?</a>", ""); // Очищаем от всех тегов

            if (match.Success)
            {
                // Если эмоция найдена, устанавливаем её в переменную faceStyle
                animName = match.Groups[1].Value;
            }
            try
            {
                // Проверка на наличие объекта Mita перед применением эмоции
                if (MitaCore.Instance.Mita == null || MitaCore.Instance.Mita.gameObject == null)
                {
                    MelonLogger.Error("Mita object is null or Mita.gameObject is not active.");
                    return cleanedResponse; // Возвращаем faceStyle и очищенный текст
                }
                // Устанавливаем лицо, если оно найдено
                switch (animName)
                {
                    case "Щелчек":
                        int randomIndex = UnityEngine.Random.Range(0, 4); // Генерация числа от 0 до 3
                        string animationName;

                        if (randomIndex == 0)
                            animationName = "Mita Click_0";
                        else if (randomIndex == 1)
                            animationName = "Mita Click_1";
                        else if (randomIndex == 2)
                            animationName = "Mita Click_2";
                        else
                            animationName = "Mita Click"; // Четвёртый кейс

                        EnqueueAnimation(animationName);
                        break;
                    case "Похлопать в ладоши":
                        EnqueueAnimation("Mita Idle Cheerful");
                        //EnqueueAnimation("Mita Cheerful");
                        break;
                    case "Указать направление":
                        EnqueueAnimation("Mita ShowTumb");
                        break;
                    case "Смотреть с презрением":
                        EnqueueAnimation("Mita IdleBat");
                        break;
                    case "Показать усталость":
                        EnqueueAnimation("Mita Start Tired");
                        setIdleAnimation("MiMita Tired");
                        break;
                    case "Притвориться отключенной и упасть":
                        EnqueueAnimation("MitaBody Fall");
                        break;
                    case "Взять предмет":
                        EnqueueAnimation("Mita TakeBat");
                        break;
                    case "Кивнуть да":
                        MitaCore.Instance.MitaLook.Nod(true);
                        break;
                    case "Кивнуть нет":
                        MitaCore.Instance.MitaLook.Nod(false);
                        break;
                    case "Глянуть глазами в случайном направлении":
                        MitaCore.Instance.MitaLook.EyesLookOffsetRandom(90);
                        break;
                    case "Повернуться в случайном направлении":
                        MitaCore.Instance.MitaLook.LookRandom();
                        break;
                    case "Развести руки":
                        EnqueueAnimation("Mita StartShow Knifes");
                        //EnqueueAnimation("Mita Throw Knifes");
                        
                        break;
                    case "Поднести палец к подбородку":
                        EnqueueAnimation("Mita TalkWithPlayer");
                        setIdleAnimation("Mita TalkWithPlayer");
                        break;
                    case "Поднять игрока одной рукой":
                        EnqueueAnimation("Mita TakeMita");
                        //EnqueueAnimation("Mita TakeMita Idle");
                        setIdleAnimation("Mita TakeMita Idle");
                        //EnqueueAnimation("Mita ThrowPlayer");
                        break;
                    case "Сложить руки перед собой":
                        EnqueueAnimation("Mita Hands Down Idle");
                        setIdleAnimation("Mita Hands Down Idle");
                        break;
                    case "Показать предмет":
                        EnqueueAnimation("Mita Selfi");
                        break;
                    case "Прикрыть глаза":
                        EnqueueAnimation("Mita Close Eyes");
                        //int randomIndex = UnityEngine.Random.Range(0, 4); // Генерация числа от 0 до 3
                        //EnqueueAnimation("Mita Open Eyes");
                        //EnqueueAnimation("Mita Open Shar Eyes");
                        break;
                    case "Обнять":
                        EnqueueAnimation("Mita StartHug");
                        //EnqueueAnimation("Mita HugIdle");
                        //EnqueueAnimation("Mita StopHug");
                        break;
                    case "Удар":
                        EnqueueAnimation("Mita Kick");
                        break;
                    case "Похвастаться предметом":
                        EnqueueAnimation("Mita Take Recorder");
                        break;
                    case "Прикрыть рот и помахать рукой":
                        EnqueueAnimation("Mita Oi");
                        //EnqueueAnimation("Mita Idle");
                        EnqueueAnimation("Mita Heh");
                        break;
                       
                    case "Случайная анимация":
                        EnqueueAnimation("");
                        break;
                    default:
                        if (animName != "")
                        {
                            EnqueueAnimation(animName);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Problem with Animation: {ex.Message}");
            }

            // Возвращаем кортеж: лицо и очищенный текст
            return cleanedResponse;
        }


        static public void EnqueueAnimation(string animName = "")
        {
           /* if (bundle == null)
            {
                bundle = AssetBundleLoader.LoadAssetBundle("assetbundle");
            }*/

            //AnimationClip anim = null;
            try
            {
                /*if (!string.IsNullOrEmpty(animName))
                {
                    anim = AssetBundleLoader.LoadAnimationClipByName(bundle, animName);
                }
                else
                {
                    anim = AssetBundleLoader.LoadRandomAnimationClip(bundle);
                }*/

                /*if (anim != null)
                {*/
                    //anim.events = Array.Empty<AnimationEvent>();
                    animationQueue.Enqueue(animName);
                    MelonLogger.Msg($"Added to queue: {animName}");

                    if (!isPlaying)
                    {
                        MelonCoroutines.Start(ProcessQueue());
                    }
               // }
            }
            catch (Exception e)
            {
                MelonLogger.Msg("Animation error: " + e);
            }
        }
        static AnimationClip FindAnimationClipByName(string animationName)
        {
            if (runtimeAnimatorController == null) return null;
            // Получаем все анимации из Animator Controller
            // AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            AnimationClip[] clips = runtimeAnimatorController.animationClips;
            // Проверяем, есть ли анимация с указанным именем
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == animationName)
                {
                    return clip;
                }
            }

            return null;
        }

        // Корутина для последовательного проигрывания
        static private IEnumerator ProcessQueue()
        {
            isPlaying = true;
            location34_Communication.enabled = false;
            while (animationQueue.Count > 0)
            {
                string animName = animationQueue.Dequeue();
                AnimationClip anim = FindAnimationClipByName(animName);
                if ( anim!=null)
                {
                    //if (mitaAnimatorFunctions.anim.runtimeAnimatorController != runtimeAnimatorController) mitaAnimatorFunctions.anim.runtimeAnimatorController = runtimeAnimatorController;
                    MelonLogger.Msg($"Crossfade");
                    MelonLogger.Msg($"Now playing: {animName}");
                    mitaAnimatorFunctions.anim.CrossFade(animName, 0.25f);
                }

                else 
                {
                    anim = AssetBundleLoader.LoadAnimationClipByName(bundle, animName);
                    if (anim != null)
                    {
                        MelonLogger.Msg($"Usual case");
                        mitaAnimatorFunctions.AnimationClipSimpleNext(anim);
                    }
                    else
                    {
                        MelonLogger.Msg($"Not found state or clip");
                    }

                }
                // Ждем завершения анимации
                yield return WaitForAnimationCompletion(anim,true,0.25f);
                
            }
            MelonLogger.Msg("Ended quque");
            animator.CrossFade("Idle",0.25f);
            location34_Communication.enabled = true;
            isPlaying = false;
        }

        static private IEnumerator WaitForAnimationCompletion(AnimationClip animation, bool isCustomAnimation, float fadeDuration)
        {
            if (isCustomAnimation)
            {
                // Для анимаций через Animator Controller
                float startTime = Time.time;

                // Ожидаем начала перехода
                while (animator.IsInTransition(0) && Time.time - startTime < fadeDuration)
                {
                    yield return null;
                }

                // Ожидаем завершения анимации
                AnimatorStateInfo stateInfo;
                do
                {
                    stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    yield return null;
                }
                while (stateInfo.IsName(animation.name) && stateInfo.normalizedTime < 1.0f);
            }
            else
            {
                // Для обычных анимаций без transitions
                yield return new WaitForSeconds(animation.length + fadeDuration);
            }
        }

        static public void setIdleAnimation(string animName)
        {
            if (bundle == null)
            {
                bundle = AssetBundleLoader.LoadAssetBundle("assetbundle");
            }

            AnimationClip anim = null;
            if (!string.IsNullOrEmpty(animName))
            {
                anim = AssetBundleLoader.LoadAnimationClipByName(bundle, animName);
                location34_Communication.mitaAnimationIdle = anim;
                if (overrideController == null) overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                overrideController.SetClip(idleAnimation, anim,true);
            }

        }
        static public void setIdleWalk(string animName)
        {
            if (bundle == null)
            {
                bundle = AssetBundleLoader.LoadAssetBundle("assetbundle");
            }

            AnimationClip anim = null;
            if (!string.IsNullOrEmpty(animName))
            {
                anim = AssetBundleLoader.LoadAnimationClipByName(bundle, animName);
                location34_Communication.mitaAnimationWalk = anim;
                mitaAnimatorFunctions.AnimationClipWalk(anim);
            }


        }

        // Очистка очереди (опционально)
        static public void ClearQueue()
        {
            animationQueue.Clear();
            isPlaying = false;
        }
    }

}