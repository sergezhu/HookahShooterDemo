namespace _Game._Scripts.InventoryItemsServices.Database.DatabaseItemsSO
{
	using System;
	using _Game._Scripts.InventoryItemsServices.Database.DatabaseItems;
	using Sirenix.OdinInspector;
	using Sirenix.Utilities;
	using UnityEngine;

	[Serializable]
	public abstract class DatabaseItemSO : ScriptableObject
	{
		[SerializeField] private uint _id;
		[SerializeField] private string _name;
		[SerializeField] private Sprite _icon;


		public uint ID => _id;
		public string Name => _name;
		public Sprite Icon => _icon;

		public static implicit operator InventoryItem( DatabaseItemSO dbItem )
		{
			var item = new InventoryItem( dbItem._id, dbItem._name );
			return item;
		}

		public abstract DatabaseItem GetItem();
		

		public void ValidateID()
		{
			#if UNITY_EDITOR
			if ( DatabaseUtilityEditor.IsIDNotUnique( _id ) )
			{
				_id = DatabaseUtilityEditor.GetUniqueID();
			}
			#endif
		}

		private void OnValidate()
		{
			//ValidateEditor();
		}

		[Button]
		private void ValidateEditor()
		{
			#if UNITY_EDITOR

			if ( _name.IsNullOrWhitespace() == false )
			{
				var trimName = _name.Trim();
				var fileName = $"[{_id:0000}] {trimName}";

				string assetPath = UnityEditor.AssetDatabase.GetAssetPath( GetInstanceID() );
				UnityEditor.AssetDatabase.RenameAsset( assetPath, fileName );
				UnityEditor.AssetDatabase.SaveAssets();
			}

			#endif
		}
	}
}