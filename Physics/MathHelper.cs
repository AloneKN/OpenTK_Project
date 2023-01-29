

namespace MyGame
{
    public class MathHelper
    {
        public static OpenTK.Mathematics.Vector3d Vec3(BulletSharp.Math.Vector3 vec) =>
            new OpenTK.Mathematics.Vector3d(vec.X, vec.Y, vec.Z);
        public static BulletSharp.Math.Vector3 Vec3(OpenTK.Mathematics.Vector3d vec) =>
            new BulletSharp.Math.Vector3(vec.X, vec.Y, vec.Z);

        public static BulletSharp.Math.Matrix Mat(OpenTK.Mathematics.Matrix4 mat) => 
            new BulletSharp.Math.Matrix
            (
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44
            );
        public static OpenTK.Mathematics.Matrix4 Mat(BulletSharp.Math.Matrix mat) => 
            new OpenTK.Mathematics.Matrix4
            (
                (float)mat.M11, (float)mat.M12, (float)mat.M13, (float)mat.M14,
                (float)mat.M21, (float)mat.M22, (float)mat.M23, (float)mat.M24,
                (float)mat.M31, (float)mat.M32, (float)mat.M33, (float)mat.M34,
                (float)mat.M41, (float)mat.M42, (float)mat.M43, (float)mat.M44
            );

        public static OpenTK.Mathematics.Vector3d GetPosition(BulletSharp.Math.Matrix mat) =>
            new OpenTK.Mathematics.Vector3d(mat.Row4.X, mat.Row4.Y, mat.Row4.Z);
        public static BulletSharp.Math.Vector3 GetPositionVec3(BulletSharp.Math.Matrix mat) =>
            new BulletSharp.Math.Vector3(mat.Row4.X, mat.Row4.Y, mat.Row4.Z);

        public static double DegreesToRadians(double degrees) => Math.PI / 180 * degrees;

    }
}