using System.Collections.Generic;

namespace SystemTextJsonPatch;

public abstract class Shape
{
	public string ShapeProperty { get; set; }
}

public class Circle : Shape
{
	public string CircleProperty { get; set; }
}

public class Rectangle : Shape
{
	public string RectangleProperty { get; set; }
}

public class Square : Shape
{
	public Rectangle Rectangle { get; set; }
}

public class Canvas
{
	public IList<Shape> Items { get; set; }
}
