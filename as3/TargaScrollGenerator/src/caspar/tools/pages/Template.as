package caspar.tools.pages
{
	import caspar.network.data.CasparItemInfoCollection;
	import caspar.network.data.IItemList;
	import caspar.network.ServerConnectionEvent;
	import caspar.tools.utils.PSDocument;
	import caspar.tools.utils.ServerItem;
	import caspar.tools.utils.Servers;
	import fl.events.DataChangeEvent;
	import flash.events.Event;
	import flash.events.EventDispatcher;
	import flash.events.FocusEvent;
	import flash.events.MouseEvent;

	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	public class Template extends EventDispatcher
	{
		private var _view:TemplateView;
		private var _templateList:IItemList;
		private var _mediaList:IItemList;
		
		public function Template(view:TemplateView) 
		{
			_view 		= view;
			init();
			
		}
		
		private function init():void
		{
			_view.mediaFolders.enabled = false;
			_view.FileName.addEventListener(FocusEvent.FOCUS_IN, onFileName);
			_view.FileName.addEventListener(FocusEvent.FOCUS_OUT, onFileNameOut);
			//_view.SetDefaultAA.addEventListener(MouseEvent.CLICK, setDefaultAA);
			_view.remoteCopy.enabled = false;
			_view.remoteCopy.addEventListener(MouseEvent.CLICK, onRemoteCopyClick);
			
			_view.autoPlay.enabled = false;
			
			_view.serversToAdd.addEventListener(Event.CHANGE, onServerToAddChange);
			_view.additionalServers.addEventListener(Event.CHANGE, onAdditionalServersChange);
			_view.btnAddServer.addEventListener(MouseEvent.CLICK, onAddServer);
			_view.btnRemoveServer.addEventListener(MouseEvent.CLICK, onRemoveServer);
			
			_view.btnAddServer.enabled 		= false;
			_view.btnRemoveServer.enabled 	= false;
			
			
		}
		
		private function onAdditionalServersChange(e:Event):void 
		{
			if (_view.additionalServers.selectedItem.data as ServerItem)
			{
				_view.btnRemoveServer.enabled = true;
			}
			else
			{
				// No server selected
				_view.btnRemoveServer.enabled = false;
			}
		}
		
		private function onRemoveServer(e:MouseEvent):void 
		{
			if (_view.additionalServers.selectedItem.data as ServerItem)
			{
				_view.additionalServers.removeItemAt(_view.additionalServers.selectedIndex);
			}
			else
			{
				// No server selected
			}
		}
		
		private function onServerToAddChange(e:Event):void 
		{
			if (_view.serversToAdd.selectedItem.data as ServerItem)
			{
				_view.btnAddServer.enabled = true;
			}
			else
			{
				// No server selected
				_view.btnAddServer.enabled = false;
			}
			
		}
		
		private function onAddServer(e:MouseEvent):void 
		{
			_view.btnAddServer.enabled = false;
			if (_view.serversToAdd.selectedItem.data is ServerItem)
			{
				var excist:Boolean = false;
				for (var i:int = 0; i < _view.additionalServers.length; i++ )
				{
					if (_view.additionalServers.getItemAt(i).data is ServerItem && _view.additionalServers.getItemAt(i).data == _view.serversToAdd.selectedItem.data) //already excist in list
					{
						excist = true;
					}
				}
				if(!excist)
					_view.additionalServers.addItem({label: ServerItem(_view.serversToAdd.selectedItem.data).displayName, data: ServerItem(_view.serversToAdd.selectedItem.data)});
				
			}
			else
			{
				//No server selected
			}
			
			_view.serversToAdd.selectedIndex = 0;
		}
		
		public function getAdditionalServers():Vector.<ServerItem>
		{
			var vec:Vector.<ServerItem> = new Vector.<ServerItem>();
			
			for (var i:int = 0; i < _view.additionalServers.length; i++ )
			{
				if ( _view.additionalServers.getItemAt(i).data is ServerItem)
				{
					vec.push(_view.additionalServers.getItemAt(i).data as ServerItem)
				}
			}
			return vec;
		}
		
		private function onFileNameOut(e:FocusEvent):void 
		{
			dispatchEvent(new Event("focusOutFileName"));
		}
		
		private function onFileName(e:FocusEvent):void 
		{
			dispatchEvent(new Event("focusOnFileName"));
		}
		
		public function setFileName(str:String):void
		{
			_view.FileName.text = str;
		}
		
		public function getFileName():String
		{
			return _view.FileName.text;
		}
		
		public function connected():void
		{
			_view.remoteCopy.enabled 	= true;
			_view.autoPlay.enabled 		= _view.remoteCopy.selected;
			_view.mediaFolders.enabled 	= _view.remoteCopy.selected;
			_view.serversToAdd.enabled 	= true;
		}
		
		public function disconnected():void
		{
			_view.mediaFolders.removeAll();
			_view.mediaFolders.addItem( { label: "-Not connected-", data: "nc" } );
			_view.remoteCopy.enabled 	= false;
			_view.autoPlay.enabled 		= false;
			_view.mediaFolders.enabled 	= false;
		}
		
		public function onGetMediaFiles(e:ServerConnectionEvent):void 
		{
			_mediaList 				= e.itemList;
			
			var folders:Array = _mediaList.getFolders();
			//_view.TemplateFolders.replaceItemAt({ label: "-Choose folder-", data: "null"}, 0);
			_view.mediaFolders.removeAll();
			for each (var currentFolder:String in folders)
			{
				
				if (currentFolder != "")
				{
					_view.mediaFolders.addItem({label: currentFolder, data: currentFolder});
				}
				else
				{
					_view.mediaFolders.addItem({label: "_ROOT", data: currentFolder});
				}
			}
			
			_view.mediaFolders.selectedIndex = 0;
			_view.mediaFolders.enabled = true;
			_view.mediaFolders.dispatchEvent(new Event(Event.CHANGE));
		}
		
		public function onGetTemplates(e:ServerConnectionEvent):void
		{
			_templateList = e.itemList;
		}
		
		public function selectFolder(folder:String):void 
		{
			for (var i:int = 0; i < _view.mediaFolders.dataProvider.length; i++ )
			{
				if (_view.mediaFolders.getItemAt(i).data == folder)
				{
					_view.mediaFolders.selectedIndex = i;
					break;
				}
			}
		}
		
		public function updateServers(servers:Servers):void 
		{
			_view.serversToAdd.removeAll();
			_view.serversToAdd.addItem( { label: "Add server" , data: "select" } );
			
			for (var i:int = 0; i < servers.items.length; i++ )
			{
				_view.serversToAdd.addItem( { label: ServerItem(servers.items[i]).displayName, data: ServerItem(servers.items[i]) } ); 
			}
			
			_view.serversToAdd.selectedIndex = 0;
			
			for (var j:int = 0; j < _view.additionalServers.length; j++ )
			{
				var index:int = servers.items.indexOf(ServerItem(_view.additionalServers.getItemAt(i).data));
				if (index == -1)
				{
					_view.additionalServers.removeItemAt(i);
				}
			}
		}
		
		public function set name(value:String):void 
		{
			_view.Name.text = value;
		}
				
		public function get name():String 
		{
			return _view.Name.text;
		}
		
		public function set email(value:String):void 
		{
			_view.Email.text = value;
		}
		
		public function get email():String 
		{
			return _view.Email.text;
		}
		
		public function set info(value:String):void 
		{
			_view.Info.text = value;
		}
		
		public function get info():String 
		{
			return _view.Info.text;
		}
		
		public function set thickness(value:int):void
		{
			_view.Thickness.text = value.toString();
		}
		
		public function get thickness():int
		{
			return int(_view.Thickness.text);
		}
		
		public function set sharpness(value:int):void
		{
			_view.Sharpness.text = value.toString();
		}
		
		public function get sharpness():int
		{
			return int(_view.Sharpness.text);
		}
		
		public function set optimizeImages(value:Boolean):void
		{
			_view.OptImages.selected = value;
		}
		
		public function get optimizeImages():Boolean
		{
			return _view.OptImages.selected;
		}
		
		public function set optimizeVideos(value:Boolean):void
		{
			_view.OptVideos.selected = value;
		}
		
		public function get optimizeVideos():Boolean
		{
			return _view.OptVideos.selected;
		}
		
		public function set optimizeTextfields(value:Boolean):void
		{
			_view.OptTextFields.selected = value;
		}
		
		public function get optimizeTextfields():Boolean
		{
			return _view.OptTextFields.selected;
		}
		
		public function get remoteCopy():Boolean 
		{
			return _view.remoteCopy.selected;
		}
		
		public function get autoPlay():Boolean 
		{
			return _view.autoPlay.selected;
		}
		
		public function get selectedFolder():String
		{
			return _view.mediaFolders.selectedItem.data;
		}

		private function onRemoteCopyClick(e:MouseEvent):void 
		{
			_view.autoPlay.enabled = _view.remoteCopy.selected;
			_view.mediaFolders.enabled = _view.remoteCopy.selected;
		}
		
		private function setDefaultAA(event:MouseEvent):void 
		{
			_view.Thickness.text = "20";
			_view.Sharpness.text = "-168";
		}
		
	}

}