using System.IO;
using Xamarin.Forms;
using FaceLoginApp.Droid.Datos;
using FaceLoginApp.Datos;
using FaceLoginApp.Helpers;

[assembly: Dependency(typeof(BaseDatosAndroid))]
namespace FaceLoginApp.Droid.Datos
{
    public class BaseDatosAndroid : IBaseDatos
    {
        public string GetDatabasePath()
        {
            return Path.Combine(
                System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal), 
                    Constantes.NombreBD);
        }
    }
}