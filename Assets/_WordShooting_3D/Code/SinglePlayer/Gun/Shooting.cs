using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shooting : GunAbstract
{
    private int currentCharIndex = 0;
    private int coloredCharCount = 0;
    private string currentTarget = "";
    private TextMeshProUGUI targetTextComponent;

    public virtual void CheckKeyInput(Transform targetTextTransform)
    {
        if (Time.timeScale > 0f && targetTextTransform != null)
        {
            if (this.targetTextComponent == null)
            {
                this.targetTextComponent = targetTextTransform.GetComponentInChildren<TextMeshProUGUI>();
                if (this.targetTextComponent != null)
                {
                    this.currentTarget = this.targetTextComponent.text.ToLower();
                    this.currentCharIndex = 0;
                    this.coloredCharCount = GetColoredCharacterCount(this.targetTextComponent, Color.green);
                    if (this.coloredCharCount >= 1)
                    {
                        this.currentCharIndex += this.coloredCharCount + 21;
                    }
                }
            }

            if (Input.anyKeyDown && !string.IsNullOrEmpty(Input.inputString))
            {
                char typedChar = Input.inputString.ToLower()[0];
                //Debug.Log("currentCharIndex:" + currentCharIndex);
                //Debug.Log("typedChar: " + typedChar + "; currentTarget: " + this.currentTarget[this.currentCharIndex]);
                if (this.currentCharIndex < this.currentTarget.Length && typedChar == this.currentTarget[this.currentCharIndex])
                {
                    if (this.currentCharIndex == this.currentTarget.Length - 1)
                    {
                        this.currentCharIndex++;
                        this.HighlightTypedText(this.targetTextComponent);
                        this.FinishText(targetTextTransform);
                    }
                    if (this.currentCharIndex < this.currentTarget.Length)
                    {
                        this.currentCharIndex++;
                        this.ShootingAtTarget(targetTextTransform);
                        this.HighlightTypedText(this.targetTextComponent);
                    }

                }
            }
        }
    }
    private int GetColoredCharacterCount(TextMeshProUGUI textComponent, Color32 colorToCheck)
    {
        int count = 0;
        TMP_TextInfo textInfo = textComponent.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (charInfo.isVisible)
            {
                Color32[] colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
                if (colors[charInfo.vertexIndex].Equals(colorToCheck))
                {
                    count++;
                }
            }
        }

        return count;
    }
    protected virtual void HighlightTypedText(TextMeshProUGUI targetTextComponent)
    {
        targetTextComponent.text = "<color=green>" + this.currentTarget.Substring(0, this.currentCharIndex) + "</color>" + this.currentTarget.Substring(this.currentCharIndex);
    }
    public virtual void ResetTarget()
    {
        this.currentCharIndex = 0;
        this.coloredCharCount = 0;
        this.targetTextComponent = null;
        this.currentTarget = "";
    }
    private void ShootingAtTarget(Transform targetTextComponent)
    {
        this.SpawnBullet(BulletSpawner.bullet, targetTextComponent);
        this.SpawnMuzzle();
    }
    public void FinishText(Transform targetTextComponent)
    {
        this.SpawnMuzzle();
        this.SpawnBullet(BulletSpawner.bulletFinish, targetTextComponent);
        this.gunCtrl.LookAtTarget.SwitchToNextTarget();
    }
    private void SpawnMuzzle()
    {
        Transform vfx_Muzzle = VFXSpawner.Instance.Spawn(VFXSpawner.muzzle, this.gunCtrl.FirePoint.position, this.gunCtrl.FirePoint.rotation);
        vfx_Muzzle.gameObject.SetActive(true);
    }
    private void SpawnBullet(string namPrefab, Transform targetTextComponent)
    {
        Transform bullet = BulletSpawner.Instance.Spawn(namPrefab, this.gunCtrl.FirePoint.position, this.gunCtrl.FirePoint.rotation);
        BulletFly bulletFly = bullet.GetComponentInChildren<BulletFly>();
        bulletFly.SetNewTarget(targetTextComponent);
        bullet.gameObject.SetActive(true);
    }
}
