package caspar.tools.pages
{
	import caspar.network.data.CasparItemInfoCollection;
	import caspar.network.data.ICasparItemInfo;
	import caspar.network.ServerConnectionEvent;
	import caspar.tools.utils.CasparConnection;
	import caurina.transitions.Tweener;
	import fl.controls.ComboBox;
	import fl.core.UIComponent;
	import flash.events.Event;
	import flash.events.MouseEvent;
	
	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	public class Playout
	{
		private var _view:PlayoutView;
		private var _casparConnection:CasparConnection;
		private var _itemList:CasparItemInfoCollection;
		private var _openTemplateFolders:Boolean;
		
		//TODO: Remove testfunction
		private var _testTrace:Function;
		//stores if the current template should be autoplayed. Used when called from generate
		private var _autoPlay:Boolean;
		private var _generatedTemplate:String = "";
		
		public function Playout(view:PlayoutView, connection:CasparConnection, testTrace:Function)
		{
			_testTrace = testTrace;
			_view = view;
			_casparConnection = connection;
			_casparConnection.addEventListener(ServerConnectionEvent.ON_CONNECT, onSocketConnect);
			_casparConnection.addEventListener(ServerConnectionEvent.ON_DISCONNECT, onSocketDisconnect);
			
			for (var i:int = 0; i < _view.numChildren; i++)
			{
				if (_view.getChildAt(i) as UIComponent != null)
					UIComponent(_view.getChildAt(i)).enabled = false;
			}
			
			init();
		}
		
		private function onGetMediaFiles(e:ServerConnectionEvent):void 
		{
			_casparConnection.connection.removeEventListener(ServerConnectionEvent.ON_GET_MEDIAFILES, onGetMediaFiles);
			var mediaList:CasparItemInfoCollection = e.itemList;
			var files:Array = e.itemList.getItems();
			//_view.TemplateFolders.replaceItemAt({ label: "-Choose folder-", data: "null"}, 0);
			_view.Mediafiles.removeAll();
			for each (var currentItem:ICasparItemInfo in files)
			{
				_view.Mediafiles.addItem({label: currentItem.name, data: currentItem});
			}
			_view.Mediafiles.enabled = true;

		}
		
		private function init():void
		{
			_view.TemplateFolders.addEventListener(Event.CHANGE, onSelectFolder);
			_view.TemplateLoad.addEventListener(MouseEvent.CLICK, onTemplateLoad);
			_view.TemplatePlay.addEventListener(MouseEvent.CLICK, onTemplatePlay);
			_view.TemplateLoadPlay.addEventListener(MouseEvent.CLICK, onTemplateLoadPlay);
			_view.TemplateStop.addEventListener(MouseEvent.CLICK, onTemplateStop);
			_view.TemplateNext.addEventListener(MouseEvent.CLICK, onTemplateNext);
			_view.TemplateSetData.addEventListener(MouseEvent.CLICK, onTemplateSetData);
			_view.TemplateInvoke.addEventListener(MouseEvent.CLICK, onTemplateInvoke);
			//_view.TemplateGoto.addEventListener(MouseEvent.CLICK, onTemplateGoto);
			_view.TemplateClear.addEventListener(MouseEvent.CLICK, onTemplateClear);
			_view.TemplateRemove.addEventListener(MouseEvent.CLICK, onTemplateRemove);
			
			_view.MediaLoad.addEventListener(MouseEvent.CLICK, onMediaLoad);
			_view.MediaLoadPlay.addEventListener(MouseEvent.CLICK, onMediaLoadPlay);
			_view.MediaPlay.addEventListener(MouseEvent.CLICK, onMediaPlay);
			_view.MediaStop.addEventListener(MouseEvent.CLICK, onMediaStop);
			
			_openTemplateFolders = false;
		}
		
		private function onMediaPlay(e:MouseEvent):void
		{
			if (_view.Mediafiles.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.PlayMedia(_casparConnection.currentServer.channel);
			}
		}
		
		private function onMediaStop(e:MouseEvent):void
		{
			if (_view.Mediafiles.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.StopMedia(_casparConnection.currentServer.channel);
			}
		}
		
		private function onMediaLoadPlay(e:MouseEvent):void
		{
			if (_view.Mediafiles.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.LoadMedia(_casparConnection.currentServer.channel, _view.Mediafiles.selectedItem.data.path, _view.Loop.selected);
				_casparConnection.connection.PlayMedia(_casparConnection.currentServer.channel);
			}
		}
		
		private function onMediaLoad(e:MouseEvent):void
		{
			if (_view.Mediafiles.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.LoadMediaBG(_casparConnection.currentServer.channel, _view.Mediafiles.selectedItem.data.path, _view.Loop.selected);
			}
		}
		
		private function onTemplateClear(e:MouseEvent):void
		{
			_casparConnection.connection.ClearTemplates(_casparConnection.currentServer.channel);
		}
		
		private function onTemplateRemove(e:MouseEvent):void
		{
			if (_view.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.RemoveTemplate(_casparConnection.currentServer.channel, int(_view.TemplateLayer.value));
			}
		}
		
		//private function onTemplateGoto(e:MouseEvent):void
		//{
			//if (_view.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			//{
				//_casparConnection.connection.GotoLabel(_casparConnection.currentServer.channel, int(_view.TemplateLayer.value), _view.TemplateGotoText.text);
			//}
		//}
		
		private function onTemplateInvoke(e:MouseEvent):void
		{
			if (_view.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.Invoke(_casparConnection.currentServer.channel, int(_view.TemplateLayer.value), _view.TemplateInvokeText.text);
			}
		}
		
		private function onTemplateSetData(e:MouseEvent):void
		{
			if (_view.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.SetData(_casparConnection.currentServer.channel, int(_view.TemplateLayer.value), generateTemplateData());
			}
		}
		
		private function generateTemplateData():XML
		{
			var templateData:XML = new XML(<templateData>
					<componentData id={_view.name0.text}>
						<data id="text" value={_view.value0.text} /> 
					</componentData>
					<componentData id={_view.name1.text}>
						< data id = "text" value = { _view.value1.text } />
					</componentData>
					<componentData id={_view.name2.text}>
						<data id="text" value={_view.value2.text} />
					</componentData>
					<componentData id={_view.name3.text}>
						<data id="text" value={_view.value3.text} />
					</componentData>
				</templateData> );
			return templateData;
		}
		
		private function onTemplateNext(e:MouseEvent):void
		{
			if (_view.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.Next(_casparConnection.currentServer.channel, int(_view.TemplateLayer.value));
			}
		}
		
		private function onTemplateLoadPlay(e:MouseEvent):void
		{
			if (_view.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.LoadTemplate(_casparConnection.currentServer.channel, int(_view.TemplateLayer.value), ICasparItemInfo(_view.TemplateTemplates.selectedItem.data).path, true, generateTemplateData());
			}
		}
		
		private function onTemplateLoad(e:MouseEvent):void
		{
			if (_view.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.LoadTemplate(_casparConnection.currentServer.channel, int(_view.TemplateLayer.value), ICasparItemInfo(_view.TemplateTemplates.selectedItem.data).path, false, generateTemplateData());
			}
		}
		
		private function onTemplateStop(e:MouseEvent):void
		{
			if (_view.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.StopTemplate(_casparConnection.currentServer.channel, int(_view.TemplateLayer.value));
			}
		}
		
		private function onTemplatePlay(e:MouseEvent):void
		{
			if (_view.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.PlayTemplate(_casparConnection.currentServer.channel, int(_view.TemplateLayer.value));
			}
		}
		
		private function onSocketDisconnect(e:ServerConnectionEvent):void
		{
			for (var i:int = 0; i < _view.numChildren; i++)
			{
				if (_view.getChildAt(i) as UIComponent != null)
					UIComponent(_view.getChildAt(i)).enabled = false;
			}
			
			
			_view.TemplateFolders.removeAll();
			_view.TemplateTemplates.removeAll();
			_view.Mediafiles.removeAll();
			_view.TemplateFolders.addItem({label: "-Not connected-", data: "nc"});
			_view.TemplateTemplates.addItem({label: "-Not connected-", data: "nc"});
			_view.Mediafiles.addItem({label: "-Not connected-", data: "nc"});
		}
		
		private function onSocketConnect(e:ServerConnectionEvent):void
		{
			for (var i:int = 0; i < _view.numChildren; i++)
			{
				if (_view.getChildAt(i) as UIComponent != null)
					UIComponent(_view.getChildAt(i)).enabled = true;
			}
			
			_casparConnection.connection.addEventListener(ServerConnectionEvent.ON_GET_MEDIAFILES, onGetMediaFiles);
			_casparConnection.connection.GetMediaFiles();
		
			//_view.TemplateTemplates.replaceItemAt({ label: "-Choose template-", data: "null"}, 0);
		}
		
		public function indicateSelectFolder():void
		{
			_openTemplateFolders = true;
			_view.TemplateFolders.open();
			//Tweener.addTween();
		}
		
		public function onGetTemplates(e:ServerConnectionEvent):void
		{
			//_testTrace("-> Har nya templater");
			_itemList = e.itemList;
			var folders:Array = _itemList.getFolders();
			//_view.TemplateFolders.replaceItemAt({ label: "-Choose folder-", data: "null"}, 0);
			_view.TemplateFolders.removeAll();
			for each (var currentFolder:String in folders)
			{
				
				if (currentFolder != "")
				{
					_view.TemplateFolders.addItem({label: currentFolder, data: currentFolder});
				}
				else
				{
					_view.TemplateFolders.addItem({label: "_ROOT", data: currentFolder});
				}
			}
			
			_view.TemplateFolders.selectedIndex = 0;
			
			_view.TemplateFolders.enabled = true;
			
			_view.TemplateFolders.dispatchEvent(new Event(Event.CHANGE));
			
			_testTrace("-> Har fyllt på");
		}
		
		//TODO: selecta folder 
		private function onSelectFolder(event:Event):void
		{
			var folder:String = ComboBox(event.target).selectedItem.data;
			var templates:Array = [];
			templates = _itemList.getItemsInFolder(folder);
			
			_view.TemplateTemplates.replaceItemAt({label: "-Choose template-", data: "null"}, 0);
			_view.TemplateTemplates.removeAll();
			var i:int = 0;
			
			for each (var currentTemplate:ICasparItemInfo in templates)
			{
				if (currentTemplate != "")
				{
					_view.TemplateTemplates.addItem({label: currentTemplate.name, data: currentTemplate});
					//autoselect template from selectTemplate()
					_testTrace("-> Letar efter mallen! " + _generatedTemplate + "nuvarande namn: " + currentTemplate.name);
					if (_generatedTemplate != "" && _generatedTemplate.toUpperCase() == currentTemplate.name.toUpperCase())
					{
						_testTrace("-> HITTADE mallen! " + _generatedTemplate + i);
						_view.TemplateTemplates.selectedIndex = i;
						_generatedTemplate = "";
						if (_autoPlay)
							_view.TemplateLoadPlay.dispatchEvent(new MouseEvent(MouseEvent.CLICK));
					}
					i++;
				}
			}
			_view.TemplateTemplates.enabled = true;
		}
		
		public function selectTemplate(folder:String, templateName:String, autoPlay:Boolean):void
		{
			_autoPlay = autoPlay;
			_generatedTemplate = templateName;
			_testTrace("-> INNE I SELECT TEMPLATE " + _view.TemplateFolders.dataProvider.length);
			for (var i:int = 0; i < _view.TemplateFolders.dataProvider.length; i++)
			{
				if (_view.TemplateFolders.getItemAt(i).data == folder)
				{
					_view.TemplateFolders.selectedIndex = i;
					_testTrace("-> HITTADE! " + folder);
					_view.TemplateFolders.dispatchEvent(new Event(Event.CHANGE));
					break;
				}
			}
		}
	
	}

}