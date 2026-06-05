using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool isSelected = false;
    public GameObject selectionVisual;
    public float currentHealth;
    public float maxHealth;
    public Color teamColor;
    public float attackDamage = 10f;
    public float attackSpeed = 1.0f;
    public float lastAttackTime;
    public bool isPlayerUnit = true;

    void Update()
    {
        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " has been destroyed!");
            Destroy(gameObject);
        }
    }

    // Simple Combat: If we stay touching an enemy, deal damage over time
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
    }

    public void SetSelected(bool value)
    {
        isSelected = value;

        if (selectionVisual != null)
        {
            selectionVisual.SetActive(value);
        }
    }
}