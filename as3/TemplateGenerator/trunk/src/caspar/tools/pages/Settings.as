package caspar.tools.pages
{
	import caspar.network.ServerConnection;
	import caspar.network.ServerConnectionEvent;
	import caspar.tools.TemplateGenerator;
	import caspar.tools.utils.ServerItem;
	import caspar.tools.utils.Servers;
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
		private var _templatePathOK:Boolean = false;
		//TODO: Remove testfunction
		private var _testTrace:Function;
		
		private var _serverConnectionTest:ServerConnection;
		
		public function Settings(view:SettingsView, servers:Servers, testTrace:Function) 
		{
			_serverConnectionTest = new ServerConnection();
			_serverConnectionTest.addEventListener(ServerConnectionEvent.ON_CONNECT, onServerOK);
			_serverConnectionTest.addEventListener(ServerConnectionEvent.ON_IO_ERROR, onServerIOError);
			_serverConnectionTest.addEventListener(ServerConnectionEvent.ON_SECURITY_ERROR, onServerSecurityError);
			_view = view;
			_servers = servers;
			_testTrace = testTrace;
			_servers.addEventListener(Event.CHANGE, onServersChange);
			init();
		}
		
		public function set verboseOutput(value:Boolean):void
		{
			_view.verboseOutput.selected = value;
		}
		
		public function get verboseOutput():Boolean
		{
			return _view.verboseOutput.selected;
		}
		
		private function onServersChange(e:Event):void 
		{
			trace("Nu har settings data");
			_templatePathOK = false;
			_view.HostCorrectIndicator.gotoAndStop("hidden");
			_view.PathCorrectIndicator.gotoAndStop("hidden");
			reloadServerList();
		}
		
		private function init():void
		{
			//for (var i:int = 0; i < _view.numChildren; i++) 
			//{
				//if (_view.getChildAt(i) as InteractiveObject != null)
				//{
					//InteractiveObject(_view.getChildAt(i)).tabEnabled = false;
					//InteractiveObject(_view.getChildAt(i)).tabIndex = -1;
				//}
			//}
			//
			
			
			_view.Save.enabled = false;
			//loadServerList();
			_view.Save.addEventListener(MouseEvent.CLICK, onSaveServers);
			_view.Delete.addEventListener(MouseEvent.CLICK, onDeleteServer);
			_view.Servers.addEventListener(Event.CHANGE, onServerChange);
			//_view.Servers.addEventListener(Event.ENTER_FRAME, onServersInit);
			
			_view.DisplayName.addEventListener(KeyboardEvent.KEY_UP, onDisplayNameKeyUp);
			_view.Channel.addEventListener(Event.CHANGE, onChannelKeyUp);
			_view.Port.addEventListener(KeyboardEvent.KEY_UP, onPortKeyUp);
			_view.TemplatesPath.addEventListener(KeyboardEvent.KEY_UP, onPathUp);
			_view.TemplatesPath.addEventListener(FocusEvent.FOCUS_OUT, onCheckPath);
			
			_view.Server.addEventListener(KeyboardEvent.KEY_UP, onServerKeyUp);
			_view.Server.addEventListener(FocusEvent.FOCUS_OUT, onCheckHost);
			_view.Port.addEventListener(FocusEvent.FOCUS_OUT, onCheckHost);
			_view.Port.addEventListener(KeyboardEvent.KEY_UP, onServerKeyUp);
			
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
			_view.HostCorrectIndicator.gotoAndStop("correct");
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
		
		private function onCheckPath(e:FocusEvent):void 
		{
			
			_view.PathCorrectIndicator.gotoAndStop("search");
			if (_view.TemplatesPath.text != "")
			{
				//var folderExists:Boolean = TemplateGenerator.toboolean(TemplateGenerator.MMExecuter('fl.runScript(fl.configURI+"WindowSWF/brew2_0.jsfl", "checkIfFolderExists", "' +formattedTemplatePath() + '");'));
				var folderExists:Boolean = TemplateGenerator.toboolean(TemplateGenerator.MMExecuter('FLfile.exists("' + formattedTemplatePath() + '");'));
				_testTrace(folderExists);
				if (folderExists)
				{
					_view.PathCorrectIndicator.gotoAndStop("correct");
					_templatePathOK = true;
				}
				else
				{
					_view.PathCorrectIndicator.gotoAndStop("incorrect");
					_templatePathOK = false;
				}
			}
			else
			{
				_view.PathCorrectIndicator.gotoAndStop("hidden");
				_templatePathOK = false;
			}
		}
		
		
		private function formattedTemplatePath():String
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
			
			_testTrace("path: " + path);
			
			return path;
			
		}
		
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
			
			_templatePathOK = false;
			_view.PathCorrectIndicator.gotoAndStop("hidden");
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
			
			
		
			//_ftGeneratorView.rcFolders.removeAll();
			//_ftGeneratorView.rcFolders.enabled = false;
			//_ftGeneratorView.Servers.addItem( { label: "-Not connected-", data: null } )
			//_ftGeneratorView.rcTemplates.removeAll();
			//_ftGeneratorView.rcTemplates.enabled = false;
			//_ftGeneratorView.rcTemplates.addItem({ label: "-Not connected-", data: null})
		}
		
		private function populateServerItems():void
		{
			if (_view.Servers.selectedItem != null)
			{
				if (_view.Servers.selectedItem.data != "{newServer}")
				{
					var serveritem:ServerItem = _view.Servers.selectedItem.data;
					_view.DisplayName.text = serveritem.displayName;
					_view.Server.text = serveritem.host;
					_view.Channel.value = serveritem.channel;
					_view.Port.text = serveritem.port.toString();
					_view.TemplatesPath.text = serveritem.templatePath.replace(/\\\\/g, "\\");
					
					onCheckPath(null);
					onCheckHost(null);
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
		}

		
		
		private function saveServerList():void
		{
			
			if (_view.DisplayName.text == "0" || int(_view.DisplayName.text) != 0 || _view.DisplayName.text == "") _view.DisplayName.text = "_" + _view.DisplayName.text;
			
			var serveritem:ServerItem;
			
			if (_view.Servers.selectedItem.data == "{newServer}")
			{
				serveritem = new ServerItem();
				serveritem.host = _view.Server.text;
				serveritem.displayName = _view.DisplayName.text;
				serveritem.channel = _view.Channel.value;
				serveritem.port = int(_view.Port.text);
				TemplateGenerator.jtrace(" pathInnan:"+_view.TemplatesPath.text);
				serveritem.templatePath = _view.TemplatesPath.text.replace(/\\/g, "\\\\");
				TemplateGenerator.jtrace(" spath:"+serveritem.templatePath);
				_selectedItem = serveritem;
				_servers.addServer(serveritem);
			}
			else
			{
				serveritem = _view.Servers.selectedItem.data;
				serveritem.displayName = _view.DisplayName.text;
				serveritem.host = _view.Server.text;
				serveritem.channel = _view.Channel.value;
				serveritem.port = int(_view.Port.text);
				TemplateGenerator.jtrace(" pathInnan:"+_view.TemplatesPath.text);
				serveritem.templatePath = _view.TemplatesPath.text.replace(/\\/g, "\\\\");
				TemplateGenerator.jtrace("spath:"+serveritem.templatePath);
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
			
			_view.PathCorrectIndicator.gotoAndStop("hidden");
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
			_view.TemplatesPath.text = "";
			
		}
		
		private function onPathUp(e:KeyboardEvent):void 
		{
			dataChanged();
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
				if(_view.Server.text == "") _view.DisplayName.text = "-new server-";
			}
			
		}
		
		private function onDisplayNameKeyUp(e:KeyboardEvent):void 
		{		
			dataChanged();
			_useAutomatedName = false;
			//trace("onDisplayNameKeyUp");
			//_view.Servers.getItemAt(_view.Servers.numChildren - 2).label = _view.DisplayName.text;
		}
		
		private function dataChanged():void 
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