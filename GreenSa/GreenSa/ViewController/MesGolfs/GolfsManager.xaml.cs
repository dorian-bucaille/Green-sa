﻿using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using GreenSa.Models.ViewElements;
using GreenSa.Persistence;
using GreenSa.ViewController.Option;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using SQLite;

// This classed is used to import, delete and manage golf courses

namespace GreenSa.ViewController.MesGolfs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GolfsManager : ContentPage
    {

        public GolfCourseListViewModel gclvm;

        public GolfsManager()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            List<GolfCourse> res = await GestionGolfs.getListGolfsAsync(null);
            // Update the list of golf courses
            gclvm = new GolfCourseListViewModel(res);
            BindingContext = gclvm;
        }


        // This method is called when the button to add a new golf course is clicked
        async private void OnAddGolfClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ImportGolfCourse());
            }
            catch (TargetInvocationException exception)
            {
                Debug.WriteLine(exception.StackTrace);
            }
        }

        // Share a golf course when it is clicked
        async private void OnGolfTapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            var tgr = image.GestureRecognizers[0] as TapGestureRecognizer;

            // For each line, the golf course name is stored in the ball image CommandParameter attribute to be able to identify an image to its golf course
            var name = tgr.CommandParameter.ToString();
            var confirmShare = await DisplayAlert("Partager", "Voulez-vous partager ce golf à vos amis ?", "Oui", "Non");
            if (confirmShare)
            {
                GolfCourse file = null;
                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                try
                {
                    // Remove golf course from database
                    connection.BeginTransaction();
                    file = connection.Get<GolfCourse>(name);
                    connection.Commit();
                }
                catch (Exception bddException)
                {
                    await this.DisplayAlert("Erreur avec la base de données", bddException.StackTrace, "Ok");
                    connection.Rollback();
                }

                string res = file.xmlFile;

                if (file != null)
                    // Display sharing methods
                    await Share.RequestAsync(new ShareTextRequest
                    {
                        Text = res,
                        Title = name
                    });
            }
        }

        async private void ImportGolf_Clicked(object sender, EventArgs e)
        {
            var import = await DisplayAlert("Importation", "Le texte du golf que vous avez reçu doit être dans votre presse-papier. " +
                "Voulez-vous continuez ?", "Oui", "Non");
            if (import)
            {
                string text = await Clipboard.GetTextAsync();
                if (string.Compare(text, "") == 0)
                {
                    await DisplayAlert("Echec", "Votre presse-papier est vide !", "Ok");
                }
                else
                {
                    if (string.Compare(text, 0, "<GolfCourse>", 0, 11) == 0)
                    {
                        GolfCourse gc;
                        try
                        {
                            gc = GolfXMLReader.getSingleGolfCourseFromText(text);
                            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                            try
                            {
                                connection.CreateTable<Hole>();
                                connection.CreateTable<MyPosition>();
                                connection.CreateTable<GolfCourse>();
                                connection.BeginTransaction();
                                connection.Insert(gc);
                                connection.Commit();
                                await this.DisplayAlert("Succès", "Le " + gc.Name + " au " + gc.NameGolf + " a bien été importer ! ", "Continuer");
                                this.OnAppearing();
                            }
                            catch (SQLiteException bddException)
                            {
                                await this.DisplayAlert("Erreur avec la base de donnée", bddException.Source + " : Ce nom de golf existe déjà ou une autre erreur inattendue s'est produite", "Ok");
                                connection.Rollback();
                            }
                        }
                        catch (Exception xmlConversionException)
                        {
                            await this.DisplayAlert("Erreur lors de la conversion XML -> GolfCourse", xmlConversionException.StackTrace, "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Echec", "Le contenu de votre presse-papier ne corresond pas à celui attendu pour importer un golf !", "Ok");
                    }
                }

            }
        }

        // Delete a golf course from the ListView and from the database
        private async void DeleteGolfCourse(object sender, EventArgs e)
        {
            var image = sender as Image;
            var tgr = image.GestureRecognizers[0] as TapGestureRecognizer;
            //for each line, the golf course name is stored in the cross image CommandParameter attribute to be able to identify an image to its golf course
            var name = tgr.CommandParameter.ToString();
            var confirmDelete = await this.DisplayAlert("Suppression d'un golf", "Voulez vous vraiment supprimer le golf : " + name + " ?", "Oui", "Non");
            if (confirmDelete)
            {
                //remove golf course cell from ListView
                var toDelete = image.BindingContext as GolfCourse;
                var vm = BindingContext as GolfCourseListViewModel;
                vm.RemoveGolfCourse.Execute(toDelete);

                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                try
                {
                    //remove golf course from database
                    connection.BeginTransaction();
                    connection.Delete<GolfCourse>(name);
                    connection.Commit();
                }
                catch (Exception bddException)
                {
                    await this.DisplayAlert("Erreur avec la base de donnée", bddException.StackTrace, "Ok");
                    connection.Rollback();
                }
            }
        }

    }
}