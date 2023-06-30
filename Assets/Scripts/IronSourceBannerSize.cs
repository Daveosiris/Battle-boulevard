public class IronSourceBannerSize
{
	private int width;

	private int height;

	private string description;

	public static IronSourceBannerSize BANNER = new IronSourceBannerSize("BANNER");

	public static IronSourceBannerSize LARGE = new IronSourceBannerSize("LARGE");

	public static IronSourceBannerSize RECTANGLE = new IronSourceBannerSize("RECTANGLE");

	public static IronSourceBannerSize SMART = new IronSourceBannerSize("SMART");

	public string Description => description;

	public int Width => width;

	public int Height => height;

	private IronSourceBannerSize()
	{
	}

	public IronSourceBannerSize(int width, int height)
	{
		this.width = width;
		this.height = height;
		description = "CUSTOM";
	}

	public IronSourceBannerSize(string description)
	{
		this.description = description;
		width = 0;
		height = 0;
	}
}
