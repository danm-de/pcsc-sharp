namespace PCSC
{
    /// <summary>
    /// Extension methods for <see cref="SCRState"/>
    /// </summary>
    public static class SCRStateExtensionMethods
    {
        /// <summary>
        /// Checks if a card is absent
        /// </summary>
        /// <param name="state">State to check</param>
        /// <returns><c>true</c> if the card is absent</returns>
        public static bool CardIsAbsent(this SCRState state) {
            return ((state & SCRState.Empty) == SCRState.Empty);
        }

        /// <summary>
        /// Checks if a card is present
        /// </summary>
        /// <param name="state">State to check</param>
        /// <returns><c>true</c> if a card is present</returns>
        public static bool CardIsPresent(this SCRState state) {
            return ((state & SCRState.Present) == SCRState.Present);
        }
    }
}