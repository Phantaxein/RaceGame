using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace Racegame
{
    public static class Game
    {
        public const float globalFriction = .975F;
        public const float globalRotDrag = .95F;
        public const float globalZeroSpeed = .01F;
        public const float rotBounceModifier = .5F;
        public const float bounceModifier = .4F;
        public const float bufferModifier = .5F;
        public static readonly Font font = new Font("Fonts\\cambria.ttf");
    }
}
