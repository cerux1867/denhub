using System;
using System.Collections.Generic;

namespace Denhub.Chat.Processor.Models {
    public class CachedEmote {
        public string Name { get; set; }
        public EmotePlatform EmotePlatform { get; set; }
        public List<Uri> Urls { get; set; }
    }
}