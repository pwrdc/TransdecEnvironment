import sys
from pathlib import Path

# Usage:
# Pass a file containing fields as an argument.
# A new C# file with the same base name should appear.
#
# Example
# python channel_generator.py Observations.ch
#
# Source file schema
# It should contain field names separated by new line characters.
# Optionally a name can be followed with ':' and a number of array cells.
# (By default it is one.)
# Empty lines are allowed.
#
# Motivation
# In this case generated classes provide the best balance between
# safety, efficiency and readability.
# Also the source files can be reused on the other side of ML-Agents.
#
# Why Python
# It works better than C# as a scripting and text processing language.
# It is also used a lot in other parts of the project.

def main():
	if len(sys.argv)<2:
		print("Provide file name as an argument.")
		exit(-1)
	if len(sys.argv)>2:
		print("Too many arguments.")
		exit(-1)
	path=sys.argv[1]

	input_file = open(path, 'r')
	name=Path(path).stem
	output_file = open(name+".cs", 'w')

	translate(input_file, output_file, path, name)

	input_file.close()
	output_file.close()

def normalize_name(name):
	return name.strip().replace(' ', '_').replace('/', '_')

def translate(input_file, output_file, path, name):
	index=0
	output_file.write(make_header(name, path))
	to_string_lines=[]
	for line in input_file:
		if line=="":
			continue
		splitted=line.split(':')
		name=normalize_name(splitted[0])
		# get count
		if len(splitted)>1:
			count=int(splitted[1])
		else:
			count=1
		# write proper property block and add line to toString function
		if count==1:
			output_file.write(make_float_property(name, index))	
			to_string_lines.append(f'''
			sb.Append("{name+" : "}");
			sb.Append({name}.ToString("0.##"));
			sb.Append("\\n");''')
		else:
			output_file.write(make_array_property(name, index, count))
			to_string_lines.append(f'''
			sb.Append("{name+" : "}");
			SliceToString(sb, {index}, {count});
			sb.Append("\\n");''')
		index+=count

	output_file.write(make_to_string_function(to_string_lines))
	output_file.write(make_footer(index))

def make_header(name, path):
	return f'''using System;
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
/// "channel_generator.py" from "{path}".
/// Do not edit it directly, instead edit either of them. 
/// </remarks>
public class {name} {{
	public float[] array {{get; private set;}}

	public {name}(float[] source) {{
		if(source.Length!=length){{
			throw new ArgumentException("The size of passed array is incorrect");
		}}
		array=source;
	}}

	public {name}() {{
		array=new float[length];
	}}

	void SliceToString(StringBuilder sb, int start, int count)
    {{
        bool first = true;
        for (int i=0; i < count; i++)
        {{
            if (!first)
                sb.Append(", ");
            sb.Append(array[start+i].ToString("0.##"));
            first = false;
        }}
    }}
'''

def make_float_property(name, index):
	return f'''
	public float {name} 
	{{
		get {{
			return array[{index}];
		}}
		set {{
			array[{index}]=value;
		}}
	}}
'''

def make_array_property(name, index, count):
	return f'''
	public float[] {name} 
	{{
		get {{
			float[] result=new float[{count}];
			for(int i=0; i<{count}; i++){{
				result[i]=array[{index}+i];
			}}
			return result;
		}}
		set {{
			if(value.Length!={count}){{
				throw new ArgumentException("The size of passed array is incorrect");
			}}
			value.CopyTo(array, {index});
		}}
	}}
'''

def make_to_string_function(to_string_lines):
	newline='\n		'
	return f'''
	public override string ToString(){{
		StringBuilder sb = new StringBuilder(256);
		{newline.join(to_string_lines)}
		
		return sb.ToString();
	}}'''

def make_footer(length):
	return f'''
	const int length={length};
}}
'''

if __name__=="__main__":
	main()