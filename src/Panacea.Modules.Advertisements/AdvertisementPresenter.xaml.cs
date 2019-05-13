using Panacea.Core.Mvvm;
using Panacea.Modules.Advertisements.Models;
using Panacea.Modules.Advertisements.ViewModels;

namespace Panacea.Modules.Advertisements
{
    /// <summary>
    /// Interaction logic for AdvertisementPresenter.xaml
    /// </summary>
    public partial class AdvertisementPresenter : AdvertisementPresenterUserControl
    {
        public AdvertisementPresenter(AdvertisementEntry ad)
        {
            ViewModel.Ad = ad;
            InitializeComponent();
        }
        public AdvertisementPresenter(AdvertisementEntry ad, AdvertisementPresenterViewModel viewModel) : base(viewModel)
        {
            ViewModel.Ad = ad;
            InitializeComponent();
        }
    }

    public abstract class AdvertisementPresenterUserControl : UserControlWithViewModel<AdvertisementPresenterViewModel>
    {
        public AdvertisementPresenterUserControl()
        {

        }
        public AdvertisementPresenterUserControl(AdvertisementPresenterViewModel viewModel) : base(viewModel)
        {

        }
    }
}