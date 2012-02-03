package caspar.tools.utils  
{
	import caspar.tools.TemplateGenerator;
	import flash.events.Event;
	import flash.events.EventDispatcher;
	import flash.events.IOErrorEvent;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	public class Servers extends EventDispatcher
	{
		private var _servers:Array;
		private var _serversXML:XML;
		
		public function Servers() 
		{
			
		}
		
		public function addServer(server:ServerItem, autoSave:Boolean = true):Boolean
		{
			var serverExists:Boolean = false;
			
			for each(var item:ServerItem in _servers)
			{
				if (item.displayName == server.displayName)
				{
					if (item != server)
					{
						item = server;
					}
					serverExists = true;
					break;
				}
			}
			
			if (!serverExists)
			{
				_servers.push(server);
			}
			
			_servers.sortOn("displayName", Array.CASEINSENSITIVE);
			
			if (autoSave) save();
			
			return true;
		}
		
		public function deleteServer(server:ServerItem, autoSave:Boolean = true):Boolean
		{
			var result:Boolean = false;
			
			for (var i:int = 0; i < _servers.length; i++ )
			{
				if (_servers[i] == server)
				{
					_servers[i] = null;
					_servers.splice(i, 1);
					result = true;
				}
			}
			
			_servers.sortOn("displayName", Array.CASEINSENSITIVE);
			
			if (autoSave) save();
			
			return result;
		}
		
		public function updateServer(autoSave:Boolean = true):void
		{
			_servers.sortOn("displayName", Array.CASEINSENSITIVE);
			if (autoSave) save();
		}
		
		public function load(url:String):void 
		{
			var loader:URLLoader = new URLLoader();
			loader.addEventListener(Event.COMPLETE, onFileRead);
			loader.addEventListener(IOErrorEvent.IO_ERROR, onError);
			loader.load(new URLRequest(url));
		}
		
		private function onError(error:IOErrorEvent):void
		{
			trace(error.text);
			TemplateGenerator.jtrace("hittar ej config: ->" + error.text);
			popultateServers();
		}
		
		private function onFileRead(event:Event):void
		{
			
			_serversXML = new XML(event.target.data);
			//TemplateGenerator.jtrace("Har läst fil: " + _serversXML);
			popultateServers();
		}
		
		private function popultateServers():void 
		{
			_servers = [];
			
			if (_serversXML != null)
			{
				for (var i:int = 0; i < _serversXML.children().length(); i++)
				{
					var item:XML = _serversXML.server[i];
					var serveritem:ServerItem = new ServerItem();
					serveritem.host = item.@host;
					serveritem.displayName = item.@name;
					serveritem.channel = item.@channel;
					serveritem.port = item.@port;
					serveritem.templatePath = item.@path;
					_servers.push(serveritem);
				}
			}
			
			_servers.sortOn("displayName", Array.CASEINSENSITIVE);
			this.dispatchEvent(new Event(Event.CHANGE));
		}
		
		private function getDefaultServers():XML
		{
			var defaultServersXML:XML = new XML(<servers>
												</servers>
												);
			return defaultServersXML;
		}
		
		private function save():void 
		{
			_serversXML = new XML((_servers == null) ? getDefaultServers() : generateServersXML());
			TemplateGenerator.jtrace("Sparar serverlista: " + _serversXML.toString());
			TemplateGenerator.MMExecuter('fl.runScript(fl.configURI+"WindowSWF/brew2_0.jsfl", "saveServers", ' + _serversXML.toString() + ');');
			
			this.dispatchEvent(new Event(Event.CHANGE));
		}
		
		public function get items():Array 
		{
			return _servers;
		}
		
		private function generateServersXML():XML 
		{
			var serversXML:XML = new XML(<servers></servers>);
			
			for each(var item:ServerItem in _servers)
			{
				var node:XML = new XML(<server name="" host="" channel="" port="" path="" />);
				
				node.@name = item.displayName;
				node.@host = item.host;
				node.@channel = item.channel;
				node.@port = item.port;
				node.@path = item.templatePath;
				trace("pathen:" + item.templatePath);
				serversXML.appendChild(node);
			}
			
			return serversXML;
			
		}
		
	}

}