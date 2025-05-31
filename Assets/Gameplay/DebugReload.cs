using Gameplay.Misc;
using UnityEngine;

public class DebugReload : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            SceneLoader.ReloadScene();
    }
}
