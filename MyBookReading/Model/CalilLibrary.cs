using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// ListView表示用
    /// </summary>
    public class CheckTargetLibrarys
	{
		readonly Realm _realm;

		public IEnumerable<CheckTargetLibrary> Librarys { get; }

		public CheckTargetLibrarys()
		{
			_realm = Realm.GetInstance();
			Librarys = _realm.All<CheckTargetLibrary>();
		}

        public void AddLibrary(string systemId, string systemName)
        {
            _realm.Write(() =>
            {
                CheckTargetLibrary target = new CheckTargetLibrary()
                {
                    systemid = systemId,
                    systemname = systemName,
                };
                _realm.Add(target);
            });

        }

		public void DelLibrary(CheckTargetLibrary library)
		{
			if (library != null)
			{
				_realm.Write(() => _realm.Remove(library));
			}
		}

        public void DelLibrary(string systemname)
        {
            _realm.Write(() =>
            {
                var librarys = Librarys.Where(x => x.systemname == systemname); ;
                if (librarys != null && librarys.Count() > 0)
                {
                    var library = librarys.First();
                    _realm.Remove(library);
                }
            });
        }


        //Calil検索用に設定済み図書館のIDを文字列で取得する
        public string GetSystemIDList()
        {
            StringBuilder systemidList = new StringBuilder();
            foreach (CheckTargetLibrary library in Librarys)
            {
                if (systemidList.Length == 0)
                {
                    systemidList.Append(library.systemid);
                }
                else
                {
                    systemidList.Append("," + library.systemid);
                }
            }
            return systemidList.ToString();
        }


        /// <summary>
        /// Gets the name of the library
        /// </summary>
        /// <returns>The system name.</returns>
        /// <param name="systemid">Systemid.</param>
        public string GetSystemName(string systemid)
        {
            foreach( CheckTargetLibrary library in Librarys)
            {
                if(systemid == library.systemid)
                {
                    return library.systemname;
                }
            }
            return null;
        }

        /// <summary>
        /// 引数で指定した図書館がDBに登録済みか判定して返す
        /// </summary>
        /// <returns><c>true</c> 登録済み <c>false</c> 未登録</returns>
        /// <param name="systemname">systemname(Calilが返却する図書館名).</param>
        public bool IsRegist(string systemname)
        {
            var librarys = Librarys.Where(x => x.systemname == systemname);
            return librarys.Count() > 0;
        }
	}


	/// <summary>
	/// https://calil.jp/doc/api_ref.html
    /// libidがユニークかつ固定値である前提で設計する。そのためlibidが変わると破綻するので注意
	/// </summary>
	public class CalilLibrary : RealmObject

	{
		public string category { get; set; }    //SMALL(図書室・公民館) MEDIUM(図書館(地域)) LARGE(図書館(広域)) UNIV(大学) SPECIAL(専門) BM(移動・BM)
		public string city { get; set; }        //市区町村
        public string Short { get; set; }       //略称        //小文字(short)は予約語のため大文字にする。JSONパースには問題なし。
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
