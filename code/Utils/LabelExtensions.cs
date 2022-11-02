namespace Tools;

public class Heading : Label
{
	public Heading( string title ) : base( title )
	{
		SetStyles( "font-size: 14px; font-family: Poppins; font-weight: 600; color: white;" );
	}
}

public class Subheading : Label
{
	public Subheading( string title ) : base( title )
	{
		SetStyles( "font-size: 12px; font-family: Poppins; font-weight: 600; color: white;" );
	}
}
