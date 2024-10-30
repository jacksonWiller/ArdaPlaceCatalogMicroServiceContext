using System;

namespace Catalog.Domain.ValueObjects;
public class Image 
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; }
    public string Prefix { get ; private set; }
    public string Url { get; private set; }

    public Image(string prefix, string nome)
    {
        Prefix = prefix;
        Nome = nome;
        Url = $"{prefix}/{nome}";
    }
}
