using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// This class extends BuoyancyPhysics and adds calculating mass from density and volume 
/// and water current on top of it.
/// </summary>
public class WaterPhysics : BuoyancyPhysics
{
    public bool waterCurrentEnabled = true;

    public bool deduceMassFromVolume = true;
    // Selected densities for different materials 
    // taken from https://en.wikipedia.org/wiki/Density#Densities
    public enum Density
    {
        Wood = 700,// this is the goto density for floating bodies
        Plastic = 1175,
        Styrofoam = 75,
        Concrete = 2400,
        Aluminium = 2700,
        Iron = 7870,
        Air = 1,
        // the simulation isn't perfect and it's impossible to balance gravity with buoyancy force
        // so for bodies suspended in water use BuoyancyForceMode.FluctuationsOnly
        Water = 997,
        // this option will show densityManual variable and allow setting density directly as a number
        SetManually = 0
    }
    [ShowIf("deduceMassFromVolume")]
    public Density density = Density.Wood;
    bool SetDensityManually => density == Density.SetManually;
    [ShowIf(EConditionOperator.And, "SetDensityManually", "deduceMassFromVolume")]
    public float densityManual = (float)Density.Plastic;

    [ShowNativeProperty]
    float ActualDensity => SetDensityManually ? densityManual : (float)density;
    [ShowNativeProperty]
    float DeducedMass => Volume * ActualDensity;

    void AddCurrent()
    {
        body.AddForce(Environment.WaterCurrent.Instance.GetForce());
    }

    protected override void FixedUpdate()
    {
        if (deduceMassFromVolume)
            body.mass = DeducedMass;

        base.FixedUpdate();

        if (Environment.WaterCurrent.Instance.enabledInAcademy && waterCurrentEnabled && IsUnderWater(transform.position))
            AddCurrent ();
    }
}
