using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using System;

public class Menu : MonoBehaviour {

    GenericMenu_Runtime menuRuntime;
    private void Awake()
    {
        menuRuntime = new GenericMenu_Runtime();
        menuRuntime.AddItem(new GUIContent("TestItemA"), true, CallBack);
        menuRuntime.AddItem(new GUIContent("TestItemB"), false, CallBack);
        menuRuntime.AddItem(new GUIContent("TestItemC"), false, CallBack);
        menuRuntime.AddItem(new GUIContent("TestItemD"), false, CallBack);
    }

    private void CallBack()
    {
        Debug.Log("Clicked");
    }

    private void OnGUI()
    {
        if(Event.current != null && Event.current.type == EventType.mouseUp && Event.current.button == 1)
        {
            Debug.Log("RightClicked");
            menuRuntime.ShowAsContext();
        }
        menuRuntime.OnGUI();
    }
}
