
namespace API.Helpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = 20;
        private int _pageSize = 10;
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            //value es el parametro de entrda para saber cuantos registros devolver por pagina
        }
    }
}