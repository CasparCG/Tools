/*
* copyright (c) 2010 Sveriges Television AB <info@casparcg.com>
*
*  This file is part of CasparCG.
*
*    CasparCG is free software: you can redistribute it and/or modify
*    it under the terms of the GNU General Public License as published by
*    the Free Software Foundation, either version 3 of the License, or
*    (at your option) any later version.
*
*    CasparCG is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU General Public License for more details.

*    You should have received a copy of the GNU General Public License
*    along with CasparCG.  If not, see <http://www.gnu.org/licenses/>.
*
*/

package caspar.network.data
{
	/**
	 * A collection of CasparItemInfo items
	 * @author Andreas Jeansson, SVT
	 */
	
	public class CasparItemInfoCollection implements IItemList
	{
		private var _itemList:Array;
		
		public function CasparItemInfoCollection(itemList:Array)
		{
			_itemList = itemList;
			_itemList.sortOn(["folder", "name"], [ Array.CASEINSENSITIVE, Array.CASEINSENSITIVE] );
		}
		
		/**
		 * Returns an array of the ICasparItemInfo items
		 * @return an array of the ICasparItemInfo items
		 */
		public function getItems():Array
		{
			return _itemList;
		}
		
		/**
		 * Returns all the items in a specific folder
		 * @param	folder the folder to use
		 * @return an array of ICasparItemInfo-items
		 */
		public function getItemsInFolder(folder:String):Array
		{
			var items:Array;
			for each(var item:ICasparItemInfo in _itemList)
			{
				if (item.folder == folder)
				{
					if (items == null)
					{
						items = new Array();
					}
					items.push(item);
				}
			}
			items.sortOn("name", Array.CASEINSENSITIVE);
			return items;
		}
		
		/**
		 * Returns a list of folders that contains the caspar items
		 * @return an array of ICasparItemInfo-items
		 */
		public function getFolders():Array
		{
			var folders:Array;
			var currentFolder:String;
			for each(var item:ICasparItemInfo in _itemList)
			{
				if (folders == null)
				{
					folders = new Array();
				}
				if (currentFolder != item.folder)
				{
					folders.push(item.folder);
					currentFolder = item.folder;
				}
				
			}
			return folders;
		}
	}
}