﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Ability class for the chicken.
/// </summary>
public class AbilityChicken : Ability 
{
    // The prefab path to the chicken ability prefab.
    [SerializeField]
    protected string prefabString = "Abilities/ChickenAbility";

    protected override void Start()
    {
        base.Start();
    }

	public override void Use() 
    {
		if (useIsAllowed)
        {
            if (GameObject.FindGameObjectWithTag("Pie") == null)
            {
                base.Use();

                GameObject obj = Instantiate(Resources.Load<GameObject>(prefabString));

                obj.GetComponent<PieBehaviour>().OwnerScript = this.OwnerScript;
                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;

                useIsAllowed = false;

                StartCoroutine(WaitForNextAbility());
            }
            else
            {
                BasePlayer player = OwnerScript.GetComponent<BasePlayer>();

                if (player != null)
                    player.Energy += this.EnergyCost;
            }
		}
	}
}
