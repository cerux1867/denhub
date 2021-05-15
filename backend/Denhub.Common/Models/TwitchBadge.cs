using System;
using System.Collections.Generic;

namespace Denhub.Common.Models {
    public class TwitchBadge {
        public BadgeType Type { get; set; }
        public List<string> ImageUrls { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}