using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// function taking placeable and returning true if its state is incorrect
using Restriction = System.Func<Placeable, bool>;

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

    bool RestrictionSatisified(Restriction restriction, Placeable toPlace)
    {
        return restriction == null || !restriction(toPlace);
    }

    public bool Place(Placeable toPlace, Restriction restriction = null, int? maxTries = null)
    {
        placed.Remove(toPlace);
        toPlace.RotateRandomly();
        return Utils.Try(maxTries.GetValueOrDefault(this.maxTries), () =>
        {
            placingArea.Place(toPlace);
            if (RestrictionSatisified(restriction, toPlace) && !OverlapsWithAnother(toPlace))
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
    /// <returns>True on success.</returns>
    public bool PlaceNear(Placeable toPlace, Placeable other, FloatRange range, Restriction restriction = null, int? maxTries = null)
    {
        placed.Remove(toPlace);
        toPlace.RotateRandomly();
        return Utils.Try(maxTries.GetValueOrDefault(this.maxTries), () =>
        {
            Vector3 onCircle = Random.insideUnitCircle;
            onCircle.z = onCircle.y;
            onCircle.Normalize();
            toPlace.transform.position = other.transform.position + onCircle * range.GetRandom();
            if (placingArea.TryPlacingVertically(toPlace) && RestrictionSatisified(restriction, toPlace) && !OverlapsWithAnother(toPlace))
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
    public void PlaceAll(Placeable[] placeables, Restriction restriction=null, int? maxTries=null)
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
