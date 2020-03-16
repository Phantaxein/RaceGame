using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace Racegame
{
    public class CollisionHandler
    {
        public static CollisionInfo MultiColCheck(ConvexShape single, GroupShape multi) => MultiColCheck(single, multi.shapes);
        public static CollisionInfo MultiColCheck(ConvexShape single, List<ConvexShape> multi)
        {
            CollisionInfo myInfo = new CollisionInfo();
            myInfo.isCollide = false;
            myInfo.shapes = new List<ConvexShape>();
            int intervalAmount = 0;
            foreach (ConvexShape s in multi)
            {
                CollisionInfo myInfo2 = CheckCol(single, s);
                if (myInfo2.isCollide)
                {
                    myInfo.minIntervalDist += myInfo2.minIntervalDist;
                    intervalAmount += 1;
                    myInfo.isCollide = true;
                    myInfo.shapes.Add(myInfo2.shapes[0]);
                }
            }
            //if (myInfo.isCollide) { myInfo.minIntervalDist = myInfo.minIntervalDist / intervalAmount; }
            return myInfo; 
        }
        public static void ProjectPolygon(Vector2f axis, ConvexShape polygon,
                          ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            float dotProduct = Trig.DotProduct(axis, polygon.Transform.TransformPoint(polygon.GetPoint(0)));
            min = dotProduct;
            max = dotProduct;
            for (uint i = 0; i < polygon.GetPointCount(); i++)
            {
                dotProduct = Trig.DotProduct(polygon.Transform.TransformPoint(polygon.GetPoint(i)), axis);
                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else
                {
                    if (dotProduct > max)
                    {
                        max = dotProduct;
                    }
                }
            }
        }
        public static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            float f1 = minB - maxA;
            float f2 = minA - maxB;
            return ((minA < minB) ? f1 / Math.Abs(f1) : f2 / Math.Abs(f2)) * ((Math.Abs(f1) < Math.Abs(f2)) ? Math.Abs(f1) : Math.Abs(f2));
        }
        public static CollisionInfo CheckCol(GroupShape shape, ConvexShape conv) => CheckCol((ConvexShape)shape, conv);
        public static CollisionInfo CheckCol(ConvexShape shape1, ConvexShape shape2)
        {
            Vector2f max = new Vector2f(0, 0);
            for (uint i = 0; i < shape1.GetPointCount(); i++)
            {
                float x = shape1.GetPoint(i).X;
                if (x> max.X) { max.X = x; }
                float y = shape1.GetPoint(i).Y;
                if (y > max.Y) { max.Y = y; }
            }
            Vector2f center1 = shape1.Position + .5F * max;
            max = new Vector2f(0, 0);
            for (uint i = 0; i < shape2.GetPointCount(); i++)
            {
                float x = shape2.GetPoint(i).X;
                if (x > max.X) { max.X = x; }
                float y = shape2.GetPoint(i).Y;
                if (y > max.Y) { max.Y = y; }
            }
            Vector2f center2 = shape2.Position + .5F * max;


            CollisionInfo myInfo = new CollisionInfo();
            myInfo.isCollide = true;
            float intervalDist;
            float minIntervalDist = float.PositiveInfinity;
            Vector2f translationNormal = new Vector2f(0, 0);
            foreach (var polygon in new ConvexShape[] { shape1, shape2 })
            {
                for (int i1 = 0; i1 < polygon.GetPointCount(); i1++)
                {
                    int i2 = (i1 + 1) % (int)polygon.GetPointCount();
                    Vector2f p1 = polygon.Transform.TransformPoint(polygon.GetPoint((uint)i1));
                    Vector2f p2 = polygon.Transform.TransformPoint(polygon.GetPoint((uint)i2));

                    Vector2f normal = new Vector2f(p2.Y - p1.Y, p1.X - p2.X);
                    normal = Trig.Normalize(normal);

                    float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                    ProjectPolygon(normal, shape1, ref minA, ref maxA);
                    ProjectPolygon(normal, shape2, ref minB, ref maxB);
                    intervalDist = IntervalDistance(minA, maxA, minB, maxB);
                    if(minB > maxA || minA > maxB)
                    {
                        myInfo.isCollide = false;
                        break;
                    }
                    
                    intervalDist = Math.Abs(intervalDist);
                    if (intervalDist < minIntervalDist)
                    {
                        minIntervalDist = intervalDist;
                        translationNormal = normal;

                        float d = (minA + maxA) / 2 - (minB + maxB) / 2;
                        if(d < 0)
                        {
                            translationNormal = -translationNormal;
                        }
                    }
                }
                if(myInfo.isCollide == false) { break; }
            }
            if (myInfo.isCollide)
            {
                myInfo.minIntervalDist = minIntervalDist * translationNormal;
                myInfo.shapes = new List<ConvexShape>() { shape2 };
            }
            return myInfo;
        }
    }
    public enum CollisionType
    {
        RectStyle = 0,
        Individual = 1
    }
}
