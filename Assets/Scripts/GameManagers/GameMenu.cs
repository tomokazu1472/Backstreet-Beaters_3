using System;
using UnityEngine;

public enum MenuName
{
    Title,
    Gameplay,
    Gamepause
}

public class GameMenu : MonoBehaviour
{
    [SerializeField] MenuName _testMenu;
    [SerializeField] bool _debugMode;
    static private MenuName _currentMenu;
    static public MenuName currentMenu => _currentMenu;
    static Action _initAction;

    void Awake()
    {
        if (_debugMode)
        SetMenu(_testMenu);
    }

    public static void InitMenu()
    {
        _initAction?.Invoke();
    }

    public static void AddInitAction(Action action)
    {
        _initAction += action;
    }

    public static void ClearInitAction()
    {
        _initAction = null;
    }

    public static void SetMenu(MenuName menu)
    {
        _currentMenu = menu;
    }
}
