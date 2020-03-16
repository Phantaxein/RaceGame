using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racegame
{
    public class CorrectPath
    {
        public readonly GroupShape path;

        public CorrectPath(GroupShape path) => this.path = path;

        public int GetPlacement(Cart cart)
        {
            int place = 0;
            CollisionInfo myCol = CollisionHandler.MultiColCheck(cart, path);
            if (myCol.isCollide) { place = path.shapes.IndexOf(myCol.shapes[myCol.shapes.Count - 1]); }
            if(CollisionHandler.CheckCol(cart, path.shapes[0]).isCollide && CollisionHandler.CheckCol(cart, path.shapes.Last()).isCollide) { place = 0; }
            return place;
        }
    }
}
