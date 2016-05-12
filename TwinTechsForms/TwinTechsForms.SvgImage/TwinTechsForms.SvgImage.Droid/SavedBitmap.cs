using Android.Graphics;
using Color = Xamarin.Forms.Color;

namespace TwinTechsForms.SvgImage.Droid
{
    public class SavedBitmap
    {
        public string SvgPath { get; set; }
        public double HeightRequest { get; set; }
        public double WidthRequest { get; set; }
        public Color TintColor { get; set; }
        public Bitmap Bitmap { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as SavedBitmap;

            if (item == null)
            {
                return false;
            }

            return (SvgPath == item.SvgPath &&
                    HeightRequest.Equals(item.HeightRequest) &&
                    WidthRequest.Equals(item.WidthRequest) &&
                    TintColor == item.TintColor);
        }

        public override int GetHashCode()
        {
            return SvgPath?.GetHashCode() ?? 0;
        }
    }
}