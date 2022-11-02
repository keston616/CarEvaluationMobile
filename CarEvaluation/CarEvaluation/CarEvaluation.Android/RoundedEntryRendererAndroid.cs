using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics.Drawables;
using CarEvaluation;
using CarEvaluation.Droid;

#pragma warning disable CS0612 // "RoundedEntryRendererAndroid" является устаревшим.
[assembly: ExportRenderer(typeof(RoundedEntry), typeof(RoundedEntryRendererAndroid))]
#pragma warning restore CS0612 // "RoundedEntryRendererAndroid" является устаревшим.
namespace CarEvaluation.Droid
{
    [Obsolete]
    public class RoundedEntryRendererAndroid : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                //Control.SetBackgroundResource(Resource.Layout.rounded_shape);
                var gradientDrawable = new GradientDrawable();
                gradientDrawable.SetStroke(5, Android.Graphics.Color.Transparent);
                gradientDrawable.SetColor(Android.Graphics.Color.Transparent) ;
                Control.SetBackground(gradientDrawable);

                Control.SetPadding(50, Control.PaddingTop, Control.PaddingRight,
                    Control.PaddingBottom);
            }
        }
    }
}