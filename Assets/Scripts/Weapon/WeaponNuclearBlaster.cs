﻿using UnityEngine;
using System.Collections;

public class WeaponNuclearBlaster : Weapon {

    // The number of rays for the auto aim check.
    [Header("Auto-Aim")]
    [Tooltip("The number of rays for the auto aim check.")]
    [SerializeField]
    protected int numberCheckRays = 3;

    // The angle for the raycast sweep of the auto-aim.
    [SerializeField]
    protected float checkAngle = 10.0f;

    [SerializeField]
    protected bool debugMode = false;

    [SerializeField]
    protected int numberOfBullets = 3;


    private float completeAngle = 0.0f;

    /// <summary>
    /// The Shoot mechanic of the railgun.
    /// </summary>
    public override void Use()
    {
        if (shootIsAllowed)
        {
            base.Use();

            completeAngle = numberOfBullets * checkAngle;

            for(int i = 0; i < numberOfBullets; i++)
            {
                GameObject g = Instantiate(bulletPrefab) as GameObject;
                g.transform.position = transform.position;
                g.name = "RayBullet";
                PerlinRayProjectile ray;

                if (g != null && g.GetComponent<MonoBehaviour>() is PerlinRayProjectile)
                {
                    ray = g.GetComponent<MonoBehaviour>() as PerlinRayProjectile;
                    ray.OwnerScript = this.OwnerScript;
                    ray.Damage = this.WeaponDamage;

                    //Shoot
                    ray.Shoot(CalculateAim(ray, i));
                }
            }
            bulletRounds.Emit(1);

            shootIsAllowed = false;
            StartCoroutine(WaitForNextShot());
        }
    }

    /// <summary>
    /// Calculates the nearest player in range. The nearest player will be searched in a cone-like shape which is
    /// facing in the forward direction of the transform.
    /// </summary>
    /// <param name="ray">Ray projectile</param>
    /// <returns>The direction vector of the shot.</returns>
    protected virtual Vector3 CalculateAim(PerlinRayProjectile ray, int rayNumber)
    {
        // The smallast distance of the raycasts
        float minDistance = -1f;

        // The angle between 2 raycasts.
        float angleBetween = checkAngle / numberCheckRays;

        // The current angle.
        float currentAngle = -(completeAngle / 2f) + (numberCheckRays*angleBetween*rayNumber);

        // The nearest position.
        Vector3 nearestPosition = transform.forward;

        // Specifies if there was no hit at all.
        bool neverHit = true;

        for (int i = 0; i < numberCheckRays; i++)
        {
            Quaternion rotation = Quaternion.Euler(new Vector3(0f, currentAngle, 0f));
            Vector3 direction = rotation * transform.forward;

            RaycastHit hit;

            // Raycast 16: BossShield
            bool enemyHit = Physics.Raycast(transform.position, direction, out hit, ray.MaxLength, 1 << ray.TargetLayer | 1 << 16);

            //Debug
            if (debugMode)
            {
                if (enemyHit)
                    Debug.DrawLine(transform.position, hit.transform.position, Color.red, 2);
                else
                    Debug.DrawLine(transform.position, transform.position + direction * ray.MaxLength, Color.red, 2);
            }

            if (enemyHit)
            {
                neverHit = false;
                float dist = (transform.position - hit.transform.position).sqrMagnitude;

                if (minDistance == -1)
                {
                    minDistance = dist;
                    nearestPosition = hit.transform.position;
                }

                if (minDistance < dist)
                {
                    minDistance = dist;
                    nearestPosition = hit.transform.position;
                }
            }

            currentAngle += angleBetween;
        }

        if (neverHit)
        {
            return Quaternion.Euler(0, -angleBetween+(rayNumber*angleBetween), 0) * transform.forward;
        }
        else
            return (nearestPosition - transform.position).normalized;
    }
}
