public interface IInitializable
{
	#region Variables & Properties

	bool IsInitialized { get; }

	#endregion

	void Initialize();
}
