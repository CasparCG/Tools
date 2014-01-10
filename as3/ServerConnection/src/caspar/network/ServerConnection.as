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
*
*    You should have received a copy of the GNU General Public License
*    along with CasparCG.  If not, see <http://www.gnu.org/licenses/>.
*
*/

// TODO: What happens if caspar does not send a response? Create timeout.
// TODO: Check problems with disconnecting. Do you really get disconnected?
// TODO: What happens if connection to caspar is dropped? Testing needed.
// TODO: Check readResponse and see if we need to look for \r\n in the middle of the response.

package caspar.network
{
	import caspar.network.data.DataItemInfo;
	import caspar.network.data.DataItem;
	
	import flash.events.Event;
	import flash.events.EventDispatcher;
	import flash.events.TimerEvent;
	import flash.utils.Timer;

	/**
	 * ...
	 * @author Andreas Jeansson & Jesper Hansson, SVT
	 */
    public class ServerConnection extends EventDispatcher
	{
		public static const TRANSITION_CUT:String = "CUT";
		public static const TRANSITION_MIX:String = "MIX";
		public static const TRANSITION_PUSH:String = "PUSH";
		public static const TRANSITION_SLIDE:String = "SLIDE";
		public static const TRANSITION_WIPE:String = "WIPE";
		
		public static const DIRECTION_FROMLEFT:String = "FROMLEFT";
		public static const DIRECTION_FROMRIGHT:String = "FROMRIGHT";
		
		private const SOCKET_COMMAND_END:String = "";
		private const RECONNECT_DELAY:int = 1000;
		
		private var _socket:CustomSocket;
		private var _timer:Timer;

		private var _autoReconnect:Boolean = false;
		private var _userForcedDisconnection:Boolean = false;
		
        public function ServerConnection()
		{
			_socket = new CustomSocket();
			_timer = new Timer(RECONNECT_DELAY, 0);
			configureListeners();
		}
		
		///////////////////
		// Socket commands
		///////////////////
		
		/**
		 * Connects to a caspar server
		 * @param	server The server name
		 * @param	port The port number (default 5250)
		 * @param	autoReconnect Whether or not you want to keep the connection open
		 */
		public function connect(server:String, port:uint = 5250, autoReconnect:Boolean = true):void
		{
			_autoReconnect = autoReconnect;
			
			if ((_socket && _socket.connected) && (server == _socket.host) && (port == _socket.port))
			{
				//good
				trace("ServerConnection::you are connected");
			}
			else if ((_socket &&_socket.connected) && ((server != _socket.host) || (port != _socket.port)))
			{
				trace("ServerConnection::do new connection");
				disconnect();
				doNewConnection(server, port);
			}
			else
			{
				trace("ServerConnection::do connection", server, port);
				doNewConnection(server, port);
			}
		}
		
		/**
		 * Connection status
		 */
		public function get connected():Boolean
		{
			if (_socket != null)
			{
				return _socket.connected;
			}
			else
			{
				return false;
			}
		}
		
		/**
		 * Disconnect from caspar server
		 */
		public function disconnect():void 
		{
			if(_socket != null)
			{
				_userForcedDisconnection = true;
				if (_socket.connected)
				{
					_socket.close();
					
					/*var command:String = 'BYE' + SOCKET_COMMAND_END;
					_socket.addCommand( { command: command, type: ServerConnection.ON_OTHER_COMMAND } );
					*/
				}
			}
		}
		
		///////////
		// COMMANDS
		///////////
		
		/**
		 * Sends a custom command via the AMCP protocol
		 * @param	command The commmand
		 * @return
		 */
		public function SendCommand(command:String):String 
		{
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Swaps layers between channels (both foreground and background will be swapped). If layers are not specified then all layers in respective video_channel will be swapped.
		 * SWAP [channel1:int]{-[layer1:int]} [channel2:int]{-[layer2:int]}
		 * Examples:
		 * SWAP 1 2
		 * SWAP 1-1 2-3
		 * @param	channel1 Channel 1 
		 * @param	channel1 Channel 2 
		 * @param	layer1 Layer 1, if omitted the whole channel will be swapped
		 * @param	layer1 Layer 2, if omitted the whole channel will be swapped
		 */
		public function Swap(channel1:uint, channel2:uint, layer1:int = -1, layer2:int = -1):String 
		{
			var command:String = 'SWAP ' + channel1 + (layer1 != -1 ? "-" + layer1 : "") + " " + channel2 + (layer2 != -1 ? "-" + layer2 : "") + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Calls method on the specified producer with the provided param string.
		 * CALL [video_channel:int]{-[layer:int]|-0} [param:string]
		 * Examples:
		 * CALL 1 LOOP
		 * CALL 1-2 SEEK 25
		 * @param	channel The channel
		 * @param	layer The layer
		 * @param	param See example
		 */
		public function Call(channel:uint, layer:uint, param:String):String 
		{
			var command:String = 'CALL ' + channel + (layer != -1 ? "-" + layer : "") + " " + param + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		
		/**
		 * The string "parameters" will be parsed by available registered consumer factories. If a successful match is found a consumer will be created and added to the video_channel.
		 * Examples:
		 * ADD 1 DECKLINK 1
		 * ADD 1 BLUEFISH 2
		 * ADD 1 SCREEN
		 * ADD 1 AUDIO
		 * ADD 1 FILE test.mov
		 * e.g. In this last example it would be "FILE test.mov" you should pass to the "parameters" string.
		 * @param	channel The channel
		 * @param	parameters See example
		 */
		public function AddConsumer(channel:uint, parameters:String):String 
		{
			var command:String = 'ADD ' + channel + " " + parameters + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Removes an existing consumer from video_channel.
		 * REMOVE [video_channel:int] [parameters:string]
		 * Examples:
		 * REMOVE 1 DECKLINK 1
		 * REMOVE 1 BLUEFISH 2
		 * REMOVE 1 SCREEN
		 * REMOVE 1 AUDIO
		 * REMOVE 1 FILE test.mov
		 * e.g. In this last example it would be "FILE test.mov" you should pass to the "parameters" string.
		 * @param	channel The channel
		 * @param	parameters See example
		 */
		public function RemoveConsumer(channel:uint, parameters:String):String 
		{
			var command:String = 'REMOVE ' + channel + " " + parameters + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		
		/////////////////
		// Media Commands
		/////////////////
		
		/**
		 * Loads a clip in the background and prepares it for playout. It does not affect the currently playing clip in anyway. This is how you prepare a transition between two clips. Supply the LOOP parameter if you want the clip to loop.
		 * @param	channel The channel
		 * @param	layer The layer you want to load the Media to
		 * @param	file The file to load
		 * @param	loop Loop the clip (default: false)
		 * @param	transition The type of transition, use one of the transition contants in this class: ServerConnection.TRANSITION_CUT, ServerConnection.TRANSITION_MIX, ServerConnection.TRANSITION_PUSH, ServerConnection.TRANSITION_SLIDE, ServerConnection.TRANSITION_WIPE. (default: ServerConnection.TRANSITION_CUT)
		 * @param	duration The length of the transition in frames
		 * @param	direction Push, slide and wipe needs a direction, use one of the direction contants in this class: ServerConnection.DIRECTION_FROMLEFT and ServerConnection.DIRECTION_FROMRIGHT. (default: ServerConnection.DIRECTION_FROMLEFT)
		 * @param	border Push, slide and wipe can have a border. (filename / #aarrggbb).
		 * @param	borderWidth The width of the border if it’s not an image
		 */
		public function LoadMediaBG(channel:uint, layer:uint, file:String, loop:Boolean = false, transition:String = ServerConnection.TRANSITION_CUT, duration:int = 0, direction:String = ServerConnection.DIRECTION_FROMLEFT, border:String = "", borderWidth:int = 0):String 
		{
			var command:String = 'LOADBG ' + channel + '-' + layer + ' \"' + file + '\" ' + (loop ? "LOOP" : "") + ' ' + transition + ' ' + duration + ' ' + direction + ' \"' + border +'\" ' + borderWidth + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command; 
		}
		
		/**
		 * Loads a clip to the foreground and plays the first frame before pausing. If any clip is playing on the target foreground then this clip will be replaced.
		 * @param	channel The channel
		 * @param	layer The layer you want to load the Media to
		 * @param	file The file to load
		 * @param	loop Loop the clip (default: false)
		 */
		public function LoadMedia(channel:uint, layer:int, file:String, loop:Boolean = false):String 
		{
			var command:String = 'LOAD ' + channel + '-' + layer + ' \"' + file + '\" ' + (loop ? "LOOP" : "") + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Moves clip from background to foreground and starts playing it. If a transition, see LoadMediaBG(), is prepared, it will be executed. 
		 * @param	channel The channel
		 * @param	layer The layer you want to play the Media on
		 */
		public function PlayMedia(channel:uint, layer:uint):String 
		{
			var command:String = 'PLAY ' + channel + '-' + layer + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Pauses foreground clip.
		 * @param	channel The channel
		 * @param	layer The layer
		 */
		public function PauseMedia(channel:uint, layer:uint):String 
		{
			var command:String = 'PAUSE ' + channel + '-' + layer + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Removes foreground clip.
		 * @param	channel The channel
		 * @param	layer The layer
		 */
		public function StopMedia(channel:uint, layer:uint):String 
		{
			var command:String = 'STOP ' + channel + '-' + layer + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Removes all clips (both foreground and background). If no layer is specified then all layers in the specified video_channel are cleared.
		 * @param	channel The channel you want to clear
		 * @param	layer The specfic layer you want to clear
		 */
		public function ClearMedia(channel:uint, layer:int = -1):String 
		{
			var command:String = 'CLEAR ' + channel + (layer != -1 ? "-" + layer : "") + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		
		
		
		////////////////
		// Data Commands
		////////////////
		
		/**
		 * Stores a dataset, you can use help function inside DataItem to build template xml. Cannot create new directories
		 * @param	dataItem The data item, see http://casparcg.com/wiki/CasparCG_2.0_AMCP_Protocol#Template_Data
		 */
		public function StoreDataset(dataItem:DataItem):String 
		{
			var command:String = 'DATA STORE ' + dataItem.dataInfoItem.fullPath +' \"' + dataItem.contentAsString + '\"' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Returns a dataset. Will dispatch a ServerConnection.ON_DATARETRIEVE event if successfull otherwise a ServerConnection.ON_ERROR. Data is returned as a caspar.network.data.DataItem object
		 * @param	name The name of the data to retrieve
		 */
		public function GetData(dataInfoItem:DataItemInfo):String 
		{
			//BUG ON SUCCESS, CASPAR DOES NOT RETURN ANY RESPONSE CODE
			var command:String = 'DATA RETRIEVE \"' + dataInfoItem.fullPathAMCPFormatted +'\" \"' +'\"' + SOCKET_COMMAND_END;
			_socket.currentDataInfoItem = dataInfoItem;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_GET_DATA } );
			return command;
		}
		
		/**
		 * Returns a list of all saved datasets. Will dispatch a ServerConnection.ON_DATALIST event if successfull otherwise a ServerConnection.ON_ERROR.
		 */
		public function GetDatasets():String 
		{
			var command:String = 'DATA LIST' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_GET_DATASETS } );
			return command;
		}
		
		/////////////////////////////
		// Template Graphics Commands
		/////////////////////////////
		
		/**
		 * Prepares a template for displaying. It won’t show until you call CG PLAY (unless you supply the play-on-load flag, which is simply a ‘1’. ‘0’ for “don’t play on load”). data is either inline xml or a reference to a saved dataset.
		 * @param	channel The channel
		 * @param	layer The layer where the flashLayer is located
		 * @param	flashLayer The flashLayer to add the template at
		 * @param	template the name of the template
		 * @param	playOnLoad Play the template automatically after loaded
		 * @param	data The XML data to pass to the template, see http://casparcg.com/wiki/CasparCG_2.0_AMCP_Protocol#Template_Data
		 */
		public function LoadTemplate(channel:uint, layer:uint, flashLayer:int, template:String, playOnLoad:Boolean = false, data:* = ""):String 
		{
			var templateData:String = data;
			templateData = templateData.replace(/\n|\r|\t/g, "");
			
			var command:String = 'CG ' + channel + '-' + layer + ' ADD ' + flashLayer + ' \"' + template + '\" ' + (playOnLoad ? "1" : "0") + ' \"' + templateData.replace(/\"/g, "\\\"") + '\"' + SOCKET_COMMAND_END;
			trace("COMMAND:", command);
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Removes the visible template from a specific layer.
		 * @param	channel The channel
		 * @param	layer The layer where the flashLayer is located
		 * @param	flashLayer The flashLayer to remove the template from
		 */
		public function RemoveTemplate(channel:uint, layer:uint, flashLayer:int):String 
		{
			var command:String = 'CG ' + channel + '-' + layer + ' REMOVE ' + flashLayer + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Clears all layers and any state that might be stored. What this actually does behind the scene is to create a new instance of the Adobe Flash player ActiveX controller in memory.
		 * @param	channel The channel
		 * @param	layer The layer to clear the templates from
		 */
		public function ClearTemplates(channel:uint, layer:uint):String 
		{
			var command:String = 'CG ' + channel + '-' + layer + ' CLEAR' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Plays and displays the template in the specified layer.
		 * @param	channel The channel
		 * @param	layer The layer where the flashLayer is located
		 * @param	flashLayer The flashLayer to play the template at
		 */
		public function PlayTemplate(channel:uint, layer:uint, flashLayer:int):String 
		{
			var command:String = 'CG ' + channel + '-' + layer + ' PLAY ' + flashLayer + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Stops and removes the template from the specified layer. This is different than REMOVE in that the template gets a chance to animate out when it is stopped.
		 * @param	channel The channel
		 * @param	layer The layer where the flashLayer is located
		 * @param	flashLayer The flashLayer to stop the template at
		 */
		public function StopTemplate(channel:uint, layer:uint, flashLayer:int):String 
		{
			var command:String = 'CG ' + channel + '-' + layer + ' STOP ' + flashLayer + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Triggers a "continue" in the template on the specified layer. This is used to control animations that has multiple discreet steps.
		 * @param	channel The channel
		 * @param	layer The layer where the flashLayer is located
		 * @param	flashLayer The flashLayer to perform the next command on
		 */
		public function Next(channel:uint, layer:uint, flashLayer:int):String 
		{
			var command:String = 'CG ' + channel + '-' + layer + ' NEXT ' + flashLayer + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Jumps to the specified label in the template on the specified layer.
		 * @param	channel The channel
		 * @param	layer The layer where the flashLayer is located
		 * @param	flashLayer The flashLayer to perform the invoke command on
		 * @param	label The label to go to
		 */
		public function GoTo(channel:uint, layer:uint, flashLayer:int, label:String):String 
		{
			var command:String = 'CG ' + channel + '-' + layer + ' GOTO ' + flashLayer + ' \"' + label + '\"' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Sends new data to the template, via SetData(), on specified layer. Data is either inline xml or a reference to a saved dataset. 
		 * @param	channel The channel
		 * @param	layer The layer where the flashLayer is located
		 * @param	flashLayer The flashLayer to update
		 * @param	data XML data or a reference to a saved dataset
		 */
		public function SetData(channel:uint, layer:uint, flashLayer:int, data:*):String 
		{
			var templateData:String = data;
			templateData = templateData.replace(/\n|\r|\t/g, "");
			var command:String = 'CG ' + channel + '-' + layer + ' UPDATE ' + flashLayer + ' \"' + templateData.replace(/\"/g, "\\\"") + '\"' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Calls a custom method in the document class of the template on the specified layer. The method must return void and take no parameters.
		 * @param	channel The channel
		 * @param	layer The layer where the flashLayer is located
		 * @param	flashLayer The flashLayer to perform the invoke command on
		 * @param	method The method to call
		 */
		public function Invoke(channel:uint, layer:uint, flashLayer:int, method:String):String 
		{
			var command:String = 'CG ' + channel + '-' + layer + ' INVOKE ' + flashLayer + ' \"' + method + '\"' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/////////////////
		// Mixer Commands
		/////////////////
		
		/**
		 * If keyer equals 1 then the specified layer will not be rendered, instead it will be used as the key for the layer above.
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	keyer Enable or disable the specified layer as a key for the layer above
		 */
		public function MixerKeyer(channel:uint, layer:uint, keyer:int):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' KEYER ' + keyer + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Use the selected layer as a blender for the underlying layer(s).
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	blend How you want to blend the layer, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Blend_Modes
		 */
		public function MixerBlend(channel:uint, layer:uint, blend:String):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' BLEND ' + blend + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Changes the opacity of the specified layer.
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	opacity The amount of opacity you want on the layer, 0.0 - 1.0
		 * @param	duration The time in frames you want the mixer to take
		 * @param	tween The transition you want, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Animation_Types
		 */
		public function MixerOpacity(channel:uint, layer:uint, opacity:Number, duration:uint = 0, tween:String = "linear"):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' OPACITY ' + opacity + ' ' + duration + ' ' + tween + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Changes the brightness of the specified layer.
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	brightness How bright you want the layer to be, normally 0.0 - 1.0
		 * @param	duration The time in frames you want the mixer to take
		 * @param	tween The transition you want, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Animation_Types
		 */
		public function MixerBrightness(channel:uint, layer:uint, brightness:Number, duration:uint = 0, tween:String = "linear"):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' BRIGHTNESS ' + brightness + ' ' + duration + ' ' + tween + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Changes the saturation of the specified layer.
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	saturation How saturated you want the layer to be, normally 0.0 - 1.0
		 * @param	duration The time in frames you want the mixer to take
		 * @param	tween The transition you want, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Animation_Types
		 */
		public function MixerSaturation(channel:uint, layer:uint, saturation:Number, duration:uint = 0, tween:String = "linear"):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' SATURATION ' + saturation + ' ' + duration + ' ' + tween + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Changes the contrast of the specified layer.
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	contrast How high contrast you want the layer to have, normally 0.0 - 1.0
		 * @param	duration The time in frames you want the mixer to take
		 * @param	tween The transition you want, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Animation_Types
		 */
		public function MixerContrast(channel:uint, layer:uint, contrast:Number, duration:uint = 0, tween:String = "linear"):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' CONTRAST ' + contrast + ' ' + duration + ' ' + tween + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * ...
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	min_input 
		 * @param	max_input 
		 * @param	gamma 
		 * @param	min_output 
		 * @param	max_output 
		 * @param	duration The time in frames you want the mixer to take
		 * @param	tween The transition you want, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Animation_Types
		 * TODO Fix function- and parameter-descriptions.
		 */
		public function MixerLevels(channel:uint, layer:uint, min_input:Number, max_input:Number, gamma:Number, min_output:Number, max_output:Number, duration:uint = 0, tween:String = "linear"):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' LEVELS ' + min_input + ' ' + max_input + ' ' + gamma + ' ' + min_output + ' ' + max_output + ' ' + duration + ' ' + tween + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Scales the video stream on the specified layer.
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	x The left edge of the new fillSize, 0 = left edge of monitor, 0.5 = middle of monitor, 1.0 = right edge of monitor. Higher and lower values allowed.
		 * @param	y The top edge of the new fillSize, 0 = top edge of monitor, 0.5 = middle of monitor, 1.0 = bottom edge of monitor. Higher and lower values allowed.
		 * @param	scaleX The size of the new fillSize, 1 = 1x the screen size, 0.5 = half the screen size. Higher and lower values allowed.
		 * @param	scaleY The size of the new fillSize, 1 = 1x the screen size, 0.5 = half the screen size. Higher and lower values allowed.
		 * @param	duration The time in frames you want the mixer to take
		 * @param	tween The transition you want, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Animation_Types
		 */
		public function MixerFill(channel:uint, layer:uint, x:Number, y:Number, scaleX:Number, scaleY:Number, duration:uint = 0, tween:String = "linear"):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' FILL ' + x + ' ' + y + ' ' + scaleX + ' ' + scaleY + ' ' + duration + ' ' + tween + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Masks or crops the video stream on the specified layer.
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	x The left edge of the new clipSize, 0 = left edge of monitor, 0.5 = middle of monitor, 1.0 = right edge of monitor. Higher and lower values allowed.
		 * @param	y The top edge of the new clipSize,0 = top edge of monitor, 0.5 = middle of monitor, 1.0 = bottom edge of monitor. Higher and lower values allowed.
		 * @param	scaleX The size of the new clipSize, 1 = 1x the screen size, 0.5 = half the screen size. Higher and lower values allowed.
		 * @param	scaleY The size of the new clipSize, 1 = 1x the screen size, 0.5 = half the screen size. Higher and lower values allowed.
		 * @param	duration The time in frames you want the mixer to take
		 * @param	tween The transition you want, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Animation_Types
		 */
		public function MixerClip(channel:uint, layer:uint, x:Number, y:Number, scaleX:Number, scaleY:Number, duration:uint = 0, tween:String = "linear"):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' CLIP ' + x + ' ' + y + ' ' + scaleX + ' ' + scaleY + ' ' + duration + ' ' + tween + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Creates a grid of video streams in ascending order of the layer index, i.e. if resolution equals 2 then a 2x2 grid of layers will be created. Layer 0 will never be included in the grid.
		 * @param	channel The channel
		 * @param	resolution How large you want the grid to be, 2 = 2x2, 3 = 3x3
		 * @param	duration The time in frames you want the mixer to take.
		 * @param	tween The transition you want, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Animation_Types
		 */
		public function MixerGrid(channel:uint, resolution:uint, duration:uint = 0, tween:String = "linear"):String 
		{
			var command:String = 'MIXER ' + channel + ' GRID ' + resolution + ' ' + duration + ' ' + tween + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Changes the volume of the specified layer.
		 * @param	channel The channel
		 * @param	layer The layer you want to affect
		 * @param	volume The volume you want on the layer, 0 = silence, 1 = regular volume. Higher values allowed.
		 * @param	duration The time in frames you want the mixer to take
		 * @param	tween The transition you want, see http://casparcg.com/wiki/CasparCG_Server_2.0b#Animation_Types
		 */
		public function MixerVolume(channel:uint, layer:uint, volume:Number, duration:uint = 0, tween:String = "linear"):String 
		{
			var command:String = 'MIXER ' + channel + '-' + layer + ' VOLUME ' + volume + ' ' + duration + ' ' + tween + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/**
		 * Reset all transformations.
		 * @param	channel The channel you want to reset
		 * @param	layer The specfic layer you want to reset
		 */
		public function MixerClear(channel:uint, layer:int = -1):String 
		{
			var command:String = 'MIXER ' + channel + (layer != -1 ? "-" + layer : "") + ' CLEAR' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			return command;
		}
		
		/////////////////
		// Query Commands
		/////////////////
		
		/**
		 * Returns information about a mediafile
		 * @param	filename The name of the file
		 */
		public function GetMediaFileInfo(filename:String):String 
		{
			var command:String = 'CINF \"' + filename +'\"' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_MEDIAFILE_INFO } );
			return command;
		}
		
		/**
		 * Lists all mediafiles
		 */
		public function GetMediaFiles():String 
		{
			var command:String = 'CLS' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_GET_MEDIAFILES } );
			return command;
		}
		
		/**
		 * Lists all templates. Lists only templates in the specified folder, if provided.
		 * @param	folder The folder to list (default: "")
		 */
		public function GetTemplates(folder:String = ""):String 
		{
			var command:String;
			if (folder == "")
			{
				command = 'TLS' + SOCKET_COMMAND_END;
			}
			else
			{
				command = 'TLS \"' + folder + '\"' + SOCKET_COMMAND_END;
			}
			
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_GET_TEMPLATES } );
			return command;
		}
		
		/**
		 * Returns the version of the server.
		 */
		public function GetVersion():String 
		{
			var command:String = 'VERSION' + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_VERSION } );
			return command;
		}
		
		/**
		 * Returns information about the channels and/or videolayers on the server. Use this without parameters to check how many channels a server has.
		 * @param	channel The channel (default: -1)
		 * @param	layer The layer (default: -1)
		 */
		public function GetChannelInfo(channel:int = -1, layer:int = -1):String 
		{
			var command:String = 'INFO' + (channel != -1 ? " " + channel : "") + ((channel != -1 && layer != -1) ? "-" + layer : "") + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_INFO } );
			return command;
		}
		
		/**
		 * Returns information about the server.
		 * @param	type The specific info you want (CONFIG|PATHS|SYSTEM|SERVER|TEMPLATE)
		 * @param	template The name of the template, only used if type = TEMPLATE)
		 */
		public function GetServerInfo(type:String, template:String = null):String 
		{
			var command:String = 'INFO ' + type + ((type.toUpperCase() == "TEMPLATE" && template != null) ? " " + template : "") + SOCKET_COMMAND_END;
			
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_INFO } );
			return command;
		}
		
		/**
		 * Returns status for the specified layer.
		 */
		public function GetStatus(channel:int, layer:int):String 
		{
			var command:String = 'STATUS ' + channel + '-' + layer + SOCKET_COMMAND_END;
			_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_VERSION } );
			return command;
		}
		
		////////////////
		// Misc Commands
		////////////////
		
		/**
		 * Disconnects from the server.
		 */
		//public function Disconnect():void 
		//{
			//var command:String = 'BYE' + SOCKET_COMMAND_END;
			//_socket.addCommand( { command: command, type: ServerConnectionEvent.ON_OTHER_COMMAND } );
			//_userForcedDisconnection = true;
		//}
		
		private function configureListeners():void 
		{
			_socket.addEventListener(ServerConnectionEvent.ON_CONNECT, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_DISCONNECT, onSocketClosed);
			_socket.addEventListener(ServerConnectionEvent.ON_SEND_COMMAND, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_MEDIAFILE_INFO, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_GET_MEDIAFILES, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_GET_DATASETS, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_GET_DATA, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_INFO, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_OTHER_COMMAND, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_SUCCESS, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_GET_TEMPLATES, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_VERSION, dispatchAMCPEvent);
			_socket.addEventListener(ServerConnectionEvent.ON_IO_ERROR, ioErrorHandler);
			_socket.addEventListener(ServerConnectionEvent.ON_SECURITY_ERROR, securityErrorHandler);
			_socket.addEventListener(ServerConnectionEvent.ON_LOG, dispatchAMCPEvent);
			
			_timer.addEventListener(TimerEvent.TIMER, onTimerReconnect);
		}
		
		private function dispatchAMCPEvent(e:ServerConnectionEvent):void
		{
			dispatchEvent(e);
		}
		
		private function unregisterListeners():void 
		{
			_socket.removeEventListener(ServerConnectionEvent.ON_CONNECT, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_DISCONNECT, onSocketClosed);
			_socket.removeEventListener(ServerConnectionEvent.ON_SEND_COMMAND, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_MEDIAFILE_INFO, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_GET_MEDIAFILES, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_GET_DATASETS, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_GET_DATA, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_INFO, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_OTHER_COMMAND, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_SUCCESS, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_GET_TEMPLATES, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_VERSION, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_LOG, dispatchAMCPEvent);
			_socket.removeEventListener(ServerConnectionEvent.ON_IO_ERROR, ioErrorHandler);
			_socket.removeEventListener(ServerConnectionEvent.ON_SECURITY_ERROR, securityErrorHandler);
			_timer.removeEventListener(TimerEvent.TIMER, onTimerReconnect);
		}
		
		/**
		 * Is called when the socket is closed. If the closedown wasn't forced by the user, it tries to reconnect.
		 * @param	e ServerConnectionEvent
		 */
		private function onSocketClosed(e:ServerConnectionEvent):void
		{
			var host:String = _socket.host;
			var port:uint = _socket.port;
			
			trace("ServerConnection::not connected, try reconnect? " , !_userForcedDisconnection);
			if (_autoReconnect && !_userForcedDisconnection)
			{
				reconnect();
			}
			
			dispatchEvent(e);
		}
		
		/**
		 * Handle io errors from the socket and dispatch the event. 
		 * @param	e ServerConnection
		 */
		private function ioErrorHandler(e:ServerConnectionEvent):void
		{
			switch (e.command)
			{
				case "SocketCommandFailedNoConnection":
					
					break;
				default:
					if (!_socket.connected)
					{
						trace("ServerConnection::not connected, try reconnect? " , !_userForcedDisconnection);
						if (_autoReconnect && !_userForcedDisconnection)
						{
							reconnect();
						}
					}
					break;
			}
			
			dispatchEvent(e);
		}
		
		
		/**
		 * Handle security errors from the socket and dispatch the event
		 * @param	e ServerConnection
		 */
		private function securityErrorHandler(e:ServerConnectionEvent):void
		{
			trace("ServerConnection::Security error");
			dispatchEvent(e);
		}
		
		/**
		 * Will try reconnect till a connection is established
		 */
		private function reconnect():void 
		{
			_timer.reset();
			_timer.start();
		}
		
		/**
		 * Is Called on timerEvent.TIMER to check whether there is a socket connection or not. If not try to reconnect.
		 * @param	e TimerEvent
		 */
		private function onTimerReconnect(e:TimerEvent):void 
		{
			if (_socket.connected)
			{
				_timer.stop();
				_timer.reset();
			}
			else
			{
				if (_userForcedDisconnection)
				{
					_timer.stop();
					_timer.reset();
					dispatchAMCPEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_DISCONNECT));
				}
				else
				{
					doNewConnection(_socket.host, _socket.port);
				}
			}
		}
		
		/**
		 * Connects to a caspar server
		 * @param	server The server name
		 * @param	port The port number (default 5250)
		 */
		private function doNewConnection(host:String, port:uint):void
		{
			_socket.connect(host, port);
			_userForcedDisconnection = false;
		}
    }
}

import caspar.network.ServerConnectionEvent;
import caspar.network.data.*;

import flash.errors.*;
import flash.events.*;
import flash.net.Socket;
import flash.utils.Timer;

class CustomSocket extends Socket
{
	public var currentDataInfoItem:DataItemInfo;
	
    private var _response:String;
	private var _commandQueue:Array;
	private var _host:String;
	private var _port:uint;
	private var totalSize:int = 0;
	
    public function CustomSocket()
	{
        super();
        configureListeners();
    }
	
	override public function connect(host:String, port:int):void 
	{
		_host = host;
		_port = port;
		_commandQueue = [];
		
		super.connect(host, port);
	}
	
	override public function close():void 
	{
		super.close();
		dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_DISCONNECT, false, false, "SocketDisconnect", "Disconnected from " + _host + " at port " + _port));
	}
	
	public function addCommand(command:Object):void
	{
		if (this.connected)
		{
			_commandQueue.push(command);
			if (_commandQueue.length == 1)
			{
				nextCommand();
			}
		}
		else
		{
			trace("ServerConnection::No socket connection, use ServerConnection.connect to connect to a socket.");
			dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_IO_ERROR, false, false, "SocketCommandFailedNoConnection", "Tries to execute command before connected to any host"));
		}
	}
	
	private function nextCommand():void 
	{
		if (_commandQueue.length > 0)
		{
			dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_SEND_COMMAND, false, false, "", _commandQueue[0].command));
			sendRequest(_commandQueue[0].command);
		}
	}

	private function commandFinished():void
	{
		_commandQueue.splice(0, 1);
		nextCommand();
	}
	
	private function configureListeners():void
	{
        addEventListener(Event.CLOSE, closeHandler);
        addEventListener(Event.CONNECT, connectHandler);
        addEventListener(IOErrorEvent.IO_ERROR, ioErrorHandler);
        addEventListener(SecurityErrorEvent.SECURITY_ERROR, securityErrorHandler);
        addEventListener(ProgressEvent.SOCKET_DATA, socketDataHandler);
    }
	
	private function unregisterListeners():void
	{
        removeEventListener(Event.CLOSE, closeHandler);
        removeEventListener(Event.CONNECT, connectHandler);
        removeEventListener(IOErrorEvent.IO_ERROR, ioErrorHandler);
        removeEventListener(SecurityErrorEvent.SECURITY_ERROR, securityErrorHandler);
        removeEventListener(ProgressEvent.SOCKET_DATA, socketDataHandler);
    }
	
    private function writeln(str:String):void
	{
        //str += "\n";
		try {
            writeUTFBytes(str);
        }
        catch(e:IOError) {
            trace("ServerConnection::"+e);
        }
    }
	
    private function sendRequest(request:String):void
	{
        _response = "";
        writeln(request);
        flush();
		writeln("\r\n");
		flush();
    }
	
    private function readResponse():void
	{
		//totalSize += this.bytesAvailable;
		//trace(this.readInt(), this.bytesAvailable, totalSize);
        var str:String = this.readUTFBytes(bytesAvailable);
		//super.
        _response += str;
		//Är det ifall vi skickar två kommando i rad?
		//BUG: We cannot use this check for determining the end of packets, need to find a new one
		if(_commandQueue[0].type == ServerConnectionEvent.ON_GET_TEMPLATES || _commandQueue[0].type == ServerConnectionEvent.ON_GET_MEDIAFILES)
		{
			if (_response.charAt(_response.length - 1) == "\n" && _response.charAt(_response.length - 2) == "\r" && _response.charAt(_response.length - 3) == "\n" && _response.charAt(_response.length - 4) == "\r")
			{
				dispatchEvents(_response, _commandQueue[0].command, _commandQueue[0].type);
				dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_LOG, false, false, "", _response));
				commandFinished();
			}
		}
		else
		{
			if (_response.charAt(_response.length - 1) == "\n" && _response.charAt(_response.length - 2) == "\r")
			{
				dispatchEvents(_response, _commandQueue[0].command, _commandQueue[0].type);
				dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_LOG, false, false, "", _response));
				commandFinished();
			}
		}
    }
	
	private function dispatchEvents(response:String, command:String, type:String):void 
	{
		var responseArray:Array = response.split("\n");
		var responseCode:String = (responseArray[0].split(" "))[0];
		var responseMessage:String = responseArray[0];
		var data:*;
		var itemList:IItemList;
		var success:Boolean = true; 
		
		//DATARETRIEVE BUG ON SUCCESS, CASPAR DOES NOT RETURN ANY RESPONSE CODE
		if (responseArray[0].charAt(0) == "<" && responseCode.charAt(0) == "<")
		{
			responseCode = "201";
			responseMessage = "201 DATA RETRIEVE OK";
		}
		
		if (responseCode.charAt(0) == "2")
		{
			dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_SUCCESS, false, false, command, responseMessage));
			var i:int = 0;
			var size:String;
			var date:String;
			var rawData:Array;
			
			switch(type)
			{
				case ServerConnectionEvent.ON_MEDIAFILE_INFO:
					data = String(responseArray[1]);
					break;
				case ServerConnectionEvent.ON_GET_MEDIAFILES:
					var mediaList:Array = new Array();
					for (i = 1; i < responseArray.length - 2; i++)
					{
						rawData = responseArray[i].split("\"");
						var media:CasparItemInfo = new CasparItemInfo();
						var mediaLocation:String = rawData[1];
						
						var subtype:String = rawData[2].split(" ")[2];
						size = rawData[2].split(" ")[4];
						date = rawData[2].split(" ")[5];
						
						var mediaPath:String = responseArray[i];
						
						mediaPath = mediaLocation.replace(/\r/g, "");
						mediaPath = mediaPath.replace(/\"/g, "");
						mediaPath = mediaPath.replace(/\\/g, "/");
						
						media.path = mediaPath;
						
						if (mediaPath.search("/") == -1)
						{
							media.folder = "";
							media.name = mediaPath; 
						}
						else
						{
							media.folder = mediaPath.split("/")[0];
							media.name = mediaPath.slice(mediaPath.indexOf("/")+1);
						}
						media.date = date;
						media.size = size;
						media.subtype = subtype;
						media.type = CasparItemInfo.TYPE_MEDIA;						
						mediaList.push(media);
						
					}
					itemList = new CasparItemInfoCollection(mediaList);
					break;
				case ServerConnectionEvent.ON_GET_DATASETS:
					var items:Array = [];
					var item:DataItemInfo;
					
					var folder:String = "";
					var name:String;
					
					for (i = 1; i < responseArray.length - 2; i++)
					{
						var rawdata:String = String(responseArray[i]).replace("\r", "");
						name = rawdata;
						
						/*if (rawdata.search(/\\/g) != -1)
						{
							//resides inside a folder
							folder = rawdata.split("\\")[0];
							name = rawdata.split("\\")[1];
						}*/
						
						//rawdata
						
						item = new DataItemInfo(rawdata);
						items.push(item);
					}
					
					itemList = new DataItemInfoCollection(items);
					break;
				case ServerConnectionEvent.ON_GET_DATA:
					try
					{
						data = new DataItem(currentDataInfoItem, new XML(responseArray[0]));
					}
					catch(e:Error)
					{
						trace(responseArray[0] +" cannot be converted to xml");
					}
					break;
				case ServerConnectionEvent.ON_INFO:
					data = new Array();
					for (i = 1; i < responseArray.length - 2; i++)
					{
						data.push(responseArray[i]);
					}
					break;
				case ServerConnectionEvent.ON_GET_TEMPLATES:
					var templateList:Array = new Array();
					for (i = 1; i < responseArray.length - 2; i++)
					{
						rawData = responseArray[i].split("\"");
						var template:CasparItemInfo = new CasparItemInfo();
						var templateLocation:String = rawData[1];
						size = rawData[2].split(" ")[1];
						date = rawData[2].split(" ")[2];
						
						var templatePath:String = responseArray[i];
						
						templatePath = templateLocation.replace(/\r/g, "");
						templatePath = templatePath.replace(/\"/g, "");
						templatePath = templatePath.replace(/\\/g, "/");
						
						template.path = templatePath;
						
						if (templatePath.search("/") == -1)
						{
							template.folder = "";
							template.name = templatePath; 
						}
						else
						{
							template.folder = templatePath.split("/")[0];
							template.name = templatePath.slice(templatePath.indexOf("/")+1);
						}
						template.date = date;
						template.size = size;
						template.type = CasparItemInfo.TYPE_TEMPLATE;						
						templateList.push(template);
					}
					itemList = new CasparItemInfoCollection(templateList);
					break;
				default:
					dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_OTHER_COMMAND, false, false, command, responseMessage));
			}
		}
		else
		{
			dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_ERROR, false, false, command, responseMessage));
			success = false;
		}
	
		if(success)
		{
			var e:ServerConnectionEvent = new ServerConnectionEvent(type, false, false, command, responseMessage, data, itemList);
			dispatchEvent(e);
		}
	}
	
    private function closeHandler(event:Event):void
	{
		trace("ServerConnection::EVT: SOCKET CLOSE");
		close();
    }
	
    private function connectHandler(event:Event):void
	{
		trace("ServerConnection::EVT: SOCKET CONNECT");
		dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_CONNECT, false, false, "SocketConnect", "Connected to " + _host + " at port " + _port));
		
		//probably not needed, screws up the command flow.
		//if (_commandQueue.length > 0)
		//{
			//trace(ServerConnectionEvent::this.connected);
			//nextCommand();
		//}
    }
	
    private function ioErrorHandler(event:IOErrorEvent):void
	{
		trace("ServerConnection::EVT: SOCKET IO ERROR");
		dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_IO_ERROR, false, false, "", event.text));
		commandFinished();
    }
	
    private function securityErrorHandler(event:SecurityErrorEvent):void
	{
		trace("ServerConnection::EVT: SOCKET SECURITY ERROR");
		dispatchEvent(new ServerConnectionEvent(ServerConnectionEvent.ON_SECURITY_ERROR, false, false, "", event.text));
		commandFinished();
    }
	
    private function socketDataHandler(event:ProgressEvent):void
	{
        trace("ServerConnection::EVT: SOCKET RECIVE DATA");
		this.readResponse();
    }
	
	public function get host():String { return _host; }
	
	public function get port():uint { return _port; }
}