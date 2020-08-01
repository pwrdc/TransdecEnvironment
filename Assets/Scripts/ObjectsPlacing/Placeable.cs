using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains settings used for placing the object on the scene 
/// including the space it occupies.
/// The space occupied by the object is displayed using editor gizmos.
/// </summary>
public class Placeable : MonoBehaviour
{
    // Creating two subclasses instead of this enum would make
    // changing the shape from the editor much harder.
    public enum Shape
    {
        InfiniteCyllinder,
        Sphere
    }
    public Shape shape = Shape.Sphere;
    public enum VerticalPlacement
    {
        UnderSurface,
        InTheMiddle,
        OnBottom
    }
    public VerticalPlacement verticalPlacement=VerticalPlacement.OnBottom;
    // increasing radius has same effect as increasing each scale component
    // so the main reason behind having radius as a separate variable 
    // is to make adjusting scale easier
    public float radius = 1;
    public Vector3 scale = Vector3.one;
    public Vector3 offset;
    [System.Serializable]
    public class Limit
    {
        public bool enabled;
        public float min;
        public float max;
    }
    [System.Serializable]
    public class RandomRotation
    {
        public bool x;
        public bool y;
        public bool z;
        public Limit xLimit;
        public Limit yLimit;
        public Limit zLimit;
    }
    public RandomRotation randomRotation;
    public bool canObscureView=true;

    [HideInInspector]
    public Placer placer;

    public UnityEvent OnPlaced;

    public bool debugMode;

    [HideInInspector]
    public Vector3 probingVector = Vector3.forward;

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position=value;
        }
    }

    public void AutoAdjust()
    {
        Bounds bounds=Utils.GetComplexBounds(gameObject);
        scale=transform.rotation*bounds.extents;
        // scale might become flipped because of rotation so here it is fixed
        scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        // make it so radius decides about the size
        // and scale only adjusts the object to fit the bounds exactly
        radius = scale.magnitude;
        scale = scale.normalized;
        offset = bounds.center-transform.position;
    }

    // checks if occupied spaces of two placeables overlap
    public static bool Overlaps(Placeable a, Placeable b)
    {
        Vector3 aPosition = a.transform.position;
        Vector3 bPosition = b.transform.position;
        if (a.shape == Shape.InfiniteCyllinder || b.shape == Shape.InfiniteCyllinder)
        {
            // if either of them is a infinite cylinder calculate distance between them on plane
            aPosition.y = bPosition.y = 0;
        }
        float radiuses = a.RadiusInDirection(aPosition - bPosition) + b.RadiusInDirection(aPosition - bPosition);
        // avoid square rooting
        return Vector3.SqrMagnitude(bPosition - aPosition) <= radiuses * radiuses;
    }

    public float RadiusInDirection(Vector3 direction)
    {
        return LeadingVector(direction).magnitude;
    }

    // returns a vector from (0,0,0) to point on shape surface 
    // used for debugging RadiusInDirection
    // Sphere equation taken from: https://en.wikipedia.org/wiki/Sphere
    private Vector3 LeadingVector(Vector3 direction)
    {
        if (shape == Shape.InfiniteCyllinder)
        {
            direction.y = 0;
        }
        direction = Quaternion.Inverse(transform.rotation) * direction;
        direction = Utils.DivideVectorsFields(direction, scale * radius);

        direction.Normalize();
        float alpha = Vector3.Angle(Vector3.forward, direction) * Mathf.Deg2Rad;
        Vector3 projectionOnXY = new Vector3(direction.x, direction.y, 0);
        float beta = Vector3.Angle(Vector3.right, projectionOnXY) * Mathf.Deg2Rad;

        Vector3 leadingVector = new Vector3();
        leadingVector.x = radius * scale.x * Mathf.Sin(alpha) * Mathf.Cos(beta);
        leadingVector.y = radius * scale.y * Mathf.Sin(alpha) * Mathf.Sin(beta);
        leadingVector.z = radius * scale.z * Mathf.Cos(alpha);
        return leadingVector;
    }

    // returns rotation around an axis taking the asisLimit into consideration if it is enabled
    private float RotateAroundAxis(Limit axisLimit)
    {
        if (randomRotation.xLimit.enabled)
            return Random.Range(axisLimit.min, axisLimit.max);
        else
            return Random.Range(0, 360);
    }

    // rotates placeable according to its randomRotation field
    public void RotateRandomly()
    {
        Vector3 rotation = new Vector3();
        // id doesn't make sense to rotate the object horizontally for some options
        bool rotateHorizontally = verticalPlacement != VerticalPlacement.OnBottom && verticalPlacement != VerticalPlacement.UnderSurface && shape != Shape.InfiniteCyllinder;
        if (rotateHorizontally && randomRotation.x) rotation.x = RotateAroundAxis(randomRotation.xLimit);
        if (randomRotation.y)                       rotation.y = RotateAroundAxis(randomRotation.yLimit);
        if (rotateHorizontally && randomRotation.z) rotation.z = RotateAroundAxis(randomRotation.zLimit);
        transform.rotation *= Quaternion.Euler(rotation);
    }

    // draws the area of Placeable and placing area bounds for placing this placeable
    // yellow means correct placement and red means conflict
    public void OnDrawGizmosSelected()
    {
        if (debugMode)
            DrawProbingVector();
        Gizmos.color = Color.yellow;
        if (placer != null)
        {
            // draw bounds gimzo of placing area shows it's bounds reduced by radius in each direction
            // basically the area where the center of the object can be placed
            if(debugMode)
                placer.placingArea.DrawBoundsForPlaceable(this);
            if (!placer.Allowed(this))
                Gizmos.color = Color.red;
        }
        Matrix4x4 saved = Gizmos.matrix;
        if (shape==Shape.Sphere)
        {
            Gizmos.matrix *= Matrix4x4.Translate(transform.position + offset);
            Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
            Gizmos.matrix *= Matrix4x4.Scale(scale);
            Gizmos.DrawWireSphere(Vector3.zero, radius);
        } else if(shape == Shape.InfiniteCyllinder)
        {
            Gizmos.matrix *= Matrix4x4.Translate(transform.position+offset);
            Gizmos.matrix *= Matrix4x4.Scale(new Vector3(radius*2 * scale.x, 0, radius * 2* scale.z));
            Gizmos.DrawWireMesh(PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cylinder));
        } else
        {
            throw new InvalidEnumValueException(shape);
        }
        Gizmos.matrix = saved;
    }

    // draws white ray showing probing vector and yellow ray drawing
    // leading vector of placeable shape in probing vector's direction
    void DrawProbingVector()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position + offset, probingVector);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + offset, transform.rotation * LeadingVector(probingVector));
    }
}
