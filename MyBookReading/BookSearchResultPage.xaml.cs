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
        ObservableCollection<ViewModel.SearchResultBook> ResultBookList;
        AmazonBookSearch SearchContext;
        public BookSearchResultPage(BookShelf bookshelf, AmazonBookSearch search, ObservableCollection<Book> books)
        {
            InitializeComponent();

            SearchContext = search;

            InitList(bookshelf, books);
        }

        private void addResultBook(ObservableCollection<Book> books, ObservableCollection<ViewModel.SearchResultBook> resultBooks)
        {
			foreach (var book in books)
			{
				var bookResult = new ViewModel.SearchResultBook(book);
				resultBooks.Add(bookResult);
			}
		}

        async private void InitList(BookShelf bookshelf, ObservableCollection<Book> books)
        {
            ResultBookList = new ObservableCollection<ViewModel.SearchResultBook>();
            addResultBook(books, ResultBookList);

            SearchResultVM = new SearchResultVM { BookResultList = ResultBookList };
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

            listBook.ItemAppearing += async (object sender, ItemVisibilityEventArgs e) => {
            	// ObservableCollection の最後が ListView の Item と一致した時に ObservableCollection にデータを追加する。
            	if (ResultBookList.Last() == e.Item as SearchResultBook)
            	{
            		// ObservableCollection にデータを追加する処理
            		stack.IsVisible = true;
            		await AmazonSearch(SearchContext); // 実際の処理を入れてください。
            		stack.IsVisible = false;
            	}
            };

            SearchResultVM.CheckBooks(books);
        }

        async Task<bool> AmazonSearch(AmazonBookSearch search)
        {
            ObservableCollection<Book> books = new ObservableCollection<Book>();
            bool ret = await search.ContinueSearch(books);
            if(ret)
            {
				addResultBook(books, ResultBookList);
				SearchResultVM.CheckBooks(books);
			}
            return ret;
        }
    }
}
