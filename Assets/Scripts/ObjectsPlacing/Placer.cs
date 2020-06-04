using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public PlacingArea placingArea;
    public Transform folder;
    Placeable folderPlaceables;
    Placeable placed;

    public Vector3 FindPlace(Placeable placeable)
    {
        return Vector3.zero;
    }

    public void Place(Placeable placeable)
    {

    }

    public void PlaceAll()
    {

    }
}
