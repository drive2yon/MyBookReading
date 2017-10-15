using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Karamem0.LinqToCalil;
using MyBookReading.Model;
using Realms;
using Xamarin.Forms;
using System.Threading.Tasks;
using MyBookReading.ViewModel;

namespace MyBookReading
{

    public partial class BookSearchResultPage : ContentPage
    {
        SearchResultVM SearchResultVM;
        public BookSearchResultPage(BookShelf bookshelf, AmazonBookSearch search, ObservableCollection<Book> books)
        {
            InitializeComponent();

            InitList(bookshelf, books);
        }

        async private void InitList(BookShelf bookshelf, ObservableCollection<Book> books)
        {
            ObservableCollection<ViewModel.SearchResultBook> resultList = new ObservableCollection<ViewModel.SearchResultBook>();
            {

                //本棚に登録済みか判定する
                BookShelf bookVM = new BookShelf();
                foreach (var book in books)
                {
                    var bookResult = new ViewModel.SearchResultBook(book);
                    resultList.Add(bookResult);
                }
            }

            SearchResultVM = new SearchResultVM { BookResultList = resultList };
            this.BindingContext = SearchResultVM;

            listBook.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                {
                    return;
                }
                ((ListView)sender).SelectedItem = null;
                ViewModel.SearchResultBook item = e.SelectedItem as ViewModel.SearchResultBook;

                //本棚に登録済みの場合は詳細を表示しない
                if (item != null && item.IsRegistBookShelf == false)
                {
                    await Navigation.PushAsync(new BookDetailPage(bookshelf, item.CreateBook(), item.IsRegistBookShelf));
                }
            };

            SearchResultVM.CheckBooks(books);
        }
    }
}
