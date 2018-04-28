using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FaceLoginApp.Servicios;

namespace FaceLoginApp.Paginas
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaginaLogin : ContentPage
	{
		public PaginaLogin ()
		{
			InitializeComponent ();
		}

        void Loading(bool mostrar)
        {
            indicator.IsEnabled = mostrar;
            indicator.IsRunning = mostrar;
        }

        async void btnLogin_Clicked(object sender, EventArgs e)
        {
            try
            {
                Loading(true);
                var foto = await ServicioImagen.TomarFoto();

                if (foto != null)
                {
                    imagen.Source = ImageSource.FromStream(foto.GetStream);

                    var faceId = await ServicioFace.DetectarRostro(foto.GetStream());
                    var personId = await ServicioFace.IdentificarEmpleado(faceId);

                    if (personId != Guid.Empty)
                    {
                        var bd = new ServicioBaseDatos();
                        var usuario = await bd.ObtenerUsuario(personId.ToString());
                        usuario.FotoActual = foto.Path;

                        var emocion = await ServicioFace.ObtenerEmocion(foto);
                        usuario.EmocionActual = emocion.Nombre;
                        usuario.ScoreActual = emocion.Score;
                        var update = await bd.ActualizarUsuario(usuario);

                        await DisplayAlert("Correcto", $"Bienvenido {usuario.Nombre}", "OK");
                        await Navigation.PushAsync(new PaginaBienvenido(usuario));
                    }
                    else
                    {
                        await DisplayAlert("Error", "Persona no identificada", "OK");
                    }
                }
                else
                    await DisplayAlert("Error", "No se pudo tomar la fotografía.", "OK");

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
            await Navigation.PushAsync(new PaginaRegistro());
        }

        async void btnGrupo_Clicked(object sender, EventArgs e)
        {
            if (await ServicioFace.CrearGrupoEmpleados())
                await DisplayAlert("Correcto", "Grupo creado exitosamente", "OK");
            else
                await DisplayAlert("Error", "Error al crear el grupo", "OK");
        }

        async void btnEntrenar_Clicked(object sender, EventArgs e)
        {
            if (await ServicioFace.EntrenarGrupoEmpleados())
                await DisplayAlert("Correcto", "Grupo entrenado exitosamente", "OK");
            else
                await DisplayAlert("Error", "Error al crear el grupo", "OK");
        }
    }
}