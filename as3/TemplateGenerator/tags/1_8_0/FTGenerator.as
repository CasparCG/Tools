/*
* copyright (c) 2010 Sveriges Television AB <info@casparcg.com>
*
*  This file is part of CasparCG.
*
*    CasparCG is free software: you can redistribute it and/or modify
*    it under the terms of the GNU General Public License as published by
*    the Free Software Foundation, either version 3 of the License, or
*    (at your option) any later version.
*
*    CasparCG is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU General Public License for more details.

*    You should have received a copy of the GNU General Public License
*    along with CasparCG.  If not, see <http://www.gnu.org/licenses/>.
*
*/

package 
{
	import adobe.utils.MMExecute;
	import fl.controls.ComboBox;
	import flash.display.MovieClip;
	import flash.display.StageAlign;
	import flash.events.*;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.text.TextField;
	import se.svt.caspar.communication.*;
	
	/**
	 * ...
	 * @author Andreas Jeansson, SVT
	 */
	public class FTGenerator extends MovieClip 
	{
		public var rcFolders:ComboBox;
		public var btnGenerate:MovieClip;
		public var rcServer:TextField;
		private var _currentServer:String;
		private var _hasConnection:Boolean = false;
		
		private var amcp:CasparAMCP;
		private var remotePath:String;
		private var remoteTemplatePath:String;
		private var templateData:XML;
		private var templateName:String;
		
		public function FTGenerator()
		{
			templateData = new XML(<templateData>
										<componentData id="f0">
											<data id="text" value="Niklas P Andersson" /> </componentData>
										<componentData id="f1">
											<data id="text" value="Developer" />
										</componentData>
										<componentData id="f2">
											<data id="text" value="Providing an example" />
										</componentData>
									</templateData> );
			//amcp.addEventListener(CasparAMCPEvent.ON_RESPONSE, onResponse);
			//amcp.socketConnect("d40020");
			//amcp.CGADD(1, 11, "Agenda/4021", true, "<templateData><componentData id=\"f0\"><data id=\"text\" value=\"Niklas P Andersson\"></data> </componentData><componentData id=\"f1\"><data id=\"text\" value=\"developer\"></data></componentData><componentData id=\"f2\"><data id=\"text\" value=\"Providing an example\"></data></componentData></templateData>");
			//amcp.CGADD(1, 11, "Agenda/4021", true, "testfromflash");
			//amcp.TLS("VetenskapensVarld\\");
			//amcp.TLS("VetenskapensVarld\\");
			//amcp.LOAD(1, "METTE", true);
			//amcp.LOADBG(1, "METTE", true, CasparAMCP.TRANSITION_SLIDE, 30, "FROMLEFT", "Valet_VR_Bg.jpg", 100);
			//amcp.STORE("testfromflash", new XML("<templateData><componentData id=\"f0\"><data id=\"text\" value=\"Niklas P Andersson\"></data> </componentData><componentData id=\"f1\"><data id=\"text\" value=\"developer\"></data></componentData><componentData id=\"f2\"><data id=\"text\" value=\"Providing an example\"></data></componentData></templateData>"));
			//amcp.PLAY(1);
			//amcp.CINF("X-2010-0602-SLOTT2");
			//amcp.RETRIEVE("TEST");
			//amcp.CGINVOKE(1, 11, "Stop");
			//amcp.CGUPDATE(1, 11, "<templateData><componentData id=\"f0\"><data id=\"text\" value=\"Niklas P Andersson\"></data> </componentData><componentData id=\"f1\"><data id=\"text\" value=\"developer\"></data></componentData><componentData id=\"f2\"><data id=\"text\" value=\"Providing an example\"></data> </componentData></templateData>");
			//amcp.CGGOTO(1, 11, "outro");
			//amcp.CGSTOP(1, 11);
			//amcp.CGSTOP(1, 11);
			//amcp.socketClose();
			
			stage.align = StageAlign.TOP_LEFT;
			stage.scaleMode = "noScale";
			stage.showDefaultContextMenu = false;

			btnGenerate.buttonMode = true;
			btnSetDefaultAA.buttonMode = true;
			btnGenerate.hl.alpha = 0;
			btnGenerate.addEventListener(MouseEvent.MOUSE_OVER, onMouseOverGenerate);
			btnGenerate.addEventListener(MouseEvent.MOUSE_OUT, onMouseOutGenerate);
			btnGenerate.addEventListener(MouseEvent.CLICK, generate);
			btnSetDefaultAA.addEventListener(MouseEvent.CLICK, setDefaultAA);
			
			btnPlay.buttonMode = true;
			btnPlay.addEventListener(MouseEvent.CLICK, onClickPlay);
			
			btnStop.buttonMode = true;
			btnStop.addEventListener(MouseEvent.CLICK, onClickStop);

			tinfo.text = "";
			
			//btnPreview.addEventListener(MouseEvent.CLICK, preview);

			readCache();
			//generate(null);
			remoteCopy.addEventListener(Event.CHANGE, onRemoteClick );
			
			rcServer.addEventListener(Event.CHANGE, onServerChange);
			
			_currentServer = rcServer.text;
			
			connected.green.visible = false;
			connected.red.visible = false;
			connected.buttonMode = true;
			this.rcFolders.enabled = false;
			connected.addEventListener(MouseEvent.CLICK, onConnect)
			stage.addEventListener(KeyboardEvent.KEY_UP, onKeyUp)
			stage.addEventListener(Event.DEACTIVATE, onDeactivate);
			stage.addEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
			stage.addEventListener(Event.CLOSE, onClose);
			stage.addEventListener(Event.REMOVED_FROM_STAGE, onRemoved);
			stage.addEventListener(Event.UNLOAD, onUnload);
			stage.addEventListener(MouseEvent.ROLL_OVER, onRollOver);
			
			
			
			
			var command:String = 'fl.runScript(fl.configURI+"WindowSWF/brew1_8.jsfl", "init");';
			MMExecute(command);
		}
		
		private function onClickStop(e:MouseEvent):void 
		{
			
			xtrace("onClickPlay " +templateName + " " + remoteTemplatePath);
			if (_hasConnection && remoteTemplatePath != "" )
			{
				xtrace("SKICKA TILL CASPAR ");
				amcp.CGSTOP(1, 99);
			}
		}
		
		private function onClickPlay(e:MouseEvent):void 
		{
			
			xtrace("onClickPlay " +templateName + " " + remoteTemplatePath);
			if (_hasConnection && remoteTemplatePath != "" )
			{
				xtrace("SKICKA TILL CASPAR ");
				amcp.CGADD(1, 99, remoteTemplatePath, true, templateData);
			}
		}
		
		//private function onResponse(e:CasparAMCPEvent):void 
		//{
			//trace("e.success", e.success, "e.error", e.error)
		//}
		
		private function onRollOver(e:MouseEvent):void 
		{
			MMExecute('fl.trace("onRollOver");');
		}
		
		private function onUnload(e:Event):void 
		{
			MMExecute('fl.trace("onUnload");');
		}
		
		private function onRemoved(e:Event):void 
		{
			removeEventListener(Event.REMOVED_FROM_STAGE, onRemoved);
			MMExecute('fl.trace("onRemove");');
		}
		
		private function onClose(e:Event):void 
		{
			MMExecute('fl.trace("onClose");');
		}
		
		private function onAddedToStage(e:Event):void 
		{
			removeEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
			MMExecute('fl.trace("onAddedToStage");');
		}
		
		private function onDeactivate(e:Event):void 
		{
			MMExecute('fl.trace("onDeactivate");');
		}
		
		private function onKeyUp(e:KeyboardEvent):void 
		{
			if (e.keyCode == 13)
			{
				if (!_hasConnection)
				{
					checkServerConnection();
				}
			}
		}
		
		private function onConnect(e:MouseEvent):void 
		{
			if (!_hasConnection)
			{
				checkServerConnection();
			}
		}
		
		private function onServerChange(e:Event):void 
		{
			disableRemoteCopy();
			rcFolders.removeAll();
			rcFolders.enabled = false;
			rcFolders.addItem({ label: "-Not connected-", data: null})
		}
		
		private function enableRemoteCopy():void 
		{
			connected.green.visible = true;
			connected.red.visible = false;
		}
		
		private function disableRemoteCopy():void 
		{
			connected.green.visible = false;
			connected.red.visible = false;
			_hasConnection = false;
		}
		
		private function onRemoteClick(e:Event):void 
		{
			_currentServer = rcServer.text;
			
			if (remoteCopy.selected && !_hasConnection)
			{
				checkServerConnection();
			}
			else
			{
				connected.green.visible = false;
				connected.red.visible = false;
			}
		}
		
		private function checkServerConnection():void 
		{
			_currentServer = rcServer.text;
			var command:String = 'fl.runScript(fl.configURI+"WindowSWF/brew1_8.jsfl", "checkServer", "' + _currentServer + '");';
			xtrace("kollar uppkoppling");
			if (MMExecute(command) == "true")
			{
				amcp = new CasparAMCP();
				amcp.socketConnect(_currentServer);
				enableRemoteCopy();
				getRemoteFolders();
				xtrace("klar med anrop!");
			}
			else
			{
				connected.green.visible = false;
				connected.red.visible = true;
				remoteCopy.selected = false;
			}
		}
		
		public function xtrace(s:String):void
		{
			out.text += "\n"+s;
		}
		
		private function getRemoteFolders():void 
		{
			var command:String = 'fl.runScript(fl.configURI+"WindowSWF/brew1_8.jsfl", "getFolders", "' + _currentServer + '");';
			//xtrace("FOLDERS:" +  folders);
			var folders:String = MMExecute(command);
			
			var folderArray:Array = folders.split("@@");
			if (folderArray.length > 0)
			{
				rcFolders.replaceItemAt({ label: "-Choose folder-", data: "null"}, 0);
				
				for (var i:int = 0; i < folderArray.length; i++)
				{
					//xtrace("populerar" +  folderArray[i]);
					rcFolders.addItem( { label: folderArray[i], data: folderArray[i] } );
				}
				
				rcFolders.enabled = true;
				_hasConnection = true;
			}
			
		}

		function onFileRead(event:Event):void
		{
			var a:Array = (event.target.data as String).split("#");
			if(a[0] != null)
			{
				aname.text = a[0];
			}
			if(a[1] != null)
			{
				amail.text = a[1];
			}
		}

		function onError(error:IOErrorEvent):void
		{
			trace(error.text);
		}
		
		function readCache():void
		{
			var url:String = "FTGeneratorCache.dat";
			var loader:URLLoader = new URLLoader();
			loader.addEventListener(Event.COMPLETE, onFileRead);
			loader.addEventListener(IOErrorEvent.IO_ERROR, onError);
			loader.load(new URLRequest(url));

		}

		function generate(event:MouseEvent):void {
			var info:String = tinfo.text;
			info = info.replace(/\r/g, " ");
			info = info.replace(/\&/g, "&amp;");
			info = info.replace(/\"/g, "&quot;");
			info = info.replace(/\</g, "&lt;");
			info = info.replace(/\>/g, "&gt;");
			info = info.replace(/\'/g, "&apos;");
			
			if(info == " ") info = "";
			
			var sname = aname.text.replace(/\r/g, "");
			sname = sname.replace(/\r/g, " ");
			sname = sname.replace(/\&/g, "&amp;");
			sname = sname.replace(/\"/g, "&quot;");
			sname = sname.replace(/\</g, "&lt;");
			sname = sname.replace(/\>/g, "&gt;");
			sname = sname.replace(/\'/g, "&apos;");
			
			var smail = amail.text.replace(/\r/g, "");
			smail = smail.replace(/\r/g, " ");
			smail = smail.replace(/\&/g, "&amp;");
			smail = smail.replace(/\"/g, "&quot;");
			smail = smail.replace(/\</g, "&lt;");
			smail = smail.replace(/\>/g, "&gt;");
			smail = smail.replace(/\'/g, "&apos;");
			
			remotePath = "";
			remoteTemplatePath = "";
			
			if (remoteCopy.selected)
			{
				if (rcServer.text != "" && rcFolders.text != "")
				{
					remotePath = "file://" + rcServer.text + "/" + "Caspar/" + rcFolders.text + "/"
					
				}
				else
				{
					trace("\n***WARNING***\nThe remote copy failed, you must specify both Caspar name and folder\n");
				}
			}
			
			var command:String;
			if(v16.selected)
			{
				command = 'fl.runScript(fl.configURI+"WindowSWF/brew1_6.jsfl", "init", ' + int(thickness.text) + ', ' + int(sharpness.text) + ', ' + false + ', "' + remotePath + '");';
			} 
			else if(v18.selected)
			{
				command = 'fl.runScript(fl.configURI+"WindowSWF/brew1_8.jsfl", "generate", ' + int(thickness.text) + ', ' + int(sharpness.text) + ', ' + false + ', "' + sname + '", "' + smail + '", "' + String(info) + '", "' + optImages.selected + '", "' + optTextFields.selected + '", "' + optVideos.selected + '", "' + remotePath + '");';
			}
			trace("command" + command);
			templateName = "";
			
			templateName = MMExecute(command);
			
			xtrace("TemplateName: " +templateName);
			
			if(templateName != "")
				remoteTemplatePath = rcFolders.text + "/" + templateName;
			
		}

		function onMouseOverGenerate(event:MouseEvent):void {
			btnGenerate.hl.alpha = 1;
		}

		function onMouseOutGenerate(event:MouseEvent):void {
			btnGenerate.hl.alpha = 0;
		}

		
		function preview(event:MouseEvent):void {
			MMExecute('fl.runScript(fl.configURI+"WindowSWF/preview.jsfl");');
		}

		function setDefaultAA(event:MouseEvent):void {
			thickness.text = "20";
			sharpness.text = "-168";
		}

	
	}
	
}