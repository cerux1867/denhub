using System;
using System.Collections.Generic;

namespace Denhub.Chat.Processor.Models {
    public class TwitchEmote {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public List<Uri> EmoteUrl { get; set; }
    }
}