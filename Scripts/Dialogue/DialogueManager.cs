using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
//using Ink.UnityIntegration;

public class DialogueManager : MonoBehaviour, IDataPersistence
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    // variable for the load_globals.ink JSON
    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying {get; private set;}

    private Coroutine displayLineCoroutine;

    private bool canContinueToNextLine = false;
    private static DialogueManager instance;

    private DialogueVariables dialogueVariables;

    private void Awake(){
        if(instance != null){
            Debug.LogWarning("Found more than one Dialogue Manager in the scene.");
        }
        instance = this;

        //pass that variable to the DialogueVariables constructor in the Awake method
        //dialogueVariables = new DialogueVariables(loadGlobalsJSON);
        dialogueVariables = new DialogueVariables(loadGlobalsJSON);

    }

    public static DialogueManager GetInstance(){
        return instance;
    }

    private void Start(){
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        //get all of the choices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices){
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update(){
        //return right away if dialogue is playing
        if(!dialogueIsPlaying){
            return;
        }
        //handle player input to continue dialogue
        if(canContinueToNextLine && CharacterController2D.GetInstance().GetInteractPressed()){
            //continue story
            ContinueStory();
        }

    }

    public void EnterDialogueMode(TextAsset inkJSON){
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode(){
        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory(){

        CharacterController2D.GetInstance().StopMoving();
        if(currentStory.canContinue){
            
            if(displayLineCoroutine != null){

                StopCoroutine(displayLineCoroutine);

            }
            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));

        }else{
            StartCoroutine(ExitDialogueMode());
        }
    }

    
    private IEnumerator DisplayLine(string line){

        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        //loop to display each letter at a time
        //ainda tem o problema de imprimir numa linha e depois ir pra outra conforme a palavra aumenta...
        foreach(char letter in line.ToCharArray()){
            //não ta fazendo o typing sound effect direito ...
            AudioManager.GetInstance().Play("Typing Sound 01");
            //if the player has pressed the submit button
            if(CharacterController2D.GetInstance().GetInteractPressed()){

                dialogueText.maxVisibleCharacters = line.Length;
                break;

            }

            if(letter == '<' || isAddingRichTextTag){

                isAddingRichTextTag = true;

                if(letter == '>'){
                    isAddingRichTextTag = false;
                }

            }else{
                
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);

            }

        }

        continueIcon.SetActive(true);

        //display choices, if any
        DisplayChoices();

        canContinueToNextLine = true;

    }

    private void HideChoices(){

        foreach(GameObject choiceButton in choices){

            choiceButton.SetActive(false);

        }

    }

    private void DisplayChoices(){
        List<Choice> currentChoices = currentStory.currentChoices;

        if(currentChoices.Count > choices.Length){
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " 
                + currentChoices.Count);
        }

        int index = 0;
        //inicializa as choices
        foreach(Choice choice in currentChoices){
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for(int i = index; i < choices.Length; i++){
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice(){
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex){

        if(canContinueToNextLine){

            currentStory.ChooseChoiceIndex(choiceIndex);

        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName){

        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if(variableValue == null){
            Debug.Log("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }

    public void SaveData(GameData data){
        Debug.Log("It is saving the json ink variables.");
        if(dialogueVariables.globalVariablesStory != null){

            Debug.Log(dialogueVariables.globalVariablesStory.state.ToJson());

            dialogueVariables.VariablesToStory(dialogueVariables.globalVariablesStory);
            data.INKVariables = dialogueVariables.globalVariablesStory.state.ToJson();
        }
    }

    public void LoadData(GameData data){
        if(data.INKVariables != null){

            string jsonState = data.INKVariables;
            dialogueVariables.globalVariablesStory.state.LoadJson(jsonState);
            

        }

    }

}
