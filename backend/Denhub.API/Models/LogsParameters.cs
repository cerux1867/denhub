using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Denhub.API.Models {
    public record LogsParameters {
        /// <summary>
        /// Optional starting timestamp in ISO 8601 format
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// Optional ending timestamp in ISO 8601 format
        /// </summary>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// Optional pagination cursor to continue a previous paged query
        /// </summary>
        public string Cursor { get; set; }
        
        /// <summary>
        /// Optional pagination limit
        /// </summary>
        [Range(0, 100)] 
        public int Limit { get; set; } = 100;

        /// <summary>
        /// Defines sorting order. Allowed values are 'asc' and 'desc'. Default is 'desc'.
        /// </summary>
        public string Order { get; set; } = "desc";
        
        /// <summary>
        /// Optional Twitch username by which to filter results 
        /// </summary>
        public string Username { get; set; }
    }
}