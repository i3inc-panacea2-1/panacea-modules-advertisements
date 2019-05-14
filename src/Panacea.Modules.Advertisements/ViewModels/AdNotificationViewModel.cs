using Panacea.Controls;
using Panacea.Core;
using Panacea.Core.Mvvm;
using Panacea.Modules.Advertisements;
using Panacea.Modules.Advertisements.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Panacea.Modules.Advertisements.ViewModels
{
    [View(typeof(AdNotification))]
    public class AdNotificationViewModel : ViewModelBase
    {
        public AdvertisementPresenterViewModel Presenter;
        public ICommand Close { get; private set; }
        public ICommand Click { get; private set; }
        private readonly PanaceaServices _core;
        public AdNotificationViewModel(AdvertisementPresenterViewModel presenter, PanaceaServices core)
        {
            _core = core;
            Presenter = presenter;
            Close = new RelayCommand(async (arg) =>
            {
                //TODO: Cannot refrain "THIS" ?!!
                //_core.GetUiManager().Refrain(this);
            });
            Click = new RelayCommand(async (arg) => {
                //_core.GetUiManager().Refrain(this);
            });
        }
    }
}
