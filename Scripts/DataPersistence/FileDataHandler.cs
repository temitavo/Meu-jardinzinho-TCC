using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName){
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load(){
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;

        if(File.Exists(fullPath)){

            try{
                //load the serialized data from file
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)){
                    
                    using(StreamReader reader = new StreamReader(stream)){

                        dataToLoad = reader.ReadToEnd();

                    }

                }

                //now to deserialize the dataaa
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            }
            catch(Exception e){
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }

        }
        return loadedData;
    }

    public void Save(GameData data){
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try{
            //create directory path just in case it does not exist yet
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //then we'll serialize the C# game data into a JSON
            string dataToStore = JsonUtility.ToJson(data, true);

            //write the serialized data to the file
            using(FileStream stream = new FileStream(fullPath, FileMode.Create)){
                
                using(StreamWriter writer = new StreamWriter(stream)){

                    writer.Write(dataToStore);

                }

            }
            Debug.Log("Saved at: " + fullPath);
        }
        catch(Exception e){
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}
