using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanquecasManager : MonoBehaviour, IDataPersistence
//o IDataPersistence é pra permitir que esse script consiga salvar e carregar dados
{
    [Header("Panquecas UI")]
    [SerializeField] private TextMeshProUGUI panquecasText;
    public int panquecasAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        panquecasAmount = ((Ink.Runtime.IntValue) DialogueManager
                            .GetInstance()
                            .GetVariableState("panquecas_amount")).value;
        panquecasText.text = "Panquecas: " + panquecasAmount.ToString();
    }

    //pra utilizar o IDataPersistence precisa dos métodos aquii
    public void LoadData(GameData data){
        this.panquecasAmount = data.panquecasAmount;

        ((Ink.Runtime.IntValue) DialogueManager
            .GetInstance()
            .GetVariableState("panquecas_amount")).value = this.panquecasAmount;
    }

    public void SaveData(GameData data){
        data.panquecasAmount = this.panquecasAmount;
    }

}
