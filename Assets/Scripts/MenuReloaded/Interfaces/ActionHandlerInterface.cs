﻿using UnityEngine;
using System.Collections;

public interface ActionHandlerInterface
{
    void PerformAction<T>(T triggerInstance);
}