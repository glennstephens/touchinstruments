using System;
using SpriteKit;
using UIKit;

namespace PianoTouch
{
	public partial class GameViewController : UIViewController
	{
		public GameViewController (IntPtr handle) : base (handle)
		{
			
		}

		public static MidiControl MidiControl = new MidiControl();

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.MultipleTouchEnabled = true;

			// Configure the view.
			var skView = (SKView)View;
			skView.ShowsFPS = false;
			skView.ShowsNodeCount = false;

			/* Sprite Kit applies additional optimizations to improve rendering performance */
			skView.IgnoresSiblingOrder = true;

			// Create and configure the scene.
			var scene = SKNode.FromFile<GameScene> ("GameScene");
			scene.ScaleMode = SKSceneScaleMode.AspectFill;

			// Present the scene.
			skView.PresentScene (scene);
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? UIInterfaceOrientationMask.AllButUpsideDown : UIInterfaceOrientationMask.All;
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}
	}
}

