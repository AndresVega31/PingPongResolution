using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MenuManager 
{
    public static bool IsInitialised { get; private set; }
    public static GameObject mainMenu, searchMenu, privateMenu, multiplayerMenu, loadingScreen;
    public static void Init()
    {
        GameObject list = GameObject.Find("list");
        mainMenu = list.transform.Find("mainMenu").gameObject;
        searchMenu = list.transform.Find("searchMenu").gameObject;
        privateMenu = list.transform.Find("privateMenu").gameObject;
        multiplayerMenu = list.transform.Find("multiplayerMenu").gameObject;
        loadingScreen = list.transform.Find("loadingScreen").gameObject;
    }


    public static void OpenMenu(Menu menu, GameObject callingMenu)
    {
        if (!IsInitialised)
            Init();

        switch (menu)
        {
            case Menu.MAIN_MENU:
                mainMenu.SetActive(true);
                break;
            case Menu.SEARCH_GAME:
                searchMenu.SetActive(true);
                break;
            case Menu.PRIVATE_GAME:
                privateMenu.SetActive(true);
                break;
            case Menu.MULTIPLAYER:
                multiplayerMenu.SetActive(true);
                break;
            case Menu.LOADING:
                loadingScreen.SetActive(true);
                break;
        }
        callingMenu.SetActive(false);

    }

}
