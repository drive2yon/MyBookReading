using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class RatingStars : ContentView
    {
		private Label ReviewsLabel { get; set; }
		private List<Image> StarImages { get; set; }

		public RatingStars()
		{
            //InitializeComponent();
			GeneralDisplay();
		}

		private void GeneralDisplay()
		{
			ReviewsLabel = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
			};

			StarImages = new List<Image>();
			for (int i = 0; i < 5; i++)
			{
				StarImages.Add(new Image());
			}

			StackLayout startStack = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Start,
				Padding = 0,
				Spacing = 0,
				Children = {
					StarImages[0],
					StarImages[1],
					StarImages[2],
					StarImages[3],
					StarImages[4],
					ReviewsLabel
				}
			};

			updateReviewsDisplay();
			updateStarsDisplay();
			this.Content = startStack;
		}

		public void updateReviewsDisplay()
		{
			ReviewsLabel.Text = Reviews > 0 ? " (" + Convert.ToString(Reviews) + ")" : "";
		}

		public void updateStarsDisplay()
		{
			for (int i = 0; i < StarImages.Count; i++)
			{
                StarImages[i].Source = ImageSource.FromResource(GetStarFileName(i));
			}
		}

		private string GetStarFileName(int position)
		{
			int currentStarMaxRating = (position + 1) * 2;
			if (Rating >= currentStarMaxRating)
			{
				return "MyBookReading.Assets.rating_star_on.png";
			}
			else if (Rating >= currentStarMaxRating - 1)
			{
				return "MyBookReading.Assets.rating_star_half.png";
			}
			else
			{
				return "MyBookReading.Assets.rating_star_off.png";
			}
		}

		public static BindableProperty RatingProperty = BindableProperty.Create<RatingStars, int>(ctrl => ctrl.Rating,
																								   defaultValue: 0,
																								   defaultBindingMode: BindingMode.OneWay,
																								   propertyChanged: (bindable, oldValue, newValue) =>
																								   {
																									   var ratingStars = (RatingStars)bindable;
																									   ratingStars.updateStarsDisplay();
																								   }
																								  );

		public int Rating
		{
			get { return (int)GetValue(RatingProperty); }
			set { SetValue(RatingProperty, value); }
		}


		public static BindableProperty ReviewsProperty = BindableProperty.Create<RatingStars, int>(ctrl => ctrl.Reviews,
																								   defaultValue: 0,
																								   defaultBindingMode: BindingMode.OneWay,
																								   propertyChanged: (bindable, oldValue, newValue) =>
																								   {
																									   var ratingStars = (RatingStars)bindable;
																									   ratingStars.updateReviewsDisplay();
																								   }
																								  );

		public int Reviews
		{
			get { return (int)GetValue(ReviewsProperty); }
			set { SetValue(ReviewsProperty, value); }
		}
	}
}
