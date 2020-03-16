using System;
using SFML.System;
using SFML.Graphics;

namespace Racegame
{
    public class CarInterface
    {
        private readonly Cart car;
        private readonly CorrectPath path;
        public CarInterface(Cart car, CorrectPath path)
        {
            this.car = car;
            this.path = path;
        }

        /// <summary>
        /// The amount your car will accelarate this frame. -1 is maximum backwards and 1 is maximum forwards
        /// </summary>
        /// <param name="amount"></param>
        public void Drive(float amount) => car.Drive(amount);
        /// <summary>
        /// The direction your car will turn this frame. -1 is maximum left and 1 is maximum right.
        /// </summary>
        /// <param name="amount"></param>
        public void Turn(float amount) => car.Turn(amount);
        /// <summary>
        /// The amount your car will brake this frame. 0 is no brake and 1 is maximum brake.
        /// </summary>
        /// <param name="amount"></param>
        public void Brake(float amount) => car.Brake(amount);
        /// <summary>
        /// Gets the amount of shapes in the correct path
        /// </summary>
        /// <returns></returns>
        public int GetShapeCount() => path.path.shapes.Count;
        /// <summary>
        /// Gets the amount of shapes in the correct path
        /// </summary>
        /// <returns></returns>
        public Vector2f GetShapePosition(int shapeIndex)
        {
            if (shapeIndex < GetShapeCount())
            {
                return path.path.shapes[shapeIndex].Position;
            }
            return new Vector2f(0, 0); 

        }
        /// <summary>
        /// Get the amount of points in shape x from the correct path
        /// </summary>
        /// <param name="shapeIndex"></param>
        /// <returns></returns>
        public int GetPointCount(int shapeIndex)
        {
            if(shapeIndex < GetShapeCount())
            {
                return (int)path.path.shapes[shapeIndex].GetPointCount();
            }
            return 0;
        }
        /// <summary>
        /// Get point x from shape y in the correct path
        /// </summary>
        /// <param name="shapeIndex"></param>
        /// <param name="pointIndex"></param>
        /// <returns></returns>
        public Vector2f GetPoint(int shapeIndex, int pointIndex)
        {
            if (shapeIndex < GetShapeCount() && pointIndex < GetPointCount(shapeIndex) )
            {
                return path.path.shapes[shapeIndex].GetPoint((uint)pointIndex);
            }
            return new Vector2f(0, 0);
        }
        /// <summary>
        /// Gets the zero based index of the frontmost shape your car is colliding with.
        /// </summary>
        /// <returns></returns>
        public int GetPlacement()
        {
            if(car != null)
            {
                return path.GetPlacement(car);
            }
            return 0;
        }
        /// <summary>
        /// Returns the car's rotation
        /// </summary>
        /// <returns></returns>
        public float GetRotation() => car.Rotation;
        /// <summary>
        /// Returns the car's rotational velocity (angular velocity)
        /// </summary>
        /// <returns></returns>
        public float GetRotationVelocity() => car.rotVelocity;
        /// <summary>
        /// Returns the CENTER of the car's position on the screen
        /// </summary>
        /// <returns></returns>
        public Vector2f GetPosition()
        {
            float centerDist = Trig.GetPythag(car.Position, car.Position + .5F * car.Size);
            float centAngle = Trig.FindAngleToTarget(car.Position, car.Position + .5F * car.Size);
            return car.Position + new Vector2f((float)(Math.Cos(Trig.Rad(car.Rotation + centAngle)) * centerDist), (float)(Math.Sin(Trig.Rad(car.Rotation + centAngle)) * centerDist));
        }
        /// <summary>
        /// Returns the car's velocity
        /// </summary>
        /// <returns></returns>
        public Vector2f GetVelocity() => car.Velocity;
        /// <summary>
        /// Returns the car's size (car is a rectangle with flat edges)
        /// </summary>
        /// <returns></returns>
        public Vector2f GetSize() => car.Size;
    }
}
