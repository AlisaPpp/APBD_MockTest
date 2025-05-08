namespace Application;

public class CurrencyRequest
{
    public string Name { get; set; }
    public float Rate { get; set; }
    public List<string> Countries { get; set; }
}