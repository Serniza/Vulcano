using System;

public partial struct TriBool
{
	#region Variables & Properties

	TriBoolValue value;

	#endregion

	TriBool(TriBoolValue value)
	{ 
		this.value = value; 
	}

	public static TriBool Undefined => new TriBool(TriBoolValue.Undefined);

	public static implicit operator TriBool(int value) => (value < 3) ? (TriBool)value : throw new ArgumentOutOfRangeException(nameof(value));

	public static implicit operator int(TriBool triBool) => (int)triBool.value;

	public static implicit operator TriBool(bool value) => new TriBool(value ? TriBoolValue.True : TriBoolValue.False);

	public static implicit operator bool(TriBool triBool) => triBool.value == TriBoolValue.True;

	public static bool operator < (TriBool a, TriBool b) => a.value < b.value;

	public static bool operator <= (TriBool a, TriBool b) => a.value <= b.value;

	public static bool operator > (TriBool a, TriBool b) => a.value > b.value;	

	public static bool operator >= (TriBool a, TriBool b) => a.value >= b.value;

	public static bool operator == (TriBool a, TriBool b) => a.value == b.value;

	public static bool operator != (TriBool a, TriBool b) => a.value != b.value;

	public override int GetHashCode() => value.GetHashCode();

	public override bool Equals(object @object) => @object is TriBool triBool && value == triBool.value;

	public override string ToString() => value.ToString();
}
