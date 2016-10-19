using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
	public class SimpleQuadTreePolygon : SimpleQuadTreeElement {
		public Polygon Polygon { get; set; }
		public SimpleQuadTreePolygon(Polygon polygon) {
			this.Polygon = polygon;
			double minx = Double.MaxValue;
			double miny = Double.MaxValue;
			double maxx = Double.MinValue;
			double maxy = Double.MinValue;
			for (int i = 0; i < polygon.NumContours; ++i) {
				if (!polygon.IsHole(i)) {
					for (int j = 0; j < polygon[i].Count; ++j) {
						if (polygon[i][j].X < minx) minx = polygon[i][j].X;
						if (polygon[i][j].Y < miny) miny = polygon[i][j].Y;
						if (polygon[i][j].X > maxx) maxx = polygon[i][j].X;
						if (polygon[i][j].Y > maxy) maxy = polygon[i][j].Y;
					}
				}
			}
			this.Bounds = new DoubleRect(minx, miny, maxx, maxy);
		}
	}

	public class SimpleQuadTreeLinestrip : SimpleQuadTreeElement {
		public List<Vec2d> Linestrip { get; set; }
		public SimpleQuadTreeLinestrip(List<Vec2d> linestrip) {
			this.Linestrip = linestrip;
			double minx = Double.MaxValue;
			double miny = Double.MaxValue;
			double maxx = Double.MinValue;
			double maxy = Double.MinValue;
			foreach (Vec2d v in linestrip) {
				if (v.X < minx) minx = v.X;
				if (v.Y < miny) miny = v.Y;
				if (v.X > maxx) maxx = v.X;
				if (v.Y > maxy) maxy = v.Y;
			}
			this.Bounds = new DoubleRect(minx, miny, maxx, maxy);
		}
	}

	public class SimpleQuadTreeElement {
		public DoubleRect Bounds { get; set; }
	}

	public class DoubleRect {
		public double MinX { get; set; }
		public double MinY { get; set; }
		public double MaxX { get; set; }
		public double MaxY { get; set; }

		public DoubleRect(double MinX, double MinY, double MaxX, double MaxY) {
			this.MinX = MinX;
			this.MinY = MinY;
			this.MaxX = MaxX;
			this.MaxY = MaxY;
		}

		public double GetWidth() {
			return MaxX - MinX;
		}

		public double GetHeight() {
			return MaxY - MinY;
		}

		public bool Contains(DoubleRect other) {
			return (this.MinX <= other.MinX && this.MinY <= other.MinY &&
					this.MaxX >= other.MaxX && this.MaxY >= other.MaxY);
		}

		public bool Overlaps(DoubleRect other) {
			return !(this.MinX > other.MaxX || this.MinY > other.MaxY ||
					 this.MaxX < other.MinX || this.MaxY < other.MinY);
		}
	}

	public class SimpleQuadTree {
		static int MaxDepth = 10;

		SimpleQuadTree parent;
		SimpleQuadTree[] children = null;
		List<SimpleQuadTreeElement> elements = new List<SimpleQuadTreeElement>();
		DoubleRect[] childBounds;
		int depth;
		int elementCount = 0;

		public SimpleQuadTree(DoubleRect bounds) : this(bounds, 0) { }

		public SimpleQuadTree(DoubleRect bounds, int depth) : this(bounds, depth, null) { }

		public SimpleQuadTree(DoubleRect bounds, int depth, SimpleQuadTree parent) {
			this.depth = depth;
			this.parent = parent;
			double midx = (bounds.MaxX - bounds.MinX) / 2;
			double midy = (bounds.MaxY - bounds.MinY) / 2;
			childBounds = new DoubleRect[4];
			childBounds[0] = new DoubleRect(bounds.MinX, bounds.MinY, midx, midy); //NW
			childBounds[1] = new DoubleRect(midx, bounds.MinY, bounds.MaxX, midy); //NE
			childBounds[2] = new DoubleRect(bounds.MinX, midy, midx, bounds.MaxY); //SW
			childBounds[3] = new DoubleRect(midx, midy, bounds.MaxX, bounds.MaxY); //SE
		}

		public bool IsEmpty() {
			return elementCount == 0;
		}

		/*public SimpleQuadTreeElement PopElement() {
			SimpleQuadTreeElement result = GetElement();
			this.RemoveElement(result);
			return result;
		}*/

		SimpleQuadTreeElement GetElement() {
			SimpleQuadTreeElement result = null;
			if (this.elements.Count > 0) {
				result = elements[0];
			} else if (children != null) {
				for (int i = 0; i < 4; ++i) {
					if (!children[i].IsEmpty()) {
						return children[i].GetElement();
					}
				}
			}
			return result;
		}

		public void AddElement(SimpleQuadTreeElement element) {
			elementCount++;

			if (depth == SimpleQuadTree.MaxDepth) {
				elements.Add(element);
				return;
			}

			int child = -1;
			for (int i = 0; i < 4; ++i) {
				if (childBounds[i].Contains(element.Bounds)) {
					child = i;
				}
			}
			if (child != -1) {
				if (children == null) {
					children = new SimpleQuadTree[4];
					for (int i = 0; i < 4; ++i) {
						children[i] = new SimpleQuadTree(childBounds[i], depth + 1, this);
					}
				}
				children[child].AddElement(element);
			} else {
				elements.Add(element);
			}
		}

		public bool RemoveElement(SimpleQuadTreeElement element) {
			if (removeElement(element)) {
				elementCount--;
				if (children != null && elementCount == elements.Count) {
					children = null;
				}
				return true;
			} else {
				return false;
			}
		}

		bool removeElement(SimpleQuadTreeElement element) {
			int child = -1;
			if (children != null) {
				for (int i = 0; i < 4; ++i) {
					if (childBounds[i].Contains(element.Bounds)) {
						child = i;
					}
				}
			}

			if (child != -1) {
				return children[child].RemoveElement(element);
			} else {
				return this.elements.Remove(element);
			}
		}

		public List<SimpleQuadTreeElement> GetElementsInside(DoubleRect bounds) {
			List<SimpleQuadTreeElement> result = new List<SimpleQuadTreeElement>();
			if (children != null) {
				for (int i = 0; i < 4; ++i) {
					if (bounds.Contains(childBounds[i])) {
						result.AddRange(children[i].GetAllElements());
					} else if (bounds.Overlaps(childBounds[i])) {
						result.AddRange(children[i].GetElementsInside(bounds));
					}
				}
			}
			foreach (SimpleQuadTreeElement element in elements) {
				if (bounds.Contains(element.Bounds)) {
					result.Add(element);
				}
			}
			return result;
		}

		public List<SimpleQuadTreeElement> GetElements(DoubleRect bounds) {
			List<SimpleQuadTreeElement> result = new List<SimpleQuadTreeElement>();
			if (children != null) {
				for (int i = 0; i < 4; ++i) {
					if (bounds.Contains(childBounds[i])) {
						result.AddRange(children[i].GetAllElements());
					} else if (bounds.Overlaps(childBounds[i])) {
						result.AddRange(children[i].GetElements(bounds));
					}
				}
			}
			foreach (SimpleQuadTreeElement element in elements) {
				if (element.Bounds.Overlaps(bounds)) {
					result.Add(element);
				}
			}
			return result;
		}

		public List<SimpleQuadTreeElement> GetAllElements() {
			List<SimpleQuadTreeElement> result = new List<SimpleQuadTreeElement>();
			if (children != null) {
				for (int i = 0; i < 4; ++i) {
					result.AddRange(children[i].GetAllElements());
				}
			}
			result.AddRange(elements);
			return result;
		}
	}
}
