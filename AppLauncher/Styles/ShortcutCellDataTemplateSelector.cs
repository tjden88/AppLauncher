using System.Windows;
using System.Windows.Controls;
using AppLauncher.ViewModels;

namespace AppLauncher.Styles
{
    public class ShortcutCellDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //получаем вызывающий контейнер

            if (container is not FrameworkElement element || item is not ShortcutCellViewModel vm) return null;

            if(vm.Id == 0)

                return element.FindResource("MockShortcutCellDataTemplate") as DataTemplate;
            return element.FindResource("ShortcutCellDataTemplate") as DataTemplate;
        }
    }
}
