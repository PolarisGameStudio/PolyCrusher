﻿using UnityEngine;
using System.Collections;

public class PieBehaviour : MonoBehaviour
{
    // Player prefix
    protected string playerPrefix = null;

    [Header("Pie settings")]
    [SerializeField]
    [Tooltip("Damage of the explosion")]
    protected int explosionDamage = 100;

    [SerializeField]
    [Tooltip("Radius of the damage")]
    protected float damageRadius = 6f;

    [SerializeField]
    [Tooltip("The time after the explosion can be triggered.")]
    protected float waitForTriggerTime = 0.6f;

    [Header("Explosion particle")]
    [SerializeField]
    protected GameObject explosionParticle;

    [Header("Explosion sound")]
    [SerializeField]
    protected AudioClip explosionSound;


    protected bool explosionAllowed = false;

    // The owner of the projectile.
    protected MonoBehaviour ownerScript;

    /// <summary>
    /// Gets or sets the player prefix.
    /// </summary>
    public string PlayerPrefix
    {
        get { return this.playerPrefix; }
        set { this.playerPrefix = value; }
    }

    /// <summary>
    /// Gets or sets the owner script.
    /// </summary>
    public MonoBehaviour OwnerScript
    {
        get { return this.ownerScript; }
        set { this.ownerScript = value; }
    }

    void Start()
    {
        StartCoroutine(WaitForAllowExplosion());
    }

    void Update()
    {
        // Handle the user input
        HandleInput();
    }

    /// <summary>
    /// Handles the player input for the explosion trigger.
    /// </summary>
    protected void HandleInput()
    {
        if (playerPrefix != null)
        {
            // Explode if the player presses the ability button.
            if (explosionAllowed && Input.GetButtonDown(playerPrefix + "Ability"))
            {
                PerformExplosionProcedure();
            }
        }
    }

    /// <summary>
    /// Performs the explosion and deals the damage.
    /// </summary>
    protected void PerformExplosionProcedure()
    {
        Transform[] enemies = GetAllEnemiesInRange(damageRadius);

        // Loop through all enemies.
        foreach (Transform enemy in enemies)
        {
            if (enemy.GetComponent<MonoBehaviour>() is BaseEnemy)
            {
                BaseEnemy e = (enemy.GetComponent<MonoBehaviour>() as BaseEnemy);

                // Deal damage to the enemy
                e.TakeDamage(explosionDamage, this.OwnerScript);
            }
        }

        // Explosion
        if (explosionParticle != null)
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);

        // Sound
        if (explosionSound != null)
            SoundManager.SoundManagerInstance.Play(explosionSound, transform.position);

        // Camera shake
        CameraManager.CameraReference.ShakeOnce();

        // Destroy pie.
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Returns all enemy transforms in the given range.
    /// </summary>
    /// <param name="range">Range</param>
    /// <returns></returns>
    protected Transform[] GetAllEnemiesInRange(float range)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, 1 << 9); // Only Enemies
        Transform[] enemies = new Transform[hits.Length];

        for (int i = 0; i < hits.Length; i++)
            enemies[i] = hits[i].transform;

        return enemies;
    }

    /// <summary>
    /// Waits for the trigger time.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator WaitForAllowExplosion()
    {
        yield return new WaitForSeconds(waitForTriggerTime);
        explosionAllowed = true;
    }
}