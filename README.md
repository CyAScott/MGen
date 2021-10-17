# [![MGen](https://github.com/CyAScott/MGen/blob/main/assets/mgen-64.png?raw=true "MGen")](https://www.nuget.org/packages/MGen/) MGen

MGen is a code generator library that can generate C# classes from interfaces. Here is a simple example of how it works:

```
[Generate]
public interface IHaveAnGuidId
{
    Guid Id { get; set; }
}
```

The generated code will look like:

```
public class HaveAnGuidIdModel : IHaveAnGuidId
{
    public System.Guid Id
    {
        get
        {
            return _Id;
        }
        set
        {
            _Id = value;
        }
    }
    private System.Guid _Id;
        
    public HaveAnGuidIdModel()
    {
    }
        
}
```

For a full list of supported features read [this](https://github.com/CyAScott/MGen/wiki/Supported-Features).
