using System;
using System.IO;
using System.Drawing;

namespace Bespoke.Common.LinearAlgebra
{
	/// <summary>
	/// 
	/// </summary>
	public struct Vector2
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="location"></param>
		public Vector2(Point location)
			: this(location.X, location.Y)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		#region Operators

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Vector2 operator -(Vector2 value)
		{
			return new Vector2(-value.X, -value.Y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
		{
			Vector2 vector;
			vector.X = lhs.X + rhs.X;
			vector.Y = lhs.Y + rhs.Y;

			return vector;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
		{
			Vector2 vector;
			vector.X = lhs.X - rhs.X;
			vector.Y = lhs.Y - rhs.Y;

			return vector;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scaleFactor"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Vector2 operator *(float scaleFactor, Vector2 value)
		{
			return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		/// <summary>
		/// Compares a vector for equality with another vector.
		/// </summary>
		/// <param name="lhs">Source vector.</param>
		/// <param name="rhs">Source vector.</param>
		/// <returns>true if the vectors are equal; false otherwise.</returns>
		public static bool operator ==(Vector2 lhs, Vector2 rhs)
		{
			return (lhs.X == rhs.X && lhs.Y == rhs.Y);
		}

		/// <summary>
		/// Tests a vector for inequality with another vector.
		/// </summary>
		/// <param name="lhs">The vector on the left of the equal sign.</param>
		/// <param name="rhs">The vector on the right of the equal sign.</param>
		/// <returns>true if the vectors are not equal; false otherwise.</returns>
		public static bool operator !=(Vector2 lhs, Vector2 rhs)
		{
			return !(lhs == rhs);
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static float Dot(Vector2 lhs, Vector2 rhs)
		{
			return ((lhs.X * rhs.X) + (lhs.Y * rhs.Y));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static float Distance(Vector2 lhs, Vector2 rhs)
		{
			float deltaX = lhs.X - rhs.X;
			float deltaY = lhs.Y - rhs.Y;
			float distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);

			return (float)Math.Sqrt((double)distanceSquared);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Vector2 Normalize(Vector2 vector)
		{
			Vector2 normalizedVector;

			float inverseLength = 1.0f / vector.Length();
			normalizedVector.X = vector.X * inverseLength;
			normalizedVector.Y = vector.Y * inverseLength;
			
			return normalizedVector;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromVector"></param>
        /// <param name="toVector"></param>
        /// <returns></returns>
        public static float GetAngle(Vector2 fromVector, Vector2 toVector)
        {
            fromVector.Normalize();
            toVector.Normalize();

            float angle = (float)Math.Acos(Vector2.Dot(fromVector, toVector));

            if (toVector.X - fromVector.X < 0.0f)
            {
                angle *= -1;
            }

            return angle;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static Vector2 Load(BinaryReader reader)
		{
			Assert.ParamIsNotNull("reader", reader);

			float x = reader.ReadSingle();
			float y = reader.ReadSingle();

			return new Vector2(x, y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Save(BinaryWriter writer)
		{
			Assert.ParamIsNotNull("writer", writer);

			writer.Write(X);
			writer.Write(Y);
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the Vector.
		/// </summary>
		/// <param name="other">The System.Object to compare with the current Vector.</param>
		/// <returns>true if the specified System.Object is equal to the current Vector; false otherwise.</returns>
		public override bool Equals(object other)
		{
			if (!(other is Vector2))
			{
				return false;
			}

			return this == (Vector2)other;
		}

		/// <summary>
		/// Gets the hash code of this object.
		/// </summary>
		/// <returns>Hash code of this object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Normalize()
		{
			float inverseLength = 1.0f / Length();
			X *= inverseLength;
			Y *= inverseLength;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public float Length()
		{
			double lengthSquared = (X * X) + (Y * Y);
			return (float)Math.Sqrt(lengthSquared);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

		/// <summary>
		/// 
		/// </summary>
		public static readonly Vector2 Zero = new Vector2(0.0f, 0.0f);

		/// <summary>
		/// 
		/// </summary>
		public float X;

		/// <summary>
		/// 
		/// </summary>
		public float Y;
	}
}
