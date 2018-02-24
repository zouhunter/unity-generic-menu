using System;
using System.Collections;
using UnityEngine;
/// <summary>
///   <para>GenericMenu_Runtime lets you create custom context menus and dropdown menus.</para>
/// </summary>
public sealed class GenericMenu_Runtime
{
    private static GUIStyle style;
    /// <summary>
    ///   <para>Callback function, called when a menu item is selected.</para>
    /// </summary>
    public delegate void MenuFunction();

    public class DisplayCustomMenu
    {
        public int[] array;
        public int[] array2;
        public SelectMenuItemFunction callback;
        public string[] options;
        public Rect position;
        public int[] selected;
        public bool showHotkey;
        public object userData;

        public DisplayCustomMenu(Rect position, string[] options, int[] array, int[] array2, int[] selected, SelectMenuItemFunction callback, object userData, bool showHotkey)
        {
            this.position = position;
            this.options = options;
            this.array = array;
            this.array2 = array2;
            this.selected = selected;
            this.callback = callback;
            this.userData = userData;
            this.showHotkey = showHotkey;
        }
    }
    /// <summary>
    ///   <para>Callback function with user data, called when a menu item is selected.</para>
    /// </summary>
    /// <param name="userData">The data to pass through to the callback function.</param>
    public delegate void MenuFunction2(object userData);

    private sealed class MenuItem
    {
        public GUIContent content;

        public bool separator;

        public bool on;

        public GenericMenu_Runtime.MenuFunction func;

        public GenericMenu_Runtime.MenuFunction2 func2;

        public object userData;

        public MenuItem(GUIContent _content, bool _separator, bool _on, GenericMenu_Runtime.MenuFunction _func)
        {
            this.content = _content;
            this.separator = _separator;
            this.on = _on;
            this.func = _func;
        }

        public MenuItem(GUIContent _content, bool _separator, bool _on, GenericMenu_Runtime.MenuFunction2 _func, object _userData)
        {
            this.content = _content;
            this.separator = _separator;
            this.on = _on;
            this.func2 = _func;
            this.userData = _userData;
        }
    }

    private static DisplayCustomMenu customMenu;
    static GenericMenu_Runtime()
    {
        style = new GUIStyle();
        style.border = new RectOffset(10, 10, 10, 10);
        style.fontStyle = FontStyle.Normal;
        style.fontSize = 20;
        style.fixedHeight = 30;
    }
    internal void OnGUI()
    {
        if (customMenu != null && customMenu.options != null)
        {
            var position = customMenu.position;
            var yPos = position.y;
            
            for (int i = 0; i < customMenu.options.Length; i++)
            {
                var rect = GUILayoutUtility.GetRect(new GUIContent(customMenu.options[i]), style);
                rect.x = position.x;
                rect.y = yPos;
                yPos += rect.height;
                GUI.DrawTexture(rect, new Texture2D((int)rect.x, (int)rect.y));
                GUI.Label(rect, customMenu.options[i]);
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
                {
                    customMenu.callback.Invoke(customMenu.userData, customMenu.options, i);
                    return;
                }
            }
        }
    }

    private ArrayList menuItems = new ArrayList();

    public void AddItem(GUIContent content, bool on, GenericMenu_Runtime.MenuFunction func)
    {
        this.menuItems.Add(new GenericMenu_Runtime.MenuItem(content, false, on, func));
    }

    public void AddItem(GUIContent content, bool on, GenericMenu_Runtime.MenuFunction2 func, object userData)
    {
        this.menuItems.Add(new GenericMenu_Runtime.MenuItem(content, false, on, func, userData));
    }

    /// <summary>
    ///   <para>Add a disabled item to the menu.</para>
    /// </summary>
    /// <param name="content">The GUIContent to display as a disabled menu item.</param>
    public void AddDisabledItem(GUIContent content)
    {
        this.menuItems.Add(new GenericMenu_Runtime.MenuItem(content, false, false, null));
    }

    /// <summary>
    ///   <para>Add a seperator item to the menu.</para>
    /// </summary>
    /// <param name="path">The path to the submenu, if adding a separator to a submenu. When adding a separator to the top level of a menu, use an empty string as the path.</param>
    public void AddSeparator(string path)
    {
        this.menuItems.Add(new GenericMenu_Runtime.MenuItem(new GUIContent(path), true, false, null));
    }

    /// <summary>
    ///   <para>Get number of items in the menu.</para>
    /// </summary>
    /// <returns>
    ///   <para>The number of items in the menu.</para>
    /// </returns>
    public int GetItemCount()
    {
        return this.menuItems.Count;
    }

    /// <summary>
    ///   <para>Show the menu under the mouse when right-clicked.</para>
    /// </summary>
    public void ShowAsContext()
    {
        if (Event.current != null)
        {
            this.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }
    }

    /// <summary>
    ///   <para>Show the menu at the given screen rect.</para>
    /// </summary>
    /// <param name="position">The position at which to show the menu.</param>
    public void DropDown(Rect position)
    {
        string[] array = new string[this.menuItems.Count];
        bool[] array2 = new bool[this.menuItems.Count];
        ArrayList arrayList = new ArrayList();
        bool[] array3 = new bool[this.menuItems.Count];
        for (int i = 0; i < this.menuItems.Count; i++)
        {
            GenericMenu_Runtime.MenuItem menuItem = (GenericMenu_Runtime.MenuItem)this.menuItems[i];
            array[i] = menuItem.content.text;
            array2[i] = (menuItem.func != null || menuItem.func2 != null);
            array3[i] = menuItem.separator;
            if (menuItem.on)
            {
                arrayList.Add(i);
            }
        }
        DisplayCustomMenuWithSeparators(position, array, array2, array3, (int[])arrayList.ToArray(typeof(int)),
            new SelectMenuItemFunction(this.CatchMenu), null, true);
    }

    internal void Popup(Rect position, int selectedIndex)
    {
        this.DropDown(position);
    }

    private void CatchMenu(object userData, string[] options, int selected)
    {
        GenericMenu_Runtime.MenuItem menuItem = (GenericMenu_Runtime.MenuItem)this.menuItems[selected];
        if (menuItem.func2 != null)
        {
            menuItem.func2(menuItem.userData);
        }
        else if (menuItem.func != null)
        {
            menuItem.func();
        }

        customMenu = null;
    }

    public delegate void SelectMenuItemFunction(object userData, string[] options, int selected);


    internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] enabled, bool[] separator, int[] selected, SelectMenuItemFunction callback, object userData, bool showHotkey)
    {
        Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
        position.x = vector.x;
        position.y = vector.y;
        int[] array = new int[options.Length];
        int[] array2 = new int[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            array[i] = ((!enabled[i]) ? 0 : 1);
            array2[i] = ((!separator[i]) ? 0 : 1);
        }

        Debug.Log(position);
        Debug.Log(options);
        customMenu = new DisplayCustomMenu(position, options, array, array2, selected, callback, userData, showHotkey);
        Event.current.Use();
    }
}
