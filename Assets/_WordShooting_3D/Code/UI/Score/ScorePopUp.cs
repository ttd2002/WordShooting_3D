using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePopUp : TextBase
{
    protected override void Update()
    {
        transform.position += new Vector3(0, 2f) * Time.deltaTime;
    }
}
