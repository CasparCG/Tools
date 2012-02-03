package caspar.tools.utils 
{
	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	public class ServerItem 
	{
		private var _host:String;
		private var _channel:uint;
		private var _port:uint;
		private var _templatePath:String;
		private var _displayName:String;
		
		public function ServerItem() 
		{
			
		}
		
		public function get host():String 
		{
			return _host;
		}
		
		public function set host(value:String):void 
		{
			_host = value;
		}
		
		public function get channel():uint 
		{
			return _channel;
		}
		
		public function set channel(value:uint):void 
		{
			_channel = value;
		}
		
		public function get port():uint 
		{
			return _port;
		}
		
		public function set port(value:uint):void 
		{
			_port = value;
		}
		
		public function get templatePath():String 
		{
			return _templatePath;
		}
		
		public function set templatePath(value:String):void 
		{
			if (value.indexOf("\\") != -1)
			{
				if (value.charAt(value.length - 1) != "\\") value += "\\\\";
			}
			else if(value.indexOf("/") != -1)
			{
				if (value.charAt(value.length - 1) != "/") value += "/";
			}
			else
			{
				if (value.charAt(value.length - 1) != "/") value += "/";
			}
			
			//value = value.replace(/\\/g, "\\\\");
			
			_templatePath = value;
		}
		
		public function get displayName():String 
		{
			return _displayName;
		}
		
		public function set displayName(value:String):void 
		{
			_displayName = value;
		}
		
	}

}