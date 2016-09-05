﻿using UnityEngine;
using System.Collections;

public class PowerUpMango : PowerUp
{
    protected float range = 5f;
    protected GameObject explosion;

    public override void Use()
    {
        Instantiate(explosion, transform.position, transform.rotation);

        Collider[] hits = Physics.OverlapSphere(transform.position, range, 1 << 9);
        for(int i = 0; i < hits.Length; i++)
        {
            BaseEnemy enemy = hits[i].GetComponent<BaseEnemy>();

            if (enemy)
                enemy.TakeDamage(enemy.Health, this, transform.position);
        }

        Destroy(this);
        Destroy(transform.parent);
    }

    public void SetExplosionProperties(GameObject explosion, float range)
    {
        this.explosion = explosion;
        this.range = range;
    }
}