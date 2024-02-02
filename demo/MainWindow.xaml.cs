using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaUI.PrintToPDF.Demo
{
  public class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new Model(this);
    }

    private void InitializeComponent()
    {
#if DEBUG
        this.AttachDevTools();
#endif
      AvaloniaXamlLoader.Load(this);
    }
  }
}