using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public PlacingArea placingArea;
    public Transform folder;
    List<Placeable> placed=new List<Placeable>();

    public int maxTries = 3;

    bool placeOnStart = true;

    private void Start()
    {
        if (placeOnStart)
        {
            PlaceAll();
        }
    }

    public bool Place(Placeable placeable)
    {
        bool success = false;
        int tries = 0;
        while (!success && tries< maxTries)
        {
            placeable.transform.position=placingArea.RandomPosition(placeable);
            success = true;
            foreach (var otherPlaceable in placed)
            {
                if(Placeable.Overlaps(placeable, otherPlaceable))
                {
                    success = false;
                    break;
                }
            }
            tries++;
        }
        if (success)
        {
            placed.Add(placeable);
        }
        return success;
    }

    public void PlaceAll()
    {
        Placeable[] folderPlaceables=folder.GetComponentsInChildren<Placeable>();
        foreach(var placeable in folderPlaceables)
        {
            int count = placeable.count.GetRandom();
            for (int i = 0; i < count; i++)
            {
                Placeable placeableInstance = Instantiate(placeable);
                if (!Place(placeableInstance))
                {
                    Destroy(placeableInstance);
                }
            }
        }
    }
}
