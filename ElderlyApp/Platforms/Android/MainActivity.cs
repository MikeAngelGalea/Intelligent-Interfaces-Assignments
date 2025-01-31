using Android.App;
using Android.Content.PM;
using Android.OS;
using Android;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace ElderlyApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Check and request calendar write permissions
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteCalendar) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteCalendar }, 0);
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadCalendar) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadCalendar }, 1);
            }
        }
    }
}
