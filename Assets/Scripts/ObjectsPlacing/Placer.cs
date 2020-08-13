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

    public bool Place(Placeable toPlace, System.Func<Placeable, bool> restriction=null, int? maxTries = null)
    {
        placed.Remove(toPlace);
        toPlace.RotateRandomly();
        return Utils.Try(maxTries.GetValueOrDefault(this.maxTries), () =>
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

    /// <summary>
    /// Places one object near other
    /// </summary>
    /// <param name="toPlace">Object that needs to be placed.</param>
    /// <param name="other">Object in which proximity it needs to be placed</param>
    /// <param name="range">How close they can be</param>
    /// <param name="restriction">Additional restriction, if it returns true the placement is not acepted</param>
    /// <param name="maxTries">How many tries can be made, by default this.maxTries</param>
    /// <remarks>
    /// toPlace's horizontal and vertical placement will be ignored.
    /// The only thing checked is if it's inside of placing area and doesn't collide with anything.
    /// </remarks>
    /// <returns>True on success.</returns>
    public bool PlaceNear(Placeable toPlace, Placeable other, FloatRange range, System.Func<Placeable, bool> restriction = null, int? maxTries = null)
    {
        placed.Remove(toPlace);
        toPlace.RotateRandomly();
        return Utils.Try(maxTries.GetValueOrDefault(this.maxTries), () =>
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

    /// <summary>
    /// Places an array of placeables, disables those that couldn't be placed.
    /// </summary>
    /// <param name="placeables">Placeables that need to be placed</param>
    /// <param name="restriction">Additional restriction, if it returns true the placement is not acepted</param>
    /// <param name="maxTries">How many tries can be made, by default this.maxTries</param>
    public void PlaceAll(Placeable[] placeables, System.Func<Placeable, bool> restriction=null, int? maxTries=null)
    {
        foreach(var placeable in placeables)
        {
            placeable.gameObject.SetActive(false);
            if(Place(placeable, restriction, maxTries))
            {
                placeable.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// Stop remembering previously placed objects for new placements.
    /// </summary>
    public void Clear()
    {
        placed.Clear();
    }
}
