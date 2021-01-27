using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private DamageText damageTextPrefab;

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private Image healthBar;
    private float startHealth = 0f;
    public static UIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }
    public void OnMainMenuButtonPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitGameButtonPressed()
    {
        Application.Quit();
    }
    public void OnPause()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);

        if (pausePanel.active)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

    }

    public void ShowDamageText(string text, Vector2 location, Color color)
    {
        DamageText damageTextInstance = Instantiate(damageTextPrefab, location, Quaternion.identity);
        damageTextInstance.text.text = text;
        damageTextInstance.text.color = color;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (startHealth == 0f)
            startHealth = maxHealth;
        StartCoroutine(SmoothBar(currentHealth, maxHealth));
    }

    private IEnumerator SmoothBar(float currentHealth, float maxHealth)
    {
        Vector2 a = new Vector2(currentHealth, maxHealth);

        while (startHealth > currentHealth)
        {
            startHealth -= (startHealth - currentHealth) * 0.025f;
            healthBar.fillAmount = startHealth / maxHealth;
            if (startHealth <= currentHealth)
            {
                startHealth = currentHealth;
                break;
            }
            yield return null;
        }
    }


}