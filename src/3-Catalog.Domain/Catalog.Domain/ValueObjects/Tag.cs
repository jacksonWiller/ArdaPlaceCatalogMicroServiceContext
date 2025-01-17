using System;

namespace Catalog.Domain.ValueObjects;

public class Tag
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }

    public Tag() { }


    public Tag(string name)
    {
        Name = Name = name;
    }

}
