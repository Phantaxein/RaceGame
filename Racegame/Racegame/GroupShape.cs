using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Racegame
{
    public class GroupShape
    {
        public Vector2f Size {  get; private set; }
        private Vector2f pos;

        List<Vector2f> shapePoints = new List<Vector2f>();
        public Vector2f Position { get { return pos; }
            set
            {
                Vector2f change = value - pos;
                shapes.ForEach(x => x.Position += change);
                shapePoints.ForEach(x => x += change);
                pos = value;
            }
        }

        public static implicit operator ConvexShape(GroupShape shape)
        {
            ConvexShape myShape = new ConvexShape(4);
            myShape.Position = shape.Position;
            myShape.Rotation = shape.rotation;
            myShape.SetPoint(0, new Vector2f(0, 0));
            myShape.SetPoint(1, new Vector2f(shape.Size.X, 0));
            myShape.SetPoint(2, new Vector2f(shape.Size.X, shape.Size.Y));
            myShape.SetPoint(3, new Vector2f(0, shape.Size.Y));
            return myShape;
        }


        private float rotation = 0;
        public float Rotation { get { return rotation; }
            set
            {
                float centerDist = Trig.GetPythag(Position, Position + .5F * Size);
                float centAngle = Trig.FindAngleToTarget(Position, Position + .5F * Size);
                Vector2f oldCenter = Position + new Vector2f((float)(Math.Cos(Trig.Rad(Rotation + centAngle)) * centerDist), (float)(Math.Sin(Trig.Rad(Rotation + centAngle)) * centerDist));
                float rotDif = value - rotation;
                rotation = value;
                rotation %= 360;
                Vector2f newCenter = Position + new Vector2f((float)(Math.Cos(Trig.Rad(Rotation + centAngle)) * centerDist), (float)(Math.Sin(Trig.Rad(Rotation + centAngle)) * centerDist));
                Position += oldCenter - newCenter;
                foreach (ConvexShape shape in shapes)
                { 
                    float dist = Trig.GetPythag(shape.Position, Position);
                    float ang1 = Trig.FindAngleToTarget(Position, shape.Position);
                    shape.Position = Position + new Vector2f((float)(Math.Cos(Trig.Rad(rotDif + ang1)) * dist), (float)(Math.Sin(Trig.Rad(rotDif + ang1)) * dist));
                    shape.Rotation = rotation;
                }

                shapePoints.Clear();
                shapePoints.Add(Position);
                shapePoints.Add(Position + new Vector2f((float)(Math.Cos(Trig.Rad(Rotation)) * Size.X), (float)(Math.Sin(Trig.Rad(Rotation)) * Size.X)));
                shapePoints.Add(Position + new Vector2f((float)(Math.Cos(Trig.Rad((Rotation + 90F) % 360F)) * Size.Y), (float)(Math.Sin(Trig.Rad((Rotation + 90F) % 360F)) * Size.Y)));
                shapePoints.Add(shapePoints[1] + shapePoints[2] - Position); 
            } 
        }

        public List<ConvexShape> shapes { get; private set; }

        public GroupShape(List<ConvexShape> shapeList)
        {
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;
            foreach (ConvexShape shape in shapeList)
            {
                for(uint i = 0; i < shape.GetPointCount(); i++)
                {
                    Vector2f point = shape.GetPoint(i);
                    if (point.X + shape.Position.X > maxX) { maxX = point.X + shape.Position.X; }
                    if (point.Y + shape.Position.Y > maxY) { maxY = point.Y + shape.Position.Y; }
                }
                Size = new Vector2f(maxX, maxY);
            }
            shapes = shapeList;
            Position = new Vector2f(0, 0);

        }
        public GroupShape(List<ConvexShape> shapeList, Vector2f position)
        {
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;
            foreach (ConvexShape shape in shapeList)
            {
                for (uint i = 0; i < shape.GetPointCount(); i++)
                {
                    Vector2f point = shape.GetPoint(i);
                    if (point.X + shape.Position.X > maxX) { maxX = point.X + shape.Position.X; }
                    if (point.Y + shape.Position.Y > maxY) { maxY = point.Y + shape.Position.Y; }
                }
                Size = new Vector2f(maxX, maxY);
            }
            shapes = shapeList;
            Position = position;
        }

        public void Draw(RenderWindow window, RenderStates states)
        {
            foreach(ConvexShape shape in shapes)
            {
                shape.Draw(window, states);
            }
        }
    }
}
