﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The selector is used to handle the menu element selection.
/// It defines the the behaviour of 'scrolling' through the menu items.
/// </summary>
public abstract class AbstractSelector : SelectorInterface
{
    protected int currentSelectionIndex;

    // A map of all menu item components which can be scrolled through.
    protected readonly Dictionary<int, GameObject> components;

    protected readonly TransitionHandlerInterface[] transitionHandler;
    protected readonly ElementPressedHandler[] elementPressedHandler;

    public Dictionary<int, GameObject> Components
    { get { return this.components; } }

    public AbstractSelector(int startIndex,
        Dictionary<int, GameObject> components,
        TransitionHandlerInterface[] transitions,
        ElementPressedHandler[] pressedHandler,
        bool initialFocus)
    {
        this.currentSelectionIndex = startIndex;
        this.components = components;
        this.transitionHandler = transitions;
        this.elementPressedHandler = pressedHandler;

        if(initialFocus)
            SetInitialFocus();
    }

    public int Current
    {
        get { return this.currentSelectionIndex; }
        protected set { this.currentSelectionIndex = value; }
    }

    /// <summary>
    /// Checks if the index is valid.
    /// </summary>
    protected bool CheckIndex(int key)
    {
        return components.ContainsKey(key);
    }

    protected GameObject GetElementyByKey(int key)
    {
        GameObject gameobject = null;
        try
        {
            if (CheckIndex(key))
                components.TryGetValue(key, out gameobject);

            return gameobject;
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogException(e);
            throw e;
        }
    }

    protected virtual void SetInitialFocus()
    {
        GameObject current = GetElementyByKey(Current);

        if (current != null)
            InvokeTransitionFocus(current);
    }

    public void Next()
    {
        if (components.Count > 0)
        {
            BeforeSelection(GetElementyByKey(Current));
            OnNext();
            AfterSelection(GetElementyByKey(Current));
        }
    }

    public virtual void HandleSelectedElement()
    {
        foreach (ElementPressedHandler pressed in elementPressedHandler)
        {
            GameObject pressedGameObject = GetElementyByKey(Current);
            pressed.ElementPressed(pressedGameObject);
        }
    }

    protected virtual void BeforeSelection(GameObject currentElement)
    {
        InvokeTransitionDeFocus(currentElement);
    }

    protected virtual void AfterSelection(GameObject currentElement)
    {
        InvokeTransitionFocus(currentElement);
    }

    internal virtual void InvokeTransitionFocus(GameObject currentElement)
    {
        foreach (TransitionHandlerInterface transition in transitionHandler)
            transition.OnFocus(currentElement);
    }

    internal virtual void InvokeTransitionDeFocus(GameObject currentElement)
    {
        foreach (TransitionHandlerInterface transition in transitionHandler)
            transition.OnDefocus(currentElement);
    }

    public void Previous()
    {
        if (components.Count > 0)
        {
            BeforeSelection(GetElementyByKey(Current));
            OnPrevious();
            AfterSelection(GetElementyByKey(Current));
        }
    }

    #region Abstract Methods
    protected abstract void OnNext();
    protected abstract void OnPrevious();
    #endregion
}