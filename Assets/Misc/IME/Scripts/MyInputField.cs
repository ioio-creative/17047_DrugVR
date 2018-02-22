using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityIME
{
public class MyInputField : InputField
{
    public bool Activated = false;

    new public void ActivateInputField()
    {
        Activated = true;
        base.ActivateInputField ();
    }

    protected override void LateUpdate ()
    {
        base.LateUpdate ();

        /**
         * In origin InputField class, it will call SelectAll() after OnFocus.
         * 
         * This means if we use InputField.ActivateInputField or InputField.Select or EventSystem ..whatever
         * to set focus on InputField, all text will be "highlight".
         * 
         * In order to show caret instead of highlight, we must call MoveTextEnd.
         * 
         * BTW, OnFocus()->SelectAll() occurs in 1 frame.
         * Before that "frame" completes, no other action can be taken on InputField.
         * Thus, we call MoveTextEnd in LateUpdate()
         **/
        if (Activated)
        {
            // only once, DO NOT MoveTextEnd in every frame.
            MoveTextEnd (true);
            Activated = false;
        }
    }
}
}