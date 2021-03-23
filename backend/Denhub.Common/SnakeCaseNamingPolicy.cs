using System.Text.Json;
using Denhub.Common.Extensions;

namespace Denhub.Common {
    public class SnakeCaseNamingPolicy : JsonNamingPolicy {
        public static SnakeCaseNamingPolicy Instance { get; } = new();
        
        public override string ConvertName(string name) {
            return name.ToSnakeCase();
        }
    }
}