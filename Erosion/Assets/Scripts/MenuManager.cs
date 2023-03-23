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
    public TMP_InputField heightField, scaleField, offsetField, saveField, iterationField;
    public Slider removalSlider, maxSedimentSlider;
    public TMP_Text heightText, scaleText, offsetText, invalidTerrainText, invalidIterationsText, fileExists, progress, fileError, noFileError, iterationText, removalText, maxSedimentText;
    public TerrainGeneration tg;
    public Erosion er;
    public CameraController cc;
    private int currentMenu = 0;

    private void Update()//Set number of iterations of erosion algorithm completed each frame
    {
        progress.text = String.Join("/", er.numIterationsRun, er.values[0]);
    }

    private void Start()//Run at start of program - refresh dropdown for files and add listeners to menu buttons
    {
        refreshFiles();
        quitButton.onClick.AddListener(Quit);
        randomButton.onClick.AddListener(GenRandom);
        returnButton.onClick.AddListener(ChangeMenu);
        loadButton.onClick.AddListener(LoadTerrain);
        regenerateButton.onClick.AddListener(Regenerate);
        saveButton.onClick.AddListener(SaveTerrain);
        startButton.onClick.AddListener(StartErosion);
    }

    private void StartErosion()//Starts erosion with user specified values
    {
        er.tg = tg;
        try
        {
            if (Convert.ToInt32(iterationField.text) > 0)
            {
                progress.gameObject.SetActive(true);
                invalidIterationsText.gameObject.SetActive(false);
                er.values = new float[] { float.Parse(iterationField.text), removalSlider.value, maxSedimentSlider.value };
                StartCoroutine(er.StartErosion());
            }
            else
            {
                progress.gameObject.SetActive(false);
                invalidIterationsText.gameObject.SetActive(true);
            }
        }
        catch
        {
            progress.gameObject.SetActive(false);
            invalidIterationsText.gameObject.SetActive(true);
        }
    }

    private void refreshFiles()//Finds saved terrain files and adds to dropdown menu
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.*", SearchOption.TopDirectoryOnly);
        var result = files.Select(a => Path.GetFileName(a));
        dropdown.ClearOptions();
        dropdown.AddOptions(result.ToListPooled());
    }
    private void SaveTerrain()//Save terrain that was just generated to file
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

    private void LoadTerrain()//Generate terrain based upon values in selected file
    {
        if (dropdown.options.Count > 0)
        {
            noFileError.gameObject.SetActive(false);
            float[] values = new float[3];
            string path = Application.persistentDataPath + "/" + dropdown.options[dropdown.value].text;
            StreamReader st = new StreamReader(path);
            string line1 = st.ReadLine();
            string line2 = st.ReadLine();
            string line3 = st.ReadLine();

            if (float.TryParse(line1, out _) && float.TryParse(line2, out _) && float.TryParse(line3, out _))//Validate that file is of correct format
            {
                fileError.gameObject.SetActive(false);
                values[0] = float.Parse(line1);
                values[1] = float.Parse(line2);
                values[2] = float.Parse(line3);
                tg.values = values;
                tg.GenerateTerrain();
                ChangeMenu();
            }
            else
            {
                fileError.gameObject.SetActive(true);
            }
            st.Close();
        }
        else
        {
            noFileError.gameObject.SetActive(true);
        }
    }
    private void Regenerate()//Regenerates terrain with new user specified values
    {
        try
        {
            invalidTerrainText.gameObject.SetActive(false);
            tg.values = new float[] { float.Parse(heightField.text), float.Parse(scaleField.text), float.Parse(offsetField.text) };
        }
        catch
        {
            invalidTerrainText.gameObject.SetActive(true);
        }
        
        tg.GenerateTerrain();
        UpdateTerrainValues();
    }

    private void GenRandom()//Choose random values then generate terrain with them
    {
        float[] values = new float[]
        {
            Random.Range(40f, 60f),
            Random.Range(30f, 60f),
            Random.Range(0, 100000)
        };
        tg.values = values;
        tg.GenerateTerrain();
        ChangeMenu();
    }

    private void ChangeMenu()//Change between two menus
    {
        cc.enabled = !cc.isActiveAndEnabled;
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
        UpdateTerrainValues();
        refreshFiles();
    }

    private void UpdateTerrainValues()//Updates displayed values for terrain parameters
    {
        float[] values = tg.values;
        heightText.text = Convert.ToString(values[0]);
        scaleText.text = Convert.ToString(values[1]);
        offsetText.text = Convert.ToString(values[2]);
        heightField.text = null;
        scaleField.text = null;
        offsetField.text = null;
    }

    public void UpdateErosionValues()//Updates displayed values for erosion parameters
    {
        iterationText.text = iterationField.text;
        removalText.text = removalSlider.value.ToString();
        maxSedimentText.text = maxSedimentSlider.value.ToString();
    } 
    private void Quit()
    {
        Application.Quit();
    }
}
