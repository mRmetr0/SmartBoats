using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Logger : MonoBehaviour
{
    public static Logger instance;
    private string path = "Assets/Resources/";
    private StreamReader reader;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            Debug.Log("Logger activated");
        }
        else
        {
            Destroy(gameObject);
        }
        
        var info = new DirectoryInfo(path);

        string id = "Log "+(info.GetFiles().Length/2);
        path += id + ".txt";
        
        File.WriteAllText(path, "Gen: \nHerbi: \nCarni: \nOmni: \nCount: \nHerbiCount: \nCardiCount: \nOmniCount: ");
    }

    public void SaveData(int gencount, float herbiPoints, float carniPoints, float omniPoints, float herbiAmount = 0, float carniAmount = 0, float omniAmount = 0)
    {
        reader = new StreamReader(path);
        reader.ReadLine();
        //Read info about points:
        string herbi = reader.ReadLine();
        string carni = reader.ReadLine();
        string omni = reader.ReadLine();
        herbi = AddTo(herbi, CheckString(herbiPoints.ToString()));
        carni = AddTo(carni, CheckString(carniPoints.ToString()));
        omni = AddTo(omni, CheckString(omniPoints.ToString()));
        
        reader.ReadLine();
        //Read info about population density:
        string herbiCount = reader.ReadLine();
        string carniCount = reader.ReadLine();
        string omniCount = reader.ReadLine();
        herbiCount = AddTo(herbiCount, CheckString(herbiAmount.ToString()));
        carniCount = AddTo(carniCount, CheckString(carniAmount.ToString()));
        omniCount = AddTo(omniCount, CheckString(omniAmount.ToString()));
        reader.Close();
        Debug.Log("Gen: " + gencount + "\n"+ herbi + carni + omni);
        try
        {
            File.WriteAllText(path, "Gen: "+gencount + "\n"+ herbi + carni + omni +"Count: \n"+ herbiCount + carniCount + omniCount);
        }
        catch(Exception e)
        {
            Debug.Log("Could not read, Error:\n"+e);
        }
    }

    private string AddTo(string core, string add)
    {
        if (!Char.IsNumber((core[core.Length-1])))
        {
            core += add+"\n";
        }
        else
        {
            core += "; " + add+"\n";
        }
        return core;
    }

    private string CheckString(string core)
    {
        string newcore = "";
        for (int i = 0; i < core.Length; i++)
        {
            if (core[i] == '.')
            {
                newcore += ",";
            }
            else
            {
                newcore += core[i];
            }
        }
        return newcore;
    }
}
