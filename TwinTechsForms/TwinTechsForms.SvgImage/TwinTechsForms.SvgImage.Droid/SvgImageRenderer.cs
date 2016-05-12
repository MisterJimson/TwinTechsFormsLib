using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.Runtime;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using NGraphics;
using TwinTechs;
using TwinTechs.Droid;
using TwinTechsForms.SvgImage.Droid;
using Color = Xamarin.Forms.Color;
using Size = NGraphics.Size;

[assembly: ExportRenderer (typeof(SvgImage), typeof(SvgImageRenderer))]
namespace TwinTechs.Droid
{
	[Preserve (AllMembers = true)]
	public class SvgImageRenderer : ViewRenderer<SvgImage, ImageView>
	{
	    private Paint paint;
	    private static List<SavedBitmap> savedBitmaps = new List<SavedBitmap>();

        public new static void Init ()
		{
			var temp = DateTime.Now;
		}

		public SvgImageRenderer ()
		{
            paint = new Paint();

            // Offer to do our own drawing so Android will actually call `Draw`.
            SetWillNotDraw (willNotDraw: false);
		}

		private SvgImage _formsControl {
			get {
				return Element as SvgImage;
			}
		}

		// Don't need to deal with screen scaling on Android.
		const double ScreenScale = 1.0;

		public override void Draw (Android.Graphics.Canvas canvas)
		{
			base.Draw (canvas);
		    if (_formsControl == null) return;

		    var savedBitmap = savedBitmaps.FirstOrDefault(x => x.SvgPath == _formsControl.SvgPath &&
		                                                       x.HeightRequest.Equals(_formsControl.HeightRequest) &&
		                                                       x.WidthRequest.Equals(_formsControl.WidthRequest) &&
		                                                       x.TintColor == _formsControl.TintColor);
		    Bitmap bitmap = null;
		    if (savedBitmap != null)
		    {
		        bitmap = savedBitmap.Bitmap;
		    }
		    else
		    {
		        var outputSize = new Size(canvas.Width, canvas.Height);
		        var finalCanvas = _formsControl.RenderSvgToCanvas(outputSize, ScreenScale, CreatePlatformImageCanvas);
		        var image = (BitmapImage) finalCanvas.GetImage();
		        bitmap = image.Bitmap;

		        if (_formsControl.TintColor != Color.Default)
		        {
		            var filter = new PorterDuffColorFilter(_formsControl.TintColor.ToAndroid(), PorterDuff.Mode.SrcIn);
		            paint.SetColorFilter(filter);
		            Canvas colorCanvas = new Canvas(bitmap);
                    colorCanvas.DrawBitmap(bitmap, 0, 0, paint);

                    savedBitmaps.Add(new SavedBitmap()
                    {
                        SvgPath = _formsControl.SvgPath,
                        HeightRequest = _formsControl.HeightRequest,
                        WidthRequest = _formsControl.WidthRequest,
                        TintColor = _formsControl.TintColor,
                        Bitmap = bitmap
                    });
		        }
		    }

		    Control.SetImageBitmap(bitmap);
		}

		protected override void OnElementChanged (ElementChangedEventArgs<SvgImage> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement != null) {
				(e.OldElement as SvgImage).OnInvalidate -= HandleInvalidate;
			}

			if (e.NewElement != null) {
				(e.NewElement as SvgImage).OnInvalidate += HandleInvalidate;
			}

			Invalidate ();

			if (_formsControl != null) {
				Device.BeginInvokeOnMainThread (() => {
					var imageView = new ImageView (Context);

					imageView.SetScaleType (ImageView.ScaleType.FitXy);

					// TODO: ?Reuse existing Control instead?
					SetNativeControl (imageView);
					Invalidate ();
				});
			}
		}

		public override SizeRequest GetDesiredSize (int widthConstraint, int heightConstraint)
		{
			return new SizeRequest (new Xamarin.Forms.Size (_formsControl.WidthRequest, _formsControl.WidthRequest));
		}

		static Func<Size, double, IImageCanvas> CreatePlatformImageCanvas = (size, scale) => new AndroidPlatform ().CreateImageCanvas (size, scale);

		/// <summary>
		/// Handles view invalidate.
		/// </summary>
		void HandleInvalidate (object sender, System.EventArgs args)
		{
			Invalidate ();
		}

		/// <summary>
		/// http://stackoverflow.com/questions/24465513/how-to-get-detect-screen-size-in-xamarin-forms
		/// </summary>
		/// <param name="pixel"></param>
		/// <returns></returns>
		private int PixelToDP (int pixel)
		{
			var scale = Resources.DisplayMetrics.Density;
			return (int)((pixel * scale) + 0.5f);
		}
	}
}