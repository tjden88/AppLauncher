using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AppLauncher.Views
{
    /// <summary>
    /// Логика взаимодействия для AppGroup.xaml
    /// </summary>
    public partial class AppGroup : UserControl
    {
        public AppGroup()
        {
            InitializeComponent();
        }

        #region ColumnNumber : int - Номер колонки, к которой привязана группа

        /// <summary>Номер колонки, к которой привязана группа</summary>
        public static readonly DependencyProperty ColumnNumberProperty =
            DependencyProperty.Register(
                nameof(ColumnNumber),
                typeof(int),
                typeof(AppGroup),
                new PropertyMetadata(1));

        /// <summary>Номер колонки, к которой привязана группа</summary>
        [Category("AppGroup")]
        [Description("Номер колонки, к которой привязана группа")]
        public int ColumnNumber
        {
            get => (int) GetValue(ColumnNumberProperty);
            set => SetValue(ColumnNumberProperty, value);
        }

        #endregion
    }
}
