

using System.Text.Json;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header) {
            var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            response.Headers.Add("Pagination",JsonSerializer.Serialize(header,jsonOptions));
            //politica para que la cabecera se pueda enviar, porque la cabecera es personalizada
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}