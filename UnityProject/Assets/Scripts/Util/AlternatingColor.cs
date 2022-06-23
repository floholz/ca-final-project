using UnityEngine;

public class AlternatingColor
{
	private Color primeColor;
	private Color secondaryColor;
	private bool select = true;

	public AlternatingColor(Color color1, Color color2)
	{
		primeColor = color1;
		secondaryColor = color2;
	}

	public Color next()
	{
		Color color = select ? primeColor : secondaryColor;
		select = !select;
		return color;
	}
}
