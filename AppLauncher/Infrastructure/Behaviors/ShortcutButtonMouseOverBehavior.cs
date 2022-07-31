using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace AppLauncher.Infrastructure.Behaviors
{
    public class ShortcutButtonMouseOverBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MouseEnter += OnMouseEntrer;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }
        protected override void OnDetaching()
        {
            AssociatedObject.MouseEnter -= OnMouseEntrer;
            AssociatedObject.MouseLeave -= OnMouseLeave;
        }


        private void OnMouseEntrer(object sender, MouseEventArgs e)
        {
            App.MainWindowViewModel.Title = Text;
        }


        private void OnMouseLeave(object Sender, MouseEventArgs E)
        {
            App.MainWindowViewModel.Title = string.Empty;
        }

        #region Text : string - Имя ярлыка

        /// <summary>Имя ярлыка</summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(ShortcutButtonMouseOverBehavior),
                new PropertyMetadata(default(string)));

        /// <summary>Имя ярлыка</summary>
        [Category("ShortcutButtonMouseOverBehavior")]
        [Description("Имя ярлыка")]
        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion
    }
}
