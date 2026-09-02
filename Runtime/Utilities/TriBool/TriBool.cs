using System;

public partial struct TriBool
{
	#region Variables & Properties

	TriBoolState value;

	#endregion

	TriBool(TriBoolState value)
	{ 
		this.value = value; 
	}

	public static TriBool Undefined => new TriBool(TriBoolState.Undefined);

	public static implicit operator TriBool(int value)
	{
		return value switch
		{
			0 => new TriBool(TriBoolState.False),
			1 => new TriBool(TriBoolState.Undefined),
			2 => new TriBool(TriBoolState.True),
			_ => throw new ArgumentOutOfRangeException(nameof(value))
		};
	}

	public static implicit operator int(TriBool triBool) => (triBool.value == TriBoolState.False) ? 0 : ((triBool.value == TriBoolState.Undefined) ? 1 : 2);

	public static implicit operator TriBool(bool value) => new TriBool(value ? TriBoolState.True : TriBoolState.False);

	public static implicit operator bool(TriBool triBool) => triBool.value == TriBoolState.True;

	public override int GetHashCode() => value.GetHashCode();

	public static bool operator < (TriBool a, TriBool b) => a.value < b.value;
	public static bool operator <= (TriBool a, TriBool b) => a.value <= b.value;
	public static bool operator > (TriBool a, TriBool b) => a.value > b.value;	
	public static bool operator >= (TriBool a, TriBool b) => a.value >= b.value;

	public static bool operator == (TriBool a, TriBool b) => a.value == b.value;
	public static bool operator != (TriBool a, TriBool b) => a.value != b.value;

	public override bool Equals(object @object) => @object is TriBool triBool && value == triBool.value;

	public override string ToString() => value.ToString();
}
