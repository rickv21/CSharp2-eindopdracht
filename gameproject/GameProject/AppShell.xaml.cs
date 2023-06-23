namespace GameProject;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("ConnectFour", typeof(ConnectFour));
	}
}
