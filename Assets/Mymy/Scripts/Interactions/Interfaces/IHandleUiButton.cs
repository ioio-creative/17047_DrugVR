using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for UI controls to interact 
// with raycast from controllers
public interface IHandleUiButton
{
    void HandleEnter();
    void HandleDown();
    void HandleExit();
    void HandleUp();
}
