using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class UI_Popup : UI_Base
{
    public override void Init()
    {
        UIManager.instance.SetCanvas(gameObject, true, canvasLayer);
    }

    public virtual void ClosePopUI()
    {
        UIManager.instance.ClosePopupUI(this);
    }

    protected void SetPivot(Vector3 pos, RectTransform pivot)
    {
        pivot.position += pos;
    }
}
