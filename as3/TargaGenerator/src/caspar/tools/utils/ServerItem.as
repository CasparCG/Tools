package caspar.tools.utils 
{
	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	public class ServerItem 
	{
		static public const UPLOAD_TYPE_FTP:String = "uploadTypeFtp";
		static public const UPLOAD_TYPE_COPY:String = "uploadTypeCopy";
		
		private var _host:String;
		private var _channel:uint;
		private var _port:uint;
		private var _templatePath:String; //old
		private var _displayName:String;
		
		private var _uploadType:String;
		
		//FTP
		private var _ftpMediaPath:String;
		private var _ftpUsername:String;
		private var _ftpPassword:String;
		
		//Copy
		private var _copyMediaPath:String;
		
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
		
		public function get ftpMediaPath():String 
		{
			return _ftpMediaPath;
		}
		
		public function set ftpMediaPath(value:String):void 
		{
			_ftpMediaPath = value;
		}
		
		public function get ftpPassword():String 
		{
			return _ftpPassword;
		}
		
		public function set ftpPassword(value:String):void 
		{
			_ftpPassword = value;
		}
		
		public function get ftpUsername():String 
		{
			return _ftpUsername;
		}
		
		public function set ftpUsername(value:String):void 
		{
			_ftpUsername = value;
		}
		
		public function get copyMediaPath():String 
		{
			return _copyMediaPath;
		}
		
		public function set copyMediaPath(value:String):void 
		{
			_copyMediaPath = value;
		}
		
		public function get uploadType():String 
		{
			return _uploadType;
		}
		
		public function set uploadType(value:String):void 
		{
			_uploadType = value;
		}
		
	}

}