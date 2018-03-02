using UnityEngine.EventSystems;

// for 3D physical objects to interact 
// with raycast from controllers
public interface IHandlePointer :    
    IPointerEnterHandler,
    IPointerDownHandler,
    IPointerExitHandler,
    IPointerUpHandler
{

}
