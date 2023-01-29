using BulletSharp;
using BulletSharp.Math;
using OpenTK.Graphics.OpenGL4;

using Vector3d = OpenTK.Mathematics.Vector3d;
using Color4 = OpenTK.Mathematics.Color4;

namespace MyGame
{
    public struct ShapeScale
    {
        public object Name;
        public Matrix MatrixScale;
        public BroadphaseNativeType Type;
    }
    public class PhysicsWorld
    {
        private static CollisionConfiguration collisionConfig = new DefaultCollisionConfiguration();
        private static CollisionDispatcher collisiondispatcher = new CollisionDispatcher(collisionConfig);
        private static DbvtBroadphase broadphase = new DbvtBroadphase();
        private static ShaderProgram Shader = new ShaderProgram("Physics/shaderPhysic.vert", "Physics/shaderPhysic.frag");
        private static List<Color4> colors = new List<Color4>();

        // External uses
        public static DiscreteDynamicsWorld World { get; } = new DiscreteDynamicsWorld(collisiondispatcher, broadphase, null, collisionConfig); 
        private static List<ShapeScale> ShapesScale = new List<ShapeScale>();
        public static AlignedCollisionObjectArray ObjectsArray { get => World.CollisionObjectArray; }
        private static double _gravity = -9.807;
        public static double Gravity
        {
            get => _gravity;
            set
            {
                _gravity = -value;
                World.Gravity = new Vector3(0.0, _gravity, 0.0);
            }
        }
        public static void ModelShape(string name, double[] points, Vector3d startPosition, double mass)
        {
            CollisionShape shape = new ConvexHullShape(points);
            CreateBody(shape, Vector3d.One, startPosition, mass, name);
        }
        public static void BoxShape(string name, Vector3d scale, Vector3d startPosition, double mass)
        {
            CollisionShape shape = new BoxShape(new Vector3(scale.X, scale.Y, scale.Z));
            CreateBody(shape, scale, startPosition, mass, name);
        }
        public static void SphereShape(string name, double radius, Vector3d startPosition, double mass)
        {
            CollisionShape shape = new SphereShape(radius);
            CreateBody(shape, new Vector3d(radius), startPosition, mass, name);
        }
        private static void CreateBody(CollisionShape shape, Vector3d scale, Vector3d startPosition, double mass, string name)
        {
            Vector3 localInertia = Vector3.Zero;

            if (mass > 0.0)
            {
                localInertia = shape.CalculateLocalInertia(mass);
            }

            DefaultMotionState myMotionState = new DefaultMotionState(Matrix.Translation(new Vector3(startPosition.X, startPosition.Y, startPosition.Z)));

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);

            RigidBody body = new RigidBody(rbInfo);

            body.UserObject = name;

            ShapesScale.Add( new ShapeScale()
            {
                Name = (object)name,
                MatrixScale = Matrix.Scaling(new Vector3(scale.X, scale.Y, scale.Z)),
                Type = shape.ShapeType,
            });
            World.AddRigidBody(body);
            colors.Add(RandomColors.Colors[new Random().Next(0, RandomColors.MaxColors)]);

            rbInfo.Dispose();
        }
        public static void RenderObjects()
        {
            Shader!.Use();
            Shader!.SetUniform("projection", Camera.ProjectionMatrix);
            Shader!.SetUniform("view", Camera.ViewMatrix);

            var index = Values.PhysicsWorld.PrimitiveTypeIndex;

            GL.Enable(EnableCap.CullFace);
            for( int i = 0; i < ObjectsArray.Count; i++)
            {
                var matrixPosition = ObjectsArray[i].WorldTransform;
                var shapeScale = PhysicsWorld.ShapesScale[i];

                var model = shapeScale.MatrixScale * matrixPosition;

                Shader!.SetUniform("model", model);

                Shader!.SetUniform("color", colors[i]);

                if( shapeScale.Type == BroadphaseNativeType.ConvexHullShape || "Piso".Equals(shapeScale.Name))
                {
                    continue;
                }
                else if(shapeScale.Type == BroadphaseNativeType.BoxShape)
                {
                    Cube.RenderCube(Values.PhysicsWorld.prim[index]);
                }
                else if(shapeScale.Type == BroadphaseNativeType.SphereShape)
                {
                    if(index == 0)
                    {
                        Sphere.RenderSphere(PrimitiveType.TriangleStrip);
                    }
                    else
                    {
                        Sphere.RenderSphere(Values.PhysicsWorld.prim[index]);
                    }
                }
            }
            GL.Disable(EnableCap.CullFace);
        }
        public void UpdateFrameSimulation(double ElapsedTime)
        {
            Gravity = Values.PhysicsWorld.Gravity;
            World.StepSimulation(ElapsedTime);

        }
        public void Dispose()
        {
            for (int i = World.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = World.GetConstraint(i);
                World.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            for (int i = World.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = World.CollisionObjectArray[i];
                RigidBody? body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                World.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            ShapesScale.Clear();

            World.Dispose();
            broadphase.Dispose();

            if (collisiondispatcher != null)
            {
                collisiondispatcher.Dispose();
            }
            collisionConfig.Dispose();
        }

        //---------------------------------------- Get Set Values -----------------------------------------
        public static void SetNewMatrix(object Name, OpenTK.Mathematics.Matrix4 modelTransform)
        {
            try
            {
                (from Select in ObjectsArray where Select.UserObject == Name select Select)
                .First().WorldTransform = MathHelper.Mat(modelTransform);
            }
            catch
            {
                Console.WriteLine($"Object: {Name.ToString()} Matrix Not applied");
            }

        }
        public static OpenTK.Mathematics.Matrix4 GetMatrix(object Name)
        {
            try
            {
                var matrixPosition = (from Select in ObjectsArray where Select.UserObject == Name select Select)
                .First().WorldTransform;

                var matrixScale = (from Selected in ShapesScale where Selected.Name == Name select Selected)
                .First().MatrixScale;

                var model = matrixScale * matrixPosition;

                return MathHelper.Mat(model);
            }
            catch
            {
                Console.WriteLine($"Object: {Name.ToString()} Return Matrix Empty");
                return OpenTK.Mathematics.Matrix4.Zero;
            }
            
        }
        public static void SetPosition(object Name, OpenTK.Mathematics.Vector3d position)
        {
            try
            {
                var model = (from Select in ObjectsArray where Select.UserObject == Name select Select)
                .First().WorldTransform;

                model.Row4 = new Vector4(position.X, position.Y, position.Z, model.M44);

                (from Select in ObjectsArray where Select.UserObject == Name select Select)
                .First().WorldTransform = model;
            }
            catch
            {
                Console.WriteLine($"Object: {Name.ToString()} not found! The Vector position was not applied");

            }

        }
        public static OpenTK.Mathematics.Vector3d GetPosition(object Name)
        {

            try
            {
                var model = (from Select in ObjectsArray where Select.UserObject == Name select Select)
                .First().WorldTransform;

                return new OpenTK.Mathematics.Vector3d(MathHelper.GetPosition(model));

            }
            catch
            {
                Console.WriteLine($"Object: {Name.ToString()} not found! Return Vector Empty");
                return OpenTK.Mathematics.Vector3.Zero;
            }

        }

        public static void SetRotationX(object Name, double degrees)
        {

            try
            {

                var model = (from Select in ObjectsArray where Select.UserObject == Name select Select)
                .First().WorldTransform;

                Vector3 position = MathHelper.GetPositionVec3(model);

                var matrixRotate = Matrix.RotationX(MathHelper.DegreesToRadians(degrees));
                matrixRotate = matrixRotate * Matrix.Translation(position);

                (from Select in ObjectsArray where Select.UserObject == Name select Select)
                .First().WorldTransform = matrixRotate;

            }
            catch
            {
                Console.WriteLine($"Object: {Name.ToString()} not found! X-axis rotation has not been applied");
            }

        }
        public static void SetRotationY(object Name, double degrees)
        {
            var model = Matrix.RotationY( OpenTK.Mathematics.MathHelper.DegreesToRadians(degrees));

            try
            {
                (from Select in ObjectsArray where Select.UserObject == Name select Select)
                .First().WorldTransform *= model;
            }
            catch
            {
                Console.WriteLine($"Object: {Name.ToString()} not found! Y-axis rotation has not been applied");
            }

        }
        public static void SetRotationZ(object Name, double degrees)
        {
            var model = Matrix.RotationZ( OpenTK.Mathematics.MathHelper.DegreesToRadians(degrees));

            try
            {
                (from Select in ObjectsArray where Select.UserObject == Name select Select)
                .First().WorldTransform *= model;
            }
            catch
            {
                Console.WriteLine($"Object: {Name.ToString()} not found! Z-axis rotation has not been applied");
            }

        }
        // ------------------------------------------------------------------------------------------------
    }
    public static class RandomColors
    {
        public static List<Color4> Colors = new List<Color4>()
        {
            Color4.AliceBlue,
            Color4.White,
            Color4.Red,
            Color4.Green,
            Color4.GreenYellow,
            Color4.OrangeRed,
            Color4.Blue,
            Color4.BlueViolet,
            Color4.Cyan,
            Color4.Azure,
            Color4.Gold,
            Color4.YellowGreen,
            Color4.Pink,
            Color4.Tomato,
            Color4.SkyBlue,
        };
        public static int MaxColors = Colors.Count;

    }
}