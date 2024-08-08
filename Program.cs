using System;
using System.Collections;
using System.Windows.Forms;

// Demo Saver -- Main() entry-point for executable.
// Windows Screen Saver in C# using GDI+
// Written by Taber Henderson
// 13 October 2012

// Note: One may simply rename output exe to *.scr, and then place within the
//       Windows system directory to be found by the OS Screen Saver Settings.

namespace DemoSaver
{
   public class Program
   {
      enum ScreenSaverStates
      {
         Normal,
         Config,
         Preview,
         Test
      }

      struct ScreenSaverArgs
      {
         public ScreenSaverStates SaverMode;
         public IntPtr TargetHandle;
      }

      static ScreenSaverArgs ParseSaverArgs( string[] arguments )
      {
         ScreenSaverArgs saverArgs = new ScreenSaverArgs();

         if( arguments.Length > 0 )
         {
            string temp = string.Empty;
            foreach( string arg in arguments )
               temp += arg.Trim().ToLower();

            if( temp.IndexOf( "/c" ) > -1 )
               saverArgs.SaverMode = ScreenSaverStates.Config;
            else if( temp.IndexOf( "/p" ) > -1 )
               saverArgs.SaverMode = ScreenSaverStates.Preview;
            else if( temp.IndexOf( "/s" ) > -1 )
               saverArgs.SaverMode = ScreenSaverStates.Test;

            // see if a window handle has been passed
            string handle = string.Empty;
            for( int i = 0; i < temp.Length; ++i )
               if( char.IsDigit( temp[i] ) )
                  handle += temp[i];

            if( handle != string.Empty )
               saverArgs.TargetHandle = new IntPtr( int.Parse( handle ) );
         }

         return saverArgs;
      }

      [STAThread]
      static void Main( string[] args )
      {
         ScreenSaverArgs saverArgs = ParseSaverArgs( args );

         // create screen saver visualizer instance, will perform drawing on target surface
         IScreenSaverVisualizer saverVisualizer = new ScreenSaverVisualizer();

         // create screen saver host instance
         ScreenSaverWindow screenSaverWin = new ScreenSaverWindow( saverVisualizer );

         bool runSaver = false;
         switch( saverArgs.SaverMode )
         {
            case ScreenSaverStates.Normal:
            case ScreenSaverStates.Test:
               runSaver = true;
               break;

            case ScreenSaverStates.Preview:
               runSaver = true;
               if( saverArgs.TargetHandle != IntPtr.Zero )
               {
                  // set custom parent window for screen saver window to appear within
                  screenSaverWin.SetSaverPreviewMode( saverArgs.TargetHandle );
               }
               break;

            case ScreenSaverStates.Config:
               MessageBox.Show( "No configuration available.", "Test Saver" );
               break;
         }

         if( runSaver )
            Application.Run( screenSaverWin );
      }
   }

}