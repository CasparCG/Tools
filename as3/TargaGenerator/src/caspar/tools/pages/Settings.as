package caspar.tools.pages
{
	import caspar.network.data.CasparInfoPaths;
	import caspar.network.ServerConnection;
	import caspar.network.ServerConnectionEvent;
	import caspar.tools.utils.ServerItem;
	import caspar.tools.utils.Servers;
	import caurina.transitions.Tweener;
	import fl.controls.ComboBox;
	import fl.core.UIComponent;
	import flash.display.InteractiveObject;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.events.FocusEvent;
	import flash.events.IOErrorEvent;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	
	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	 
	public class Settings 
	{
		
		private var _view:SettingsView;
		
		//private var _serversXML:XML;
		private var _useAutomatedName:Boolean = false;
		private var _dataChanged:Boolean = false;
		private var _servers:Servers;
		private var _selectedItem:ServerItem;
		//TODO: Remove testfunction
		private var _testTrace:Function;
		
		private var _serverConnectionTest:ServerConnection;
		
		public function Settings(view:SettingsView, servers:Servers, testTrace:Function) 
		{
			_serverConnectionTest = new ServerConnection();
			_serverConnectionTest.addEventListener(ServerConnectionEvent.ON_CONNECT, onServerOK);
			_serverConnectionTest.addEventListener(ServerConnectionEvent.ON_IO_ERROR, onServerIOError);
			_serverConnectionTest.addEventListener(ServerConnectionEvent.ON_SECURITY_ERROR, onServerSecurityError);
			_view 		= view;
			_servers 	= servers;
			_testTrace 	= testTrace;
			
			enableUIComponents(_view.copySettingsView, false);
			enableUIComponents(_view.ftpSettingsView, false);
			
			_servers.addEventListener(Event.CHANGE, onServersChange);
			init();
		}
		
		private function enableUIComponents(mc:MovieClip, enable:Boolean = true):void
		{
			for (var i:int = 0; i < mc.numChildren; i++)
			{
				if (mc.getChildAt(i) as UIComponent != null)
					UIComponent(mc.getChildAt(i)).enabled = enable;
			}
		}
		
		/*public function set verboseOutput(value:Boolean):void
		{
			_view.verboseOutput.selected = value;
		}
		
		public function get verboseOutput():Boolean
		{
			return _view.verboseOutput.selected;
		}*/
		
		private function onServersChange(e:Event):void 
		{
			trace("Nu har settings data");
			_view.HostCorrectIndicator.gotoAndStop("hidden");
			reloadServerList();
		}
		
		private function init():void
		{
			
			_view.Save.enabled = false;
			//loadServerList();
			_view.Save.addEventListener(MouseEvent.CLICK, onSaveServers);
			_view.Delete.addEventListener(MouseEvent.CLICK, onDeleteServer);
			_view.Servers.addEventListener(Event.CHANGE, onServerChange);
			//_view.Servers.addEventListener(Event.ENTER_FRAME, onServersInit);
			
			_view.DisplayName.addEventListener(KeyboardEvent.KEY_UP, onDisplayNameKeyUp);
			_view.Channel.addEventListener(Event.CHANGE, onChannelKeyUp);
			_view.Port.addEventListener(KeyboardEvent.KEY_UP, onPortKeyUp);
			
			_view.Server.addEventListener(KeyboardEvent.KEY_UP, onServerKeyUp);
			_view.Server.addEventListener(FocusEvent.FOCUS_OUT, onCheckHost);
			_view.Port.addEventListener(FocusEvent.FOCUS_OUT, onCheckHost);
			_view.Port.addEventListener(KeyboardEvent.KEY_UP, onServerKeyUp);
			
			_view.ftpSettingsView.FTPMediaPath.addEventListener(KeyboardEvent.KEY_UP, dataChanged);
			_view.ftpSettingsView.FTPUsername.addEventListener(KeyboardEvent.KEY_UP, dataChanged);
			_view.ftpSettingsView.FTPPassword.addEventListener(KeyboardEvent.KEY_UP, dataChanged);
			_view.copySettingsView.MediaPath.addEventListener(KeyboardEvent.KEY_UP, dataChanged);
			
			_view.AutoUploadTypes.addEventListener(Event.CHANGE, dataChanged);
			
			initUploadTypes();
		}
		
		private function initUploadTypes():void 
		{
			_view.AutoUploadTypes.removeAll();
			
			_view.AutoUploadTypes.addItem({label: "FTP", data: ServerItem.UPLOAD_TYPE_FTP});
			_view.AutoUploadTypes.addItem({label: "Copy", data: ServerItem.UPLOAD_TYPE_COPY});
			
			_view.AutoUploadTypes.selectedIndex 	= 0;
			_view.AutoUploadTypes.enabled 			= true;
			
			_view.AutoUploadTypes.addEventListener(Event.CHANGE, onSelectUploadType);
			_view.AutoUploadTypes.dispatchEvent(new Event(Event.CHANGE));
			_view.ftpSettingsView.alpha 			= 0;
			_view.copySettingsView.alpha 			= 0;
			
		}
		
		private function onSelectUploadType(e:Event = null):void 
		{
			var type:String = _view.AutoUploadTypes.selectedItem.data;
			
			if (type == ServerItem.UPLOAD_TYPE_FTP)
			{
				_view.ftpSettingsView.visible = true;
				Tweener.addTween(_view.ftpSettingsView, { delay: 0, time: 0.5, alpha: 1, onComplete: hideUploadTypeView, onCompleteParams: [_view.copySettingsView] });
				Tweener.addTween(_view.copySettingsView, { delay: 0, time: 0.5, alpha: 0 } );
				
				enableUIComponents(_view.ftpSettingsView, true);
				enableUIComponents(_view.copySettingsView, false);
			}
			else if (type == ServerItem.UPLOAD_TYPE_COPY)
			{
				_view.copySettingsView.visible = true;
				Tweener.addTween(_view.ftpSettingsView, { delay: 0, time: 0.5, alpha: 0 } );
				Tweener.addTween(_view.copySettingsView, { delay: 0, time: 0.5, alpha: 1, onComplete: hideUploadTypeView, onCompleteParams: [_view.ftpSettingsView] } );
				
				enableUIComponents(_view.ftpSettingsView, false);
				enableUIComponents(_view.copySettingsView, true);
			}
		}
		
		private function hideUploadTypeView(mc:MovieClip):void
		{
			mc.visible = false;
		}
		
		private function onHostFocus(e:FocusEvent):void 
		{
			_view.HostCorrectIndicator.gotoAndStop("hidden");
		}
		
		private function onCheckHost(e:FocusEvent):void 
		{
			_view.HostCorrectIndicator.gotoAndStop("search");
			_serverConnectionTest.connect(_view.Server.text, int(_view.Port.text), false);
		}
		
		private function onServerOK(e:ServerConnectionEvent):void 
		{
			Main.tracer("connected: " + _view.Server.text);
			_view.HostCorrectIndicator.gotoAndStop("correct");
			_serverConnectionTest.addEventListener(ServerConnectionEvent.ON_INFO_PATHS, onGetInfoPaths);
			_serverConnectionTest.GetInfoPaths();
			
		}
		
		private function onGetInfoPaths(e:ServerConnectionEvent):void 
		{
			_serverConnectionTest.removeEventListener(ServerConnectionEvent.ON_INFO_PATHS, onGetInfoPaths);
			var infoPaths:CasparInfoPaths = CasparInfoPaths(e.data);
			
			Main.tracer("host: " + _view.Server.text + " got paths: " + infoPaths.mediaPath);
			if (_view.ftpSettingsView.FTPMediaPath.text == "")
			{
				var mediaFolder:String = infoPaths.mediaPath.slice(infoPaths.mediaPath.lastIndexOf("/", infoPaths.mediaPath.lastIndexOf("/") - 1) + 1);
				_view.ftpSettingsView.FTPMediaPath.text = mediaFolder;
			}
			
			Main.tracer(_view.copySettingsView.MediaPath.text);
			if (_view.copySettingsView.MediaPath.text == "")
			{
				if (_view.Server.text == "localhost" || _view.Server.text == "127.0.0.1")
				{
					if (infoPaths.mediaPath.indexOf(":/") == -1 && infoPaths.mediaPath.indexOf("/") != 0)	//relative path
					{
						_view.copySettingsView.MediaPath.text = infoPaths.initialPath + infoPaths.mediaPath;
					}
					else
					{
						_view.copySettingsView.MediaPath.text = infoPaths.mediaPath;
					}
					
				}
				else if (infoPaths.mediaPath.indexOf(":/") != -1 )
				{
					
					_view.copySettingsView.MediaPath.text = _view.Server.text + infoPaths.mediaPath.slice(infoPaths.mediaPath.indexOf(":") + 1);
					Main.tracer("1: " + _view.copySettingsView.MediaPath.text);
				}
				else
				{
					_view.copySettingsView.MediaPath.text = _view.Server.text + infoPaths.mediaPath.slice(infoPaths.mediaPath.indexOf(":") + 1);
					Main.tracer("2: " + _view.copySettingsView.MediaPath.text);
				}
				
				
			}
			else
			{
				Main.tracer("3: " + _view.copySettingsView.MediaPath.text);
			}
			
			
			_serverConnectionTest.disconnect();
		}
		
		private function onServerSecurityError(e:ServerConnectionEvent):void 
		{
			_view.HostCorrectIndicator.gotoAndStop("warning");
		}
		
		private function onServerIOError(e:ServerConnectionEvent):void 
		{
			_view.HostCorrectIndicator.gotoAndStop("warning");
		}
		
		/*private function formattedTemplatePath():String
		{
			var path:String = _view.TemplatesPath.text;
			
			if (path.indexOf("\\") != -1)
			{
				if (path.charAt(path.length - 1) != "\\") path += "\\";
			}
			else if(path.indexOf("/") != -1)
			{
				if (path.charAt(path.length - 1) != "/") path += "/";
			}
			else
			{
				if (path.charAt(path.length - 1) != "/") path += "/";
			}
			
			path = convertToURIPath(path);
			
			//_testTrace("path: " + path);
			
			return path;
			
		}*/
		
		private function convertToURIPath(fp:String):String
		{
			if (fp.indexOf("file:/") == -1)
			{
				var prefix:String;
				if (fp.indexOf(":") == -1)
				{
					prefix = "file://";
				}
				else
				{
					prefix = "file:///";
				}
				var newPath:String = fp.replace(/\\/g, "/");
				//var newPath = newPath.replace(/:/, "|");
				newPath = prefix + newPath;
				return newPath;
			}
			else
			{
				return fp;
			}
		}
		
		
		//private function onServersInit(e:Event):void 
		//{
			//trace("init nu", _view.Servers.selectedItem);
			//_view.Servers.addEventListener(Event.ENTER_FRAME, onServersInit);
			//
		//}
		
		private function onServerChange(e:Event):void 
		{
			//var _currentItem:String = _view.Servers.value;
			
			if (_dataChanged)
			{
				//warn
			}
			
			_view.HostCorrectIndicator.gotoAndStop("hidden");
			
			trace("sdfsdf", _view.Servers.getItemAt(0).data);
			
			if (_view.Servers.getItemAt(0).data == "{newServer}")
			{
				_view.Servers.removeItemAt(0);
			}
			
			_view.Save.label = "Save";
			
			_useAutomatedName = false;
			
			//reloadServerList();
			
			if (_view.Servers.value == "{newServer}")
			{
				//trace(_view.Server.numChildren);
				reloadServerList();
				addNewServer();
			}
			else
			{
				populateServerItems();
			}
		}
		
		private function populateServerItems():void
		{
			if (_view.Servers.selectedItem != null)
			{
				if (_view.Servers.selectedItem.data != "{newServer}")
				{
					var serveritem:ServerItem 			= _view.Servers.selectedItem.data;
					_view.DisplayName.text 				= serveritem.displayName;
					_view.Server.text 					= serveritem.host;
					_view.Channel.value 				= serveritem.channel;
					_view.Port.text 					= serveritem.port.toString();
					
					
					for (var i:int = 0; i < _view.AutoUploadTypes.length; i++ )
					{
						if (_view.AutoUploadTypes.getItemAt(i).data == serveritem.uploadType)
						{
							_view.AutoUploadTypes.selectedItem = _view.AutoUploadTypes.getItemAt(i);
							onSelectUploadType();
						}
					}
					
					_view.ftpSettingsView.FTPMediaPath.text 	= serveritem.ftpMediaPath;
					_view.ftpSettingsView.FTPUsername.text 		= serveritem.ftpUsername;
					_view.ftpSettingsView.FTPPassword.text 		= serveritem.ftpPassword;
					_view.copySettingsView.MediaPath.text 		= serveritem.copyMediaPath;
					//Main.tracer("populateServerItems: " + _view.copySettingsView.MediaPath.text);
					//_view.TemplatesPath.text = serveritem.templatePath.replace(/\\\\/g, "\\");
					//_view.MediaPath.text = serveritem.mediaPath.replace(/\\\\/g, "\\");
					
				}
			}
		}
		
		private function onDeleteServer(e:MouseEvent):void 
		{
			if (_view.Servers.selectedItem != null)
			{
				if (_view.Servers.selectedItem.data != "{newServer}")
				{
					//TODO: Warn for deletion
					_servers.deleteServer(ServerItem (_view.Servers.selectedItem.data));
				}
				else
				{
					reloadServerList();
				}
			}
		}
		
		private function onSaveServers(e:MouseEvent):void 
		{
			saveServerList();
			onCheckHost(null);
		}
		
		private function saveServerList():void
		{
			
			if (_view.DisplayName.text == "0" || int(_view.DisplayName.text) != 0 || _view.DisplayName.text == "") _view.DisplayName.text = "_" + _view.DisplayName.text;
			
			var serveritem:ServerItem;
			
			if (_view.Servers.selectedItem.data == "{newServer}")
			{
				serveritem 					= new ServerItem();
				serveritem.host 			= _view.Server.text;
				serveritem.displayName 		= _view.DisplayName.text;
				serveritem.channel 			= _view.Channel.value;
				serveritem.port 			= int(_view.Port.text);
				serveritem.uploadType		= _view.AutoUploadTypes.selectedItem.data;
				serveritem.ftpMediaPath 	= _view.ftpSettingsView.FTPMediaPath.text;
				serveritem.ftpUsername 		= _view.ftpSettingsView.FTPUsername.text;
				serveritem.ftpPassword 		= _view.ftpSettingsView.FTPPassword.text;
				serveritem.copyMediaPath 	= _view.copySettingsView.MediaPath.text;
				
				//serveritem.templatePath = _view.TemplatesPath.text.replace(/\\/g, "\\\\");
				//serveritem.mediaPath = _view.MediaPath.text.replace(/\\/g, "\\\\");
				
				_selectedItem = serveritem;
				_servers.addServer(serveritem);
			}
			else
			{
				serveritem 					= _view.Servers.selectedItem.data;
				serveritem.displayName 		= _view.DisplayName.text;
				serveritem.host 			= _view.Server.text;
				serveritem.channel 			= _view.Channel.value;
				serveritem.port 			= int(_view.Port.text);
				serveritem.uploadType		= _view.AutoUploadTypes.selectedItem.data;
				serveritem.ftpMediaPath 	= _view.ftpSettingsView.FTPMediaPath.text;
				serveritem.ftpUsername 		= _view.ftpSettingsView.FTPUsername.text;
				serveritem.ftpPassword 		= _view.ftpSettingsView.FTPPassword.text;
				serveritem.copyMediaPath 	= _view.copySettingsView.MediaPath.text;
				
				//serveritem.templatePath = _view.TemplatesPath.text.replace(/\\/g, "\\\\");
				//serveritem.mediaPath = _view.MediaPath.text.replace(/\\/g, "\\\\");
				
				_servers.updateServer();
			}
			
			//if (_servers == null || _serversXML == "") _serversXML = getDefaultServers();
			
			//_servers.save();
			_view.Save.label = "Save";
			_view.Save.enabled = false;
			reloadServerList();
		}
		
		private function reloadServerList():void 
		{
			//var 
			if (_view.Servers.selectedItem != null)
			{
				if ((_selectedItem != _view.Servers.selectedItem.data) && _view.Servers.selectedItem.data != "{newServer" )
				{
					if (_view.Servers.selectedItem.data as ServerItem != null) 
					{
						_selectedItem = _view.Servers.selectedItem.data as ServerItem;
					}
				}
			}
			
			_view.Servers.removeAll();
			
			if (_servers.items.length == 0)
			{
				addNewServer();
			}
			else
			{
			
				//var i:int = 0;
				
				for each(var item:ServerItem in _servers.items)
				{
					_view.Servers.addItem( { label: item.displayName, data: item } );
					//i++;
				}
				//
				
				_view.Servers.selectedIndex = 0;
				
				if (_selectedItem != null)
				{
					for (var i:int = 0; i < _view.Servers.dataProvider.length; i++ )
					{
						if (_view.Servers.getItemAt(i).data == _selectedItem)
						{
							_view.Servers.selectedIndex = i;
							break;
						}
					}
				}
				
				//_view.Servers.selectedIndex = 0;
				
				_view.Servers.addItem( { label: "->Add new server", data: "{newServer}" } );
				
				
				populateServerItems();
			}
		}

		public function addNewServer():void 
		{			
			//reloadServerList();
			
			_view.HostCorrectIndicator.gotoAndStop("hidden");
			
			_view.Save.enabled = false;
			_view.Save.label = "Add";
			if (_view.Servers.dataProvider.length > 0)
			{
				if (_view.Servers.getItemAt(0).data != "{newServer}") _view.Servers.addItemAt( { label: "-new server-", data: "{newServer}" }, 0 );
			}
			else
			{
				_view.Servers.addItemAt( { label: "-new server-", data: "{newServer}" }, 0 );
			}
			_view.Servers.selectedIndex = 0;
				
			_view.Server.text = "";
			_view.DisplayName.text = "-new server-";
			_view.Channel.value = 1;
			_view.Port.text = "5250";
			
			_view.AutoUploadTypes.selectedIndex = 0;
			onSelectUploadType();
					
			_view.ftpSettingsView.FTPMediaPath.text 	= "";
			_view.ftpSettingsView.FTPUsername.text 		= "";
			_view.ftpSettingsView.FTPPassword.text 		= "";
			_view.copySettingsView.MediaPath.text 		= "";
			Main.tracer("add: " + _view.copySettingsView.MediaPath.text, true);
			
			
		}
		
		private function onPortKeyUp(e:KeyboardEvent):void 
		{
			dataChanged();
		}
		
		private function onChannelKeyUp(e:Event):void 
		{
			dataChanged();
			if (_useAutomatedName)
			{
				_view.DisplayName.text = _view.Server.text + ":" + _view.Channel.value;
				if(_view.Server.text == "") _view.DisplayName.text = "-new server-";
			}
		}
		
		private function onServerKeyUp(e:KeyboardEvent):void 
		{
			dataChanged();
			_view.HostCorrectIndicator.gotoAndStop("hidden");
			trace("onServerKeyUp");
			if (_view.DisplayName.text == "-new server-" || _view.DisplayName.text == "")
			{
				_useAutomatedName = true;
			}
			
			if (_useAutomatedName)
			{
				_view.DisplayName.text = _view.Server.text + ":" + _view.Channel.value;
				if (_view.Server.text == "") _view.DisplayName.text = "-new server-";
				
			}
			
		}
		
		private function onDisplayNameKeyUp(e:KeyboardEvent):void 
		{		
			dataChanged();
			_useAutomatedName = false;
			//trace("onDisplayNameKeyUp");
			//_view.Servers.getItemAt(_view.Servers.numChildren - 2).label = _view.DisplayName.text;
		}
		
		private function dataChanged(e:Event = null):void 
		{
			_dataChanged = true;
			
			if (_view.Server.text != "" && _view.Server.text != " " && _view.DisplayName.text != "" && _view.DisplayName.text != "-new server-")
			{
				_view.Save.enabled = true;
			}
			else
			{
				_view.Save.enabled = false;
			}
		}
		
	}

}