namespace IntuitiveBackSystem.Runtime
{
    /// <summary>
    /// Interface that can be implemented to handle the back action.
    /// </summary>
    public interface IIntuitiveBackHandler
    {
        /// <summary>
        /// The tooltip that can be used to display the back action in the UI.
        /// </summary>
        public string toolTip { get; }

        /// <summary>
        /// Called when the back action is performed and this IBackHandler is the current one.
        /// It will be called every time the back action is performed until this IBackHandler is unregistered.
        /// </summary>
        public void OnBack();
    }
}