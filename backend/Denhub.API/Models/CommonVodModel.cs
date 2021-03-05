using System;

namespace Denhub.API.Models {
    public record CommonVodModel {
        public int PlatformVodId { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Length { get; set; }
        public DateTime Date { get; set; }
        public VodType Type { get; set; }
        public int ViewCount { get; set; } 
    }
}