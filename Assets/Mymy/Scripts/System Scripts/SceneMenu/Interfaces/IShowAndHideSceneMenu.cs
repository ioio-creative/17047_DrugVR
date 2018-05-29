using System;

public interface IShowAndHideSceneMenu
{
    event Action OnShowMenu;
    event Action OnHideMenu;

    void ShowSceneMenu();
    void HideSceneMenu();
}
