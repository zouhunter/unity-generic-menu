using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
    public RadialMenu radiaMenu;

    private void Start()
    {
        radiaMenu.Reset();
        radiaMenu.AddSlider("slider", 0, 100, 30, OnSelect);
        radiaMenu.AddList("list", new string[] { "A", "B" }, OnSelect);
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            radiaMenu.SetPosition(Input.mousePosition);
            radiaMenu.ActivateMenu();
        }
    }
    private void OnSelect(object data)
    {
        Debug.Log(data);
    }
}
