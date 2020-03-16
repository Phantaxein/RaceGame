using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.System;
using SFML.Graphics;

namespace Racegame
{
    public class CartHandler
    {
        private List<Cart> cartList = new List<Cart>();
        
        public void AddCart(Cart cart)
        {
            cartList.Add(cart);
        }
        public void Update()
        {
            cartList.ForEach(x => x.Update());
        }
        public void Draw(RenderWindow window, RenderStates rs)
        {
            cartList.ForEach(x => x.Draw(window, rs));
        }
    }
}
