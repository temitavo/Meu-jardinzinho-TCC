using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu UI")]
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Select Save File Menu UI")]
    [SerializeField] private GameObject selectSavePanel;

    [Header("Settings Menu UI")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Start Button UI")]
    [SerializeField] private GameObject startButton;

    [Header("Game Select Save File Buttons UI")]
    [SerializeField] private GameObject gameButton01GameObj;
    [SerializeField] private Button gameButton01;
    [SerializeField] private Button gameButton02;
    [SerializeField] private Button gameButton03;
    [SerializeField] private Button goBackFromSelectSaveButton;

    [Header("Volume Slider UI")]
    [SerializeField] private GameObject volumeButton;
    [Header("Settings Button UI")]
    [SerializeField] private GameObject settingsButton;

    [Header("Fading Panel UI")]
    [SerializeField] private GameObject fadingPanel;

    public AudioMixer audioMixer;

    private static MainMenuManager instance;

    private bool confirmPressed = false;
	private bool cancelPressed = false;

    private float volume;

    private void Awake(){
        if(instance != null){
            Debug.LogWarning("Found more than one Main Menu Manager in the scene.");
        }
        instance = this;
    }

    private void Start(){

        Time.timeScale = 1;

        //não sei guardar o valor do volume do audio mixer por isso fiz isso
        //pra evitar bugs por enquanto
        audioMixer.SetFloat("Volume", volume);
        volumeButton.GetComponent<Slider>().value = 0;
        //
        //caso não tenha save muda o texto pra iniciarrr
        if(!DataPersistenceManager.instance.HasGameData()){
            //por enquanto é um save pra todos os botões...
            gameButton01.GetComponentInChildren<Text>().text = "Iniciar Jogo 01";
            gameButton02.GetComponentInChildren<Text>().text = "Iniciar Jogo 02";
            gameButton03.GetComponentInChildren<Text>().text = "Iniciar Jogo 03";
        }

        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        selectSavePanel.SetActive(false);
        DisplayOptions(startButton);

    }

    public static MainMenuManager GetInstance(){
        return instance;
    }

    //Gustavo
	public void Move(InputAction.CallbackContext context)
	{
		if (context.performed){
            Debug.Log("...");
		}
        else if (context.canceled)
        {
        }

	}

	public void Interact(InputAction.CallbackContext context){

		// If the player should confirm...
		if (context.performed){
			confirmPressed = true;
			Debug.Log("Confirm.");
		}
        else if (context.canceled)
        {
            confirmPressed = false;
        } 
	}

    public void Sit(InputAction.CallbackContext context){

		// If the player should cancel...
		if (context.performed){
			cancelPressed = true;
			Debug.Log("Cancel.");
		}
		else if (context.canceled)
        {
            cancelPressed = false;
        } 
	}

    public bool GetConfirmPressed() 
    {
        bool result = confirmPressed;
        confirmPressed = false;
        return result;
    }

    public bool GetCancelPressed() 
    {
        bool result = cancelPressed;
        cancelPressed = false;
        return result;
    }
	//Gustavo

    public void StartGame01(){
        DisableMenuButtons();
        Debug.Log("The game should start from Save File 01.");
        SceneManager.LoadSceneAsync("FloriculturaScene");
    }

    public void StartGame02(){
        /*
        DisableMenuButtons();
        Debug.Log("The game should start from Save File 02.");
        SceneManager.LoadSceneAsync("FloriculturaScene");
        */
    }

    public void StartGame03(){
        /*
        DisableMenuButtons();
        Debug.Log("The game should start from Save File 03.");
        SceneManager.LoadSceneAsync("FloriculturaScene");
        */
    }

    private void QuitGame(){
        Debug.Log("The application should be closed.");
        Application.Quit();
    }

    private void DisplayOptions(GameObject button){
        StartCoroutine(SelectFirstOption(button));
    }

    private IEnumerator SelectFirstOption(GameObject button){
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void StartButton01(){
        StartCoroutine(FadeToLevel(1));
    }

    public void StartButton02(){
        //StartCoroutine(FadeToLevel(2));
    }

    public void StartButton03(){
        //StartCoroutine(FadeToLevel(3));
    }

    public void QuitButton(){
        QuitGame();
    }

    public IEnumerator FadeToLevel(int saveFile){

        fadingPanel.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);

        switch(saveFile){
            case 1:
                StartGame01();
                break;
            case 2:
                StartGame02();
                break;
            case 3:
                StartGame03();
                break;
            default:
                Debug.LogWarning("I don't know which save file I have to start.");
                break;
        }
        

    }

    public void SettingsButton(){
        ShowSettings();
    }

    public void ShowSettings(){
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        DisplayOptions(volumeButton);
    }

    public void BackFromSettingsButton(){
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        DisplayOptions(settingsButton);
        
    }

    public void SelectSaveButton(){
        ShowSaveFiles();
    }

    public void ShowSaveFiles(){
        mainMenuPanel.SetActive(false);
        selectSavePanel.SetActive(true);
        DisplayOptions(gameButton01GameObj);
    }

    public void BackFromSelectSaveButton(){
        selectSavePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        DisplayOptions(startButton);
        
    }

    public void SetVolume(float volume){

        audioMixer.SetFloat("Volume", volume);

    }

    //ainda nao to usando
    public void OnNewGamePressed(){
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadSceneAsync("FloriculturaScene");
    }

    private void DisableMenuButtons(){

    gameButton01.interactable = false;
    gameButton02.interactable = false;
    gameButton03.interactable = false;
    goBackFromSelectSaveButton.interactable = false;

    }

}
