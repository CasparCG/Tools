package caspar.tools.utils 
{
	import caspar.network.data.CasparInfoPaths;
	import caspar.network.ServerConnection;
	import caspar.network.ServerConnectionEvent;
	import flash.events.EventDispatcher;
	
	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	public class CasparConnection extends EventDispatcher
	{
		private var _casparServer:ServerConnection;		
		private var _currentServer:ServerItem;
		
		private var _casparInfoPaths:CasparInfoPaths;
		
		public function CasparConnection() 
		{
			super();
			_casparServer = new ServerConnection();
			_casparServer.addEventListener(ServerConnectionEvent.ON_CONNECT, onConnect);
			_casparServer.addEventListener(ServerConnectionEvent.ON_DISCONNECT, onDisconnect);
			_casparServer.addEventListener(ServerConnectionEvent.ON_GET_MEDIAFILES, onMedia);
			_casparServer.addEventListener(ServerConnectionEvent.ON_INFO_PATHS, onInfoPaths);
		}
		
		private function onOtherCommand(e:ServerConnectionEvent):void 
		{
			
		}
		
		private function onMedia(e:ServerConnectionEvent):void 
		{
			
		}
		
		public function get connection():ServerConnection
		{
			if (_casparServer.connected)
			{
				return _casparServer;
			}
			else
			{
				trace("disconnected, connect trough the connect method");
				return null;
			}
		}
		
		public function connect(server:ServerItem):void
		{
			_currentServer = server;
			_casparServer.connect(server.host, server.port);
		}
		
		public function disconnect():void
		{
			_currentServer = null;
			_casparServer.disconnect();
		}
		
		private function onDisconnect(e:ServerConnectionEvent):void 
		{
			dispatchEvent(e);
		}
		
		private function onConnect(e:ServerConnectionEvent):void 
		{
			_casparServer.GetInfoPaths();
			
			dispatchEvent(e);
			
		}
		
		private function onInfoPaths(e:ServerConnectionEvent):void 
		{
			_casparInfoPaths = CasparInfoPaths(e.data);
		}
		
		public function get currentServer():ServerItem 
		{
			return _currentServer;
		}
		
		
	}

}