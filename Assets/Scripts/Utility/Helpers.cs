using System;
using UnityEngine;

public static class Helpers
{

    public static GameObject GetChildByName(GameObject parent, string name)
    {
        // Loop through all child transforms of the parent
        foreach (Transform child in parent.transform)
        {
            // Check if the name matches
            if (child.gameObject.name == name)
            {
                return child.gameObject;
            }
        }

        // Return null if no child with the given name was found
        return null;
    }
}

