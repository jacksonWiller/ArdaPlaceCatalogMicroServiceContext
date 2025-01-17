using System;

namespace Catalog.Domain.ValueObjects;
public class Image 
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public string Prefix { get ; private set; }
    public string Url { get; private set; }
    public string Width { get; private set; }
    public string Height { get; private set; }

    public Image() { }

    public Image(string prefix, string nome)
    {
        Prefix = prefix;
        Name = nome;
        Url = $"{prefix}/{nome}";
    }
}
