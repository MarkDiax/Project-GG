using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "PlayerControllerConfig", menuName = "Config/PlayerController")]
public class PlayerControllerConfig : ScriptableObject
{
    #region Serialized
    [Range(0, 1)]
    public float airControl;
    public float gravityMod;
    #endregion
}
