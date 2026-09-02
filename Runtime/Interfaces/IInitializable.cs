namespace SernizaGamesCore
{
	public interface IInitializable
	{
		#region Variables & Properties

		TriBool IsInitialized { get; }

		#endregion

		void Initialize(object[] parameters = null);
	}
}
