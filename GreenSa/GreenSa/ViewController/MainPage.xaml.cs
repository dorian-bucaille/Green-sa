﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GreenSa.ViewController.Play;
using GreenSa.ViewController.Option;
using GreenSa.ViewController.MesGolfs;
using GreenSa.ViewController.Profile;
using GreenSa.Models.GolfModel;
using GreenSa.Models.ViewElements;
using SQLite;
using System.Collections.ObjectModel;
using GreenSa.Persistence;
using GreenSa.Models.Profiles;
using GreenSa.ViewController.Test;
using Xamarin.Forms.PlatformConfiguration;
using GreenSa.Models.Tools;
using GreenSa.ViewController.Help;

namespace GreenSa.ViewController
{
    public partial class MainPage : ContentPage
    {
        private SQLiteConnection DBconnection;

        public MainPage()
        {
            InitializeComponent();
            this.InitBDD();
            NavigationPage page = App.Current.MainPage as NavigationPage;
            IOSAdapter.SafeArea(this,"green");//Method needed to ensure correct display under iOS because no NavigationBar on this page
        }

        protected override void OnAppearing()
        {
            this.titre.Margin = new Thickness(0, 0, 0, 42);


        }

        /**
         * Initialized the database and adds a new profile if no one exists
         */
        public void InitBDD()
        {
            DBconnection = DependencyService.Get<ISQLiteDb>().GetConnection();
            DBconnection.CreateTable<Profil>();
            if (!DBconnection.Table<Profil>().Any())
            {
                AddLocalUser();
            }
        }

        /**
         * Adds a new profile in the database
         */
        public void AddLocalUser()
        {
            DBconnection.Insert(new Profil());
        }

        /**
         * Transforms a given value in pixel so that it's responsive with the screen size
         * pix : the value in pixel
         * return the converted value as an integer
         */
        public static int responsiveDesign(int pix)
        {
            return (int)((pix * 4.1 / 1440.0) * Xamarin.Forms.Application.Current.MainPage.Width);
        }


        /**
         * These methods are called when the corresponding button is pressed and redirects to a new page  
         */
        async private void OnPlayClicked(object sender, EventArgs e)
        {
            Partie partie = new Partie();
            await Navigation.PushAsync(new Play.GolfSelectionPage(partie));
        }


        async private void OnProfilClicked(object sender, EventArgs e)
        {
            //((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("3ab54a");

            await Navigation.PushAsync(new ProfilePage());
        }


        async private void OnGolfClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GolfsManager());
        }

        // Create a new option page when user clicks the options icon
        async private void OnOptionsClicked(object sender, EventArgs e){
            //await Navigation.PushAsync(new SeeBDContent()); //A special page to view the database's content, disabled for release
            await Navigation.PushAsync(new OptionPage());
        }

        // Create a new help page when user clicks the options icon
        async private void OnHelpClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HelpPage());
        }
    }
}
