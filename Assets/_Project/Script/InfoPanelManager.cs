using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    [SerializeField] private MainInfoPanel _infoPanelBrute;
    [SerializeField] private MainInfoPanel _infoPanelArya;
    [SerializeField] private MainInfoPanel _infoPanelYanling;

    [SerializeField] private MainInfoPanel _classPanelBrute;
    [SerializeField] private MainInfoPanel _classPanelArya;
    [SerializeField] private MainInfoPanel _classPanelYanling;

    public static InfoPanelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public MainInfoPanel GetInfoPanel(CharacterInfo info)
    {
        if(info.Name == "Brute")
        {
            return _infoPanelBrute;
        }
        else if(info.Name == "Yanling")
        {
            return _infoPanelYanling;
        }
        else if(info.Name == "Arya")
        {
            return _infoPanelArya;
        }
        else
        {
            Debug.LogError("Failed to get info panel for " + info.Name);
            Debug.Break();
            return null;
        }
    }

    public MainInfoPanel GetClassInfoPanel(CharacterInfo info)
    {
        if (info.Name == "Brute")
        {
            return _classPanelBrute;
        }
        else if (info.Name == "Yanling")
        {
            return _classPanelYanling;
        }
        else if (info.Name == "Arya")
        {
            return _classPanelArya;
        }
        else
        {
            Debug.LogError("Failed to get class info panel for " + info.Name);
            Debug.Break();
            return null;
        }
    }
}
