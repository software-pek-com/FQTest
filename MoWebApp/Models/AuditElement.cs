using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MoWebApp.Models
{
    /// <summary>
    /// Represents audit properties of a <see cref="UserDocument"/>.
    /// </summary>
    public class AuditElement
    {
        /// <summary>
        /// Returns the creation date.
        /// </summary>
        [BsonRequired]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Returns the user who created this data.
        /// </summary>
        [BsonRequired]
        public string CreationUser { get; set; }

        /// <summary>
        /// Returns the last update date.
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// Returns the user who last updated this data.
        /// </summary>

        public string LastUpdateUser { get; set; }
    }
}
