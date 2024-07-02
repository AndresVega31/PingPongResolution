using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Menu : MonoBehaviour
{
    public void onTouch_searchGame()
    {
        MenuManager.OpenMenu(Menu.SEARCH_GAME, gameObject);
    }

    public void onTouch_privateGame()
    {
        MenuManager.OpenMenu(Menu.PRIVATE_GAME, gameObject);
    }

    public void onTouch_multiplayerGame()
    {
        MenuManager.OpenMenu(Menu.MULTIPLAYER, gameObject);
    }
}
