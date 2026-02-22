using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class AudioGenerator : MonoBehaviour
{
    // Start is called before the first frame update

    //public const string voice = "Prabhat";
    public const string voice = "Dilip";
    //public const string voice = "Neerja";
    public string[] dataArray;
    private const string RESULTFOLDER = "GeneratedAudio"; // Folder within the Assets directory
                                                                  // private const string FILE_PREFIX = "result";
    private const string FILE_EXTENSION = ".mp3";
    private const string API_KEY = "RwXSTR9TdJ8FoSmP15fpx6OOIgBDUGZ91bBDsP92"; // Replace with your actual API key
                                                                               // Hirakud_Actuator_AnimData[] animData;

    [ContextMenu("GenerateAudio")]

    public void DownloadAudio()
    {
        StartCoroutine(audioDownload());
    }
    public IEnumerator audioDownload()
    {

        if (string.IsNullOrEmpty(API_KEY))
        {
            Debug.LogError("API key is missing!");
            yield break;
        }
        DirectoryInfo generateAudioDir = new DirectoryInfo(Path.Combine("Assets", RESULTFOLDER));
        //print(generateAudioDir.GetFiles().Length/2);

        int i = generateAudioDir.GetFiles().Length / 2;
        foreach (var data in dataArray)
        {
            Debug.Log($"Iterating {data}");
            string fileName = $"Audio_{data}.mp3";
            yield return GenerateAudio(data, fileName);
            i++;
        }

        //  Hirakud_Actuator_GoogleSheetsManager googlemanager = GameObject.Find("AR_object").GetComponent<Hirakud_Actuator_GoogleSheetsManager>();
        ///string FILE_PPE = "PPE";
        // int i = 0;
        //foreach (var item in googlemanager.sheetData.values[0])
        //{
        //    string at = item;
        //    //  print(item);
        //    //animData[0].baseAnimDatas[i].subtitle = at;
        //    string fileName = $"{FILE_PPE}{i + 1}{FILE_EXTENSION}";
        //    yield return GenerateAudio(at, fileName);
        //    i++;
        //}


        //string FILE_Process = "Process";
        //int k = 0;
        //foreach (var item in googlemanager.sheetData.values[1])
        //{
        //    string at = item;
        //    // print(item);
        //    //animData[0].baseAnimDatas[i].subtitle = at;
        //    string fileName = $"{FILE_Process}{k + 1}{FILE_EXTENSION}";
        //    yield return GenerateAudio(at, fileName);
        //    k++;
        //}
    }
    private IEnumerator GenerateAudio(string data, string fileName)
    {
        string resultPath = Path.Combine("Assets", RESULTFOLDER, fileName);
        string url = $"https://api.narakeet.com/text-to-speech/mp3?voice={voice}";
        byte[] postData = Encoding.UTF8.GetBytes(data);

        using (WWW www = new WWW(url, postData, GetHeaders()))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                // Create the directory if it doesn't exist
                Directory.CreateDirectory(Path.Combine("Assets", RESULTFOLDER));
                File.WriteAllBytes(resultPath, www.bytes);
                Debug.Log("Conversion successful. Audio saved to " + resultPath);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }

        // Refresh the Unity Editor to recognize the new file
        //UnityEditor.AssetDatabase.Refresh();
    }

    private Dictionary<string, string> GetHeaders()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Accept"] = "application/octet-stream";
        headers["x-api-key"] = API_KEY;

        return headers;
    }

    //public void loadAudio()
    //{
    //    StartCoroutine(audioDownload());
    //}

    //private void Start()
    //{
    //    Invoke("loadAudio", 6);
    //}
}
