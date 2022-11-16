using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button quitButton, randomButton, loadButton, returnButton;
    public Dropdown dropdown;
    public TerrainGeneration tg;
    private int currentMenu = 0;

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.*", SearchOption.TopDirectoryOnly);
        var result = files.Select(a => Path.GetFileName(a));
        dropdown.ClearOptions();
        dropdown.AddOptions(result.ToListPooled());

        quitButton.onClick.AddListener(Quit);
        randomButton.onClick.AddListener(GenRandom);
        returnButton.onClick.AddListener(changeMenu);
        loadButton.onClick.AddListener(LoadTerrain);
    }

    private void LoadTerrain()
    {
        float[] values = new float[2];
        string path = Application.persistentDataPath + dropdown.options[dropdown.value].text;
        if (File.Exists(path)){
            StreamReader st = new StreamReader(path);
            values[0] = float.Parse(st.ReadLine());
            values[1] = float.Parse(st.ReadLine());
            values[2] = float.Parse(st.ReadLine());
        }
        tg.values = values; ////Needs testing
    }
    private void GenRandom()
    {
        tg.randomiseValues();
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
    }
    private void Quit()
    {
        Application.Quit();
    }
}
