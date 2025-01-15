using Fusion;
using TMPro;
using UnityEngine;

public class TextNetworkObject : NetworkBehaviour
{
    [Networked]
    private string textValue { get; set; }

    [Networked]
    private bool isReticleActive { get; set; } 
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private GameObject reticle; 

    public void Initialize(string text)
    {
        textValue = text;
        UpdateText();
    }

    public void UpdateText()
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = textValue;
        }
    }

    public void SetActiveReticle(bool isActive)
    {
        isReticleActive = isActive; 
        UpdateReticleVisibility();
    }

    public override void Spawned()
    {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        reticle = transform.Find("Canvas/Reticle").gameObject;
        UpdateText();
        UpdateReticleVisibility(); 
    }

    private void UpdateReticleVisibility()
    {
        if (reticle != null)
        {
            reticle.SetActive(isReticleActive);
        }
    }

    public void OnReticleActiveChanged()
    {
        UpdateReticleVisibility();
    }
}
