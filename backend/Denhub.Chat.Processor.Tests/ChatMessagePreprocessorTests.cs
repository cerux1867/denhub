using System.Threading.Tasks;
using Denhub.Chat.Processor.Processors;
using Xunit;

namespace Denhub.Chat.Processor.Tests {
    public class ChatMessagePreprocessorTests {
        [Fact]
        public async Task ProcessMessage_ValidMessageNoBadgesNoEmotes_ParsedMessage() {
            var rawMsg =
                "@badge-info=;badges=;color=#B61CE0;display-name=tortugitaw;emotes=;flags=;id=f076933d-cb1d-4436-ac13-d4098c6a7c73;mod=0;room-id=22484632;subscriber=0;tmi-sent-ts=1619641716729;turbo=0;user-id=144820811;user-type= :tortugitaw!tortugitaw@tortugitaw.tmi.twitch.tv PRIVMSG #forsen :KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW KEKW";
            var preprocessor = new ChatMessagePreprocessor();

            var parsed = preprocessor.ProcessMessage(rawMsg);
            
            Assert.Empty(parsed.RawBadges);
            Assert.Equal("tortugitaw", parsed.UserDisplayName);
            Assert.Equal("forsen", parsed.ChannelDisplayName);
        }
        
        [Fact]
        public async Task ProcessMessage_ValidMessageBadgesEmotes_ParsedMessage() {
            var rawMsg =
                "@badge-info=;badges=premium/1;client-nonce=e60d1932d7cf269630e65bb8f5c64e80;color=;display-name=kresos007;emotes=555555558:6-7;flags=;id=9f969801-73c2-42e8-a008-5e500ff211b7;mod=0;room-id=26261471;subscriber=0;tmi-sent-ts=1619642734508;turbo=0;user-id=161316080;user-type= :kresos007!kresos007@kresos007.tmi.twitch.tv PRIVMSG #asmongold :Sadge :(";
            var preprocessor = new ChatMessagePreprocessor();

            var parsed = preprocessor.ProcessMessage(rawMsg);
            
            Assert.Single(parsed.RawBadges);
            Assert.Single(parsed.Emotes);
            Assert.Equal("kresos007", parsed.UserDisplayName);
            Assert.Equal("asmongold", parsed.ChannelDisplayName);
        }
    }
}