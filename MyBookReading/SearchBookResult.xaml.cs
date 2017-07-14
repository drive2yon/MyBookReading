using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class SearchBookResult : ContentPage
    {
        private ObservableCollection<Book> books = new ObservableCollection<Book>();
        public SearchBookResult( string keyword, AmazonCresidentials amazonKey )
        {
            InitializeComponent();

            AmazonBookSearch search = new AmazonBookSearch(amazonKey);
			search.Search(keyword, books);
			this.BindingContext = books;
		}
    }
}
