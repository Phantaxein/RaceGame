using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace Racegame
{
    public class Cart
    {
        private const float accelSpeed = .18F;
        private const float turnSpeed = 3.5F;
        private const float torqueSpeed = .25F;

        private readonly GroupShape shape;

        public Vector2f Position { get { return shape.Position; } set { shape.Position = value; } }

        public Vector2f Velocity = new Vector2f(0, 0);
        public float Rotation { get { return shape.Rotation; } set { shape.Rotation = value; } }

        private Vector2f Accelaration = new Vector2f(0, 0);

        private float turnFrame = 0;
        private float braking = 0;

        private float Torque = 0;

        public float rotVelocity = 0; // rotational acceleration
        public Vector2f Size { get { return shape.Size; } }

        public static implicit operator ConvexShape(Cart cart)
        {
            return cart.shape;
        }

        public Cart(GroupShape shape)
        {
            this.shape = shape;
            this.Position = shape.Position;
        }
        
        public void Drive(float amount)
        {
            bool neg = amount < 0 ? true : false;
            if (amount != 1 && amount != -1) { amount %= 1; }
            amount = Math.Abs(amount);
            amount *= accelSpeed;
            amount *= (neg ? -1 : 1);
            float rotDeg = Rotation - 90;
            Accelaration = new Vector2f((float)(Math.Cos(Trig.Rad(rotDeg)) * amount), (float)(Math.Sin(Trig.Rad(rotDeg)) * amount));
        }
        public void Turn(float amount)
        {
            bool neg = amount < 0 ? true : false;
            if (amount != 1 && amount != -1) { amount %= 1; }
            amount = Math.Abs(amount);
            amount *= torqueSpeed;
            amount *= (neg ? -1 : 1);
            turnFrame = amount;
        }

        public void Brake(float brakeForce)
        {
            brakeForce = Math.Abs(brakeForce);
            if (brakeForce != 1 ) { brakeForce %= 1; }
            braking = Game.globalFriction - (brakeForce * 2 * (1 - Game.globalFriction));
        }

        public void Update()
        {
            Velocity += Accelaration;
            Accelaration = new Vector2f(0, 0);
            float dist = Trig.GetPythag(Position, Position + Size * .5F);
            float centAngle = Trig.FindAngleToTarget(Position, Position + .5F * Size);
            Vector2f centerChange = new Vector2f((float)(Math.Cos(Trig.Rad(Rotation + centAngle)) * dist), (float)(Math.Sin(Trig.Rad(Rotation + centAngle)) * dist));
            float angle = Trig.FindAngleToTarget(Position + centerChange, Position + centerChange + Velocity);

            Torque = Math.Abs(angle + 360 - Rotation + 360) % 360 < 180 ? -turnFrame : turnFrame;
            Torque *= (float)Trig.GetPythag(new Vector2f(0, 0), Velocity) * turnSpeed ;
            turnFrame = 0;
            rotVelocity += Torque;
            Position += Velocity;
            Rotation += rotVelocity;
            Velocity *= braking != 0 ? braking : Game.globalFriction;
            braking = 0;
            rotVelocity -= Torque; //Undo torque from last frame
            rotVelocity *= Game.globalRotDrag;
            if (Velocity.X < Game.globalZeroSpeed && Velocity.X > -Game.globalZeroSpeed) { Velocity.X = 0; }
            if (Velocity.Y < Game.globalZeroSpeed && Velocity.Y > -Game.globalZeroSpeed) { Velocity.Y = 0; }

        }

        public void CollisionCheck(List<GroupShape> mapList)
        {
            foreach(GroupShape shape in mapList)
            {


                CollisionInfo myInfo = CollisionHandler.MultiColCheck(this, shape);

                if (myInfo.isCollide) 
                { 
                    this.Position += myInfo.minIntervalDist;
                    //CHECK ROTATION VELOCITY
                    this.Rotation += 1;
                    CollisionInfo rotInfo1 = CollisionHandler.MultiColCheck(this, myInfo.shapes);
                    this.Rotation -= 2;
                    CollisionInfo rotInfo2 = CollisionHandler.MultiColCheck(this, myInfo.shapes);
                    this.Rotation += 1;
                    if(Trig.GetPythag(new Vector2f(0, 0), rotInfo1.minIntervalDist) > Trig.GetPythag(new Vector2f(0, 0), rotInfo2.minIntervalDist))
                    {
                        this.rotVelocity -= Trig.GetPythag(new Vector2f(0, 0), this.Velocity) * Game.rotBounceModifier;
                    }
                    else
                    {
                        this.rotVelocity += Trig.GetPythag(new Vector2f(0, 0), this.Velocity) * Game.rotBounceModifier;
                    }

                    this.Velocity += Trig.Normalize(myInfo.minIntervalDist) * Trig.GetPythag(new Vector2f(0, 0), this.Velocity) * Game.bounceModifier;
                    this.Velocity *= Game.bufferModifier;
                }
                
            }
        }

        public void Draw(RenderWindow dispWind, RenderStates states)
        {
            shape.Draw(dispWind, states);
        }

    }
}
