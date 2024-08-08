using System;
using System.Drawing;

namespace DemoSaver
{
   /// <summary>
   /// Two component floating point vector structure.
   /// </summary>
   struct Vector2
   {
      public float X;
      public float Y;

      public Vector2( float x, float y )
      {
         this.X = x;
         this.Y = y;
      }

      public float Length
      {
         get { return (float)Math.Sqrt( this.X * this.X + this.Y * this.Y ); }
      }

      public void Normalize()
      {
         float len = this.Length;
         this.X /= len;
         this.Y /= len;
      }

      public bool IsEmpty()
      {
         return this.X == 0f && this.Y == 0f;
      }

      public float Dot( Vector2 source )
      {
         return this.X * source.X + this.Y * source.Y;
      }

      public void Scale( float size )
      {
         this.X *= size;
         this.Y *= size;
      }

      public void Scale( float x, float y )
      {
         this.X *= x;
         this.Y *= y;
      }

      public void Translate( float x, float y )
      {
         this.X += x;
         this.Y += y;
      }

      public void Translate( Vector2 vector )
      {
         this.X += vector.X;
         this.Y += vector.Y;
      }

      public void Rotate( float angle )
      {
         this = Rotate( this, angle );
      }

      public override string ToString()
      {
         return string.Format( "{0}, {1}", this.X, this.Y );
      }

      public static Vector2 Rotate( Vector2 vector, float angle )
      {
         Vector2 vec = new Vector2();
         double cos = Math.Cos( angle );
         double sin = Math.Sin( angle );
         vec.X = (float)( vector.X * cos - vector.Y * sin );
         vec.Y = (float)( vector.X * sin + vector.Y * cos );
         return vec;
      }

      public static Vector2 Scale( Vector2 vector, float size )
      {
         vector.Scale( size );
         return vector;
      }

      public static Vector2 Normalize( Vector2 vector )
      {
         vector.Normalize();
         return vector;
      }

      public static float Distance( Vector2 v1, Vector2 v2 )
      {
         float dx = v1.X - v2.X;
         float dy = v1.Y - v2.Y;
         float dist = (float)Math.Sqrt( dx * dx + dy * dy );
         return dist;
      }

      public static float DistanceSq( Vector2 v1, Vector2 v2 )
      {
         float dx = v1.X - v2.X;
         float dy = v1.Y - v2.Y;
         float distSq = dx * dx + dy * dy;
         return distSq;
      }

      public static Vector2 operator +( Vector2 left, Vector2 right )
      {
         return new Vector2( left.X + right.X, left.Y + right.Y );
      }

      public static Vector2 operator -( Vector2 left, Vector2 right )
      {
         return new Vector2( left.X - right.X, left.Y - right.Y );
      }


      public static Vector2 operator -( Vector2 vec )
      {
         return new Vector2( -vec.X, -vec.Y );
      }


      public static Vector2 operator *( Vector2 left, Vector2 right )
      {
         return new Vector2( left.X * right.X, left.Y * right.Y );
      }

      public static Vector2 operator *( Vector2 vector, float scale )
      {
         return new Vector2( vector.X * scale, vector.Y * scale );
      }

      public static Vector2 operator /( Vector2 left, Vector2 right )
      {
         return new Vector2( left.X / right.X, left.Y / right.Y );
      }

      public static Vector2 operator /( Vector2 vector, float scale )
      {
         return new Vector2( vector.X / scale, vector.Y / scale );
      }


      public static readonly Vector2 Zero = new Vector2();
      public static readonly Vector2 UnitX = new Vector2( 1f, 0f );
      public static readonly Vector2 UnitY = new Vector2( 0f, 1f );


      /// <summary> Converts from a System.Drawing.PointF. </summary>
      public static implicit operator Vector2( PointF point )
      {
         return new Vector2( point.X, point.Y );
      }

      /// <summary> Converts to a System.Drawing.PointF. </summary>
      public static implicit operator PointF( Vector2 vector )
      {
         return new PointF( vector.X, vector.Y );
      }

   }

}