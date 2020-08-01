using UnityEngine;

/// <summary>
/// Replicates object count times.
/// Used to make more copies of objects for scene generation 
/// without the need to manage copies by hand in the editor.
/// In order for this script to work with ScenesGenerator
/// it needs to be placed higher in Scripts Execution Order in Project Settings.
/// </summary>
public class ReplicateOnStart : MonoBehaviour
{
    public int count=5;

    // This can't be done in Awake because calling 
    // Instantiate in Awake crashes the editor.
    private void Start()
    {
        // Here the script is disabled so that the copies don't replicate anymore
        // because that would lead to infinite duplication and editor crash.
        // (Disabling the script won't stop the running function and the scripts state is copied during instantiation).
        enabled = false;
        for (int i = 0; i < count; i++)
        {
            Instantiate(this, transform.parent);
        }
    }
}
