using Panacea.Core;
using Panacea.Core.Mvvm;
using Panacea.Modularity.UiManager.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Panacea.Modules.Advertisements
{
    /// <summary>
    /// Interaction logic for AdNotification.xaml
    /// </summary>
    public partial class AdNotification : UserControl
    {
        private readonly PanaceaServices _core;

        public AdNotification(FrameworkElement content, PanaceaServices core)
        {
            InitializeComponent();
            Presenter.Content = content;
            _core = core;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _core.GetUiManager().Refrain(this);
        }

        private void Presenter_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _core.GetUiManager().Refrain(this);
        }
    }
}
