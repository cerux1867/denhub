namespace Denhub.API.Models.Twitch {
    public class TwitchResponseModel<T> {
        public T Data { get; set; }
        public TwitchPagination Pagination { get; set; }
    }
}