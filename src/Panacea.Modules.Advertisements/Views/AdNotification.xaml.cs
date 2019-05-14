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
        public AdNotification()
        {
            InitializeComponent();
        }
        private void Presenter_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //_core.GetUiManager().Refrain(this);
        }
    }
}
