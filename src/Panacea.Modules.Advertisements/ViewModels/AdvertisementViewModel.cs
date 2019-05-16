using Panacea.Core;
using Panacea.Mvvm;
using Panacea.Modules.Advertisements.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
namespace Panacea.Modules.Advertisements.ViewModels
{
    [View(typeof(Advertisement))]
    public class AdvertisementViewModel : ViewModelBase
    {
        private readonly ISerializer _serializer;
        PanaceaServices _core;

        DispatcherTimer bannerTimer, _refreshTimer;
        System.Timers.Timer _impressionsTimer = new System.Timers.Timer();
        List<AdvertisementEntry> _advertisements;
        Dictionary<string, int> _impressions = new Dictionary<string, int>();
        Dictionary<string, int> _clicks = new Dictionary<string, int>();
        List<AdvertisementEntry> _splash, _banners, _notifications;
        CancellationTokenSource _notificationCancellation, _splashCancellation;
        string _currentPlugin;

        AdvertisementPresenterViewModel _adPresenter;
        public AdvertisementPresenterViewModel AdPresenter
        {
            get => _adPresenter;
            protected set
            {
                _adPresenter = value;
                OnPropertyChanged();
            }
        }

        public PanaceaServices Core
        {
            get => _core;
        }

        public AdvertisementViewModel(PanaceaServices core)
        {
            _core = core;
            bannerTimer = new DispatcherTimer();
            bannerTimer.Tick += BannerTimer_Elapsed;
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Tick += _refreshTimer_Tick;
            _refreshTimer.Interval = TimeSpan.FromMinutes(new Random().Next(50, 70));
            _impressionsTimer.Interval = TimeSpan.FromMinutes(new Random().Next(10, 20)).TotalMilliseconds;
            _impressionsTimer.Elapsed += _storeImpressions;
            _impressionsTimer.Start();
            InvalidateAdvertisements().ContinueWith((task) =>
            {

            }, TaskContinuationOptions.AttachedToParent);
            //_core.UserService.UserLoggedIn += async (oo) => { await InvalidateAdvertisements(); };
            //_core.UserService.UserLoggedOut += async (oo) => { await InvalidateAdvertisements(); };
            //TODO: _core.GetUiManager().PluginCalled -> _pluginManager.PluginCalled += Host_PluginCalled;
        }

        private void Host_PluginCalled(object sender, string e)
        {
            _currentPlugin = e;
            if (_splash == null || _notifications == null) return;
            var adToShow = _splash.FirstOrDefault(s => s.Plugins?.Any(p => p == e) == true);
            if (adToShow != null)
            {
                //todo
                //var control = new AdvertisementPresenter(adToShow, new AdvertisementPresenterViewModel(_core));
                //_splash.Remove(adToShow);
                //_splash.Add(adToShow);
                //ShowSplash(adToShow);
            }
            else
            {
                adToShow = _notifications.FirstOrDefault(s => s.Plugins?.Any(p => p == e) == true);
                if (adToShow == null) return;
                ShowNotification(adToShow);
            }

        }

        private async Task InvalidateAdvertisements()
        {
            _refreshTimer.Stop();
            _notificationCancellation?.Cancel();
            _notificationCancellation = new CancellationTokenSource();
            _splashCancellation?.Cancel();
            _splashCancellation = new CancellationTokenSource();
            bannerTimer.Stop();
            await GetAdvertisementsFromServer();
            SetupAdvertisements();
            _refreshTimer.Start();
        }

        private async Task GetAdvertisementsFromServer()
        {
            try
            {
                //TODO: make url into a method
                var response = await _core.HttpClient.GetObjectAsync<AdResponse>("ad/get_adcategories/");
                if (!response.Success) return;
                var cats = response.Result.Advertisements.AdCategories;
                _advertisements = new List<AdvertisementEntry>();
                foreach (var cat in cats)
                {
                    foreach (var bi in cat.Banners)
                    {
                        _advertisements.Add(bi.Banner);
                    }
                }
            }
            catch { }
        }

        void SetupAdvertisements()
        {
            if (_advertisements == null) return;
            _advertisements = _advertisements.Where(ad =>
            {
                try
                {
                    return ad.Type == AdType.Rotating || ad.DisplayPlans == null || ad.DisplayPlans.Count == 0 || ad.DisplayPlans?.Any(dp =>
                    (dp.Day.ToLower() == "all"
                        || dp.Day.ToLower() == DateTime.Now.ToString("dddd", CultureInfo.InvariantCulture).ToLower()) == true
                    && ad.StartDate.Date <= DateTime.Now.Date
                    && (ad.ExpirationDate.Date.Year == 1 || ad.ExpirationDate.Date >= DateTime.Now.Date)) == true;
                }
                catch { }
                return false;
            })
            .GroupBy(ad => ad.Id)
            .Select(ad => ad.First())
            .ToList();

            _banners = _advertisements.Where(ad => ad.Type == AdType.Rotating).ToList();
            SetupBanners();
            _notifications = _advertisements.Where(ad => ad.Type == AdType.Notification).ToList();
            _splash = _advertisements.Where(ad => ad.Type == AdType.Splash).ToList();
            PlanNextNotification();
            PlanNextSplash();
        }

        async void PlanNextSplash()
        {
            var source = _splashCancellation;
            var nextNotification = GetNextAdvertisement(_splash, out int seconds);
            if (nextNotification == null) return;

            await Task.Delay((seconds * 1000));
            if (source.IsCancellationRequested) return;
            ShowSplash(nextNotification);
            PlanNextSplash();
        }

        async void ShowSplash(AdvertisementEntry ad)
        {
            if (ad == null) return;
            var control = GetAdvertisementPresenter(ad);
            //todo
            //var pop = _core.GetUiManager().ShowPopup(control);
            //await Task.Delay(ad.DurationOnScreen * 1000);
            //_core.GetUiManager().HidePopup(pop);
        }

        async void PlanNextNotification()
        {
            var source = _notificationCancellation;
            var nextNotification = GetNextAdvertisement(_notifications, out int seconds);
            if (nextNotification == null) return;
            await Task.Delay((seconds * 1000));
            if (source.IsCancellationRequested) return;
            ShowNotification(nextNotification);
            PlanNextNotification();
        }

        async void ShowNotification(AdvertisementEntry ad)
        {
            if (ad == null) return;
            var control = GetAdvertisementPresenter(ad);
            //todo
            //var notification = new AdNotification(control, _core);
            //_core.GetUiManager().Notify(notification);
            //await Task.Delay(ad.DurationOnScreen * 1000);
            //_core.GetUiManager().Refrain(notification);
        }

        AdvertisementEntry GetNextAdvertisement(IEnumerable<AdvertisementEntry> ads, out int seconds)
        {
            AdvertisementEntry nextNotification = null;
            seconds = 0;
            var now = DateTime.Now;
            var diff = TimeSpan.FromDays(1);
            foreach (var ad in ads)
            {
                foreach (var dp in ad.DisplayPlans)
                {
                    var time = int.Parse(dp.Hour);
                    var timeMinute = int.Parse(dp.Minute);
                    var date = new DateTime(now.Year, now.Month, now.Day, time, timeMinute, 0);
                    if (date > now && now.AddHours(1) > date && date.Subtract(now) < diff)
                    {
                        nextNotification = ad;
                        diff = date.Subtract(now);
                        seconds = (int)diff.TotalSeconds + 1;
                    }

                }
            }
            return nextNotification;
        }

        private void SetupBanners()
        {
            bannerTimer.Stop();
            if (_banners.Count == 0)
            {
                return;
            }
            currentBannerIndex = -1;
            var intervalSeconds = 60 * 60 / _banners.Count;
            if (intervalSeconds < 10) intervalSeconds = 10;
            if (intervalSeconds > 60) intervalSeconds = 60;
            bannerTimer.Interval = TimeSpan.FromSeconds(intervalSeconds);
            bannerTimer.Start();
            ShowNextBanner();

        }

        private void BannerTimer_Elapsed(object sender, EventArgs e)
        {
            ShowNextBanner();
        }

        int currentBannerIndex = -1;
        private void ShowNextBanner()
        {
            if (_banners == null || _banners.Count == 0) return;
            currentBannerIndex++;
            if (currentBannerIndex >= _banners.Count)
            {
                currentBannerIndex = 0;
            }
            if (_banners[currentBannerIndex].DurationOnScreen <= 0) return;
            bannerTimer.Interval = TimeSpan.FromSeconds(_banners[currentBannerIndex].DurationOnScreen);
            AdPresenter = GetAdvertisementPresenter(_banners[currentBannerIndex]);
        }

        public AdvertisementPresenterViewModel GetAdvertisementPresenter(AdvertisementEntry ad)
        {
            var ap = new AdvertisementPresenterViewModel(_core) { Ad = ad };
            if (!_impressions.ContainsKey(ad.Id))
            {
                _impressions[ad.Id] = 1;
            }
            _impressions[ad.Id]++;
            return ap;
        }

        private async void _storeImpressions(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_impressions.Keys.Count == 0) return;
                //TODO: URL SHOULD BE A METHOD
                var res = await _core.HttpClient.GetObjectAsync<object>("ad/storeImpressions/", new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("impressions", _serializer.Serialize(_impressions))
                });
                if (res.Success)
                {
                    _impressions.Clear();
                }
            }
            catch { }
        }

        private async void _refreshTimer_Tick(object sender, EventArgs e)
        {
            await InvalidateAdvertisements();
        }
    }
}
