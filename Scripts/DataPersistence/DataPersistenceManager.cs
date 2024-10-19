using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;

    //aquii ta o nome do arquivo de save caso eu queira mudar algo
    //pra possibilitar os 3 save files
    [Header("File Storage Config")]
    [SerializeField] private string fileName;


    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        //Debug.Log("OnSceneLoaded Called");

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

    }

    public void OnSceneUnloaded(Scene scene){
        //Debug.Log("OnSceneUnloaded Called");

        //desse jeito ele salva sempre que carrega uma cena diferente
        //terei de mudar quando tiver os outros cômodos da floricultura
        SaveGame();
        
    }

    private void Awake(){

        if(instance != null){
            //Debug.LogError("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    public void NewGame(){
        this.gameData = new GameData();
    }

    public void LoadGame(){


        //load any save data from a file using the data handler
        this.gameData = dataHandler.Load();

        if(this.gameData == null && initializeDataIfNull){
            NewGame();
        }

        //if there's no data to load, do not continue
        if(this.gameData == null){
            //Debug.Log("No data was found. A New Game needs to be started before loading data.");
            return;
        }

        //push the loaded data to all other scripts that need it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects){
            dataPersistenceObj.LoadData(gameData);
        }

        //Debug.Log("Loaded panquecas: " + gameData.panquecasAmount);
    }

    public void SaveGame(){

        if(this.gameData == null){
            //Debug.LogWarning("No data was found. A New Game needs to be started first.");
            return;
        }

        //pass the data to other scripts so they can update it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects){
            dataPersistenceObj.SaveData(gameData);
        }

        Debug.Log("Saved panquecas: " + gameData.panquecasAmount);

        //save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects(){
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    //nao to usando ainda tambemm
    public bool HasGameData(){
        return gameData != null;
    }
}
