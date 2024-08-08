using System;
using System.Drawing;

// Demo Saver -- Interface and abstract base implementation for a screen saver visualizer.
// Windows Screen Saver in C# using GDI+
// Written by Taber Henderson
// 13 October 2012

namespace DemoSaver
{
   interface IScreenSaverVisualizer
   {
      void Initialize( Size screenSize, bool isPreview );

      void Draw( Graphics grfx );

      void Update( float elapsed );

      // future support edit configuration request
      //void ShowConfig();
   }

   public abstract class ScreenSaverBase : IScreenSaverVisualizer
   {
      Size displaySize;
      public Size DisplaySize { get { return this.displaySize; } }

      bool displayInPreview;
      public bool DisplayInPreview { get { return this.displayInPreview; } }

      public void Initialize( Size screenSize, bool isPreview )
      {
         this.displaySize = screenSize;
         this.displayInPreview = isPreview;

         InitializeVisualizer();
      }

      public abstract void InitializeVisualizer();

      public abstract void Draw( Graphics grfx );

      public abstract void Update( float elapsed );

      /*/ future ScreenSaverBase helper methods to load/save configurations
      private static void ReadSettings()
      {
         //Microsoft.Win32.Registry...
      }

      private static void WriteSettings()
      {
         //Microsoft.Win32.Registry...
      }
      //*/
   }

}