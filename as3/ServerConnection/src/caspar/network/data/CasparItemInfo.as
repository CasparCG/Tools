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
	 * Contains info on a caspar item, e.g. a template (.ft) or a media file (.mov, .tga....)
	 * @author Andreas Jeansson, SVT
	 */

	public class CasparItemInfo implements ICasparItemInfo
	{
		public static const TYPE_TEMPLATE:String = "type_template";
		public static const TYPE_MEDIA:String = "type_media";
		
		private var _folder:String;
		private var _name:String;
		private var _path:String;
		private var _size:String;
		private var _date:String;
		private var _type:String;
		private var _subtype:String;

		public function CasparItemInfo()
		{
			
		}
		
		/* INTERFACE se.svt.caspar.network.data.ICasparItemInfo */

		/**
		 * The folder where the file resides
		 */
		public function get folder():String { return _folder; }
		
		public function set folder(value:String):void 
		{
			_folder = value;
		}
		
		/**
		 * The name of the file without extension
		 */
		public function get name():String { return _name; }
		
		public function set name(value:String):void 
		{
			_name = value;
		}
		
		/**
		 * The path to the file
		 */
		public function get path():String { return _path; }
		
		public function set path(value:String):void 
		{
			_path = value;
		}
		
		/**
		 * The size of the file in bytes
		 */
		public function get size():String { return _size; }
		
		public function set size(value:String):void 
		{
			_size = value;
		}
		
		/**
		 * The modification date of the file
		 */
		public function get date():String { return _date; }
		
		public function set date(value:String):void 
		{
			_date = value;
		}
		
		/**
		 * The type of caspar item, either CasparItemInfo.TYPE_MEDIA or CasparItemInfo.TYPE_TEMPLATE
		 */
		public function get type():String 
		{
			return _type;
		}
		
		public function set type(value:String):void 
		{
			_type = value;
		}
		
		/**
		 * If the type is CasparItemInfo.TYPE_MEDIA the subtype will be either "STILL" or "MOVIE"
		 */
		public function get subtype():String 
		{
			return _subtype;
		}
		
		public function set subtype(value:String):void 
		{
			_subtype = value;
		}
		
	}

}