# [![MGen](https://github.com/CyAScott/MGen/blob/main/assets/mgen-64.png?raw=true "MGen")](https://www.nuget.org/packages/MGen/) MGen

[![NuGet Badge](https://buildstats.info/nuget/MGen?includePreReleases=true)](https://www.nuget.org/packages/MGen/)

MGen is a code generator library that can generate C# classes from interfaces.

### Simple Exmaple

The first thing you will need to do is to install the MGen NuGet packages.

```
Install-Package MGen
Install-Package MGen.Abstractions
```

Here is a simple example of how it works:

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

#### Icon Created By

Icons made by [Freepik](http://www.freepik.com) from [www.flaticon.com](http://www.flaticon.com) is licensed by [CC 3.0 B](http://creativecommons.org/licenses/by/3.0/)
