using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Menu UI")]
    [SerializeField] public GameObject pauseMenuPanel;

    [Header("Resume Button UI")]
    [SerializeField] private GameObject resumeButton;

    [Header("Inventory Button UI")]
    [SerializeField] private GameObject inventoryButton;

    [Header("Saved Game Message UI")]
    [SerializeField] private GameObject savedGameMessage;

    public bool isPaused {get; private set;}

    private static PauseMenuManager instance;

    //lembrar que pros animator funcionarem com o jogo pausado tem de usar o
    //unscaled time etc

    private void Awake(){
        if(instance != null){
            Debug.LogWarning("Found more than one Pause Menu Manager in the scene.");
        }
        instance = this;
    }

    private void Start(){
        
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        savedGameMessage.SetActive(false);

    }

    public static PauseMenuManager GetInstance(){
        return instance;
    }

    private void Update(){

        //handle player input to pause
        if(CharacterController2D.GetInstance().GetPausePressed()){
            
            if(isPaused == true){
                //resume game
                Resume();

            }else{
                //pause game
                Pause();

            }
        }

    }

    private void Pause(){
        //Debug.Log("The game should be paused.");
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        DisplayOptions(resumeButton);

    }

    private void Resume(){
        //Debug.Log("The game should resume.");
        savedGameMessage.SetActive(false);
        pauseMenuPanel.SetActive(false);
        InventoryManager.GetInstance().inventoryMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

    }

    private void Save(){

        DataPersistenceManager.instance.SaveGame();
        MessageGameSaved();

    }

    private void DisplayOptions(GameObject button){
        StartCoroutine(SelectFirstOption(button));
    }

    private IEnumerator SelectFirstOption(GameObject button){
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void ResumeButton(){
        Resume();
    }

    public void InventoryButton(){
        Debug.Log("The inventory should appear.");
        pauseMenuPanel.SetActive(false);
        InventoryManager.GetInstance().ShowInventory();
    }

    public void ShopButton(){
        Debug.Log("The shop should appear.");
    }

    public void SettingsButton(){
        Debug.Log("The game settings should appear.");
    }

    public void SaveButton(){

        Debug.Log("The game state should be saved.");
        Save();

    }

    //just exit really
    public void SaveAndExitButton(){
        //eu não coloquei pra ser obrigatório salvar na verdade
        //mas quealquer coisa muda aqui depois !
        Debug.Log("The player should go back to the Main Menu.");
        SceneManager.LoadScene("MainMenuScene");
    }

    private void MessageGameSaved(){
        savedGameMessage.SetActive(true);
        //it should animate but i was not able to do so
    }

    public void ComingBackFromInventory(){
        DisplayOptions(inventoryButton);
    }
}
