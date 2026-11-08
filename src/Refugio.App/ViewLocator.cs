using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Refugio.App.ViewModels;
using System;

namespace Refugio.App;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null) return null;

        var name = param.GetType().FullName!
            .Replace("Refugio.App.ViewModels", "Refugio.App.Views", StringComparison.Ordinal)
            .Replace("ViewModel", "View", StringComparison.Ordinal);

        var type = Type.GetType(name);
        if (type is null)
        {
            return new TextBlock { Text = $"Vista no encontrada: {name}" };
        }

        return (Control)Activator.CreateInstance(type)!;
    }

    public bool Match(object? data) => data is ViewModelBase;
}
