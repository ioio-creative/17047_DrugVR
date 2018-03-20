using UnityEngine.EventSystems;

// for 3D physical objects to interact 
// with raycast from controllers
public interface IHandlePhysicalObject:    
    IPointerEnterHandler,
    IPointerDownHandler,
    IPointerExitHandler,
    IPointerUpHandler,
    IDragHandler,
    IDropHandler
{

}
