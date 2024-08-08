using System;
using System.Collections;
using System.Drawing;

namespace DemoSaver
{
   class ScreenSaverVisualizer : ScreenSaverBase
   {
      struct Particle
      {
         public Vector2 Position;
         public Vector2 Direction;
         public float Speed;
         public float Size;
         public Color Color;
         public float LifeSpan;
      }

      Particle[] particles;

      Random rand;

      int maxParticles = 144;

      float defaultSpeed = 100f; // units (pixels) per second
      float defaultAcceleration = 100f; // units (pixels) per second per second
      float defaultSize = 3f; // units (pixels)
      float defaultLife = 3f; // seconds

      Vector2 gravity = new Vector2( 0f, -100f );

      Vector2 axisRestraint = new Vector2( 0f, 1f );

      public ScreenSaverVisualizer()
      {
      }

      void GenerateParticle( ref Particle particle )
      {
         particle.Position = new Vector2( this.rand.Next( this.DisplaySize.Width ), this.rand.Next( this.DisplaySize.Height ) );
         particle.Direction = GetRandDir();
         particle.Speed = this.defaultSpeed + (float)( this.rand.NextDouble() * this.defaultSpeed );
         particle.Size = this.defaultSize + (float)( this.rand.NextDouble() * this.defaultSize );
         particle.Color = GetRandShade( 32 );
         //particle.Color = GetRandColor( 32 );
         particle.LifeSpan = 0.5f + (float)( this.rand.NextDouble() * this.defaultLife );
      }

      Vector2 GetRandDir()
      {
         double randAngle = Math.PI * 2.0 * this.rand.NextDouble();
         return new Vector2( (float)Math.Cos( randAngle ), (float)Math.Sin( randAngle ) );
      }

      Color GetRandShade( int min )
      {
         int brightness = this.rand.Next( min, 256 );
         return Color.FromArgb( brightness, brightness, brightness );
      }

      Color GetRandColor( int min )
      {
         int r = this.rand.Next( min, 256 );
         int g = this.rand.Next( min, 256 );
         int b = this.rand.Next( min, 256 );
         return Color.FromArgb( r, g, b );
      }

      #region ScreenSaverBase

      public override void InitializeVisualizer()
      {
         this.maxParticles = Math.Max( this.DisplaySize.Width, this.DisplaySize.Height ) / 10;

         this.rand = new Random();

         this.particles = new Particle[this.maxParticles];
         for( int i = 0; i < particles.Length; ++i )
         {
            GenerateParticle( ref particles[i] );
         }
      }

      public override void Update( float elapsed )
      {
         Vector2 offset;
         for( int i = 0; i < particles.Length; ++i )
         {
            if( this.particles[i].LifeSpan > 0f )
            {
               offset = this.particles[i].Direction * this.axisRestraint * ( this.particles[i].Speed * elapsed );

               particles[i].Position += offset;
               particles[i].Speed += this.defaultAcceleration * elapsed;

               this.particles[i].LifeSpan -= elapsed;
            }
            else
            {
               GenerateParticle( ref this.particles[i] );
            }
         }
      }

      SolidBrush particleBrush = new SolidBrush( Color.White );

      public override void Draw( Graphics grfx )
      {
         for( int i = 0; i < particles.Length; ++i )
         {
            if( this.particles[i].LifeSpan > 0f )
            {
               this.particleBrush.Color = this.particles[i].Color;
               grfx.FillRectangle( this.particleBrush, this.particles[i].Position.X, this.particles[i].Position.Y, this.particles[i].Size, this.particles[i].Size );
            }
         }
      }

      #endregion ScreenSaverBase
   }

}