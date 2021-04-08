using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;
using System.Collections.Generic;
using System.Text;
using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using GreenSa.Persistence;
using System.Reflection;
using SQLite;
using GreenSa.Models.Exceptions;
using System.Globalization;

// Class used to import golf courses

namespace GreenSa.ViewController.Option
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportGolfCourse : ContentPage
    {

        // List of the current pins placed on the map
        readonly List<Pin> pins;

        public ImportGolfCourse()
        {
            InitializeComponent();
            this.pins = new List<Pin>();

            // Init some content on the view
            this.validPar.BorderColor = Color.FromHex("0C5E11");
            this.validPar.BorderWidth = 2;
            this.deletePin.BorderColor = Color.FromHex("0C5E11");
            this.deletePin.BorderWidth = 2;
            this.deleteAllPins.BorderColor = Color.FromHex("0C5E11");
            this.deleteAllPins.BorderWidth = 2;
            this.localizeCity.BorderColor = Color.FromHex("0C5E11");
            this.localizeCity.BorderWidth = 2;
            this.SetParVisibility(false);
            this.SetCourseNameVisibility(false);
            map.update();

            // Suscribe to get a notification about the pin that was added on the map by the user
            MessagingCenter.Subscribe<Pin>(this, "getAddGolfMapPins", (pin) =>
            {
                this.pins.Add(pin);
                this.SetParVisibility(true);
                this.SetCourseNameVisibility(false);
            });
        }


        // Localize the golf based on address when the localize button is pressed
        private async void OnLocalizeClick(object sender, EventArgs e)
        {
            if ("".Equals(cityEntry.Text))
            {
                await this.DisplayAlert("Erreur", "Le champ spécifiant le nom du golf ne peut pas être vide", "Ok");
            }
            else
            {
                var address = cityEntry.Text;
                var locations = await Geocoding.GetLocationsAsync(address);
                var location = locations?.FirstOrDefault();
                if (location != null)
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMiles(0.12)));
                }
            }
        }


        // Create a new golf course when the create button is pressed
        private async void OnCreateCourseClick(object sender, EventArgs e)
        {

            if (this.pins.Count == 9 || this.pins.Count == 18)
            {
                var confirmDelete = await this.DisplayAlert("Création du parcours", "Voulez-vous créer ce parcours ?", "Oui", "Non");
                if (confirmDelete)
                {
                    try
                    {
                        String xmlGolfCourse = this.CreateXmlGolfCourse();
                        this.InsertGolfCourseBdd(xmlGolfCourse);
                    }
                    catch (EmptyStringException)
                    {
                        await this.DisplayAlert("Erreur", "Le nom du golf doit être renseigné", "Ok");
                    }

                }
            }
            else
            {
                await this.DisplayAlert("Erreur", "Un parcours doit contenir au moins un trou", "Ok");
            }
        }


        // Delete the last placed hole pin
        private async void OnDeletePinClick(object sender, EventArgs e)
        {
            if (this.pins.Any())
            {
                var confirmDelete = await this.DisplayAlert("Suppression du dernier trou", "Voulez-vous supprimer le trou n°" + this.pins.Count + " ?", "Oui", "Non");
                if (confirmDelete)
                {
                    this.pins.RemoveAt(this.pins.Count - 1);//remove in the common list
                    MessagingCenter.Send<Object>(this, "deleteLastPin");
                    PinsCourseNameManagement();
                    this.SetParVisibility(false);
                }
            }
            else
            {
                await this.DisplayAlert("Erreur de suppression", "Aucun trou à supprimer", "Ok");
            }
        }


        // Delete every single placed hole pin
        private async void OnDeleteAllPinsClick(object sender, EventArgs e)
        {
            if (this.pins.Any())
            {
                var confirmDelete = await this.DisplayAlert("Suppression de tous les trous", "Voulez-vous supprimer tous les trous ?", "Oui", "Non");
                if (confirmDelete)
                {
                    this.ManageAllPinsDelete();
                }
            }
            else
            {
                await this.DisplayAlert("Erreur de suppression", "Aucun trou à supprimer", "Ok");
            }
        }

        // Validate hole addition to golf course
        private void OnValidParClick(object sender, EventArgs e)
        {
            if ("".Equals(this.golfParEntry.Text))
            {
                this.DisplayAlert("Erreur", "Le champ spécifiant le par du trou ne peut pas être vide", "Ok");
            }
            else
            {
                int par = Convert.ToInt32(this.golfParEntry.Text);

                this.pins[this.pins.Count - 1].MarkerId = par;  // Given that the pin MarkerId isn't used, let's use it to store the par

                try
                {
                    MessagingCenter.Send<Object>(par, "validPar");
                }
                catch (TargetInvocationException exception)
                {
                    this.DisplayAlert("OnValidParClick", exception.InnerException.StackTrace, "Ok");
                }
                this.SetParVisibility(false);
                PinsCourseNameManagement();
            }
        }


        // Return the XML string describing the golf course which data is specified in this pane by the user
        private String CreateXmlGolfCourse()
        {
            if ("".Equals(this.cityEntry.Text))
            {
                throw new EmptyStringException("Le champ spécifiant le nom du golf ne peut pas être vide");
            }
            else if ("".Equals(this.golfNameEntry.Text))
            {
                throw new EmptyStringException("Le champ spécifiant le nom de ce parcours ne peut pas être vide");
            }
            else
            {
                // See Ressources/GolfCourses for a more understandable pattern 
                StringBuilder xmlGolfCourse = new StringBuilder("<GolfCourse>");
                xmlGolfCourse.Append("<Name>" + this.golfNameEntry.Text + "</Name>");
                xmlGolfCourse.Append("<NbTrous>" + this.pins.Count + "</NbTrous>");
                xmlGolfCourse.Append("<NomGolf>" + this.cityEntry.Text + "</NomGolf>");
                xmlGolfCourse.Append("<Coordinates>");

                CultureInfo format = new CultureInfo("en-US");  // Used to write latitude and longitude with '.' as decimal separator (default is ',' which is not SQL-friendly)

                foreach (Pin hole in this.pins)
                {
                    xmlGolfCourse.Append("<Trou>");
                    xmlGolfCourse.Append("<par>" + hole.MarkerId + "</par>");  // hole.MarkerId is used to store the hole's par
                    xmlGolfCourse.Append("<lat>" + hole.Position.Latitude.ToString("G", format) + "</lat>");
                    xmlGolfCourse.Append("<lng>" + hole.Position.Longitude.ToString("G", format) + "</lng>");
                    xmlGolfCourse.Append("</Trou>");
                }
                xmlGolfCourse.Append("</Coordinates>");
                xmlGolfCourse.Append("</GolfCourse>");
                return xmlGolfCourse.ToString();
            }
        }


        // Insert a golf course in the database from its XML string
        public void InsertGolfCourseBdd(String xmlGolfCourse)
        {
            GolfCourse gc;
            try
            {
                gc = GolfXMLReader.getSingleGolfCourseFromText(xmlGolfCourse);
                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                try
                {
                    connection.CreateTable<Hole>();
                    connection.CreateTable<MyPosition>();
                    connection.CreateTable<GolfCourse>();
                    connection.BeginTransaction();
                    SQLiteNetExtensions.Extensions.WriteOperations.InsertWithChildren(connection, gc, true);
                    connection.Commit();
                    this.DisplayAlert("Succès", "Le " + this.pins.Count + " trous : " + golfNameEntry.Text + " a été créé avec succès", "Continuer");
                    this.ManageAllPinsDelete();
                }
                catch (SQLiteException bddException)
                {
                    this.DisplayAlert("Erreur avec la base de donnée", bddException.Source + " : Ce nom de golf existe déjà ou une autre erreur inattendu s'est produite", "Ok");
                    connection.Rollback();
                }
            }
            catch (Exception xmlConversionException)
            {
                this.DisplayAlert("Erreur lors de la conversion XML -> GolfCourse", xmlConversionException.StackTrace, "Ok");
            }
        }


        // Manage the deletion of all the pins currently placed on the map
        private void ManageAllPinsDelete()
        {
            this.pins.Clear();
            MessagingCenter.Send<Object>(this, "deleteAllPins");  // Dend a message to delete all pins from the map
            this.SetCourseNameVisibility(false);
            this.SetParVisibility(false);
        }


        // Set the visibility to the golf course name input area
        private void SetCourseNameVisibility(Boolean isVisible)
        {
            golfNameLabel.IsVisible = isVisible;
            golfNameEntry.IsVisible = isVisible;
        }


        // Set the visibility to the par validation input area
        private void SetParVisibility(Boolean isVisible)
        {
            golfParLabel.Text = "Par du trou n°" + this.pins.Count + " :";
            golfParEntry.IsVisible = isVisible;
            golfParLabel.IsVisible = isVisible;
            validPar.IsVisible = isVisible;
        }


        // Set visible the golf course name input area if 9 or 18 pins are placed on the map, not visible otherwise
        private void PinsCourseNameManagement()
        {
            if (this.pins.Count == 9)
            {
                this.golfNameEntry.Text = "9 trous de ";
                this.SetCourseNameVisibility(true);
            }
            else if (this.pins.Count == 18)
            {
                this.SetCourseNameVisibility(true);
                this.golfNameEntry.Text = "18 trous de ";
            }
            else
            {
                this.SetCourseNameVisibility(false);
            }
        }
    }
}