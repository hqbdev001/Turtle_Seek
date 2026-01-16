using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrtMain : MonoBehaviour
{
    public enum IngamePhase
    {
        Preparing,
        Showing,
        Guessing,
        Resulting
    }

    // UI Elements
    [SerializeField]
    private GameObject ui_Menu;

    [SerializeField]
    private GameObject ui_Ingame;

    [SerializeField]
    private GameObject ui_Result;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private TextMeshProUGUI textUI_RemainTime;

    [SerializeField]
    private TextMeshProUGUI textUI_Result;

    [SerializeField]
    private Image imgUI_TargetTurtle;

    // Ingame Elements
    [SerializeField]
    private PrtIngameSettings ingameSettings;

    [SerializeField]
    private List<PrtTurtle> turtles;

    [SerializeField]
    private List<Sprite> turtleSprites;

    private int targetTurtleIndex = -1;

    private List<int> turtleSpriteIndexes = new List<int>();
    private List<int> turtleSettingIndexes = new List<int>();

    private IEnumerator remainTimeControl = null;

    private IngamePhase currentPhase = IngamePhase.Preparing;

    /// Unity Methods
    private void Start()
    {
        currentPhase = IngamePhase.Preparing;

        for (int i = 0; i < ingameSettings.TurtleCount; i++)
            turtleSpriteIndexes.Add(i);

        for (int i = 0; i < ingameSettings.SettingCount; i++)
            turtleSettingIndexes.Add(i);

        // initialize
        SceneInitialize();
    }

    /// Public Methods
    public void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked!");

        IngameInitialize();

        // Start Ingame Logic
        IngameStart();
    }

    /// Private Methods
    // 게임 초기화
    private void SceneInitialize()
    {
        SetMainUIsByPhase();
    }

    private void IngameInitialize()
    {
        currentPhase = IngamePhase.Showing;

        // Set Turtles
        ShuffleIndexes();

        SetTurtleObjects();

        SetTargetTurtle();

        // Set UI
        SetMainUIsByPhase();
    }

    private void SetMainUIsByPhase()
    {
        switch (currentPhase)
        {
            case IngamePhase.Preparing:
                ui_Menu.SetActive(true);
                ui_Ingame.SetActive(false);
                ui_Result.SetActive(false);
                break;
            case IngamePhase.Showing:
                ui_Menu.SetActive(false);
                ui_Ingame.SetActive(true);
                ui_Result.SetActive(false);
                break;
            case IngamePhase.Guessing:
                ui_Menu.SetActive(false);
                ui_Ingame.SetActive(true);
                ui_Result.SetActive(false);
                break;
            case IngamePhase.Resulting:
                ui_Menu.SetActive(false);
                ui_Ingame.SetActive(true);
                ui_Result.SetActive(true);
                break;
            default:
                break;
        }

    }
    
    private void ShuffleIndexes()
    {
        turtleSpriteIndexes = turtleSpriteIndexes.OrderBy(x => UnityEngine.Random.value).ToList();
        turtleSettingIndexes = turtleSettingIndexes.OrderBy(x => UnityEngine.Random.value).ToList();

        targetTurtleIndex = turtleSpriteIndexes[0];
    }

    private void SetTurtleObjects()
    {

        for (int i = 0; i < turtles.Count; i++)
        {
            turtles[i].ResetTurtle();

            if (i < ingameSettings.TurtleCount)
            {
                int spriteIndex = turtleSpriteIndexes[i];
                int settingIndex = turtleSettingIndexes[i];

                turtles[i].Setindex(spriteIndex);

                // turtles 설정
                turtles[i].SetTurtleImage(turtleSprites[spriteIndex]);

                PrtIngameSettings.TurtleSettings settings = ingameSettings.GetTurtleSettings(settingIndex);

                turtles[i].SetTurtlePosition(settings.startPosition);
                turtles[i].SetTurtleSize(settings.startSize);

                turtles[i].SetOnTurtle((spriteIndex) =>
                {
                    ClearCheck(spriteIndex);
                });

                turtles[i].SetAlpha(1.0f);
            }
        }
    }

    private void SetTargetTurtle()
    { 
        imgUI_TargetTurtle.sprite = turtleSprites[targetTurtleIndex];
        imgUI_TargetTurtle.gameObject.SetActive(false);
    }

    // 인게임 로직
    private void IngameStart()
    {
        SetRemainTime(ingameSettings.TurtleShowingTime, () => {
            GuessingPhase();
        });

        if (ingameSettings.HidingTurtle)
            HidingTurtles();
    }

    private void GuessingPhase()
    {
        currentPhase = IngamePhase.Guessing;

        imgUI_TargetTurtle.gameObject.SetActive(true);

        SetRemainTime(ingameSettings.GuessingTime, () => {
            ResultingPhase(false);
        });
    }

    private void ResultingPhase(bool result)
    {
        currentPhase = IngamePhase.Resulting;
        
        SetMainUIsByPhase();

        textUI_Result.text = result ? "Bingo~!" : "Failed..";

        SetRemainTime(3.0f, () => {
            currentPhase = IngamePhase.Preparing;

            SetMainUIsByPhase();
        });
    }

    private void ClearCheck(int turtleIndex)
    {
        if (currentPhase != IngamePhase.Guessing)
            return;

        if (turtleIndex == targetTurtleIndex)
        {
            Debug.Log("Bingo!!!");

            StopCoroutine(remainTimeControl);
            remainTimeControl = null;

            ResultingPhase(true);
        }
        else
        {
            Debug.Log("Try Again~~");
        }
    }

    private void SetRemainTime(float second, Action onEnd)
    {
        remainTimeControl = Coroutine_SetRemainTime(second, onEnd);
        StartCoroutine(remainTimeControl);
    }

    private IEnumerator Coroutine_SetRemainTime(float second, Action onEnd)
    {
        float remainTime = second;
        while (remainTime >= 0)
        {
            textUI_RemainTime.text = Mathf.CeilToInt(remainTime).ToString();

            remainTime -= Time.deltaTime;

            yield return null;
        }

        onEnd?.Invoke();
    }

    private void HidingTurtles()
    {
        StartCoroutine(Coroutine_HidingTurtles(ingameSettings.TurtleShowingTime));
    }

    private IEnumerator Coroutine_HidingTurtles(float second)
    {
        float remainTime = second;
        while (remainTime >= 0)
        {
            float alpha = remainTime / second;

            for (int i = 0; i < ingameSettings.TurtleCount; i++)
                turtles[i].SetAlpha(alpha);

            remainTime -= Time.deltaTime;

            yield return null;
        }
    }
}
