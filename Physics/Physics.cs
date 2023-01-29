using BulletSharp;
using OpenTK.Mathematics;

namespace MyGame
{
    class Contact : ContactResultCallback
    {
        private RigidBody monitoredBody;
        private object Name;
        public Contact(RigidBody monitoredBody, string contextName)
        {
            this.monitoredBody = monitoredBody;
            this.Name = (object)contextName;
        }
        public override bool NeedsCollision(BroadphaseProxy proxy)
        {
            if (base.NeedsCollision(proxy))
            {

            }
            return false;
        }

        public override double AddSingleResult(ManifoldPoint contact,
            CollisionObjectWrapper colObj0, int partId0, int index0,
            CollisionObjectWrapper colObj1, int partId1, int index1)
        {


            Vector3d collisionPoint;
            if (colObj0.CollisionObject == monitoredBody)
            {

                var vec = contact.LocalPointA;
                collisionPoint = new Vector3d((float)vec.X, (float)vec.Y, (float)vec.Z);

            }
            else
            {

                System.Diagnostics.Debug.Assert(colObj1.CollisionObject == monitoredBody);

                var vec = contact.LocalPointA;
                collisionPoint = new Vector3d((float)vec.X, (float)vec.Y, (float)vec.Z);
                
            }

            return 0; 
        }
    }
    public class CharacterPhysic
    {
        public object Name;
        public CharacterPhysic(string Name, Vector3d scale, Vector3d position, double mass)
        {
            this.Name = (object)Name;

            PhysicsWorld.BoxShape(Name, scale, position, mass);
        }
        public CharacterPhysic(string Name, double radius, Vector3d position, double mass)
        {
            this.Name = (object)Name;

            PhysicsWorld.SphereShape(Name, radius, position, mass);
        }
        public CharacterPhysic(string Name, Span<double> points, Vector3d position, double mass)
        {
            this.Name = (object)Name;

            PhysicsWorld.ModelShape(Name, points.ToArray(), position, mass);
        }
        public Vector3d Position
        {
            get => PhysicsWorld.GetPosition(Name);
            set => PhysicsWorld.SetPosition(Name, value);
        }
        public Matrix4 Model 
        {
            get => PhysicsWorld.GetMatrix(Name);
            set => PhysicsWorld.SetNewMatrix(Name, value);
        }
        
        // rotations
        private double _Xaxis = 0;
        public double RotationX_Axis
        {
            get => _Xaxis;
            set
            {
                _Xaxis = value;
                PhysicsWorld.SetRotationX(Name, _Xaxis);
            }
        }
        private double _Yaxis = 0;
        public double RotationY_Axis
        {
            get => _Yaxis;
            set
            {
                _Yaxis = value;
                PhysicsWorld.SetRotationY(Name, _Yaxis);
            }
        }
        private double _Zaxis = 0;
        public double RotationZ_Axis
        {
            get => _Zaxis;
            set
            {
                _Zaxis = value;
                PhysicsWorld.SetRotationZ(Name, _Zaxis);
            }
        }

    }
}