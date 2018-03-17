namespace Zynga.Protobuf.Runtime {
	public interface IEventSubscribable {
		/// <summary>
		/// Notify any event listeners
		/// </summary>
		void NotifySubscribers();
	}
}