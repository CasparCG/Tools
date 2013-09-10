package caspar.tools.pages
{
	import caspar.network.data.CasparItemInfoCollection;
	import caspar.network.data.ICasparItemInfo;
	import caspar.network.data.IItemList;
	import caspar.network.ServerConnectionEvent;
	import caspar.tools.utils.CasparConnection;
	import caurina.transitions.Tweener;
	import fl.controls.ComboBox;
	import fl.core.UIComponent;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.events.MouseEvent;
	
	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	public class Playout
	{
		static public const PLAY_OUT_TYPE_TEMPLATE:String = "playOutTypeTemplate";
		static public const PLAY_OUT_TYPE_MEDIA:String = "playOutTypeMedia";
		
		private var _view:PlayoutView;
		private var _casparConnection:CasparConnection;
		private var _templateList:IItemList;
		private var _mediaList:IItemList;
		private var _openTemplateFolders:Boolean;
		
		//TODO: Remove testfunction
		private var _testTrace:Function;
		//stores if the current template should be autoplayed. Used when called from generate
		private var _autoPlay:Boolean;
		private var _generatedTemplate:String = "";
		private var _generatedMediafile:String = "";
		
		private var _mediaPlayOutView:MediaPlayOutView;
		private var _templatePlayOutView:TemplatePlayOutView;
		
		private var _playOutTypes:Vector.<Object>;
		
		
		public function Playout(view:PlayoutView, connection:CasparConnection, testTrace:Function)
		{
			_testTrace 				= testTrace;
			
			_view 					= view;
			_mediaPlayOutView 		= _view.mediaPlayOutView;
			_templatePlayOutView 	= _view.templatePlayOutView;
			
			_casparConnection = connection;
			_casparConnection.addEventListener(ServerConnectionEvent.ON_CONNECT, onSocketConnect);
			_casparConnection.addEventListener(ServerConnectionEvent.ON_DISCONNECT, onSocketDisconnect);
			
			enableUIComponents(_view, false);
			enableUIComponents(_mediaPlayOutView, false);
			enableUIComponents(_templatePlayOutView, false);
			
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
		
		private function init():void
		{
			_mediaPlayOutView.ScrollSpeed.restrict = "0-9\\+\\-";
			
			_templatePlayOutView.TemplateFolders.addEventListener(Event.CHANGE, onSelectTemplateFolder);
			_templatePlayOutView.TemplateLoad.addEventListener(MouseEvent.CLICK, onTemplateLoad);
			_templatePlayOutView.TemplatePlay.addEventListener(MouseEvent.CLICK, onTemplatePlay);
			_templatePlayOutView.TemplateLoadPlay.addEventListener(MouseEvent.CLICK, onTemplateLoadPlay);
			_templatePlayOutView.TemplateStop.addEventListener(MouseEvent.CLICK, onTemplateStop);
			_templatePlayOutView.TemplateNext.addEventListener(MouseEvent.CLICK, onTemplateNext);
			_templatePlayOutView.TemplateSetData.addEventListener(MouseEvent.CLICK, onTemplateSetData);
			_templatePlayOutView.TemplateInvoke.addEventListener(MouseEvent.CLICK, onTemplateInvoke);
			_templatePlayOutView.TemplateGoto.addEventListener(MouseEvent.CLICK, onTemplateGoto);
			_templatePlayOutView.TemplateClear.addEventListener(MouseEvent.CLICK, onClear);
			_templatePlayOutView.TemplateRemove.addEventListener(MouseEvent.CLICK, onTemplateRemove);
			
			_mediaPlayOutView.MediaFolders.addEventListener(Event.CHANGE, onSelectMediaFolder);
			_mediaPlayOutView.MediaLoad.addEventListener(MouseEvent.CLICK, onMediaLoad);
			_mediaPlayOutView.MediaLoadPlay.addEventListener(MouseEvent.CLICK, onMediaLoadPlay);
			_mediaPlayOutView.MediaPlay.addEventListener(MouseEvent.CLICK, onMediaPlay);
			_mediaPlayOutView.MediaStop.addEventListener(MouseEvent.CLICK, onMediaStop);
			_mediaPlayOutView.MediaClear.addEventListener(MouseEvent.CLICK, onClear);
			
			_openTemplateFolders = false;
			
			initPlayOutTypes();
			
		}
		
		private function initPlayOutTypes():void 
		{
			_view.PlayOutTypes.removeAll();
			_playOutTypes = new Vector.<Object>();
			_playOutTypes.push({type: PLAY_OUT_TYPE_MEDIA, label: "Media"});
			_playOutTypes.push({type: PLAY_OUT_TYPE_TEMPLATE, label: "Template"});
			
			for (var i:uint = 0; i < _playOutTypes.length; i++ )
			{
				_view.PlayOutTypes.addItem({label: _playOutTypes[i].label, data: _playOutTypes[i].type});
			}
			
			_view.PlayOutTypes.selectedIndex 	= 0;
			_view.PlayOutTypes.enabled 			= true;
			
			_view.PlayOutTypes.addEventListener(Event.CHANGE, onSelectPlayOutType);
			_view.PlayOutTypes.dispatchEvent(new Event(Event.CHANGE));
			_mediaPlayOutView.alpha = 0;
			_templatePlayOutView.alpha = 0;
			
		}
		
		private function onClear(e:MouseEvent):void
		{
			_casparConnection.connection.SendCommand("CLEAR 1");
			_casparConnection.connection.SendCommand("MIXER 1 CLEAR");
		}
		
		private function onSelectPlayOutType(e:Event):void 
		{
			var type:String = ComboBox(e.target).selectedItem.data;
			
			if (type == PLAY_OUT_TYPE_MEDIA)
			{
				_mediaPlayOutView.visible = true;
				Tweener.addTween(_mediaPlayOutView, { delay: 0, time: 0.5, alpha: 1, onComplete: hidePlayOutTypeView, onCompleteParams: [_templatePlayOutView] });
				Tweener.addTween(_templatePlayOutView, { delay: 0, time: 0.5, alpha: 0 } );
				
				enableUIComponents(_mediaPlayOutView, true);
				enableUIComponents(_templatePlayOutView, false);
			}
			else if (type == PLAY_OUT_TYPE_TEMPLATE)
			{
				_templatePlayOutView.visible = true;
				Tweener.addTween(_mediaPlayOutView, { delay: 0, time: 0.5, alpha: 0 } );
				Tweener.addTween(_templatePlayOutView, { delay: 0, time: 0.5, alpha: 1, onComplete: hidePlayOutTypeView, onCompleteParams: [_mediaPlayOutView] } );
				
				enableUIComponents(_mediaPlayOutView, false);
				enableUIComponents(_templatePlayOutView, true);
			}
		}
		
		private function hidePlayOutTypeView(mc:MovieClip):void
		{
			mc.visible = false;
		}
		
		private function onMediaPlay(e:MouseEvent):void
		{
			trace("ON MEDIA PLAY");
			if (_mediaPlayOutView.Mediafiles.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.PlayMedia(_casparConnection.currentServer.channel, int(_mediaPlayOutView.MediaVideoLayer.value));
			}
		}
		
		private function onMediaStop(e:MouseEvent):void
		{
			if (_mediaPlayOutView.Mediafiles.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.StopMedia(_casparConnection.currentServer.channel, int(_mediaPlayOutView.MediaVideoLayer.value));
			}
		}
		
		private function onMediaLoadPlay(e:MouseEvent):void
		{
			//Main.tracer("PLAY 1");
			if (_mediaPlayOutView.Mediafiles.selectedItem.data as ICasparItemInfo != null)
			{
				//Main.tracer("PLAY 2");
				if (_mediaPlayOutView.ScrollSpeed.text != "")
				{
					//Main.tracer("PLAY 3");
					_casparConnection.connection.SendCommand("PLAY " + _casparConnection.currentServer.channel + "-" + int(_mediaPlayOutView.MediaVideoLayer.value) + " " + _mediaPlayOutView.Mediafiles.selectedItem.data.path + " SPEED " + _mediaPlayOutView.ScrollSpeed.text);
				}
				else
				{
					//Main.tracer("PLAY 4");
					_casparConnection.connection.LoadMedia(_casparConnection.currentServer.channel, int(_mediaPlayOutView.MediaVideoLayer.value), _mediaPlayOutView.Mediafiles.selectedItem.data.path, _mediaPlayOutView.Loop.selected);
					_casparConnection.connection.PlayMedia(_casparConnection.currentServer.channel, int(_mediaPlayOutView.MediaVideoLayer.value));
				}
				
				
			}
		}
		
		private function onMediaLoad(e:MouseEvent):void
		{
			if (_mediaPlayOutView.Mediafiles.selectedItem.data as ICasparItemInfo != null)
			{
				
				if (_mediaPlayOutView.ScrollSpeed.text != "")
				{
					_casparConnection.connection.SendCommand("LOADBG " + _casparConnection.currentServer.channel + "-" + int(_mediaPlayOutView.MediaVideoLayer.value) + " " +  _mediaPlayOutView.Mediafiles.selectedItem.data.path + " SPEED " + _mediaPlayOutView.ScrollSpeed.text);
				}
				else
				{
					_casparConnection.connection.LoadMediaBG(_casparConnection.currentServer.channel, int(_mediaPlayOutView.MediaVideoLayer.value), _mediaPlayOutView.Mediafiles.selectedItem.data.path, _mediaPlayOutView.Loop.selected);
				}
				
			}
		}
		
		private function onTemplateClear(e:MouseEvent):void
		{
			_casparConnection.connection.ClearTemplates(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value));
		}
		
		private function onTemplateRemove(e:MouseEvent):void
		{
			if (_templatePlayOutView.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.RemoveTemplate(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value), int(_templatePlayOutView.TemplateFlashLayer.value));
			}
		}
		
		private function onTemplateGoto(e:MouseEvent):void
		{
			if (_templatePlayOutView.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.GoTo(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value), int(_templatePlayOutView.TemplateFlashLayer.value), _templatePlayOutView.TemplateGotoText.text);
			}
		}
		
		private function onTemplateInvoke(e:MouseEvent):void
		{
			if (_templatePlayOutView.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.Invoke(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value), int(_templatePlayOutView.TemplateFlashLayer.value), _templatePlayOutView.TemplateInvokeText.text);
			}
		}
		
		private function onTemplateSetData(e:MouseEvent):void
		{
			if (_templatePlayOutView.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.SetData(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value), int(_templatePlayOutView.TemplateFlashLayer.value), generateTemplateData());
			}
		}
		
		private function generateTemplateData():XML
		{
			var templateData:XML = new XML(<templateData>
					<componentData id={_templatePlayOutView.name0.text}>
						<data id="text" value={_templatePlayOutView.value0.text} /> 
					</componentData>
					<componentData id={_templatePlayOutView.name1.text}>
						< data id = "text" value = { _templatePlayOutView.value1.text } />
					</componentData>
					<componentData id={_templatePlayOutView.name2.text}>
						<data id="text" value={_templatePlayOutView.value2.text} />
					</componentData>
					<componentData id={_templatePlayOutView.name3.text}>
						<data id="text" value={_templatePlayOutView.value3.text} />
					</componentData>
				</templateData> );
			return templateData;
		}
		
		private function onTemplateNext(e:MouseEvent):void
		{
			if (_templatePlayOutView.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.Next(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value), int(_templatePlayOutView.TemplateFlashLayer.value));
			}
		}
		
		private function onTemplateLoadPlay(e:MouseEvent):void
		{
			trace("on TEMPLATE LOAD PLAY");
			if (_templatePlayOutView.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.LoadTemplate(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value), int(_templatePlayOutView.TemplateFlashLayer.value), ICasparItemInfo(_templatePlayOutView.TemplateTemplates.selectedItem.data).path, true, generateTemplateData());
			}
		}
		
		private function onTemplateLoad(e:MouseEvent):void
		{
			if (_templatePlayOutView.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.LoadTemplate(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value), int(_templatePlayOutView.TemplateFlashLayer.value), ICasparItemInfo(_templatePlayOutView.TemplateTemplates.selectedItem.data).path, false, generateTemplateData());
			}
		}
		
		private function onTemplateStop(e:MouseEvent):void
		{
			if (_templatePlayOutView.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.StopTemplate(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value), int(_templatePlayOutView.TemplateFlashLayer.value));
			}
		}
		
		private function onTemplatePlay(e:MouseEvent):void
		{
			if (_templatePlayOutView.TemplateTemplates.selectedItem.data as ICasparItemInfo != null)
			{
				_casparConnection.connection.PlayTemplate(_casparConnection.currentServer.channel, int(_templatePlayOutView.TemplateVideoLayer.value), int(_templatePlayOutView.TemplateFlashLayer.value));
			}
		}
		
		private function onSocketDisconnect(e:ServerConnectionEvent):void
		{
			enableUIComponents(_view, false);
			enableUIComponents(_templatePlayOutView, false);
			enableUIComponents(_mediaPlayOutView, false);
			
			
			_templatePlayOutView.TemplateFolders.removeAll();
			_templatePlayOutView.TemplateTemplates.removeAll();
			_mediaPlayOutView.Mediafiles.removeAll();
			_mediaPlayOutView.MediaFolders.removeAll();
			_templatePlayOutView.TemplateFolders.addItem({label: "-Not connected-", data: "nc"});
			_templatePlayOutView.TemplateTemplates.addItem({label: "-Not connected-", data: "nc"});
			_mediaPlayOutView.MediaFolders.addItem({label: "-Not connected-", data: "nc"});
			_mediaPlayOutView.Mediafiles.addItem({label: "-Not connected-", data: "nc"});
		}
		
		private function onSocketConnect(e:ServerConnectionEvent):void
		{
			trace("********************conect");
			enableUIComponents(_view, true);
			enableUIComponents(_templatePlayOutView, true);
			enableUIComponents(_mediaPlayOutView, true);
			
			_casparConnection.connection.addEventListener(ServerConnectionEvent.ON_GET_MEDIAFILES, onGetMediaFiles);
			//_casparConnection.connection.GetMediaFiles();
			
			_casparConnection.connection.addEventListener(ServerConnectionEvent.ON_GET_TEMPLATES, onGetTemplates);
			//_casparConnection.connection.GetTemplates();
			
			//_view.TemplateTemplates.replaceItemAt({ label: "-Choose template-", data: "null"}, 0);
		}
		
		public function indicateSelectFolder():void
		{
			_openTemplateFolders = true;
			_templatePlayOutView.TemplateFolders.open();
			//Tweener.addTween();
		}
		
		public function onGetMediaFiles(e:ServerConnectionEvent):void 
		{
			_mediaList 				= e.itemList;
			
			var folders:Array = _mediaList.getFolders();
			//_view.TemplateFolders.replaceItemAt({ label: "-Choose folder-", data: "null"}, 0);
			_mediaPlayOutView.MediaFolders.removeAll();
			for each (var currentFolder:String in folders)
			{
				if (currentFolder != "")
				{
					_mediaPlayOutView.MediaFolders.addItem({label: currentFolder, data: currentFolder});
				}
				else
				{
					_mediaPlayOutView.MediaFolders.addItem({label: "_ROOT", data: currentFolder});
				}
			}
			
			_mediaPlayOutView.MediaFolders.selectedIndex = 0;
			
			_mediaPlayOutView.MediaFolders.enabled = true;
			
			/*var files:Array 		= e.itemList.getItems();
			//_view.TemplateFolders.replaceItemAt({ label: "-Choose folder-", data: "null"}, 0);
			_view.Mediafiles.removeAll();
			for each (var currentItem:ICasparItemInfo in files)
			{
				_view.Mediafiles.addItem({label: currentItem.name, data: currentItem});
			}
			_view.Mediafiles.enabled = true;
			*/
			_mediaPlayOutView.MediaFolders.dispatchEvent(new Event(Event.CHANGE));
			
			//_testTrace("-> Har fyllt på");
			
			
		}
		
		private function onSelectMediaFolder(event:Event):void
		{
			//Main.tracer("onSelectMediaFolder", true);
			var folder:String = ComboBox(event.target).selectedItem.data;
			var files:Array = [];
			files = _mediaList.getItemsInFolder(folder);
			
			_mediaPlayOutView.Mediafiles.replaceItemAt({label: "-Choose media-", data: "null"}, 0);
			_mediaPlayOutView.Mediafiles.removeAll();
			var i:int = 0;
			
			for each (var currentItem:ICasparItemInfo in files)
			{
				if (currentItem != "")
				{
					_mediaPlayOutView.Mediafiles.addItem({label: currentItem.name, data: currentItem});
					//autoselect template from selectTemplate()
					//_testTrace("-> Letar efter mallen! " + _generatedMediafile + "nuvarande namn: " + currentItem.name);
					//Main.tracer("Playout onSelectMediaFolder: " + _generatedMediafile.toUpperCase() +  " == " + currentItem.name.toUpperCase());
					if (_generatedMediafile != "" && _generatedMediafile.toUpperCase() == currentItem.name.toUpperCase())
					{
						//Main.tracer("FOUND MEDIA: " + _autoPlay);
						//_testTrace("-> HITTADE mallen! " + _generatedTemplate + i);
						_mediaPlayOutView.Mediafiles.selectedIndex = i;
						_generatedMediafile = "";
						if (_autoPlay)
							_mediaPlayOutView.MediaLoadPlay.dispatchEvent(new MouseEvent(MouseEvent.CLICK));
					}
					i++;
				}
			}
			_mediaPlayOutView.Mediafiles.enabled = true;
		}
		
		public function onGetTemplates(e:ServerConnectionEvent):void
		{
			trace("''''''''''''''''' got templates!!");
			//_testTrace("-> Har nya templater");
			_templateList = e.itemList;
			var folders:Array = _templateList.getFolders();
			//_templatePlayOutView.TemplateFolders.replaceItemAt({ label: "-Choose folder-", data: "null"}, 0);
			_templatePlayOutView.TemplateFolders.removeAll();
			for each (var currentFolder:String in folders)
			{
				trace(currentFolder);
				if (currentFolder != "")
				{
					_templatePlayOutView.TemplateFolders.addItem({label: currentFolder, data: currentFolder});
				}
				else
				{
					_templatePlayOutView.TemplateFolders.addItem({label: "_ROOT", data: currentFolder});
				}
			}
			
			_templatePlayOutView.TemplateFolders.selectedIndex = 0;
			
			_templatePlayOutView.TemplateFolders.enabled = true;
			
			_templatePlayOutView.TemplateFolders.dispatchEvent(new Event(Event.CHANGE));
			
			//_testTrace("-> Har fyllt på");
		}
		
		//TODO: selecta folder 
		private function onSelectTemplateFolder(event:Event):void
		{
			var folder:String = ComboBox(event.target).selectedItem.data;
			var templates:Array = [];
			templates = _templateList.getItemsInFolder(folder);
			
			_templatePlayOutView.TemplateTemplates.replaceItemAt({label: "-Choose template-", data: "null"}, 0);
			_templatePlayOutView.TemplateTemplates.removeAll();
			var i:int = 0;
			
			for each (var currentTemplate:ICasparItemInfo in templates)
			{
				if (currentTemplate != "")
				{
					_templatePlayOutView.TemplateTemplates.addItem({label: currentTemplate.name, data: currentTemplate});
					//autoselect template from selectTemplate()
					//_testTrace("-> Letar efter mallen! " + _generatedTemplate + "nuvarande namn: " + currentTemplate.name);
					if (_generatedTemplate != "" && _generatedTemplate.toUpperCase() == currentTemplate.name.toUpperCase())
					{
						//_testTrace("-> HITTADE mallen! " + _generatedTemplate + i);
						_templatePlayOutView.TemplateTemplates.selectedIndex = i;
						_generatedTemplate = "";
						if (_autoPlay)
							_templatePlayOutView.TemplateLoadPlay.dispatchEvent(new MouseEvent(MouseEvent.CLICK));
					}
					i++;
				}
			}
			_templatePlayOutView.TemplateTemplates.enabled = true;
		}
		
		public function selectTemplate(folder:String, templateName:String, autoPlay:Boolean):void
		{
			_autoPlay = autoPlay;
			_generatedTemplate = templateName;
			//_testTrace("-> INNE I SELECT TEMPLATE " + _templatePlayOutView.TemplateFolders.dataProvider.length);
			for (var i:int = 0; i < _templatePlayOutView.TemplateFolders.dataProvider.length; i++)
			{
				if (_templatePlayOutView.TemplateFolders.getItemAt(i).data == folder)
				{
					_templatePlayOutView.TemplateFolders.selectedIndex = i;
					//_testTrace("-> HITTADE! " + folder);
					_templatePlayOutView.TemplateFolders.dispatchEvent(new Event(Event.CHANGE));
					break;
				}
			}
		}
		
		public function selectMediaFile(folder:String, name:String, autoPlay:Boolean):void
		{
			_autoPlay = autoPlay;
			_generatedMediafile = name;
			//Main.tracer("SELECT MEDIA: " + _generatedMediafile + " :: " + folder + " :: " + _mediaPlayOutView.MediaFolders.dataProvider.length, true);
			
			for (var i:int = 0; i < _mediaPlayOutView.MediaFolders.dataProvider.length; i++)
			{
				//Main.tracer("FOLDER: " + _view.MediaFolders.getItemAt(i).data );
				if (_mediaPlayOutView.MediaFolders.getItemAt(i).data == folder)
				{
					_mediaPlayOutView.MediaFolders.selectedIndex = i;
					//Main.tracer("FOUND MEDIA: " + _mediaPlayOutView.MediaFolders.selectedIndex, true );
					_mediaPlayOutView.MediaFolders.dispatchEvent(new Event(Event.CHANGE));
					break;
				}
			}
		}
	
	}

}