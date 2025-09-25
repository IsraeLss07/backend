using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public string? Search { get; set; }
    }

    public class PaginationRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor a 0")]
        public int Page { get; set; } = 1;

        [Range(5, 100, ErrorMessage = "El tamaño de página debe estar entre 5 y 100")]
        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }
    }
}