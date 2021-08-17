using MGen.Builder.BuilderContext;
using System;

namespace MGen.Builder.Writers
{
    partial class WritePropertyBinders
    {
        public bool SupportsNotifyPropertyChanged { get; set; }

        public bool SupportsNotifyPropertyChanging { get; set; }

        public static readonly WritePropertyBinders Instance = new();
    }

    partial class WritePropertyBinders : IHandleBuildingClasses, IHandleBuildingNestedClasses
    {
        public void Handle(ClassBuilderContext context, Action next) => Write(context, next);

        public void Handle(NestedClassBuilderContext context, Action next) => Write(context, next);

        public void Write(ClassBuilderContext context, Action next)
        {
            SupportsNotifyPropertyChanged = false;
            SupportsNotifyPropertyChanging = false;

            foreach (var inheritInterface in context.Interface.AllInterfaces)
            {
                if (inheritInterface.ContainingAssembly.Name == "System.ObjectModel" &&
                    inheritInterface.ContainingNamespace.Name == "ComponentModel")
                {
                    switch (inheritInterface.Name)
                    {
                        case "INotifyPropertyChanged":
                            SupportsNotifyPropertyChanged = true;
                            break;
                        case "INotifyPropertyChanging":
                            SupportsNotifyPropertyChanging = true;
                            break;
                    }
                }
            }

            next();
        }
    }

    partial class WritePropertyBinders : IHandleBuildingPropertySetters
    {
        public void Handle(PropertySetterBuilderContext context, Action next)
        {
            if (SupportsNotifyPropertyChanging && !context.Primary.IsIndexer)
            {
                context.Builder.AppendLine(builder => builder
                    .Append("PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(\"").Append(context.Primary.Name).Append("\"));"));
            }

            next();

            if (SupportsNotifyPropertyChanged && !context.Primary.IsIndexer)
            {
                context.Builder.AppendLine(builder => builder
                    .Append("PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(\"").Append(context.Primary.Name).Append("\"));"));
            }
        }
    }
}
