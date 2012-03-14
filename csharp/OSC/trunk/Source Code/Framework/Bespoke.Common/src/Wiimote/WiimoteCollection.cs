using System.Collections.ObjectModel;

namespace Bespoke.Common.Wiimote
{
	/// <summary>
	/// Used to manage multiple Wiimotes
	/// </summary>
	public class WiimoteCollection : Collection<Wiimote>
	{
		/// <summary>
		/// Finds all Wiimotes connected to the system and adds them to the collection
		/// </summary>
		public void FindAllWiimotes()
		{
			Wiimote.FindWiimote(WiimoteFound);
		}

		private bool WiimoteFound(string devicePath)
		{
			this.Add(new Wiimote(devicePath));
			return true;
		}
	}
}
