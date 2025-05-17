using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public event System.Action<int> OnDamageTaken;
    public GameObject damageNumberPrefab;
    public RectTransform headPos;


    void Start() => currentHealth = maxHealth;

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnDamageTaken?.Invoke(damage);

        // Éú³ÉÉËº¦Êý×Ö
        var number = Instantiate(damageNumberPrefab,
            headPos.position + Vector3.up * 30f,
            Quaternion.identity);
        number.transform.SetParent(headPos.transform, true);

        number.GetComponent<DamageNumberController>()
            .ShowDamage(damage);
    }
    public void UpdateMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = newMaxHealth;
    }

}
