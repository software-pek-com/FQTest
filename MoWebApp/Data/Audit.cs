using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MoWebApp.Data
{
    /// <summary>
    /// Represents audit properties of a <see cref="User"/>.
    /// </summary>
    public class Audit
    {
        /// <summary>
        /// Gets the creation date.
        /// </summary>
        [BsonRequired]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets the user who created this user.
        /// </summary>
        [BsonRequired]
        public string CreationUser { get; set; }

        /// <summary>
        /// Gets the last update date.
        /// Null if data has not been changed since <see cref="CreationDate"/>.
        /// </summary>
        public DateTime? LastUpdateDate { get; set; }

        /// <summary>
        /// Gets the user who last updated this user.
        /// </summary>

        public string LastUpdateUser { get; set; }
    }
}
