namespace MoWebApp.Documents
{
    /// <summary>
    /// Represents new user details.
    /// </summary>
    public class NewUserDetails
    {
        /// <summary>
        /// Returns the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Returns the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Returns the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Returns the current environment's url.
        /// </summary>
        public string LoginUrl { get; set; }
    }
}
