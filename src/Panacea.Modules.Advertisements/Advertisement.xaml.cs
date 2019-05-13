using Panacea.Core.Mvvm;
using Panacea.Modules.Advertisements.ViewModels;

namespace Panacea.Modules.Advertisements
{
    /// <summary>
    /// Interaction logic for Advertisement.xaml
    /// </summary>
    public partial class Advertisement : AdvertisementUserControl {
        public Advertisement()
        {
            InitializeComponent();
        }
        public Advertisement(AdvertisementViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
    public class AdvertisementUserControl : UserControlWithViewModel<AdvertisementViewModel>
    {
        public AdvertisementUserControl()
        {

        }
        public AdvertisementUserControl(AdvertisementViewModel viewModel) : base(viewModel)
        {
            
        }
    }

}
