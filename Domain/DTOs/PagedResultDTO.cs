namespace Domain.DTOs
{
    public class PagedResultDTO<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => TotalCount == 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
