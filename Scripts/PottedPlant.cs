using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class PottedPlant : MonoBehaviour, IDataPersistence
{

    //pro teste de trocar a cor através da variavel da ink file
    [SerializeField] private Color defaultColor =  Color.white;
    [SerializeField] private Color redRoseColor = Color.red;
    [SerializeField] private Color callaLilyColor = Color.yellow;
    [SerializeField] private Color carnationColor = Color.magenta;

    private SpriteRenderer spriteRenderer;

    //Gustavo
    public SpriteRenderer plant;

    public Sprite[] redRoseStages;
    public Sprite[] callaLilyStages;
    public Sprite[] carnationStages;

    private int plantStage = 0;

    //Gustavo

    private bool toPlant = false;
    private bool toHarvest = false;
    private string plantNameAtPlantPot = "";

    private string vaso = "plant_pot_0";

    private string toPlantControl = "to_plant_0";
    private string plantStageControl = "plant_0";
    private string harvestControl = "harvest_0";

    private bool inProgress;
    private DateTime timerStart;
    private DateTime timerEnd;

    [Header("Production Time")]
    public int days;
    public int hours;
    public int minutes;
    public int seconds;

    [Header("Timer UI")]
    [SerializeField] private GameObject vasoTimeInfo;
    [SerializeField] private Image clock;
    [SerializeField] private TextMeshProUGUI timerText;

    private bool canSkip = false;

    private Coroutine lastTimer;
    private Coroutine lastDisplay;

    private void Awake(){
        switch(gameObject.name){
            case "Vaso01":
                vaso += "1";
                toPlantControl += "1";
                plantStageControl += "1_stage";
                harvestControl += "1";
                break;
            case "Vaso02":
                vaso += "2";
                toPlantControl += "2";
                plantStageControl += "2_stage";
                harvestControl += "2";
                break;
            default:
                Debug.LogWarning("Que vaso é esse: " + gameObject.name);
                break;
        }
    }

    private void Start(){
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        vasoTimeInfo.GetComponent<CanvasGroup>().alpha = 0;

        ((Ink.Runtime.StringValue) DialogueManager
        .GetInstance()
        .GetVariableState(vaso)).value = this.plantNameAtPlantPot;

        ((Ink.Runtime.IntValue) DialogueManager
        .GetInstance()
        .GetVariableState(plantStageControl)).value = this.plantStage;

        UpdatePlantStage(plantNameAtPlantPot);
        ChangePlantColor(plantNameAtPlantPot);
        if(plantNameAtPlantPot != "" && plantStage < 3){
            StartTimer();
        }
    }

    private void Update(){

        if(plantNameAtPlantPot != ""){

            if(plantStage == 3){
                //Debug.Log("Estágio final da planta.");

                timerText.text = "Fim!";
                vasoTimeInfo.GetComponent<CanvasGroup>().alpha = 1;

                toHarvest = ((Ink.Runtime.BoolValue) DialogueManager
                .GetInstance()
                .GetVariableState(harvestControl)).value;

                if(toHarvest){
                    //Debug.Log("To Harvest?" + toHarvest);
                    Harvest();
                }
            }

        }else{
            toPlant = ((Ink.Runtime.BoolValue) DialogueManager
                .GetInstance()
                .GetVariableState(toPlantControl)).value;
            if(toPlant){
                plantNameAtPlantPot = ((Ink.Runtime.StringValue) DialogueManager
                .GetInstance()
                .GetVariableState(vaso)).value;
                Plant();
            }

        }

    }

    private void ChangePlantColor(string plantName){
        //Debug.Log("Mudar cor do vaso de: " + plantName);
        switch(plantName){
            case "":
                spriteRenderer.color = defaultColor;
                break;
            case "roseira vermelha":
                spriteRenderer.color = redRoseColor;
                break;
            case "copo-de-leite":
                spriteRenderer.color = callaLilyColor;
                break;
            case "craveiro":
                spriteRenderer.color = carnationColor;
                break;
            default:
                Debug.LogWarning("Muda não reconhecida pelo switch statement: " + plantName);
                break;
        }

    }

    public void UpdatePlantStage(string plantName){
        //Debug.Log("Planta: " + plantName);
        switch(plantName){
            case "":
                plant.sprite = null;
                break;
            case "roseira vermelha":
                plant.sprite = redRoseStages[plantStage];
                break;
            case "copo-de-leite":
                plant.sprite = callaLilyStages[plantStage];
                break;
            case "craveiro":
                plant.sprite = carnationStages[plantStage];
                break;
            default:
                Debug.LogWarning("Planta não reconhecida pelo switch statement: " + plantName);
                break;
        }

    }

    public void Plant(){

        StartTimer();
        InitializeTimerInfo();

        ChangePlantColor(plantNameAtPlantPot);
        plantStage = 0;
        UpdatePlantStage(plantNameAtPlantPot);

        toPlant = false;
        ((Ink.Runtime.BoolValue) DialogueManager
        .GetInstance()
        .GetVariableState(toPlantControl)).value = toPlant;

    }

    public void Harvest(){

        plantNameAtPlantPot = "";

        ((Ink.Runtime.StringValue) DialogueManager
        .GetInstance()
        .GetVariableState(vaso)).value = plantNameAtPlantPot;
        
        ChangePlantColor(plantNameAtPlantPot);
        
        plantStage = 0;
        vasoTimeInfo.GetComponent<CanvasGroup>().alpha = 0;
        timerText.text = "";

        ((Ink.Runtime.IntValue) DialogueManager
        .GetInstance()
        .GetVariableState(plantStageControl)).value = plantStage;

        UpdatePlantStage(plantNameAtPlantPot);

        ((Ink.Runtime.BoolValue) DialogueManager
        .GetInstance()
        .GetVariableState(harvestControl)).value = false;

    }

    public void LoadData(GameData data){

        switch(vaso){
            case "plant_pot_01":
                this.plantNameAtPlantPot = data.plantInPot01;
                this.plantStage = data.plantInPot01Stage;
/*
                if(!String.IsNullOrEmpty(data.timeStart01)){
                    timerStart = DateTime.Parse(data.timeStart01);
                    timerEnd = DateTime.Parse(data.timeEnd01);
                }
*/
                break;
            case "plant_pot_02":
                this.plantNameAtPlantPot = data.plantInPot02;
                this.plantStage = data.plantInPot02Stage;
/*
                if(!String.IsNullOrEmpty(data.timeStart02)){
                    timerStart = DateTime.Parse(data.timeStart02);
                    timerEnd = DateTime.Parse(data.timeEnd02);
                }
*/
                break;
            default:
                Debug.LogWarning("Que vaso é esse: " + gameObject.name);
                break;
        }

    }

    public void SaveData(GameData data){

        switch(vaso){
            case "plant_pot_01":
                data.plantInPot01 = this.plantNameAtPlantPot;
                data.plantInPot01Stage = this.plantStage;

                data.timeStart01 = timerStart.ToString();
                data.timeEnd01 = timerEnd.ToString();

                break;
            case "plant_pot_02":
                data.plantInPot02 = this.plantNameAtPlantPot;
                data.plantInPot02Stage = this.plantStage;

                data.timeStart02 = timerStart.ToString();
                data.timeEnd02 = timerEnd.ToString();

                break;
            default:
                Debug.LogWarning("Que vaso é esse: " + gameObject.name);
                break;
        }

    }

    private void InitializeTimerInfo(){
        if(inProgress){
            vasoTimeInfo.GetComponent<CanvasGroup>().alpha = 1;
            lastDisplay = StartCoroutine(DisplayTime());
        }
        else{
            vasoTimeInfo.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    //Tamara Makes Games
    private IEnumerator DisplayTime(){
        DateTime start = DateTime.Now;
        TimeSpan timeLeft = timerEnd - start;
        double totalSecondsLeft = timeLeft.TotalSeconds;
        double totalSeconds = (timerEnd - timerStart).TotalSeconds;
        string text;

        while(vasoTimeInfo.activeSelf){
            text = "";
            clock.fillAmount = 1 - Convert.ToSingle((timerEnd - DateTime.Now).TotalSeconds/totalSeconds);

            if(totalSecondsLeft > 1){

                if(timeLeft.Days != 0){
                    text += timeLeft.Days + "d ";
                    text += timeLeft.Hours + "h";
                    yield return new WaitForSeconds(timeLeft.Minutes * 60);
                }
                else if(timeLeft.Hours != 0){
                    text += timeLeft.Hours + "h ";
                    text += timeLeft.Minutes + "m";
                    yield return new WaitForSeconds(timeLeft.Seconds);
                }
                else if(timeLeft.Minutes != 0){
                    TimeSpan ts = TimeSpan.FromSeconds(totalSecondsLeft);
                    text += ts.Minutes + "m ";
                    text += ts.Seconds + "s";
                }
                else{
                    text += Mathf.FloorToInt((float) totalSecondsLeft) + "s";
                }

                timerText.text = text;
                totalSecondsLeft -= Time.unscaledDeltaTime;
                yield return null;

            }
            else{
                timerText.text = "Fim!";
                clock.fillAmount = 1;
                canSkip = false;
                inProgress = false;
                break;
            }
        }
        yield return null;
    }

    public void SkipTimer(){
        //passa o tempo de um estagio

        if(canSkip){
            timerEnd = DateTime.Now;
            StopCoroutine(lastTimer);
            StopCoroutine(lastDisplay);
            clock.fillAmount = 1;
            timerText.text = "";
            canSkip = false;

            if(plantStage < 3){

                plantStage++;

                ((Ink.Runtime.IntValue) DialogueManager
                .GetInstance()
                .GetVariableState(plantStageControl)).value = plantStage;

                UpdatePlantStage(plantNameAtPlantPot);
                if(plantStage != 3){
                    StartTimer();
                }
            }
        }
    }

    #region Timed Events

    private void StartTimer(){

        timerStart = DateTime.Now;
        TimeSpan time = new TimeSpan(days, hours, minutes, seconds);
        timerEnd = timerStart.Add(time);
        inProgress = true;
        canSkip = true;

        Debug.Log("Timer: " + timerEnd);

        lastTimer = StartCoroutine(Timer());

        InitializeTimerInfo();

    }

    private IEnumerator Timer(){
        DateTime start = DateTime.Now;
        double secondsToFinished = (timerEnd - start).TotalSeconds;
        yield return new WaitForSeconds(Convert.ToSingle(secondsToFinished));

        Debug.Log("Terminou este estágio.");

        if(plantStage < 3){
            plantStage++;

            ((Ink.Runtime.IntValue) DialogueManager
            .GetInstance()
            .GetVariableState(plantStageControl)).value = plantStage;

            UpdatePlantStage(plantNameAtPlantPot);
            if(plantStage != 3){
                StartTimer();
            }
        }
    }

    #endregion

}
