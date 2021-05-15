using System;
using System.Collections.Generic;

namespace Denhub.Common.Models {
    public class TwitchEmote {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public List<string> EmoteUrl { get; set; }
    }
}