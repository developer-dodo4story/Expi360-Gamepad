using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Stores the data of the ScriptableObject class Skin
/// </summary>
[System.Serializable]
public class SkinData
{
    /// <summary>
    /// Initialize with a default constructor
    /// </summary>
    public SkinData()
    {
        game = EnhancedDodoServer.Consts.Game.BellyRumble;
        layoutOption = GamePad.ButtonsLayoutOptions.BellyRumble_ButtonsOnly;
        themeColor = Color.white;
        textColor = Color.black;
    }
    /// <summary>
    /// A game where this skin should be used
    /// </summary>
    public EnhancedDodoServer.Consts.Game game;
    /// <summary>
    /// ButtonLayout default for this skin
    /// </summary>
    public GamePad.ButtonsLayoutOptions layoutOption;    
    /// <summary>
    /// Theme color of the skin (buttons,images etc)
    /// </summary>
    public Color themeColor;
    /// <summary>
    /// Text color of the skin (font color)
    /// </summary>
    public Color textColor;    
    public bool applayThemeColorAndText = true;
}
