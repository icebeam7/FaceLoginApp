using Xamarin.Forms;

namespace FaceLoginApp
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

			MainPage = new NavigationPage(new Paginas.PaginaLogin());
		}
	}
}
