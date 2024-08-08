using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace DemoSaver
{
   class ScreenSaverWindow : Form
   {
      #region Fields and Properties

      IScreenSaverVisualizer visualizer;

      // track initial mouse state
      bool initialPointSet;
      Point initPoint;

      Timer timer;
      long lastTime;

      bool enablePreviewMode;

      bool enableInfoDisplay;
      float infoDisplayElapsedCount;
      string infoDisplayText;
      int infoDisplayFrameCount;

      #endregion Fields and Properties

      #region Construction

      public ScreenSaverWindow( IScreenSaverVisualizer saverVisualizer )
      {
         // set screen saver visualizer instance
         this.visualizer = saverVisualizer;

         // set default window options
         this.BackColor = Color.Black;
         this.FormBorderStyle = FormBorderStyle.None;

         // TODO: Currently, the entire desktop screen area will be filled with a single (large) window,
         //       in case of multi-monitor setup. Perhaps in the future consider creation of multiple windows
         //       for each monitor, but that would complicate the behavior of the visualizer. So for now,
         //       just use one big window.

         this.SetStyle( ControlStyles.UserPaint, true );
         this.SetStyle( ControlStyles.AllPaintingInWmPaint, true );
         this.SetStyle( ControlStyles.OptimizedDoubleBuffer, true );
         this.SetStyle( ControlStyles.Opaque, true );
      }

      #endregion Construction

      #region Form Overrides

      protected override void OnLoad( EventArgs e )
      {
         if( !this.enablePreviewMode )
         {
            // if debug, size window to fit one monitor at aspect ratio of entire desktop for multi-monitor system..?
            // (and/or, enable form border style so window can simply be resized and moved..?)
   #if DEBUG
            // set window to fill only current screen
            this.WindowState = FormWindowState.Maximized;
            //*/
   #else
            // set window to cover entire desktop area
            this.StartPosition = FormStartPosition.Manual;
            this.Location = SystemInformation.VirtualScreen.Location;
            this.Size = SystemInformation.VirtualScreen.Size;
            //*/
   #endif

            this.TopMost = true;
            Cursor.Hide();
         }

         // initialize screen saver visualizer with correct window size
         this.visualizer.Initialize( this.Size, this.enablePreviewMode );

         // set initial time stamp for elapsed time counter
         this.lastTime = System.Diagnostics.Stopwatch.GetTimestamp();

         // begin drawing loop
         SaverStart();
      }

      protected override void OnMouseDown( MouseEventArgs e )
      {
         if( this.enablePreviewMode )
            return;

         // request the screen saver to exit
         SaverQuit();
      }

      protected override void OnMouseMove( MouseEventArgs e )
      {
         if( this.enablePreviewMode )
            return;

         const int moveDist = 5;

         if( !this.initialPointSet )
         {
            this.initPoint = new Point( e.X, e.Y );
            this.initialPointSet = true;
         }
         else if( Math.Abs( initPoint.X - e.X ) > moveDist || Math.Abs( initPoint.Y - e.Y ) > moveDist || e.Clicks > 0 )
         {
            // request the screen saver to exit
            SaverQuit();
         }
      }

      protected override void OnKeyDown( KeyEventArgs e )
      {
         if( this.enablePreviewMode )
            return;

         bool keyHandled = false;

         switch( e.KeyCode )
         {
            case Keys.I:
               this.enableInfoDisplay = !this.enableInfoDisplay;
               this.infoDisplayElapsedCount = 1f; // set 1 second to force update
               keyHandled = true;
               break;
         }

         if( !keyHandled )
         {
            // request the screen saver to exit
            SaverQuit();
         }
      }

      protected override void OnPaint( PaintEventArgs e )
      {
         if( this.enablePreviewMode )
         {
            // need to manually clear to back color when in preview mode -- window is hosted by the OS control panel
            e.Graphics.Clear( this.BackColor );
         }

         // let visualizer draw to window
         this.visualizer.Draw( e.Graphics );

         if( this.enableInfoDisplay )
         {
            e.Graphics.DrawString( this.infoDisplayText, this.Font, Brushes.White, 16, 16 );
         }
      }

      protected override void OnPaintBackground( PaintEventArgs e )
      {
         // no painting for background
      }

      #endregion Form Overrides

      #region Screen Saver Timer Methods

      void SaverStart()
      {
         if( this.timer != null )
         {
            this.timer.Stop();
            this.timer.Dispose();
         }
         this.timer = new Timer();

         this.timer.Interval = 30; // !this.previewMode ? 30 : 100; // run half frame rate if in preview mode..?
         this.timer.Tick += Timer_Tick;
         this.timer.Enabled = true;
      }

      void SaverQuit()
      {
         if( this.timer != null )
         {
            this.timer.Stop();
            this.timer.Dispose();
         }
         Application.Exit();
      }

      void Timer_Tick( object sender, EventArgs e )
      {
         long currTime = System.Diagnostics.Stopwatch.GetTimestamp();
         float elasped = (float)( ( currTime - lastTime ) / (double)System.Diagnostics.Stopwatch.Frequency );
         lastTime = currTime;

         if( this.enableInfoDisplay )
         {
            // increment elapsed time and frame counts
            this.infoDisplayElapsedCount += elasped;
            ++this.infoDisplayFrameCount;
            // check if current info string should be update
            if( this.infoDisplayElapsedCount >= 1f ) // fixed 1 second update interval
            {
               this.infoDisplayText = string.Format( "FPS: {0} \nMem: {1:N0}K", this.infoDisplayFrameCount, GC.GetTotalMemory( false ) / 1000 );
               // reset elapsed time and frame counts
               this.infoDisplayElapsedCount = 0f;
               this.infoDisplayFrameCount = 0;
            }
         }

         this.visualizer.Update( elasped );

         // request repaint now
         this.Invalidate();
      }

      #endregion Screen Saver Timer Methods

      #region Public Methods

      public void SetSaverPreviewMode( IntPtr parentWindowHandle )
      {
         // Set the preview window of the screen saver selection dialog in Windows as the parent of this form.
         SetParent( this.Handle, parentWindowHandle );

         // Set this form to a child form, so that when the screen saver selection dialog in Windows is closed, this form will also close.
         SetWindowLong( this.Handle, -16, new IntPtr( GetWindowLong( this.Handle, -16 ) | 0x40000000 ) );
         // TODO: lookup up Win32 SetWindowLong method and figure out what -16 value represents, then define as aptly named const...

         // Set the size of the screen saver to the size of the screen saver preview window in the screen saver selection dialog in Windows.
         Rectangle parentRect;
         GetClientRect( parentWindowHandle, out parentRect );
         this.Size = parentRect.Size;
         this.Location = Point.Empty;

         // set internal flag to indicate when running in preview, as we don't want to exit on mouse moves or buttons...
         this.enablePreviewMode = true;
      }

      #endregion Public Methods

      #region Win API Access

      // checks if the window associated with the given handle is currently visible
      [System.Runtime.InteropServices.DllImport( "user32.dll" )]
      private static extern bool IsWindowVisible( int hWnd );

      // Changes the parent window of the specified child window
      [System.Runtime.InteropServices.DllImport( "user32.dll" )]
      private static extern IntPtr SetParent( IntPtr hWndChild, IntPtr hWndNewParent );

      // Changes an attribute of the specified window
      [System.Runtime.InteropServices.DllImport( "user32.dll" )]
      private static extern int SetWindowLong( IntPtr hWnd, int nIndex, IntPtr dwNewLong );

      // Retrieves information about the specified window
      [System.Runtime.InteropServices.DllImport( "user32.dll", SetLastError = true )]
      private static extern int GetWindowLong( IntPtr hWnd, int nIndex );

      // Retrieves the coordinates of a window's client area
      [System.Runtime.InteropServices.DllImport( "user32.dll" )]
      private static extern bool GetClientRect( IntPtr hWnd, out Rectangle lpRect );

      #endregion Win API Access
   }

}