package caspar.tools.pages
{
	import caspar.network.data.CasparItemInfoCollection;
	import caspar.network.ServerConnectionEvent;
	import fl.events.DataChangeEvent;
	import flash.events.Event;
	import flash.events.MouseEvent;

	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	public class Template 
	{
		private var _view:TemplateView;
		private var _itemList:CasparItemInfoCollection;
		
		public function Template(view:TemplateView) 
		{
			_view = view;
			init();
		}
		
		private function init():void
		{
			_view.TemplateFolders.enabled = false;
			_view.SetDefaultAA.addEventListener(MouseEvent.CLICK, setDefaultAA);
			_view.remoteCopy.enabled = false;
			_view.remoteCopy.addEventListener(MouseEvent.CLICK, onRemoteCopyClick);
			_view.autoPlay.enabled = false;
		}
		
		public function connected():void
		{
			_view.remoteCopy.enabled = true;
			_view.autoPlay.enabled = true;
			_view.autoPlay.enabled = _view.remoteCopy.selected;
			_view.TemplateFolders.enabled = _view.remoteCopy.selected;
		}
		
		public function disconnected():void
		{
			_view.TemplateFolders.removeAll();
			_view.TemplateFolders.addItem( { label: "-Not connected-", data: "nc" } );
			_view.remoteCopy.enabled = false;
			_view.autoPlay.enabled = false;
			_view.TemplateFolders.enabled = false;
		}
		
		public function onGetTemplates(e:ServerConnectionEvent):void 
		{
			_view.TemplateFolders.removeAll();
			_itemList = e.itemList; 
			var folders:Array = _itemList.getFolders();
			for each(var currentFolder:String in folders)
			{
				if (currentFolder != "")
				{
					_view.TemplateFolders.addItem( { label: currentFolder, data: currentFolder } );
				}
				else
				{
					_view.TemplateFolders.addItem( { label: "_ROOT", data: currentFolder } );
				}
			}
			//_view.TemplateFolders.enabled = true;
		}
		
		public function selectFolder(folder:String):void 
		{
			for (var i:int = 0; i < _view.TemplateFolders.dataProvider.length; i++ )
			{
				if (_view.TemplateFolders.getItemAt(i).data == folder)
				{
					_view.TemplateFolders.selectedIndex = i;
					break;
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
			return _view.remoteCopy.selected;
		}
		
		public function get selectedFolder():String
		{
			return _view.TemplateFolders.selectedItem.data;
		}

		private function onRemoteCopyClick(e:MouseEvent):void 
		{
			_view.autoPlay.enabled = _view.remoteCopy.selected;
			_view.TemplateFolders.enabled = _view.remoteCopy.selected;
		}
		
		private function setDefaultAA(event:MouseEvent):void 
		{
			_view.Thickness.text = "20";
			_view.Sharpness.text = "-168";
		}
		
	}

}