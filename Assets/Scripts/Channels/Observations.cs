using System;
using System.Text;

/// <summary>
/// This class wraps an array of floats and gives its parts 
/// meaningful names along with static and dynamic checks.
/// It also provides a toString function printing all of the fields.
/// There are two constructors, one wraps existing array
/// and the other is used to create a new one.
/// </summary>
/// <remarks>
/// This file was generated by python script
/// "channel_generator.py" from "Observations.ch".
/// Do not edit it directly, instead edit either of them. 
/// </remarks>
public class Observations {
	public float[] array {get; private set;}

	public Observations(float[] source) {
		if(source.Length!=length){
			throw new ArgumentException("The size of passed array is incorrect");
		}
		array=source;
	}

	public Observations() {
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

	public float[] acceleration 
	{
		get {
			float[] result=new float[3];
			for(int i=0; i<3; i++){
				result[i]=array[0+i];
			}
			return result;
		}
		set {
			if(value.Length!=3){
				throw new ArgumentException("The size of passed array is incorrect");
			}
			value.CopyTo(array, 0);
		}
	}

	public float[] angular_acceleration 
	{
		get {
			float[] result=new float[3];
			for(int i=0; i<3; i++){
				result[i]=array[3+i];
			}
			return result;
		}
		set {
			if(value.Length!=3){
				throw new ArgumentException("The size of passed array is incorrect");
			}
			value.CopyTo(array, 3);
		}
	}

	public float[] rotation 
	{
		get {
			float[] result=new float[3];
			for(int i=0; i<3; i++){
				result[i]=array[6+i];
			}
			return result;
		}
		set {
			if(value.Length!=3){
				throw new ArgumentException("The size of passed array is incorrect");
			}
			value.CopyTo(array, 6);
		}
	}

	public float depth 
	{
		get {
			return array[9];
		}
		set {
			array[9]=value;
		}
	}

	public float[] bounding_box 
	{
		get {
			float[] result=new float[4];
			for(int i=0; i<4; i++){
				result[i]=array[10+i];
			}
			return result;
		}
		set {
			if(value.Length!=4){
				throw new ArgumentException("The size of passed array is incorrect");
			}
			value.CopyTo(array, 10);
		}
	}

	public float positive_negative 
	{
		get {
			return array[14];
		}
		set {
			array[14]=value;
		}
	}

	public float[] relative_position 
	{
		get {
			float[] result=new float[4];
			for(int i=0; i<4; i++){
				result[i]=array[15+i];
			}
			return result;
		}
		set {
			if(value.Length!=4){
				throw new ArgumentException("The size of passed array is incorrect");
			}
			value.CopyTo(array, 15);
		}
	}

	public float grab 
	{
		get {
			return array[19];
		}
		set {
			array[19]=value;
		}
	}

	public float torpedo 
	{
		get {
			return array[20];
		}
		set {
			array[20]=value;
		}
	}

	public override string ToString(){
		StringBuilder sb = new StringBuilder(256);
		
		sb.Append("acceleration : ");
		SliceToString(sb, 0, 3);
		sb.Append("\n");
		
		sb.Append("angular acceleration : ");
		SliceToString(sb, 3, 3);
		sb.Append("\n");
		
		sb.Append("rotation : ");
		SliceToString(sb, 6, 3);
		sb.Append("\n");
		
		sb.Append("depth : ");
		sb.Append(array[9].ToString("0.##"));
		sb.Append("\n");
		
		sb.Append("bounding box : ");
		SliceToString(sb, 10, 4);
		sb.Append("\n");
		
		sb.Append("positive/negative : ");
		sb.Append(array[14].ToString("0.##"));
		sb.Append("\n");
		
		sb.Append("relative position : ");
		SliceToString(sb, 15, 4);
		sb.Append("\n");
		
		sb.Append("grab : ");
		sb.Append(array[19].ToString("0.##"));
		sb.Append("\n");
		
		sb.Append("torpedo : ");
		sb.Append(array[20].ToString("0.##"));
		sb.Append("\n");
		
		return sb.ToString();
	}
	const int length=21;
}
