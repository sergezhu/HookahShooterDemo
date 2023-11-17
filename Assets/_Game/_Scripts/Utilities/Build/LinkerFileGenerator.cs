namespace _Game._Scripts.Utilities.Build
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Sirenix.OdinInspector;
	using UnityEngine;

	[Serializable]
	public struct LinkerTypeInfo
	{
		public string AssemblyName;
		public string FullName;
	}
	
	public class LinkerFileGenerator : MonoBehaviour
	{
		private const string FileName = "link.xml";
		private const string FileLinkerFileOpenLine = "<linker>";
		private const string FileLinkerFileCloseLine = "</linker>";

		[TextArea]
		[SerializeField] private string _fileRecordTemplate;
		[SerializeField] private string _folderPath;
		
		[Space]
		[SerializeField, ReadOnly] private LinkerTypeInfo[] _linkerTypesInfo;

		private List<Type> _types;

		private void OnValidate()
		{
			//Generate();
		}

		[Button]
		private void Generate()
		{
			CreateTypesList();
			FillLinkerTypesInfo();
			SaveLinkerFile();
		}

		private void CreateTypesList()
		{
			_types = new List<Type>()
			{
				
			};
		}

		private void FillLinkerTypesInfo()
		{
			var linkerTypesInfoTmp = new List<LinkerTypeInfo>();
			
			foreach ( var type in _types )
			{
				linkerTypesInfoTmp.Add( new LinkerTypeInfo()
				{
					AssemblyName = type.Assembly.FullName,
					FullName = type.FullName
				} );
			}

			_linkerTypesInfo = linkerTypesInfoTmp.ToArray();
		}

		private string GetLinkerFileText()
		{
			var text = $"{FileLinkerFileOpenLine}\n";

			foreach ( var info in _linkerTypesInfo )
			{
				var record = string.Format( _fileRecordTemplate, info.AssemblyName, info.FullName );
				text = $"{text}{record}\n";
			}

			text = $"{text}{FileLinkerFileCloseLine}";

			return text;
		}

		private void SaveLinkerFile()
		{
			var filePath = $"{_folderPath}/{FileName}";
			
			using ( StreamWriter sw = new StreamWriter( filePath ) )
			{
				sw.WriteLine( GetLinkerFileText() );
				sw.Close();
				
				Debug.Log( $"Linker file saved to : {filePath}" );
			}
		}
	}
}