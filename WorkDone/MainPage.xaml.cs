using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms.OpenWhatsApp;
using MarcTron.Plugin;
using MarcTron.Plugin.CustomEventArgs;

namespace WorkDone
{
    public partial class MainPage : ContentPage
    {
        private bool _shouldSetEvents = true;

        public class Result
        {
            public Query query { get; set; }
        }

        public class Query
        {
            public Dictionary<string, Page> pages { get; set; }
        }

        public class Page
        {
            public string extract { get; set; }
        }

        void SetEvents()
        {
            if (_shouldSetEvents)
            {
                _shouldSetEvents = false;
                CrossMTAdmob.Current.OnRewardedVideoStarted += Current_OnRewardedVideoStarted;
                CrossMTAdmob.Current.OnRewarded += Current_OnRewarded;
                CrossMTAdmob.Current.OnRewardedVideoAdClosed += Current_OnRewardedVideoAdClosed;
                CrossMTAdmob.Current.OnRewardedVideoAdFailedToLoad += Current_OnRewardedVideoAdFailedToLoad;
                CrossMTAdmob.Current.OnRewardedVideoAdLeftApplication += Current_OnRewardedVideoAdLeftApplication;
                CrossMTAdmob.Current.OnRewardedVideoAdLoaded += Current_OnRewardedVideoAdLoaded;
                CrossMTAdmob.Current.OnRewardedVideoAdOpened += Current_OnRewardedVideoAdOpened;
                CrossMTAdmob.Current.OnRewardedVideoAdCompleted += Current_OnRewardedVideoAdCompleted;
            }
        }
        private void DisableEvents()
        {
            _shouldSetEvents = true;
            CrossMTAdmob.Current.OnRewardedVideoStarted -= Current_OnRewardedVideoStarted;
            CrossMTAdmob.Current.OnRewarded -= Current_OnRewarded;
            CrossMTAdmob.Current.OnRewardedVideoAdClosed -= Current_OnRewardedVideoAdClosed;
            CrossMTAdmob.Current.OnRewardedVideoAdFailedToLoad -= Current_OnRewardedVideoAdFailedToLoad;
            CrossMTAdmob.Current.OnRewardedVideoAdLeftApplication -= Current_OnRewardedVideoAdLeftApplication;
            CrossMTAdmob.Current.OnRewardedVideoAdLoaded -= Current_OnRewardedVideoAdLoaded;
            CrossMTAdmob.Current.OnRewardedVideoAdOpened -= Current_OnRewardedVideoAdOpened;
        }

        private void Current_OnRewardedVideoAdOpened(object sender, EventArgs e)
        {
            //DisplayAlert("wow", "3", "ok");
        }

        private void Current_OnRewardedVideoAdLoaded(object sender, EventArgs e)
        {
            //DisplayAlert("wow", "Load ads", "ok");
        }

        private void Current_OnRewardedVideoAdLeftApplication(object sender, EventArgs e)
        {
            //DisplayAlert("wow", "5", "ok");
        }

        private void Current_OnRewardedVideoAdFailedToLoad(object sender, MTEventArgs e)
        {
            //DisplayAlert("wow", "6", "ok");
        }

        private void Current_OnRewardedVideoAdClosed(object sender, EventArgs e)
        {
            //DisplayAlert("wow", "7", "ok");
        }

        private async void Current_OnRewarded(object sender, MTEventArgs e)
        {
                DisableEvents();
                //await Navigation.PopToRootAsync();
        }

        private void Current_OnRewardedVideoStarted(object sender, EventArgs e)
        {
            //DisplayAlert("wow", "9", "ok");
        }

        private void Current_OnRewardedVideoAdCompleted(object sender, EventArgs e)
        {
            //DisplayAlert("wow","ok","ok");
        }

        public MainPage()
        {
            InitializeComponent();
            CrossMTAdmob.Current.LoadRewardedVideo("ca-app-pub-5325371552579828/2759809299");//original
            //CrossMTAdmob.Current.LoadRewardedVideo("ca-app-pub-3940256099942544/5224354917");//for test
        }

        private void trabalho_TextChanged(object sender, TextChangedEventArgs e)
        {
            status.Text = "Pesquisando...";
        }

        private async void btnDoMyWork_Clicked(object sender, EventArgs e)
        {
            CrossMTAdmob.Current.ShowRewardedVideo();
            try
            {
                WebClient client = new WebClient();
                using (Stream stream = client.OpenRead("https://pt.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&explaintext=1&titles=" + trabalho.Text.Replace(" ", "%20").Replace("  ", "%20")))
                using (StreamReader reader = new StreamReader(stream))
                {
                    JsonSerializer ser = new JsonSerializer();
                    Result result = ser.Deserialize<Result>(new JsonTextReader(reader));

                    foreach (Page page in result.query.pages.Values)
                    txtHomeWork.Text = page.extract;
                    status.Text = "Pesquisa encontrada!";
                }

                string action = await DisplayActionSheet("Vamos enviar o conteúdo do trabalho!", null, null, "Email", "WhatsApp");
                if(action=="Email")
                {
                    string em = nome.Text;
                    await Launcher.TryOpenAsync(new Uri("mailto:"+em+ "?subject=WorkDone&body="+txtHomeWork.Text));
                }
                else
                {
                    try
                    {
                        Chat.Open("+55"+escola.Text, txtHomeWork.Text);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Erro", ex.Message, "OK");
                    }
                }

            }
            catch
            {
                status.Text = "Pesquisa não encontrada.";
            }

        }
    }
    }

