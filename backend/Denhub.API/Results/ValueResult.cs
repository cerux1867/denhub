namespace Denhub.API.Results {
    public abstract class ValueResult<T> : Result {
        public abstract T Value { get; }
    }
}