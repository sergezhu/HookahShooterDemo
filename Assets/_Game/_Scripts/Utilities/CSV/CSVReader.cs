namespace _Game._Scripts.Utilities.CSV
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Sirenix.OdinInspector;
	using UnityEngine;

	public class CSVReader
	{
		public readonly List<string> FileData = new List<string>(); 

		[Button]
		public void ReadCSVFile(string filePath )
		{
			FileData.Clear();

			if ( File.Exists( filePath ) )
			{
				StreamReader streamReader = new StreamReader( filePath );

				while ( !streamReader.EndOfStream )
				{
					string line = streamReader.ReadLine();
					FileData.Add( line );
				}

				streamReader.Close();
			}
			else
			{
				Debug.LogWarning( "Файл не найден!" );
			}
		}

		public List<T> Parse<T>(Func<string[], T> parseFunc, char separator)
		{
			var lines = FileData;
			lines.RemoveAt( 0 );

			var emptyLines = 0;
			var tempItems = new List<T>();

			foreach ( var line in lines )
			{
				if ( string.IsNullOrWhiteSpace( line ) )
				{
					emptyLines++;
					continue;
				}

				string[] parsedLine = line.Split( separator );

				if ( string.IsNullOrWhiteSpace( parsedLine[0] ) )
				{
					emptyLines++;
					continue;
				}

				var item = parseFunc( parsedLine );
				tempItems.Add( item );
			}

			return tempItems;
		}

		public void ParseCSVFile(char separator)
		{
			foreach ( string dataLine in FileData )
			{
				string[] dataValues = dataLine.Split( separator );
				
				foreach ( string dataValue in dataValues )
				{
					Debug.Log( dataValue );
				}
			}
		}
	}
}