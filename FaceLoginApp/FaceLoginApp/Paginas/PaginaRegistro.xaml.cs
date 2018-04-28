using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media.Abstractions;
using FaceLoginApp.Modelos;
using FaceLoginApp.Servicios;

namespace FaceLoginApp.Paginas
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaginaRegistro : ContentPage
	{
        MediaFile foto1, foto2, foto3;

        public PaginaRegistro()
        {
            InitializeComponent();
        }

        void Loading(bool mostrar)
        {
            indicator.IsEnabled = mostrar;
            indicator.IsRunning = mostrar;
        }

        async void btnTomarFoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                Loading(true);

                if (foto1 == null)
                {
                    foto1 = await ServicioImagen.TomarFoto();
                    if (foto1 != null)
                        imagen1.Source = ImageSource.FromStream(foto1.GetStream);
                }
                else if (foto2 == null)
                {
                    foto2 = await ServicioImagen.TomarFoto();
                    if (foto2 != null)
                        imagen2.Source = ImageSource.FromStream(foto2.GetStream);
                }
                else
                {
                    foto3 = await ServicioImagen.TomarFoto();
                    if (foto3 != null)
                        imagen3.Source = ImageSource.FromStream(foto3.GetStream);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Loading(false);
            }
        }

        async void btnRegistrar_Clicked(object sender, EventArgs e)
        {
            if (foto1 != null && foto2 != null && foto3 != null)
            {
                bool op = false;

                try
                {
                    Loading(true);
                    var nombre = txtNombre.Text;
                    var personID = await ServicioFace.RegistrarPersonaEnGrupo(nombre);

                    await ServicioFace.RegistrarRostro(personID, foto1.GetStream());
                    await ServicioFace.RegistrarRostro(personID, foto2.GetStream());
                    await ServicioFace.RegistrarRostro(personID, foto3.GetStream());

                    var usuario = new Usuario()
                    {
                        Key = personID.ToString(),
                        Nombre = nombre,
                        EmocionActual = "",
                        FotoActual = "",
                        ScoreActual = 0
                    };

                    op = await new ServicioBaseDatos().RegistrarUsuario(usuario);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (op)
                    {
                        await DisplayAlert("Correcto", "Empleado registrado correctamente", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                        await DisplayAlert("Error", "Error al registrar el empleado", "OK");

                    Loading(false);
                }
            }
            else
                await DisplayAlert("Error", "Debes tomar 3 fotografías al empleado", "OK");
        }
    }
}