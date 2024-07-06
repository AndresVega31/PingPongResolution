using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    private bool PlayPressed = false;
    public bool Loaded = false;

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

    public void onTouch_playGame()
    {
        MenuManager.OpenMenu(Menu.LOADING, gameObject);
        PlayPressed = true;

    }

    public void Update()
    {
        if (PlayPressed && Loaded)
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
}
