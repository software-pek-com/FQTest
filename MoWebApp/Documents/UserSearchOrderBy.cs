namespace MoWebApp.Documents
{
    /// <summary>
    /// Represents user search result ordering.
    /// </summary>
    public class UserSearchOrderBy
    {
        /// <summary>
        /// When true returns user search results ordered by <see cref="User.LastName"/> in ascending lexicographic order.
        /// </summary>
        public bool LastName { get; set; }

        /// <summary>
        /// When true returns user search results ordered by user's <see cref="Audit.CreationDate"/>.
        /// </summary>
        public bool CreationDate { get; set; }

        /// <summary>
        /// When true returns user search results ordered by user's <see cref="User.LastConnectionDate"/>.
        /// </summary>
        public bool LastConnectionDate { get; set; }
    }
}
