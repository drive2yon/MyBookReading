using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class SearchBookResult : ContentPage
    {
        public SearchBookResult( string keyword, AmazonCredentials amazonKey, ObservableCollection<Book> books )
        {
            InitializeComponent();

			this.BindingContext = books;
		}
    }
}
