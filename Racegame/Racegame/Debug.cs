using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Racegame
{
    public static class Debug
    {
        static List<Text> textList = new List<Text>();
        public static Vector2f position = new Vector2f(0, 0);
        static Dictionary<string, int>  countDictionary = new Dictionary<string, int>();
        
        public static void Out(string text)
        {
            textList.Add(new Text(text, Game.font));
        }
        public static void Count(string text)
        {
            if(!countDictionary.ContainsKey(text)) { countDictionary.Add(text, 1); }
            else
            {
                countDictionary[text] += 1;
            }
        }

        public static void FinishUp()
        {

            //Counts
            for (int i = 0; i < countDictionary.Count; i++)
            {
                textList.Add(new Text(countDictionary.Keys.ElementAt(i) + ": " + countDictionary.Values.ElementAt(i), Game.font));
            }
            countDictionary.Clear();

            //Finishup General
            for (int i = 0; i < textList.Count; i++)
            {
                textList[i].Position = new Vector2f(textList[i].Position.X, i * textList[i].CharacterSize);
            }
        }

        public static void Draw(RenderWindow window, RenderStates states)
        {
            textList.ForEach(x => x.Draw(window, states));
            textList.Clear();
        }
    }
}
