using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace Racegame
{
    public struct CollisionInfo
    {
        public bool isCollide;
        public Vector2f minIntervalDist;
        public List<ConvexShape> shapes;
    }
}
