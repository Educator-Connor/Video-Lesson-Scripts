using UnityEngine;
using UnityEngine.UI;

public class HealthHeartsUI : MonoBehaviour
{
    public GameObject[] hearts; // Each heart is a GameObject (not just Image)

    public void SetHealth(int currentHealth)
    {
        Debug.Log($"SetHealth called with value: {currentHealth}");
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentHealth);
        }
    }

}
