using Tools;

public class Container : Widget
{
	public Container( Widget parent ) : base( parent )
	{
		this.SetStylesheet( "/styles/container.css" );
	}
}
