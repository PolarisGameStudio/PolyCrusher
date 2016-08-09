﻿using UnityEngine;
using System.Collections;

public enum TransitionEnum
{
    TextColorChange = 1,
    SizeChange = 2,
    ImageColorChange = 3,
    LevelTransition = 4,
    NoOp = 5
}

public interface TransitionHandlerInterface
{
    void OnFocus(GameObject gameobject);
    void OnDefocus(GameObject gameobject);
}