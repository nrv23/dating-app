namespace API;

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string Summary { get; set; } // una vez que se deshabilita la opcion <Nullable>disable</Nullable>  no es ncesario usar el ? para indicar que una
    // propiedad o variable pueda ser nula.
}
