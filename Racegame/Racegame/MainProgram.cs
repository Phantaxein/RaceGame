using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SFML.Window;
using SFML.System;
using SFML.Graphics;

namespace Racegame
{
    public partial class MainProgram
    {
        bool isRunning = true;
        Vector2u windowSize = new Vector2u(1600, 900);
        Vector2f prevWindowPos = new Vector2f(0, 0);
        Vector2u zoomSize = new Vector2u(1600, 900);
        RenderWindow dispWindow;
        RenderStates rs = RenderStates.Default;
        Font sans = new Font("Fonts\\cambria.ttf");
        string saveName = "testRacePath8";
        string loadName = "saveTest4";
        string backgroundLoadName = "racemap4";

        public MainProgram() { }
        public void Run()
        {
            dispWindow = new RenderWindow(VideoMode.DesktopMode, "RaceGame", Styles.Default);
            dispWindow.Size = windowSize;
            dispWindow.SetVerticalSyncEnabled(true);
            dispWindow.Closed += DispWindow_Closed;
            dispWindow.SetView(new View((Vector2f)windowSize / 2, (Vector2f)windowSize));

            Color clearCol = new Color(40, 40, 40);

            //GroupShape myPath = new GroupShape(LoadObjects("racepath4"), new Vector2f(1, 1));
            List<Cart> cartList = new List<Cart>();

            CorrectPath path = null;


            GroupShape map = new GroupShape(LoadFromPoly(out path, "testRacePath8"), new Vector2f(1, 1));

            foreach(ConvexShape shap in path.path.shapes)
            {
                shap.FillColor = new Color(shap.FillColor.R, shap.FillColor.G, shap.FillColor.B, 30);
            }

            List<GroupShape> mapList = new List<GroupShape>() { map, };

            Cart car = new Cart(new GroupShape(LoadObjects("car1"), path.path.shapes[0].Position + (path.path.shapes[0].GetPoint(0) + path.path.shapes[0].GetPoint(1)) * .5F));
            Cart car2 = new Cart(new GroupShape(LoadObjects("car1"), path.path.shapes[0].Position + (path.path.shapes[0].GetPoint(0) + path.path.shapes[0].GetPoint(1)) * .5F));

            CarInterface carInt1 = new CarInterface(car, path);
            ExampleRobot myRobot = new ExampleRobot(carInt1);

            cartList.Add(car);
            cartList.Add(car2);

            //StartObjectEditor(dispWindow);

            //* Main game loop
            while (isRunning)
            {
                dispWindow.DispatchEvents();
                dispWindow.Clear(clearCol);

                //Begin logic
                //-----------
                Vector2f relativeMousePos = new Vector2f((((float)Mouse.GetPosition(dispWindow).X / (float)windowSize.X) * (float)zoomSize.X) + ((float)dispWindow.GetView().Center.X - (.5F * (float)zoomSize.X)),
                                                     (((float)Mouse.GetPosition(dispWindow).Y / (float)windowSize.Y) * (float)zoomSize.Y) + ((float)dispWindow.GetView().Center.Y - (.5F * (float)zoomSize.Y)));


                myRobot.Update();

                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    car.Drive(1);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                {
                    car.Drive(-1);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                {
                    car.Turn(-1);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    car.Turn(1);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
                {
                    car.Brake(1F);
                }
                if (Mouse.IsButtonPressed(Mouse.Button.Left)) { car.Position = relativeMousePos; }
                
                //Set View
                Vector2f mySize = (Vector2f)dispWindow.Size;
                Vector2f startPos = new Vector2f(float.MaxValue, float.MaxValue);
                Vector2f endPos = new Vector2f(float.MinValue, float.MinValue);
                foreach (Cart x in cartList)
                {
                    if (x.Position.X < startPos.X) { startPos.X = x.Position.X; }
                    if (x.Position.Y < startPos.Y) { startPos.Y = x.Position.Y; }
                    if (x.Position.X > endPos.X) { endPos.X = x.Position.X; }
                    if (x.Position.Y > endPos.Y) { endPos.Y = x.Position.Y; }
                }
                startPos -= new Vector2f(100, 100);
                endPos += new Vector2f(100, 100);
                if (endPos.X - startPos.X < mySize.X && endPos.Y - startPos.Y < mySize.Y)
                {
                    if (startPos.X < prevWindowPos.X) { prevWindowPos = new Vector2f(startPos.X, prevWindowPos.Y); }
                    if (startPos.Y < prevWindowPos.Y) { prevWindowPos = new Vector2f(prevWindowPos.X, startPos.Y); }
                    if (endPos.X - mySize.X > prevWindowPos.X) { prevWindowPos = new Vector2f(endPos.X - mySize.X, prevWindowPos.Y); }
                    if (endPos.Y - mySize.Y > prevWindowPos.Y) { prevWindowPos = new Vector2f(prevWindowPos.X, endPos.Y - mySize.Y); }
                    dispWindow.SetView(new View(prevWindowPos + .5F * mySize, mySize));
                }
                else
                {
                    endPos -= startPos;
                    if (endPos.X / mySize.X > endPos.Y / mySize.Y) { endPos.Y = endPos.X * mySize.Y / mySize.X; }
                    else { endPos.X = endPos.Y * mySize.X / mySize.Y; }
                    dispWindow.SetView(new View(startPos + .5F * endPos, endPos));
                }
                //--


                cartList.ForEach(x => x.Update());
                cartList.ForEach(x => x.CollisionCheck(mapList));


                //FINAL stage logic
                Debug.FinishUp();

                //Logic is done, begin drawing
                //----------------------------
                path.path.Draw(dispWindow, rs);
                map.Draw(dispWindow, rs);
                cartList.ForEach(x => x.Draw(dispWindow, rs));
                Debug.Draw(dispWindow, rs);
                dispWindow.Display();
            }// */ // End main game loop
        }

        private void DispWindow_Closed(object sender, EventArgs e)
        {
            if(sender is RenderWindow)
            {
                ((RenderWindow)sender).Close();
                isRunning = false;
            }
        }

        

        public static void SavePolys(List<ConvexShape> shapeList, string title)
        {
            if(shapeList == null || shapeList.Count < 1) { return; }
            FileStream fs = null;
            StreamWriter streamWrite = null;
            try
            {
                fs = new FileStream("Saves\\" + title + ".txt", FileMode.CreateNew);
                streamWrite = new StreamWriter(fs);
                Vector2f minPos = new Vector2f(float.PositiveInfinity, float.PositiveInfinity);

                foreach(ConvexShape shape in shapeList)
                {
                    for(uint i = 0; i < shape.GetPointCount(); i++)
                    {
                        if (shape.Position.X + shape.GetPoint(i).X < minPos.X) { minPos.X = shape.Position.X + shape.GetPoint(i).X; }
                        if (shape.Position.Y + shape.GetPoint(i).Y < minPos.Y) { minPos.Y = shape.Position.Y + shape.GetPoint(i).Y; }
                    }
                }
                shapeList.ForEach(x => x.Position -= minPos);

                foreach(ConvexShape shape in shapeList)
                {
                    streamWrite.Write(shape.Position.X + ":" + shape.Position.Y + ":" + shape.GetPointCount());
                    for(uint i = 0; i < shape.GetPointCount(); i++)
                    {
                        streamWrite.Write(":" + shape.GetPoint(i).X + ":" + shape.GetPoint(i).Y);
                    }
                    streamWrite.Write(":" + shape.FillColor.R + ":" + shape.FillColor.G + ":" + shape.FillColor.B);
                    streamWrite.WriteLine();
                }

            }
            finally
            {
                if (streamWrite != null)
                {
                    streamWrite.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        public static List<ConvexShape> LoadObjects(string title)
        {
            List<ConvexShape> returnList = new List<ConvexShape>();
            FileStream fs = null;
            StreamReader streamRead = null;
            try
            {
                string[] file = File.ReadAllLines("Saves\\" + title + ".txt");

                foreach (string line in file)
                {
                    string[] lineSplit = line.Split(':');
                    int[] lineInt = new int[lineSplit.Length];
                    for(int i = 0; i < lineSplit.Length; i++)
                    {
                        lineInt[i] = Convert.ToInt32(lineSplit[i]);
                    }
                    ConvexShape shape = new ConvexShape((uint)lineInt[2]);
                    shape.Position = new Vector2f(lineInt[0], lineInt[1]);
                    for (int i = 3; i < lineInt[2] + 3; i++)
                    {
                        shape.SetPoint((uint)i - 3, new Vector2f(lineInt[i + i - 3], lineInt[i + i - 2]));
                    }
                    int colorI = lineInt[2] * 2 + 3;
                    shape.FillColor = new Color((byte)lineInt[colorI], (byte)lineInt[colorI + 1], (byte)lineInt[colorI + 2]);

                    returnList.Add(shape);
                }

            }
            finally
            {
                if (streamRead != null)
                {
                    streamRead.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return returnList;
        }
        public static List<ConvexShape> LoadFromPoly(out CorrectPath path, string title)
        {
            const int rectsize = 60;
            const int wallSize = 45;

            ConvexShape pathShape = LoadObjects(title)[0];
            List<ConvexShape> pathList = new List<ConvexShape>();
            List<ConvexShape> mapList = new List<ConvexShape>();
            for(uint i = 0; i < pathShape.GetPointCount(); i++)
            {
                Vector2f point1 = pathShape.GetPoint(i) + pathShape.Position;
                Vector2f point2 = pathShape.GetPoint((i + 1) % pathShape.GetPointCount()) + pathShape.Position;
                float angle = Trig.FindAngleToTarget(point1, point2);
                ConvexShape myShape = new ConvexShape(4);
                List<Vector2f> pathPoint = new List<Vector2f>();
                pathPoint.Add(point1 + Trig.MoveDist(angle + 90, rectsize));
                pathPoint.Add(point1 + Trig.MoveDist(angle - 90, rectsize));
                pathPoint.Add(point2 + Trig.MoveDist(angle - 90, rectsize));
                pathPoint.Add(point2 + Trig.MoveDist(angle + 90, rectsize));
                Vector2f min = pathPoint[0];
                foreach(Vector2f point in pathPoint)
                {
                    if(point.X < min.X) { min.X = point.X; }
                    if(point.Y < min.Y) { min.Y = point.Y; }
                }
                for (int j = 0; j < pathPoint.Count; j++) { pathPoint[j] -= min; }
                myShape.Position = min;
                for(int j = 0; j < pathPoint.Count; j++) { myShape.SetPoint((uint)j, pathPoint[j]); }
                pathList.Add(myShape);
            }

            pathList = new Sempoolfi(pathList).Run();


            for (int i = 0; i < pathList.Count; i++)
            {
                int index = (i + 1) % pathList.Count;
                Vector2f point1 = pathList[i].GetPoint(3) + pathList[i].Position;
                Vector2f point2 = pathList[i].GetPoint(0) + pathList[i].Position;

                Vector2f point3 = point1;
                ConvexShape shape1 = new ConvexShape(2);
                Vector2f min = point1;
                if (point2.X < min.X) { min.X = point2.X; }
                if (point2.Y < min.Y) { min.Y = point2.Y; }
                if (point3.X < min.X) { min.X = point3.X; }
                if (point3.Y < min.Y) { min.Y = point3.Y; }
                point1 -= min;
                point2 -= min;
                point3 -= min;
                shape1.Position = min;
                shape1.SetPoint(0, point1);
                shape1.SetPoint(1, point2);
                //shape1.SetPoint(2, point3);
                shape1.FillColor = Color.Black;
                mapList.Add(shape1);

                Vector2f point1_2 = pathList[i].GetPoint(2) + pathList[i].Position;
                Vector2f point2_2 = pathList[i].GetPoint(1) + pathList[i].Position;
                Vector2f point3_2 = point1_2;
                ConvexShape shape2 = new ConvexShape(2);
                Vector2f min2 = point1_2;
                if (point2_2.X < min2.X) { min2.X = point2_2.X; }
                if (point2_2.Y < min2.Y) { min2.Y = point2_2.Y; }
                if (point3_2.X < min2.X) { min2.X = point3_2.X; }
                if (point3_2.Y < min2.Y) { min2.Y = point3_2.Y; }
                point1_2 -= min2;
                point2_2 -= min2;
                point3_2 -= min2;
                shape2.Position = min2;
                shape2.SetPoint(0, point1_2);
                shape2.SetPoint(1, point2_2);
                //shape2.SetPoint(2, point3_2);
                shape2.FillColor = Color.Black;
                mapList.Add(shape2);
            }
            for (int i = 0; i < pathList.Count; i++)
            {
                int index = (i + 1) % pathList.Count;
                Vector2f point1 = pathList[i].GetPoint(3) + pathList[i].Position;
                Vector2f point2 = pathList[index].GetPoint(0) + pathList[index].Position;

                Vector2f point3 = point1;
                ConvexShape shape1 = new ConvexShape(2);
                Vector2f min = point1;
                if (point2.X < min.X) { min.X = point2.X; }
                if (point2.Y < min.Y) { min.Y = point2.Y; }
                if (point3.X < min.X) { min.X = point3.X; }
                if (point3.Y < min.Y) { min.Y = point3.Y; }
                point1 -= min;
                point2 -= min;
                point3 -= min;
                shape1.Position = min;
                shape1.SetPoint(0, point1);
                shape1.SetPoint(1, point2);
                //shape1.SetPoint(2, point3);
                shape1.FillColor = Color.Black;
                mapList.Add(shape1);

                Vector2f point1_2 = pathList[i].GetPoint(2) + pathList[i].Position;
                Vector2f point2_2 = pathList[index].GetPoint(1) + pathList[index].Position;
                Vector2f point3_2 = point1_2;
                ConvexShape shape2 = new ConvexShape(2);
                Vector2f min2 = point1_2;
                if (point2_2.X < min2.X) { min2.X = point2_2.X; }
                if (point2_2.Y < min2.Y) { min2.Y = point2_2.Y; }
                if (point3_2.X < min2.X) { min2.X = point3_2.X; }
                if (point3_2.Y < min2.Y) { min2.Y = point3_2.Y; }
                point1_2 -= min2;
                point2_2 -= min2;
                point3_2 -= min2;
                shape2.Position = min2;
                shape2.SetPoint(0, point1_2);
                shape2.SetPoint(1, point2_2);
                //shape2.SetPoint(2, point3_2);
                shape2.FillColor = Color.Black;
                mapList.Add(shape2);
            }
            path = new CorrectPath(new GroupShape(pathList));
            return mapList;
        }
    }
}
