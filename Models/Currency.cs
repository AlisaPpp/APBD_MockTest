namespace Models;

public class Currency
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Rate { get; set; }
    public List<Country> Countries { get; set; }
}