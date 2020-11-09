namespace MoWebApp.Documents
{
    /// <summary>
    /// Represents user search properties.
    /// </summary>
    public class UserSearchFilter
    {
        /// <summary>
        /// Filter by <see cref="User.FirstName"/>.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Filter by <see cref="User.LastName"/>.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Filter by <see cref="User.LastConnectionDate"/>.
        /// </summary>
        public bool HasUserEverConnected { get; set; }
    }
}
