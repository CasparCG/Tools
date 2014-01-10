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
*
*    You should have received a copy of the GNU General Public License
*    along with CasparCG.  If not, see <http://www.gnu.org/licenses/>.
*
*/

package caspar.network.data 
{
	/**
	 * Contains info on a data item
	 * @author Andreas Jeansson, SVT
	 */

	 public class DataItemInfo
	{
		private var _fullPath:String;
		private var _label:String;
		
		/**
		 * Creates a new DataInfoItem 
		 * @param	fullPath the full path to the .ftd data file inside the caspar data folder, e.g. myDataFiles/myDataFile or amcp-style myDataFiles\\\\myFolderInFolder\\\\myDataFile
		 */
		public function DataItemInfo(fullPath:String) 
		{
			_fullPath = fullPath.replace(/\\/g, "/");
			_label = this.filename;
		}
		
		/**
		 * A custom label that can be set, defaults to filename
		 */
		public function get label():String
		{
			return _label;
		}

		public function set label(value:String):void
		{
			_label = value;
		}

		/**
		 * Returns the filename of the .ftd data file (without extension .ftd)
		 */
		public function get filename():String 
		{
			var fn:String = _fullPath;
			var a:Array = _fullPath.split("/");
			
			if (a.length > 0) fn = a[a.length - 1];
						
			return fn;
		}
		
		/**
		 * Returns the folder name of the data file
		 */
		public function get folder():String 
		{
			var f:String = "";
			var a:Array = _fullPath.split("/");
			
			if (a.length > 1) 
			{
				if(a.length == 2)
				{
					f = a[0];
				}
				else
				{
					for (var i:int = 0; i < a.length-1; i++)
					{
						if(i>=0) f += "/";
						
						f += a[a.length - 1];
					}
				}
			}
			
			return f;
		}
		
		/**
		 * Returns the full path formatted with / as separator
		 */
		public function get fullPath():String 
		{
			return _fullPath;
		}
		
		/**
		 * Returns the full path formatted with \\\\ as separator, used by the AMCP protocol
		 */
		public function get fullPathAMCPFormatted():String 
		{
			return _fullPath.replace(/\//g, "\\\\");
		}
		
	}

}