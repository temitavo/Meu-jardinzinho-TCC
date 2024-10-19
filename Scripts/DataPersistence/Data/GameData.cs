using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string plantInPot01;
    public string plantInPot02;

    public int quantiRedRose;
    public int quantiCallaLily;
    public int quantiCarnation;

    public int plantInPot01Stage;
    public int plantInPot02Stage;

    public int panquecasAmount;

    public string timeStart01;
    public string timeEnd01;
    public string timeStart02;
    public string timeEnd02;

    public string INKVariables;

    //quando não tiver save file to load porque ainda não foi iniciado o jogo
    //esses são os valores que serão utilizados
    public GameData(){
        this.panquecasAmount = 0;

        this.plantInPot01 = "";
        this.plantInPot02 = "";

        this.quantiRedRose = 0;
        this.quantiCallaLily = 0;
        this.quantiCarnation = 0;

        this.plantInPot01Stage = 0;
        this.plantInPot02Stage = 0;

        this.timeStart01 = "";
        this.timeEnd01 = "";

        this.timeStart02 = "";
        this.timeEnd02 = "";

        this.INKVariables = "";
    }

}
