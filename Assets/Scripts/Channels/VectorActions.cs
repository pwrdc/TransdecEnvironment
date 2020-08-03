using System;
using System.Text;

/// <summary>
/// This class wraps an array of floats and gives its parts 
/// meaningful names along with static and dynamic checks.
/// It also provides a ToString function printing all of the fields.
/// There are two constructors, one wraps existing array
/// and the other is used to create a new one.
/// </summary>
/// <remarks>
/// This file was generated by python script
/// "channel_generator.py" from "VectorActions.ch".
/// Do not edit it directly, instead edit either of them. 
/// </remarks>
public class VectorActions {
	public float[] array {get; private set;}

	public VectorActions(float[] source) {
		if(source.Length!=length){
			throw new ArgumentException("The size of passed array is incorrect");
		}
		array=source;
	}

	public VectorActions() {
		array=new float[length];
	}

	void SliceToString(StringBuilder sb, int start, int count)
    {
        bool first = true;
        for (int i=0; i < count; i++)
        {
            if (!first)
                sb.Append(", ");
            sb.Append(array[start+i].ToString("0.##"));
            first = false;
        }
    }

	public float longitudal 
	{
		get {
			return array[0];
		}
		set {
			array[0]=value;
		}
	}

	public float lateral 
	{
		get {
			return array[1];
		}
		set {
			array[1]=value;
		}
	}

	public float vertical 
	{
		get {
			return array[2];
		}
		set {
			array[2]=value;
		}
	}

	public float yaw 
	{
		get {
			return array[3];
		}
		set {
			array[3]=value;
		}
	}

	public float camera 
	{
		get {
			return array[4];
		}
		set {
			array[4]=value;
		}
	}

	public float ball_grapper 
	{
		get {
			return array[5];
		}
		set {
			array[5]=value;
		}
	}

	public float torpedo 
	{
		get {
			return array[6];
		}
		set {
			array[6]=value;
		}
	}

	public override string ToString(){
		StringBuilder sb = new StringBuilder(256);
		
		sb.Append("longitudal : ");
		sb.Append(array[0].ToString("0.##"));
		sb.Append("\n");
		
		sb.Append("lateral : ");
		sb.Append(array[1].ToString("0.##"));
		sb.Append("\n");
		
		sb.Append("vertical : ");
		sb.Append(array[2].ToString("0.##"));
		sb.Append("\n");
		
		sb.Append("yaw : ");
		sb.Append(array[3].ToString("0.##"));
		sb.Append("\n");
		
		sb.Append("camera : ");
		sb.Append(array[4].ToString("0.##"));
		sb.Append("\n");
		
		sb.Append("ball grapper : ");
		sb.Append(array[5].ToString("0.##"));
		sb.Append("\n");
		
		sb.Append("torpedo : ");
		sb.Append(array[6].ToString("0.##"));
		sb.Append("\n");
		
		return sb.ToString();
	}
	const int length=7;
}
