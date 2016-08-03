﻿using UnityEngine;
using System.Collections;

public class NavigationInformation : MonoBehaviour {

    [Header("Selection Information")]
    [SerializeField]
    private int selectionID;

    [Header("Button state colors")]
    [SerializeField]
    private Color normalColor = Color.white;

    [SerializeField]
    private Color highlightedColor = Color.red;

    public Color NormalColor
    {
        get { return this.normalColor; }
    }

    public Color HighlightedColor
    {
        get { return this.highlightedColor; }
    }

    /// <summary>
    /// The ID (or index) of the element.
    /// </summary>
    public int SelectionID
    {
        get { return selectionID; }
        set { this.selectionID = value; }
    }
}
