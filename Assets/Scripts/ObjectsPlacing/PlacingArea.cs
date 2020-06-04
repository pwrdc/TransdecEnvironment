using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingArea : MonoBehaviour
{
    public enum Type
    {
        Cube, CastedCircle
    }
    public Type type;

    public Vector3 RandomPosition(Placeable placeable)
    {
        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Matrix4x4 saved = Gizmos.matrix;
        Gizmos.color = Color.yellow;
        switch (type) {
            case Type.Cube:
                Gizmos.DrawWireCube(transform.position, transform.lossyScale);
                break;
            case Type.CastedCircle:
                {
                    Gizmos.matrix *= Matrix4x4.Translate(transform.position);
                    Vector3 scale = transform.lossyScale;
                    scale.y = 0;
                    Gizmos.matrix *= Matrix4x4.Scale(scale);
                    Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
                    Gizmos.matrix = saved;
                    break;
                }
            default:
                throw new InvalidEnumValueException(type);
        }
    }
}
