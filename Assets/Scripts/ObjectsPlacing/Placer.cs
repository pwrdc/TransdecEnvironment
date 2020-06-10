using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public PlacingArea placingArea;
    public Transform folder;
    List<Placeable> placed=new List<Placeable>();
    List<Placeable> instantiated = new List<Placeable>();

    public int maxTries = 3;

    bool placeOnStart = true;

    private void Start()
    {
        if (placeOnStart)
        {
            PlaceAll();
        }
    }

    bool OverlapsWithAnother(Placeable placeable)
    {
        foreach (var otherPlaceable in placed)
        {
            if (placeable !=otherPlaceable && Placeable.Overlaps(placeable, otherPlaceable))
            {
                return true;
            }
        }
        return false;
    }

    public bool Allowed(Placeable placeable)
    {
        return placingArea.Contains(placeable) && !OverlapsWithAnother(placeable);
    }

    public bool Place(Placeable toPlace, System.Func<Placeable, bool> restriction=null)
    {
        placed.Remove(toPlace);
        int tries = 0;
        while (tries< maxTries)
        {
            toPlace.transform.position=placingArea.RandomPosition(toPlace);
            bool restrictionSatisifed = restriction == null || !restriction(toPlace);
            if (restrictionSatisifed && !OverlapsWithAnother(toPlace))
            {
                placed.Add(toPlace);
                toPlace.placer = this;
                return true;
            }
            tries++;
        }
        return false;
    }

    public bool PlaceNear(Placeable toPlace, Placeable other, FloatRange range, System.Func<Placeable, bool> restriction = null)
    {
        int tries = 0;
        while (tries < maxTries)
        {
            toPlace.transform.position = other.transform.position + Random.onUnitSphere*range.GetRandom();
            if ((restriction == null || !restriction(toPlace)) && !OverlapsWithAnother(toPlace) && placingArea.Contains(toPlace))
            {
                placed.Add(toPlace);
                return true;
            }
            tries++;
        }
        return false;
    }

    public void PlaceAll(System.Func<Placeable, bool> restriction=null)
    {
        Placeable[] folderPlaceables=folder.GetComponentsInChildren<Placeable>();
        foreach(var placeable in folderPlaceables)
        {
            Place(placeable, restriction);
            /*int count = placeable.count.GetRandom();
            for (int i = 0; i < count; i++)
            {
                Placeable placeableInstance = Instantiate(placeable);
                if (!Place(placeableInstance, restriction))
                {
                    Destroy(placeableInstance);
                }
            }*/
        }
    }

    public void Clear()
    {
        placed.Clear();
        foreach(var placeable in instantiated)
        {
            // Destroy(placeable);
        }
    }
}
