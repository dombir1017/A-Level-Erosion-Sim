using System;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MenuManager : MonoBehaviour
{
    public Button quitButton, randomButton, loadButton, returnButton, regenerateButton, saveButton, startButton;
    public Dropdown dropdown;
    public TMP_InputField heightField, scaleField, offsetField, saveField;
    public TMP_Text heightText, scaleText, offsetText, errorText, fileExists, progress;
    public TerrainGeneration tg;
    public Erosion er;
    private int currentMenu = 0;

    private void Start()
    {
        refreshFiles();
        quitButton.onClick.AddListener(Quit);
        randomButton.onClick.AddListener(GenRandom);
        returnButton.onClick.AddListener(changeMenu);
        loadButton.onClick.AddListener(LoadTerrain);
        regenerateButton.onClick.AddListener(Regenerate);
        saveButton.onClick.AddListener(SaveTerrain);
        startButton.onClick.AddListener(StartErosion);
    }

    private void StartErosion()
    {
        progress.text = String.Join("/", "0", er.dropletAttempts);
        progress.gameObject.SetActive(true);
        StartCoroutine(er.StartErosion());
    }

    private void refreshFiles()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.*", SearchOption.TopDirectoryOnly);
        var result = files.Select(a => Path.GetFileName(a));
        dropdown.ClearOptions();
        dropdown.AddOptions(result.ToListPooled());
    }
    private void SaveTerrain()
    {
        string path = Application.persistentDataPath + "/" + saveField.text + ".txt";
        if (File.Exists(path))
        {
            fileExists.gameObject.SetActive(true);
        }
        else
        {
            fileExists.gameObject.SetActive(false);
            File.Create(path).Dispose();
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine(heightText.text);
            sw.WriteLine(scaleText.text);
            sw.WriteLine(offsetText.text);
            sw.Close();
        }
    }

    private void LoadTerrain()
    {
        float[] values = new float[3];
        string path = Application.persistentDataPath +"/"+ dropdown.options[dropdown.value].text; 
        StreamReader st = new StreamReader(path);
        values[0] = float.Parse(st.ReadLine());
        values[1] = float.Parse(st.ReadLine());
        values[2] = float.Parse(st.ReadLine());
        st.Close();
        tg.values = values;
        tg.GenerateTerrain();
        changeMenu();
    }
    private void Regenerate()
    {
        try
        {
            errorText.gameObject.SetActive(false);
            tg.values = new float[] { float.Parse(heightField.text), float.Parse(scaleField.text), float.Parse(offsetField.text) };
        }
        catch
        {
            errorText.gameObject.SetActive(true);
        }
        
        tg.GenerateTerrain();
        changeValues();
    }

    private void GenRandom()
    {
        float[] values = new float[]
        {
            Random.Range(60f, 100f),
            Random.Range(60f, 110f),
            Random.Range(0, 100000)
        };
        tg.values = values;
        tg.GenerateTerrain();
        changeMenu();
    }

    private void changeMenu()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform menuElement = this.gameObject.transform.GetChild(i);
            if (menuElement.tag == Convert.ToString(currentMenu))
            {
                menuElement.gameObject.SetActive(false);
            }
            else
            {
                menuElement.gameObject.SetActive(true);
            }
        }
        currentMenu = (currentMenu + 1) % 2;
        changeValues();
        refreshFiles();
    }

    private void changeValues()
    {
        float[] values = tg.values;
        heightText.text = Convert.ToString(values[0]);
        scaleText.text = Convert.ToString(values[1]);
        offsetText.text = Convert.ToString(values[2]);
        heightField.text = null;
        scaleField.text = null;
        offsetField.text = null;
    }
    private void Quit()
    {
        Application.Quit();
    }
}
