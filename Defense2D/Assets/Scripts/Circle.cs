using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Circle : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public GameManager gameManager;

    public int health = 1;

    public float size;

    public int index;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            gameManager.DestroyCircle(index);
        }
        else
        {
            UpdateSize();
            UpdateHealthText();
        }

    }

    void UpdateSize()
    {
        size = Mathf.Clamp(gameManager.circleMinimumSize + gameManager.circleSizeTimesHealth * health, gameManager.circleMinimumSize, gameManager.circleMaximumSize);
        gameObject.transform.localScale = new Vector2(size, size);
    }

    void UpdateHealthText()
    {
        healthText.text = health.ToString();
    }

    public void SetHealth(int health)
    {
        this.health = health;
        UpdateSize();
    }

    public bool OnPlayerInnerCircle()
    {
        Vector3 directionToCenter = -transform.position.normalized;
        if (Vector3.Distance(Vector3.zero,transform.position) <= Vector3.Distance(Vector3.zero,Vector3.zero + transform.position.normalized * gameManager.playerInnerCircleRadius))
        {
            return true;
        }

        return false;
    }
}
