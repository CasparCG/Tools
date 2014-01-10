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
	 * A data item containing data and info from a .ftd data file
	 * @author Andreas Jeansson, SVT
	 */
	public class DataItem 
	{
		private var _dataInfoItem:DataItemInfo;
		private var _content:XML;
		private var _contentAsString:String;
		
		public function DataItem(dataInfoItem:DataItemInfo, content:XML = null) 
		{
			_dataInfoItem = dataInfoItem;
			if(content != null) this.content = content;
		}
		
		/**
		 * Helper function. Creates a new template data xml, will overwrite content
		 */
		public function createNewTemplateDataContent():void
		{
			_content = new XML(<templateData></templateData>);
		}
		
		/**
		 * Helper function. Adds a componentData node to the content xml.
		 * @param	componentID The id of the caspar component, e.g. "f0"
		 * @param	componentDataValue The value to pass to the component
		 * @param	componentDataID The data id, normally "text" for caspar text fields
		 */
		//BUG: needs to be able to append elements to existing node
		public function addTemplateData(componentID:String, componentDataValue:String, componentDataID:String="text"):void 
		{
			if (_content == null) createNewTemplateDataContent();
			// Add data node)
			var cd:XML = new XML(<componentData id={componentID}></componentData>);
			var dataNode:XML = new XML(<data id={componentDataID} value={componentDataValue} />);
			cd.appendChild(dataNode);
			_content.appendChild(cd);
		}
		
		/**
		 * The content in the .ftd file
		 */
		public function get content():XML 
		{
			return _content;
		}
		
		public function set content(value:XML):void 
		{
			_content = new XML(value);
		}
		
		/**
		 * Returns the content xml as a single line string
		 */		
		public function get contentAsString():String
		{
			XML.prettyIndent = 0; 
			XML.prettyPrinting = true;
			XML.ignoreWhitespace = true;
			
			var ret:String = this.content.toString();
			
			ret = ret.replace(/\"/g, "\\\"")
				
			var formatedString:String = "";
			var insideTag:Boolean = false;
			
			for(var i:uint = 0; i < ret.length; ++i)
			{
				var currentChar:String = ret.charAt(i);
				
				if(ret.charAt(i) == "<")
				{	
					//inside a node
					insideTag == true;
				}
				else if(ret.charAt(i) == ">")
				{
					//outside a node
					insideTag == false;
				}
				
				//skip copy	if \n is found
				if(!insideTag && ret.charAt(i) != "\n")
				{
					formatedString += ret.charAt(i);
					
				}				
				
			}
			
			return formatedString;
		}
		
		/**
		 * The associated data info item
		 */
		public function get dataInfoItem():DataItemInfo 
		{
			return _dataInfoItem;
		}
		
		public function set dataInfoItem(value:DataItemInfo):void 
		{
			_dataInfoItem = value;
		}
	
	}

}