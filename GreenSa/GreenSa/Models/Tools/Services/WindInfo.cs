using Xamarin.Forms;

namespace GreenSa.Models.Tools.Services
{
    public class WindInfo
    {
        public double strength;  // Wind force
        public double direction;  // Wind direction
        public ImageSource icon;  // Image associated with wind direction

        public WindInfo(double strength, double direction, ImageSource icon)
        {
            this.strength = strength;
            this.direction = direction;
            this.icon = icon;
        }
    }
}
