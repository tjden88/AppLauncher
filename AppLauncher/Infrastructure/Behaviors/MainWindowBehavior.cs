using System.ComponentModel;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace AppLauncher.Infrastructure.Behaviors
{
    /// <summary>
    /// Поведение главного окна при изменении свойств
    /// </summary>
    public class MainWindowBehavior : Behavior<Window>
    {






        #region IsHidden : bool - Скрыто ли окно

        /// <summary>Скрыто ли окно</summary>
        public static readonly DependencyProperty IsHiddenProperty =
            DependencyProperty.Register(
                nameof(IsHidden),
                typeof(bool),
                typeof(MainWindowBehavior),
                new PropertyMetadata(default(bool)));

        /// <summary>Скрыто ли окно</summary>
        [Category("MainWindowBehavior")]
        [Description("Скрыто ли окно")]
        public bool IsHidden
        {
            get => (bool) GetValue(IsHiddenProperty);
            set => SetValue(IsHiddenProperty, value);
        }

        #endregion
    }
}
