﻿using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Default selector which scrolls to the next index by incrementing or decrementing by one.
/// </summary>
public class Selector : AbstractSelector
{
    protected int minValue;
    protected int maxValue;

    public Selector(int startIndex, Dictionary<int, GameObject> components, TransitionHandlerInterface[] transitionHandlers, ElementPressedHandler[] pressedHandler, bool initialFocus) 
        : base(startIndex, components, transitionHandlers, pressedHandler, initialFocus)
    {
        FindMinAndMaxKey();
    }

    private void FindMinAndMaxKey()
    {       
        foreach(var pair in components)
        {
            maxValue = Math.Max(maxValue, pair.Key);
            minValue = Math.Min(minValue, pair.Key);
        }
    }

    protected override void OnNext()
    {
        if (!CheckIndex(++Current))
            Current = minValue;
    }

    protected override void OnPrevious()
    {
        if (!CheckIndex(--Current))
            Current = maxValue;
    }
}