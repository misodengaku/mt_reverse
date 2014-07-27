using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Linq;


namespace mt_reverse
{
	class Program
	{
		static void Main(string[] args)
		{
			// 適当な数の乱数を作る
			var numbers = makeTestData(0xDEEDBEEF, 2048);

			// 先頭から624個取り出す
			var scraped_data = numbers.Take(624);
			// 状態を書き換えてしまうのでseedはなんでもいい
			MersenneTwister mt = new MersenneTwister(0);
			// MersenneTwisterの内部状態を書き換え
			int i = 0;
			foreach(var x in scraped_data)
			{
				mt.mt[i] = MersenneReverser.undoTemper(x);
				i++;
			}
			Console.WriteLine("MersennneTwister tampered.");
			
			// Test
			i = 0;
			while(i < numbers.Count - 624)
			{
				// 状態を書き換えたMTから乱数を生成
				var a = mt.NextUInt();
				// 最初に作った乱数と比較してみる
				if (a != numbers[i + 624])
				{
					Console.WriteLine("Data Missmatch! " + i);
					break;
				}
				i++;
			}
			// 最初に生成した乱数と状態を書き換えたMTで作った乱数が全て一致していたら
			if (i == numbers.Count - 624)
				Console.WriteLine("OK");

			Console.WriteLine("何かキーを押してください。");
			Console.ReadKey();
		}

		// テストデータを生成
		static List<uint> makeTestData(uint seed, uint count)
		{
			MersenneTwister mt = new MersenneTwister(seed);
			var data = new List<uint>();
			for (int i = 0; i < count; i++)
			{
				data.Add(mt.NextUInt());
			}
			return data;
		}

		// テストデータを改行区切りのファイルから取得
		static List<uint> makeTestData(string filename)
		{
			return File.OpenText(filename).ReadToEnd().Split('\n').Select(x => uint.Parse(x)).ToList<uint>();
		}
	}
 
}
