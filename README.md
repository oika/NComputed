# NComputed

An implementation of computed property in C#/WPF.  
It generates property which value computed by other properties.  
These properies to be defined in ViewModel class and can be binded to View class.

## How To Use

Define Computed in ViewModel like this.

```cs
    this.Sum = Computed<int>.Of(this, nameof(Sum))
                            // list property names to watch changes
                            .Observe(nameof(Num1), nameof(Num2))
                            // how to calculate the value
                            .ComputeAs(me => me.Num1 + me.Num2)
                            .Build();  
```

In View, bind to this property with `ComputedValueConverter`.

You can use some frameworks like [Prism](https://github.com/PrismLibrary/Prism) with it.

See sample sources in detail.
