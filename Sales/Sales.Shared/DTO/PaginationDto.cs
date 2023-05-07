namespace Sales.Shared.DTO
{
    public class PaginationDto
    {
        public int Id { get; set; }
        public string? Filter { get; set; }
        public int Page { get; set; } = 1;
        public int RecordNumber { get; set; } = 10;
    }
}
