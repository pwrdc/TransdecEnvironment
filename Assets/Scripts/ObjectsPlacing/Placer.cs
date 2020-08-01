using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Places objects in the placing area so that they don't collide with each other.
/// Its Place_ methods can also be provided with additional restriction that returns true 
/// if object placement is incorrect.
/// </summary>
public class Placer : MonoBehaviour
{
    public PlacingArea placingArea;
    public List<Placeable> placed=new List<Placeable>();

    public int maxTries = 3;

    bool OverlapsWithAnother(Placeable placeable)
    {
        foreach (var otherPlaceable in placed)
        {
            if (placeable!=otherPlaceable && Placeable.Overlaps(placeable, otherPlaceable))
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

    void FinalizePlacing(Placeable toPlace)
    {
        placed.Add(toPlace);
        toPlace.placer = this;
        toPlace.OnPlaced?.Invoke();
    }

    public bool Place(Placeable toPlace, System.Func<Placeable, bool> restriction=null)
    {
        placed.Remove(toPlace);
        toPlace.RotateRandomly();
        return Utils.Try(maxTries, () =>
        {
            placingArea.Place(toPlace);
            bool restrictionSatisifed = restriction == null || !restriction(toPlace);
            if (restrictionSatisifed && !OverlapsWithAnother(toPlace))
            {
                FinalizePlacing(toPlace);
                return true;
            }
            else return false;
        });
    }

    public bool PlaceNear(Placeable toPlace, Placeable other, FloatRange range, System.Func<Placeable, bool> restriction = null)
    {
        placed.Remove(toPlace);
        toPlace.RotateRandomly();
        return Utils.Try(maxTries, () =>
        {
            toPlace.transform.position = other.transform.position + Random.onUnitSphere*range.GetRandom();
            if ((restriction == null || !restriction(toPlace)) && !OverlapsWithAnother(toPlace) && placingArea.Contains(toPlace))
            {
                FinalizePlacing(toPlace);
                return true;
            }
            else return false;
        });
    }

    public void PlaceAll(Placeable[] placeables, System.Func<Placeable, bool> restriction=null)
    {
        foreach(var placeable in placeables)
        {
            placeable.gameObject.SetActive(false);
            if(Place(placeable, restriction))
            {
                placeable.gameObject.SetActive(true);
            }
        }
    }

    public void Clear()
    {
        placed.Clear();
    }
}
