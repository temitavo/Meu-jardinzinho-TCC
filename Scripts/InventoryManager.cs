using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryManager : MonoBehaviour, IDataPersistence
{

    [Header("Inventory Menu UI")]
    [SerializeField] public GameObject inventoryMenuPanel;

    [Header("Exit Inventory Button UI")]
    [SerializeField] private GameObject exitInventoryButton;

    [Header("Quantidades UI")]
    [SerializeField] private TextMeshProUGUI redRoseText;
    [SerializeField] private TextMeshProUGUI callaLilyText;
    [SerializeField] private TextMeshProUGUI carnationText;

    public int quantiRedRose = 0;
    public int quantiCallaLily = 0;
    public int quantiCarnation = 0;



    private static InventoryManager instance;

    private void Awake(){
        if(instance != null){
            Debug.LogWarning("Found more than one Pause Menu Manager in the scene.");
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        inventoryMenuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update(){
        
        quantiRedRose = ((Ink.Runtime.IntValue) DialogueManager
                            .GetInstance()
                            .GetVariableState("quanti_red_rose")).value;
        quantiCallaLily = ((Ink.Runtime.IntValue) DialogueManager
                            .GetInstance()
                            .GetVariableState("quanti_calla_lily")).value;
        quantiCarnation = ((Ink.Runtime.IntValue) DialogueManager
                            .GetInstance()
                            .GetVariableState("quanti_carnation")).value;
        
        redRoseText.text = "Roseiras vermelhas: x" + quantiRedRose.ToString();
        callaLilyText.text = "Copos-de-leite: x" + quantiCallaLily.ToString();
        carnationText.text = "Craveiros: x" + quantiCarnation.ToString();
    }

    public static InventoryManager GetInstance(){
        return instance;
    }

    public void ShowInventory(){
        //Debug.Log("The inventory should appear.");
        inventoryMenuPanel.SetActive(true);
        DisplayOptions(exitInventoryButton);

    }

    private void CloseInventory(){
        //Debug.Log("The inventory should be closed.");
        inventoryMenuPanel.SetActive(false);
        PauseMenuManager.GetInstance().pauseMenuPanel.SetActive(true);
        PauseMenuManager.GetInstance().ComingBackFromInventory();

    }
    private void DisplayOptions(GameObject button){
        StartCoroutine(SelectFirstOption(button));
    }

    private IEnumerator SelectFirstOption(GameObject button){
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void CloseInventoryButton(){
        CloseInventory();

    }

    public void LoadData(GameData data){
        this.quantiRedRose = data.quantiRedRose;
        this.quantiCallaLily = data.quantiCallaLily;
        this.quantiCarnation = data.quantiCarnation;

        ((Ink.Runtime.IntValue) DialogueManager
            .GetInstance()
            .GetVariableState("quanti_red_rose")).value = this.quantiRedRose;
        
        ((Ink.Runtime.IntValue) DialogueManager
            .GetInstance()
            .GetVariableState("quanti_calla_lily")).value = this.quantiCallaLily;

        ((Ink.Runtime.IntValue) DialogueManager
            .GetInstance()
            .GetVariableState("quanti_carnation")).value = this.quantiCarnation;
    }

    public void SaveData(GameData data){
        data.quantiRedRose = this.quantiRedRose;
        data.quantiCallaLily = this.quantiCallaLily;
        data.quantiCarnation = this.quantiCarnation;
    }

}
