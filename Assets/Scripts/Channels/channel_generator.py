import sys
from pathlib import Path
import string

# Usage:
# Pass a file containing fields as an argument.
# A new C# file with the same base name should appear.
#
# Example
# python channel_generator.py Observations.ch
#
# Source file schema
# It should contain field names separated by new line characters.
# Currently names can contain whitespaces other than newline, letters and '/' characters.
# Optionally a name can be followed with ':' and a number of array cells.
# (By default it is one.)
# Empty lines are allowed.
#
# Motivation
# In this case generated classes provide the best balance between
# safety, efficiency and readability.
# Also the source files can be reused on the other side of ML-Agents.
#
# Discarded alternatives 
# Indexing array directly - generates a lot of bugs and confusing source code. 
# The resulting machine code from the generated classes should look exactly the same, 
# because modern optimizers can easily inline properties. 
# Using dictionary - the code accessing the channels runs at every frame and dictionaries aren't very fast. 
# Each assignment would cost key.length integer comparisons (where key is a string key) plus writing to the array. 
# Also all of the checking would happen at runtime as opposed to this solution being partly checked at compile time. 
#
# Why Python
# It works better than C# as a scripting and text processing language.
# It is also used a lot in other parts of the project.
# 

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

def translate(input_file, output_file, path, name):
	output_file.write(make_header(name, path))
	# current index in the array
	index=0
	# this lines will be placed inside of class's ToString method
	to_string_lines=[]
	for line in input_file:
		# ignore empty lines
		if line=="":
			continue
		splitted=line.split(':')
		name=to_valid_csharp_identifier(splitted[0])
		# get count
		if len(splitted)>1:
			count=int(splitted[1])
		else:
			count=1
		# write proper property block
		if count==1:
			output_file.write(make_float_property(name, index))	
		else:
			output_file.write(make_array_property(name, index, count))
		# here the original name is passed, because it isn't used as variable name
		# and it looks better in debug view this way
		to_string_lines.append(make_to_string_line(splitted[0].strip(), index, count))
		index+=count

	output_file.write(make_to_string_function(to_string_lines))
	output_file.write(make_footer(index))

def to_valid_csharp_identifier(name):
	# here all of the characters 
	# that can exist in channels names 
	# but can't exist in C# identifiers 
	# must be replaced with spaces
	preprocessed=name.replace('/', ' ')
	# split the text into separate words and capitalize each one
	# properties names in C# start with a capital letter by convention
	capitalized_words=[word.capitalize() for word in preprocessed.split()]
	# join the words into one string
	return ''.join(capitalized_words)
		

def make_header(name, path):
	return f'''using System;
using System.Text;
using System.Globalization;

/// <summary>
/// This class wraps an array of floats and gives its parts 
/// meaningful names along with static and dynamic checks.
/// It also provides a ToString function printing all of the fields.
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

	/// <summary>
	/// Takes part of the array starting at start and containing count 
	/// elements and writes its string representation to StringBuilder.
	/// </summary>
	void SliceToString(StringBuilder sb, int start, int count, CultureInfo culture)
    {{
        bool first = true;
        for (int i=0; i < count; i++)
        {{
            if (!first)
                sb.Append(", ");
            sb.Append(array[start+i].ToString("0.##", culture));
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

def make_to_string_line(name, index, count):
	if count==1:
		return f'''
		sb.Append("{name+" : "}");
		sb.Append(array[{index}].ToString("0.##", culture));
		sb.Append("\\n");'''
	else:
		return f'''
		sb.Append("{name+" : "}");
		SliceToString(sb, {index}, {count}, culture);
		sb.Append("\\n");'''

def make_to_string_function(to_string_lines):
	newline='\n		'
	return f'''
	public override string ToString(){{
		StringBuilder sb = new StringBuilder(256);
		// Without passing this class floats like "Infinity"
		// would be displayed in your system language.
        CultureInfo culture= CultureInfo.CreateSpecificCulture("en-US");

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