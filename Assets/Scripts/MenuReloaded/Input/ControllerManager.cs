﻿using UnityEngine;
using System.Collections.Generic;
using InControl;
using System;

public class ControllerManager : MonoBehaviour {

    private List<SmartphoneController> smartphoneControllerList;
    private SmartphoneController smartphoneController;
    private KeyboardController keyboardController;

    private NetControllInterface networkController;

   
    void OnEnable()
    {
        networkController = FindObjectOfType<PlayerNetCommunicate>();
        smartphoneControllerList = new List<SmartphoneController>();        
    }

    void Start()
    {
        //-------------for testing purposes, needs to be deleted later on
        smartphoneController = new SmartphoneController(0, networkController);       
        InputManager.AttachDevice(smartphoneController);
        smartphoneControllerList.Add(smartphoneController);
        //-------------  

        keyboardController = new KeyboardController();
        InputManager.AttachDevice(keyboardController);
    }


    void OnDisable()
    {
        InputManager.DetachDevice(smartphoneController);
        InputManager.DetachDevice(keyboardController);
    }
   
    void OnSmartphoneJoin(int smartphoneID)
    {
        smartphoneController = new SmartphoneController(smartphoneID, networkController);
    }

    
}
