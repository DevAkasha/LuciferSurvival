using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ironcow.UI;

public class CooldownButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button button;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private TextMeshProUGUI cooldownText;

    [Header("Settings")]
    [SerializeField] private float cooldownTime;

    private float currentCooldown = 5f;
    private bool isCooldown = false;
    private Action onClick;

    public void Initialize((float cooldown, Action onClick) tufle)
    {
        cooldownTime = tufle.cooldown;
        onClick = tufle.onClick;
        button.onClick.AddListener(HandleClick);
    }


    private void HandleClick()
    {
        if (isCooldown) return;
        onClick?.Invoke();
        StartCooldown();
    }

    private void StartCooldown()
    {
        currentCooldown = cooldownTime;
        isCooldown = true;
        button.interactable = false;
    }

    private void Update()
    {
        if (!isCooldown) return;

        currentCooldown -= Time.deltaTime;
        float ratio = Mathf.Clamp01(currentCooldown / cooldownTime);
        cooldownImage.fillAmount = ratio;
        cooldownText.text = Mathf.CeilToInt(currentCooldown).ToString();

        if (currentCooldown <= 0f)
        {
            isCooldown = false;
            button.interactable = true;
            cooldownImage.fillAmount = 0f;
            cooldownText.text = "";
        }
    }
}