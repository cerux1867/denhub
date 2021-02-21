namespace Denhub.API.Models {
    public record VodsParameters {
        public int Page { get; set; } = 1;
        public int Limit {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
        
        private const int MaxPageSize = 100;
        private int _pageSize = 10;
    }
}