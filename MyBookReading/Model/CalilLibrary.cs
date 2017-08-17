using System;
using Realms;

namespace MyBookReading.Model
{
    /// <summary>
    /// 蔵書検索の対象となる図書館
    /// </summary>
    public class CheckTargetLibrary : RealmObject
    {
		public string systemid { get; set; }    //システムID
		public string systemname { get; set; }  //システム名称
	}

	/// <summary>
	/// https://calil.jp/doc/api_ref.html
    /// libidがユニークかつ固定値である前提で設計する。そのためlibidが変わると破綻するので注意
	/// </summary>
	public class CalilLibrary : RealmObject
	{
		public string category { get; set; }    //SMALL(図書室・公民館) MEDIUM(図書館(地域)) LARGE(図書館(広域)) UNIV(大学) SPECIAL(専門) BM(移動・BM)
		public string city { get; set; }        //市区町村
		public string Short { get; set; }       //略称        //小文字は予約後のため大文字にする。JSONパースには問題なし。
		public string tel { get; set; }         //電話番号
		public string pref { get; set; }        //都道府県
		public string faid { get; set; }        //APIリファレンスに説明なし
		public string geocode { get; set; }     //位置情報
		public string systemid { get; set; }    //システムID
		public string address { get; set; }     //住所
		public string libid { get; set; }       //図書館のユニークID
		public string libkey { get; set; }      //システム毎の図書館キー
		public string post { get; set; }        //郵便番号
		public string url_pc { get; set; }      //PC版ウェブサイト
		public string systemname { get; set; }  //システム名称
		public string isil { get; set; }        //APIリファレンスに説明なし
		public string formal { get; set; }      //正式名称

		public override string ToString()
		{
			return formal;
		}
	}
}
