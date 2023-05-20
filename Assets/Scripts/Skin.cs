using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePad;
/// <summary>
/// Class which makes it possible to create new skins for games
/// </summary>
[CreateAssetMenu(fileName = "New Skin", menuName = "Skin")]
public class Skin : ScriptableObject
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Skin() { skinData = new SkinData(); }
    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="skin"></param>
    public Skin(Skin skin)
    {
        skinData = skin.skinData;       

    }
    /// <summary>
    /// Variable holding skin-specific data
    /// </summary>
    public SkinData skinData;
}
