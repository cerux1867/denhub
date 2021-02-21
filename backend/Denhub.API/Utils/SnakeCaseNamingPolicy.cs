using System.Text.Json;
using Denhub.API.Extensions;

namespace Denhub.API.Utils {
    public class SnakeCaseNamingPolicy : JsonNamingPolicy {
        public static SnakeCaseNamingPolicy Instance { get; } = new();
        
        public override string ConvertName(string name) {
            return name.ToSnakeCase();
        }
    }
}