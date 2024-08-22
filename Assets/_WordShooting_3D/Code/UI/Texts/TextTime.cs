using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTime : TextBase
{
    protected override void Update()
    {
        this.UpdateTime();
    }
    protected virtual void UpdateTime()
    {
        this.text.SetText(TimeManager.Instance.GameDuration);
    }
}
