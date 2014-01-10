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

package caspar.network 
{
	import caspar.network.data.CasparItemInfoCollection;
	import caspar.network.data.IItemList;
	
	import flash.events.Event;
	
	/**
	 * The events that is dispatched by the ServerConnection
	 * @author Andreas Jeansson
	 */
	public class ServerConnectionEvent extends Event 
	{
		/**
		 * Dispatched when connected
		 */
		public static const ON_CONNECT:String = "onConnect";
		
		/**
		 * Dispatched when disconnected
		 */
		public static const ON_DISCONNECT:String = "onDisonnect";
		
		/**
		 * Dispatched every time a command is sent to caspar
		 */
		public static const ON_SEND_COMMAND:String = "onSendCommand";
		
		/**
		 * Dispatched if caspar returns a 2xx return code
		 */
		public static const ON_SUCCESS:String = "onSuccess";
		
		/**
		 * Dispatched if caspar returns a 4xx or 5xx return code (error)
		 */
		public static const ON_ERROR:String = "onError";
		
		public static const ON_IO_ERROR:String = "onIOError";
		
		public static const ON_SECURITY_ERROR:String = "onSecurityError";
		
		/**
		 * Dispatched on GetData. The data property will be of a XML data type.
		 */
		public static const ON_GET_DATA:String = "onDataRetrieve";
		
		/**
		 * Dispatched on GetDatasets. The data property will be of an Array data type.
		 */
		public static const ON_GET_DATASETS:String = "onDataList";
		
		/**
		 * Dispatched on GetMediaFileInfo. The data property will be of a String data type.
		 */
		public static const ON_MEDIAFILE_INFO:String = "onCINF";
		
		/**
		 * Dispatched on GetMediaFiles. The data property will be of a Array data type.
		 */
		public static const ON_GET_MEDIAFILES:String = "onCLS";
		
		/**
		 * Dispatched on GetTemplates. The data property will be of a Array data type.
		 */
		public static const ON_GET_TEMPLATES:String = "onTLS";
		
		/**
		 * Dispatched on GetVersion. The data property will be of a String data type.
		 */
		public static const ON_VERSION:String = "onVersion";
		
		/**
		 * Dispatched on GetInfo. The data property will be of an Array data type.
		 */
		public static const ON_INFO:String = "onInfo";
		
		/**
		 * Dispatched from all commands that doesn't return any data
		 */
		public static const ON_OTHER_COMMAND:String = "onOther";
		
		/**
		 * Returns all responses from caspar
		 */
		public static const ON_LOG:String = "onLog";
		
		private var _command:String;
		private var _message:String = "";
		private var _data:*;
		private var _itemList:IItemList;
		
		public function ServerConnectionEvent(type:String, bubbles:Boolean = false, cancelable:Boolean = false, command:String = "", message:String = "", data:* = null, itemList:IItemList = null ) 
		{ 
			super(type, bubbles, cancelable);
			_command = command;
			_message = message;
			_data = data;
			_itemList = itemList;
		} 
		
		public override function clone():Event 
		{ 
			return new ServerConnectionEvent(type, bubbles, cancelable, command, message, data, itemList);
		} 
		
		public override function toString():String 
		{ 
			return formatToString("CasparAMCPEvent", "type", "bubbles", "cancelable", "eventPhase"); 
		}
		
		/**
		 * [read-only] The command line sent to caspar
		 */
		public function get command():String { return _command; }
		
		/**
		 * [read-only] The message sent from Caspar
		 */
		public function get message():String { return _message; }
		
		/**
		 * [read-only] Contains data if the command listened to returns data.
		 */
		public function get data():* { return _data; }
		
		/**
		 * [read-only] Contains a list og
		 */
		public function get itemList():IItemList { return _itemList; }
		
	}
	
}