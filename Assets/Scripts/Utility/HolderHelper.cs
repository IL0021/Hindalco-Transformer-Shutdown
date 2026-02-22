using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HolderHelper : MonoBehaviour
{
    public SerializedDictionary<string, GameObject> holders = new SerializedDictionary<string, GameObject>();

    public List<TeleportInteraction> teleportInteractions = new List<TeleportInteraction>();

    [ContextMenu("GetAllTeleports")]
    public void GetAllTeleports()
    {
        var all = FindObjectsOfType<TeleportInteraction>().ToList();
        foreach (var interaction in all)
        {
            if (interaction.teleportType == TeleportType.Object) teleportInteractions.Add(interaction);
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
