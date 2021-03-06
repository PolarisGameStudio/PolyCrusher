﻿using InControl;
using UnityEngine;

public class PlayerHealthVisualizer : MonoBehaviour
{
    private BasePlayer player;
    private Material playerMaterial;

    [SerializeField]
    private float tweenTime = 0.2f;

    private void OnEnable()
    {
        // To ensure safety, reset the effect amount when the player is enabled.
        if (playerMaterial != null)
            playerMaterial.SetFloat("_EffectAmount", 0f);
    }

	private void Start ()
    {
        player = transform.root.GetComponent<BasePlayer>();
        playerMaterial = transform.GetComponentInChildren<Renderer>().material;

        player.DamageTaken += DoDamageTween;
        RumbleManager.Instance.DamageRumble += DoRumbleTween;
    }

    private void DoDamageTween(int damageDealed)
    {
        if (!player.IsDead)
            TweenMaterialProperties(tweenTime);
    }

    private void DoRumbleTween(float tweenTime, InputDevice device)
    {
        if (device == player.InputDevice && !player.IsDead)
            TweenMaterialProperties(tweenTime);
    }

    private void TweenMaterialProperties(float tweenTime)
    {
        float halfTweenTime = tweenTime * 0.5f;

        LeanTween.value(gameObject, 0f, 1f, halfTweenTime)
                .setOnUpdate((float val) =>
                {
                    playerMaterial.SetFloat("_EffectAmount", val);
                })
                .setOnComplete(() =>
                {
                    LeanTween.value(gameObject, 1f, 0f, halfTweenTime)
                        .setOnUpdate((float val) =>
                        {
                            playerMaterial.SetFloat("_EffectAmount", val);
                        });
                });
    }
}