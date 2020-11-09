using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MoWebApp.Documents
{
    /// <summary>
    /// Represents user details.
    /// </summary>
    public class UserDetails
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
        /// Returns a username which is unique in the database.
        /// </summary>
        [BsonRequired]
        public string UserName { get; set; }

        /// <summary>
        /// Returns the birthdate.
        /// </summary>
        /// <remarks>
        /// Details of how to use the preferable <see cref="DateTimeOffset"/> in MongoDB queries
        /// are <see cref="https://stackoverflow.com/questions/10480127/mongodb-and-datetimeoffset-type">here</see>.
        /// </remarks>
        public DateTime? Birthdate { get; set; }

        /// <summary>
        /// Returns the password.
        /// </summary>
        /// <remarks>
        /// In real life passwords should be hashed before being stored in database.
        /// </remarks>
        public string Password { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> of user's last connection.
        /// Returns null if user has never connected.
        /// </summary>
        public DateTime? LastConnectionDate { get; set; }
    }
}
