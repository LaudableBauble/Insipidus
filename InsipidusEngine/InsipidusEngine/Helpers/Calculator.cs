using System;

using Microsoft.Xna.Framework;

using InsipidusEngine.Physics;

namespace InsipidusEngine
{
    /// <summary>
    /// A calculator with common math functions and constants.
    /// </summary>
    public static class Calculator
    {
        public const float DegreesToRadiansRatio = 57.29577957855f;
        public const float RadiansToDegreesRatio = 1f / 57.29577957855f;
        //NOTE: Commented line, use MathHelper.TwoPi instead
        //public const float TwoPi = 6.28318531f;
        private static Vector2 _curveEnd;
        private static Random _random = new Random();
        private static Vector2 _startCurve;
        private static Vector2 _temp;

        /// Temp variables to speed up the following code.
        private static float _tPow2;

        private static float _wayToGo;
        private static float _wayToGoPow2;

        public static float Sin(float angle)
        {
            return (float)Math.Sin(angle);
        }
        public static float Cos(float angle)
        {
            return (float)Math.Cos(angle);
        }
        public static float ACos(float value)
        {
            return (float)Math.Acos(value);
        }
        public static float ATan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }
        //performs bilinear interpolation of a point
        public static float BiLerp(Vector2 point, Vector2 min, Vector2 max, float value1, float value2, float value3,
                                   float value4, float minValue, float maxValue)
        {
            float x = point.X;
            float y = point.Y;

            x = MathHelper.Clamp(x, min.X, max.X);
            y = MathHelper.Clamp(y, min.Y, max.Y);

            float xRatio = (x - min.X) / (max.X - min.X);
            float yRatio = (y - min.Y) / (max.Y - min.Y);

            float top = MathHelper.Lerp(value1, value4, xRatio);
            float bottom = MathHelper.Lerp(value2, value3, xRatio);

            float value = MathHelper.Lerp(top, bottom, yRatio);
            value = MathHelper.Clamp(value, minValue, maxValue);
            return value;
        }
        public static float Clamp(float value, float low, float high)
        {
            return Math.Max(low, Math.Min(value, high));
        }
        public static float DistanceBetweenPointAndPoint(Vector2 point1, Vector2 point2)
        {
            Vector2 v = Vector2.Subtract(point1, point2);
            return v.Length();
        }
        public static float DistanceBetweenPointAndPoint(ref Vector2 point1, ref Vector2 point2)
        {
            Vector2 v;
            Vector2.Subtract(ref point1, ref point2, out v);
            return v.Length();
        }
        public static float DistanceBetweenPointAndLineSegment(Vector2 point, Vector2 lineEndPoint1,
                                                               Vector2 lineEndPoint2)
        {
            Vector2 v = Vector2.Subtract(lineEndPoint2, lineEndPoint1);
            Vector2 w = Vector2.Subtract(point, lineEndPoint1);

            float c1 = Vector2.Dot(w, v);
            if (c1 <= 0) return DistanceBetweenPointAndPoint(point, lineEndPoint1);

            float c2 = Vector2.Dot(v, v);
            if (c2 <= c1) return DistanceBetweenPointAndPoint(point, lineEndPoint2);

            float b = c1 / c2;
            Vector2 pointOnLine = Vector2.Add(lineEndPoint1, Vector2.Multiply(v, b));
            return DistanceBetweenPointAndPoint(point, pointOnLine);
        }
        public static float Cross(Vector2 value1, Vector2 value2)
        {
            return value1.X * value2.Y - value1.Y * value2.X;
        }
        public static Vector2 Cross(Vector2 value1, float value2)
        {
            return new Vector2(value2 * value1.Y, -value2 * value1.X);
        }
        public static Vector2 Cross(float value2, Vector2 value1)
        {
            return new Vector2(-value2 * value1.Y, value2 * value1.X);
        }
        public static void Cross(ref Vector2 value1, ref Vector2 value2, out float ret)
        {
            ret = value1.X * value2.Y - value1.Y * value2.X;
        }
        public static void Cross(ref Vector2 value1, ref float value2, out Vector2 ret)
        {
            ret = value1; //necassary to get past a compile error on 360
            ret.X = value2 * value1.Y;
            ret.Y = -value2 * value1.X;
        }
        public static void Cross(ref float value2, ref Vector2 value1, out Vector2 ret)
        {
            ret = value1; //necassary to get past a compile error on 360
            ret.X = -value2 * value1.Y;
            ret.Y = value2 * value1.X;
        }
        public static Vector2 Project(Vector2 projectVector, Vector2 onToVector)
        {
            float multiplier = 0;
            float numerator = (onToVector.X * projectVector.X + onToVector.Y * projectVector.Y);
            float denominator = (onToVector.X * onToVector.X + onToVector.Y * onToVector.Y);

            if (denominator != 0)
            {
                multiplier = numerator / denominator;
            }

            return Vector2.Multiply(onToVector, multiplier);
        }
        public static void Truncate(ref Vector2 vector, float maxLength, out Vector2 truncatedVector)
        {
            float length = vector.Length();
            length = Math.Min(length, maxLength);
            if (length > 0)
            {
                vector.Normalize();
            }
            Vector2.Multiply(ref vector, length, out truncatedVector);
        }
        public static float DegreesToRadians(float degrees)
        {
            return degrees * RadiansToDegreesRatio;
        }
        public static float RandomNumber(float min, float max)
        {
            return (float)((max - min) * _random.NextDouble() + min);
        }
        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max + 1);
        }
        public static bool IsBetweenNonInclusive(float number, float min, float max)
        {
            if (number > min && number < max)
            {
                return true;
            }
            return false;
        }
        public static float VectorToRadians(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -(double)vector.Y);
        }
        public static Vector2 RadiansToVector(float radians)
        {
            return new Vector2((float)Math.Sin(radians), -(float)Math.Cos(radians));
        }
        public static void RadiansToVector(float radians, ref Vector2 vector)
        {
            vector.X = (float)Math.Sin(radians);
            vector.Y = -(float)Math.Cos(radians);
        }
        public static void RotateVector(ref Vector2 vector, float radians)
        {
            float length = vector.Length();
            float newRadians = (float)Math.Atan2(vector.X, -(double)vector.Y) + radians;

            vector.X = (float)Math.Sin(newRadians) * length;
            vector.Y = -(float)Math.Cos(newRadians) * length;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t">Value between 0.0f and 1.0f.</param>
        /// <returns></returns>
        public static Vector2 LinearBezierCurve(Vector2 start, Vector2 end, float t)
        {
            return start + (end - start) * t;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="curve"></param>
        /// <param name="end"></param>
        /// <param name="t">Value between 0.0f and 1.0f.</param>
        /// <returns></returns>
        public static Vector2 QuadraticBezierCurve(Vector2 start, Vector2 curve, Vector2 end, float t)
        {
            _wayToGo = 1.0f - t;

            return _wayToGo * _wayToGo * start
                   + 2.0f * t * _wayToGo * curve
                   + t * t * end;
        }
        public static Vector2 QuadraticBezierCurve(Vector2 start, Vector2 curve, Vector2 end, float t, ref float radians)
        {
            _startCurve = start + (curve - start) * t;
            _curveEnd = curve + (end - curve) * t;
            _temp = _curveEnd - _startCurve;

            radians = (float)Math.Atan2(_temp.X, -(double)_temp.Y);
            return _startCurve + _temp * t;
        }
        public static Vector2 CubicBezierCurve2(Vector2 start, Vector2 startPointsTo, Vector2 end, Vector2 endPointsTo,
                                                float t)
        {
            return CubicBezierCurve(start, start + startPointsTo, end + endPointsTo, end, t);
        }
        public static Vector2 CubicBezierCurve2(Vector2 start, Vector2 startPointsTo, Vector2 end, Vector2 endPointsTo,
                                                float t, ref float radians)
        {
            return CubicBezierCurve(start, start + startPointsTo, end + endPointsTo, end, t, ref radians);
        }
        public static Vector2 CubicBezierCurve2(Vector2 start, float startPointDirection, float startPointLength,
                                                Vector2 end, float endPointDirection, float endPointLength,
                                                float t, ref float radians)
        {
            return CubicBezierCurve(start,
                                    RadiansToVector(startPointDirection) * startPointLength,
                                    RadiansToVector(endPointDirection) * endPointLength,
                                    end,
                                    t,
                                    ref radians);
        }
        public static Vector2 CubicBezierCurve(Vector2 start, Vector2 curve1, Vector2 curve2, Vector2 end, float t)
        {
            _tPow2 = t * t;
            _wayToGo = 1.0f - t;
            _wayToGoPow2 = _wayToGo * _wayToGo;

            return _wayToGo * _wayToGoPow2 * start
                   + 3.0f * t * _wayToGoPow2 * curve1
                   + 3.0f * _tPow2 * _wayToGo * curve2
                   + t * _tPow2 * end;
        }
        public static Vector2 CubicBezierCurve(Vector2 start, Vector2 curve1, Vector2 curve2, Vector2 end, float t,
                                               ref float radians)
        {
            return QuadraticBezierCurve(start + (curve1 - start) * t,
                                        curve1 + (curve2 - curve1) * t,
                                        curve2 + (end - curve2) * t,
                                        t,
                                        ref radians);
        }
        //Interpolate normal vectors ...
        public static Vector2 InterpolateNormal(Vector2 vector1, Vector2 vector2, float t)
        {
            vector1 += (vector2 - vector1) * t;
            vector1.Normalize();

            return vector1;
        }
        public static void InterpolateNormal(Vector2 vector1, Vector2 vector2, float t, out Vector2 vector)
        {
            vector = vector1 + (vector2 - vector1) * t;
            vector.Normalize();
        }
        public static void InterpolateNormal(ref Vector2 vector1, Vector2 vector2, float t)
        {
            vector1 += (vector2 - vector1) * t;
            vector1.Normalize();
        }
        public static float InterpolateRotation(float radians1, float radians2, float t)
        {
            Vector2 vector1 = new Vector2((float)Math.Sin(radians1), -(float)Math.Cos(radians1));
            Vector2 vector2 = new Vector2((float)Math.Sin(radians2), -(float)Math.Cos(radians2));

            vector1 += (vector2 - vector1) * t;
            vector1.Normalize();

            return (float)Math.Atan2(vector1.X, -(double)vector1.Y);
        }
        public static void ProjectToAxis(ref Vector2[] points, ref Vector2 axis, out float min, out float max)
        {
            // To project a point on an axis use the dot product
            axis.Normalize();
            float dotProduct = Vector2.Dot(axis, points[0]);
            min = dotProduct;
            max = dotProduct;

            for (int i = 0; i < points.Length; i++)
            {
                dotProduct = Vector2.Dot(points[i], axis);
                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else
                {
                    if (dotProduct > max)
                    {
                        max = dotProduct;
                    }
                }
            }
        }
        /// <summary>
        /// Calculate an orbit position.
        /// </summary>
        /// <param name="position">The center position of the orbit.</param>
        /// <param name="rotation">The current rotation around the center of the orbit.</param>
        /// <param name="offset">The offset from the orbit center.</param>
        /// <returns>The orbit position.</returns>
        public static Vector2 CalculateOrbitPosition(Vector2 position, float rotation, float offset)
        {
            return (RadiansToVector(rotation) * offset) + position;
        }
        /// <summary>
        /// Calculate an orbit position.
        /// </summary>
        /// <param name="position">The center position of the orbit.</param>
        /// <param name="rotation">The current rotation around the center of the orbit.</param>
        /// <param name="offset">The offset from the orbit center.</param>
        /// <returns>The orbit position.</returns>
        public static Vector2 CalculateOrbitPosition(Vector2 position, float rotation, Vector2 offset)
        {
            return (RadiansToVector(rotation) * offset) + position;
        }
        /// <summary>
        /// Calculate the angle between the center of the orbit and a position on the orbit.
        /// </summary>
        /// <param name="center">The center of the orbit.</param>
        /// <param name="orbit">The orbiting position.</param>
        /// <returns>The angle between the two positions.</returns>
        public static float CalculateAngleFromOrbitPosition(Vector2 center, Vector2 orbit)
        {
            //The direction.
            Vector2 direction = Vector2.Normalize(orbit - center);
            //Return the angle.
            return ((float)Math.Atan2(direction.X, -direction.Y));
        }
        /// <summary>
        /// Get the direction of a line.
        /// </summary>
        /// <param name="start">The starting position of the line.</param>
        /// <param name="end">The ending position of the line.</param>
        /// <returns>The direction of the line.</returns>
        public static Vector2 LineDirection(Vector2 start, Vector2 end)
        {
            //The direction of the line.
            Vector2 direction = Calculator.RadiansToVector(Calculator.CalculateAngleFromOrbitPosition(start, end));
            //Return the direction if valid; otherwise return an empty one.
            return (float.IsNaN(direction.X) || float.IsNaN(direction.Y)) ? Vector2.Zero : direction;
        }
        /// <summary>
        /// Add two angles.
        /// </summary>
        /// <param name="radian1">The first angle to add.</param>
        /// <param name="radian2">The second angle to add.</param>
        /// <returns>The angle sum.</returns>
        public static float AddAngles(float radian1, float radian2)
        {
            //Add the angles together.
            float addResult = radian1 + radian2;
            //Check if the sum of the angles has overreached a full lap, aka two PI, and if so fix it.
            if (addResult > (Math.PI * 2)) { return (addResult - ((float)Math.PI * 2)); }
            else { return addResult; }
        }
        /// <summary>
        /// Subtracts an angle from an angle.
        /// </summary>
        /// <param name="radian1">The angle to subtract from.</param>
        /// <param name="radian2">The angle to subtract.</param>
        /// <returns>The subtracted angle.</returns>
        public static float SubtractAngles(float radian1, float radian2)
        {
            //Subtract the angles from eachother.
            float subtractResult = radian1 - radian2;
            //If the difference has exceeded a full lap, aka 0, fix that.
            if (subtractResult < 0) { return (subtractResult + ((float)Math.PI * 2)); }
            else { return subtractResult; }
        }
        /// <summary>
        /// Get a vector with the absolute coordinates of the given vector.
        /// </summary>
        /// <param name="v">The vector to turn absolute.</param>
        /// <returns>The absolute vector.</returns>
        public static Vector3 Absolute(Vector3 v)
        {
            return new Vector3(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
        }
        /// <summary>
        /// Calculate the direction from an angle.
        /// </summary>
        /// <param name="angleRadians">The angle in radians.</param>
        /// <returns>The direction.</returns>
        public static Vector2 Direction(float angleRadians)
        {
            return new Vector2((float)-Math.Cos(angleRadians), (float)-Math.Sin(angleRadians));
        }
        /// <summary>
        /// Calculate the direction from a vector and a length.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="length">The length.</param>
        /// <returns>The direction of the vecor.</returns>
        public static Vector2 Direction(Vector2 v, float length)
        {
            return v / length;
        }
        /// <summary>
        /// Calculate the direction from a vector and a length.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="length">The length.</param>
        /// <returns>The direction of the vecor.</returns>
        public static Vector3 Direction(Vector3 v, float length)
        {
            return v / length;
        }
        /// <summary>
        /// Calculate the direction from this Vector using its own length.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns>The direction.</returns>
        public static Vector2 Direction(Vector2 v)
        {
            return Direction(v, Vector2.Distance(v, Vector2.Zero));
        }
        /// <summary>
        /// Calculate the direction from this Vector using its own length.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns>The direction.</returns>
        public static Vector3 Direction(Vector3 v)
        {
            return Direction(v, Vector3.Distance(v, Vector3.Zero));
        }
        /// <summary>
        /// Get the angle between two Vectors.
        /// </summary>
        /// <param name="faceThis">The orbit position.</param>
        /// <param name="position">The origin position.</param>
        /// <returns>The angle between the vectors.</returns>
        public static float GetAngle(Vector2 faceThis, Vector2 position)
        {
            return (float)Math.Atan2(faceThis.Y - position.Y, faceThis.X - position.X);
        }
        /// <summary>
        /// Get a vector perpendicular to this one. (x, y) => (-y, x); Note: This assumes clockwise ordering of vertices.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns>A perpendicular vector.</returns>
        public static Vector2 Perpendicular(Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }
        /// <summary>
        /// Calculate the centroid of a body.
        /// </summary>
        /// <param name="shape">The shape in which to calculate the centroid.</param>
        /// <returns>The centroid position.</returns>
        public static Vector2 ToCentroid(Rectangle shape)
        {
            return new Vector2(shape.X + (shape.Width / 2), shape.Y + (shape.Height / 2));
        }
        /// <summary>
        /// Calculate the centroid of a body.
        /// </summary>
        /// <param name="v">The top-left corner position of the body.</param>
        /// <param name="width">The width of the body.</param>
        /// <param name="height">The height of the body.</param>
        /// <returns>The centroid position.</returns>
        public static Vector2 ToCentroid(Vector2 v, float width, float height)
        {
            return new Vector2(v.X + (width / 2), v.Y + (height / 2));
        }
        /// <summary>
        /// Calculate the centroid of a body.
        /// </summary>
        /// <param name="x">The x-coordinate of the body's top-left corner.</param>
        /// <param name="y">The y-coordinate of the body's top-left corner.</param>
        /// <param name="width">The width of the body.</param>
        /// <param name="height">The height of the body.</param>
        /// <returns>The centroid position.</returns>
        public static Vector2 ToCentroid(float x, float y, float width, float height)
        {
            return new Vector2(x + (width / 2), y + (height / 2));
        }
        /// <summary>
        /// Convert from a centroid vector a top-left vector.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns>The top-left position of the shape.</returns>
        public static Vector2 ToTopLeft(Rectangle shape)
        {
            return new Vector2(shape.X - (shape.Width / 2), shape.Y - (shape.Height / 2));
        }
        /// <summary>
        /// Convert from a centroid vector a top-left vector.
        /// </summary>
        /// <param name="shape">The shape to use.</param>
        /// <returns>The position of the shape converted into a top-left format, ignoring rotation.</returns>
        public static Vector2 ToTopLeft(Shape shape)
        {
            return new Vector2(shape.Position.X - (shape.Width / 2), shape.Position.Y - (shape.Height / 2));
        }
        /// <summary>
        /// Convert from a centroid vector a top-right vector.
        /// </summary>
        /// <param name="shape">The shape to use.</param>
        /// <returns>The position of the shape converted into a top-right format, ignoring rotation.</returns>
        public static Vector2 ToTopRight(Shape shape)
        {
            return new Vector2(shape.Position.X + (shape.Width / 2), shape.Position.Y - (shape.Height / 2));
        }
        /// <summary>
        /// Convert from a centroid vector a bottom-left vector.
        /// </summary>
        /// <param name="shape">The shape to use.</param>
        /// <returns>The position of the shape converted into a bottom-left format, ignoring rotation.</returns>
        public static Vector2 ToBottomLeft(Shape shape)
        {
            return new Vector2(shape.Position.X - (shape.Width / 2), shape.Position.Y + (shape.Height / 2));
        }
        /// <summary>
        /// Convert from a centroid vector a bottom-right vector.
        /// </summary>
        /// <param name="shape">The shape to use.</param>
        /// <returns>The position of the shape converted into a bottom-right format, ignoring rotation.</returns>
        public static Vector2 ToBottomRight(Shape shape)
        {
            return new Vector2(shape.Position.X + (shape.Width / 2), shape.Position.Y + (shape.Height / 2));
        }
        /// <summary>
        /// Convert from a centroid vector a top-left vector.
        /// </summary>
        /// <param name="x">The x-coordinate of the body's centroid.</param>
        /// <param name="y">The y-coordinate of the body's centroid.</param>
        /// <param name="width">The width of the body.</param>
        /// <param name="height">The height of the body.</param>
        /// <returns>The position of the shape converted into a top-left format, ignoring rotation.</returns>
        public static Vector2 ToTopLeft(float x, float y, float width, float height)
        {
            return new Vector2(x - (width / 2), y - (height / 2));
        }
        /// <summary>
        /// Convert from a centroid vector a top-left vector.
        /// </summary>
        /// <param name="v">The top-left corner position of the body.</param>
        /// <param name="width">The width of the body.</param>
        /// <param name="height">The height of the body.</param>
        /// <returns>The position of the shape converted into a top-left format, ignoring rotation.</returns>
        public static Vector2 ToTopLeft(Vector2 v, float width, float height)
        {
            return new Vector2(v.X - (width / 2), v.Y - (height / 2));
        }
        /// <summary>
        /// Rotate a vector around a point.
        /// </summary>
        /// <param name="position">The vector to rotate.</param>
        /// <param name="origin">The origin of the rotation.</param>
        /// <param name="rotation">The amount of rotation in radians.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2 RotateVector(Vector2 position, Vector2 origin, float rotation)
        {
            return new Vector2((float)(origin.X + (position.X - origin.X) * Math.Cos(rotation) - (position.Y - origin.Y) * Math.Sin(rotation)), (float)(origin.Y
            + (position.Y - origin.Y) * Math.Cos(rotation) + (position.X - origin.X) * Math.Sin(rotation)));
        }
        /// <summary>
        /// See if this vector overlaps another. Note: Only works for one-dimensional vectors, ie. lines.
        /// </summary>
        /// <param name="v1">The first vector to check.</param>
        /// <param name="v2">The second vector to check.</param>
        /// <returns>Whether the two vectors overlap.</returns>
        public static bool IsOverlapping(Vector2 v1, Vector2 v2)
        {
            return IsOverlapping(v1, v2, false);
        }
        /// <summary>
        /// See if this vector overlaps another. Note: Only works for one-dimensional vectors, ie. lines.
        /// </summary>
        /// <param name="v1">The first vector to check.</param>
        /// <param name="v2">The second vector to check.</param>
        /// <param name="margin">Whether to allow margin overlaps, ie. same end-point position.</param>
        /// <returns>Whether the two vectors overlap.</returns>
        public static bool IsOverlapping(Vector2 v1, Vector2 v2, bool margin)
        {
            return (margin) ? Math.Max(v1.X, v1.Y) >= Math.Min(v2.X, v2.Y) && Math.Max(v2.X, v2.Y) >= Math.Min(v1.X, v1.Y)
                : Math.Max(v1.X, v1.Y) > Math.Min(v2.X, v2.Y) && Math.Max(v2.X, v2.Y) > Math.Min(v1.X, v1.Y);
        }
        /// <summary>
        /// Get the middle values of two overlapping vectors. Example: (3, -1) and (0, 9) yields (0, 3).
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The middle values of the two vectors. A negative vector is returned if there is no overlap between the vectors.</returns>
        public static Vector2 GetMiddleValues(Vector2 v1, Vector2 v2)
        {
            // If no overlap, quit here.
            if (!IsOverlapping(v1, v2)) { return new Vector2(-1, -1); }

            // Port the vectors to an array and sort it.
            float[] v = new float[] { v1.X, v1.Y, v2.X, v2.Y };
            Array.Sort(v);

            // Return the middle values.
            return new Vector2(v[1], v[2]);
        }
        /// <summary>
        /// Get the overlap between the vectors. Example: (3, -1) and (0, 9) yields 3-0 = 3.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The overlap between the two vectors. -1 if there is no overlap.</returns>
        public static float GetOverlap(Vector2 v1, Vector2 v2)
        {
            // If no overlap, quit here.
            if (!IsOverlapping(v1, v2)) { return -1; }

            // Port the vectors to an array and sort it.
            float[] v = new float[] { v1.X, v1.Y, v2.X, v2.Y };
            Array.Sort(v);

            // Return the difference of the middle values.
            return Math.Max(v[1], v[2]) - Math.Min(v[1], v[2]);
        }
        /// <summary>
        /// Compare two vectors by their values. The vectors need to be in the order of (x = min, y = max). Allows margin overlap, ie. same end-point position.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>1 if the first vector is greater, -1 if the second vector is greater and 0 if the vectors overlap.</returns>
        public static int CheckOverlap(Vector2 v1, Vector2 v2)
        {
            // Compare the vectors to each other.
            if (!IsOverlapping(v1, v2, false))
            {
                if (v1.X > v2.Y) { return 1; }
                else if (v2.X > v1.Y) { return -1; }
            }

            // The vectors overlap, return 0.
            return 0;
        }
    }
}