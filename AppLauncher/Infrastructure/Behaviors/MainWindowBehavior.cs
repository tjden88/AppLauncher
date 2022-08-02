using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AppLauncher.Infrastructure.Helpers;
using AppLauncher.Views;
using Microsoft.Xaml.Behaviors;

namespace AppLauncher.Infrastructure.Behaviors
{
    /// <summary>
    /// Поведение главного окна при изменении свойств
    /// </summary>
    public class MainWindowBehavior : Behavior<MainWindow>
    {
        private readonly ScaleTransform _ScaleTransform = new(0,0);
        private readonly CircleEase _CircleEase = new() { EasingMode = EasingMode.EaseIn };

        private readonly Storyboard _StoryboardShow = new();
        private readonly Storyboard _StoryboardHide = new();

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += OnLoad;
        }

        private void OnLoad(object Sender, RoutedEventArgs E)
        {
            AssociatedObject.RootGrid.RenderTransform = _ScaleTransform;
            AssociatedObject.RootGrid.Opacity = 1;
            AssociatedObject.RootGrid.RenderTransformOrigin = new Point(0.5, 1);

            var duration = TimeSpan.FromSeconds(0.23);

            // Подготовка анимации
            _StoryboardShow.Children.Add(new DoubleAnimation(0, 1, duration) { EasingFunction = _CircleEase });
            _StoryboardShow.Children.Add(new DoubleAnimation(0, 1, duration) { EasingFunction = _CircleEase });

            Storyboard.SetTargetProperty(_StoryboardShow.Children[0], new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(_StoryboardShow.Children[1], new PropertyPath("RenderTransform.ScaleY"));

            _StoryboardShow.Completed += (_, _) =>
                AssociatedObject.EnableBlur();


            _StoryboardHide.Children.Add(new DoubleAnimation(1, 0, duration) { EasingFunction = _CircleEase });
            _StoryboardHide.Children.Add(new DoubleAnimation(1, 0, duration) { EasingFunction = _CircleEase });

            Storyboard.SetTargetProperty(_StoryboardHide.Children[0], new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(_StoryboardHide.Children[1], new PropertyPath("RenderTransform.ScaleY"));

            _StoryboardHide.Completed += (_, _) =>
            {
                if(App.MainWindowViewModel.CloseWhenHide)
                    Application.Current.Shutdown();
            };

        }

        private void Show()
        {
            WindowPositionHelper.SetMainWindowPosition();
            AssociatedObject.Activate();
            _StoryboardShow.Begin(AssociatedObject.RootGrid);
        }

        private void Hide()
        {
            AssociatedObject.DisableBlur();
            _StoryboardHide.Begin(AssociatedObject.RootGrid);
        }


        #region IsHidden : bool - Скрыто ли окно

        /// <summary>Скрыто ли окно</summary>
        public static readonly DependencyProperty IsHiddenProperty =
            DependencyProperty.Register(
                nameof(IsHidden),
                typeof(bool),
                typeof(MainWindowBehavior),
                new PropertyMetadata(default(bool), OnHiddenChanged));

        private static void OnHiddenChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
        {
            var behavior = (MainWindowBehavior)D;

            if (E.NewValue is true)
                behavior.Hide();
            else
                behavior.Show();

        }

        /// <summary>Скрыто ли окно</summary>
        [Category("MainWindowBehavior")]
        [Description("Скрыто ли окно")]
        public bool IsHidden
        {
            get => (bool)GetValue(IsHiddenProperty);
            set => SetValue(IsHiddenProperty, value);
        }

        #endregion
    }
}
