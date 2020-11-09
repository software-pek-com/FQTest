namespace MoWebApp.Documents
{
    /// <summary>
    /// Represents a summary information for a <see cref="User"/>.
    /// </summary>
    public class UserSummary
    {
        /// <summary>
        /// Gets user's id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets user's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets user's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Returns true if user has ever connected i.e. at least once.
        /// </summary>
        public bool HasUserEverConnected { get; set; }

        /// <summary>
        /// Gets the user's display name.
        /// </summary>
        public string DisplayName => $"{FirstName} {LastName}";
    }
}
