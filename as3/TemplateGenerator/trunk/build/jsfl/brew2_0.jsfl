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

//TODO: Go into the document class and check for illegal override of Stop and warn
//TODO: Make brew undo instead of revert to be able to keep the undo history for the .fla (not possible?)
//TODO: Refactor code, rename variables with type notation

//CONSTANTS
const SOMETHING_WENT_WRONG_PLEASE_CLOSE_AND_REOPEN_THE_FILE_AND_TRY_AGAIN = "";
const VERSION 						= "2.0.0";
const EMBED_RANGES 					= "1|2|3|4|5|17|18";
//const CLASS_PATH 					= "se.svt.caspar.template.CasparTemplate";
const CLASS_PATH 					= "caspar.template.TemplateMain";
const DESCRIPTION_XML_VAR_NAME		= "description";
const ORIGINAL_WIDTH_VAR_NAME 		= "originalWidth";
const ORIGINAL_HEIGHT_VAR_NAME 		= "originalHeight";
const ORIGINAL_FRAME_RATE_VAR_NAME 	= "originalFrameRate";
const STOP_ON_FIRST_FRAME_VAR_NAME 	= "stopOnFirstFrame";

//VARIABLES

var output = fl.outputPanel;

//true if brew finds an unnamed dynamic textfield
var bWarningFoundUnnamedCasparComponent = false;

var bWarningFoundUnnamedTextField = false;
// true if brew finds an unnamed movieclip instance that contains dynamic textfields
var bWarningFoundUnnamedMovieClip= false;
//true if a component has invalid description data
var bWarningFoundInvalidComponent = false;
//true if there is two or more instances with the same name but of different types
var bWarningFoundDuplicateInstanceNames = false;
//true if author name is missing
var bWarningFoundMissingAuthorName = false;
//true if there is an unnamed video and optimizeVideo is true 
var bWarningFoundUnnamedVideo = false;

//Reference to the current document
var thisDocument = fl.getDocumentDOM();

//The path to the current .fla
var filePath = thisDocument.path; 
//defenition of custom variables found in the document class
var customParameterDescription = "";

var allTimeLines = thisDocument.timelines;

var nFoundOutro = -1;

var nDisposeAt = -1;

var foundCasparTextField = false;

var xmlOutput = "";

var xmlComponentDefinitions = "<components>";

var xmlKeyframes = "<keyframes>";

var ComponentInstances = [];

var xmlComponentInstances = "<instances>";

var mcInstCount = 0;

var CasparComponentTypes = [];

var CasparTextFieldDescription = "<component name=\"CasparTextField\"><property name=\"text\" type=\"string\" info=\"String data\" /></component>";

var thickness;

var sharpness;

var bSaveOk = false;

var bShowPreview;

var stopOnFirstFrame = false;

var bFoundStopAfterOutro = false;

var authorName = "";
var authorEmail = "";
var templateInfo = "";

var optimizeImages = false;
var optimizeTextFields = false;
var optimizeVideos = false;

var copyToPath = "";

var fileName = "";

var remoteCopySuccess = false;

var verbose = true;

// FUNCTIONS

function init()
{
	/*this does not work
	var storedID = getStoredEventID();
	//fl.removeEventListener("documentChanged", docummentChangedEventID);
	if(storedID != "")
	{
		fl.removeEventListener("documentChanged", storedID);
	}
		
	var documentChangedEventID = fl.addEventListener("documentChanged", onDocumentChanged);
	
	setStoredEventID(documentChangedEventID);

	//fl.removeEventListener("documentChanged", onDocumentChanged);
	//fl.addEventListener("documentChanged", onDocumentChanged);
	*/
}

//This functionality is moved to MMExecute, it seems like Flash leaks memory when calling runScript
//function getDocumentName()
//{
	//return "GAMLLE";
	//return thisDocument.name;
//}

function onDocumentChanged()
{
	readMetadata();
	//fl.swfPanels[2].call("alive");
}

//function deactivate() {
	//fl.trace( "Tool is no longer active" );
//}

function generate(tn, sn, showPreview, aname, aemail, tinfo, optImages, optTextFields, optVideos, copyPath, verboseOutput) 
{
	output.clear();
	
	setClassPath();
	setLibraryPath();
	
	if(aname != "" && aname != undefined && aname != null)
	{
		authorName = aname;
	}
	else
	{
		bWarningFoundMissingAuthorName = true;
	}
	
	if(aemail != "" && aemail != undefined && aemail != null)
	{
		authorEmail = aemail;
	}
	
	if(tinfo != "" && tinfo != undefined && tinfo != null)
	{
		templateInfo = tinfo;
	}
	
	if(optImages == "true")
	{
		optimizeImages = 1
	}
	else
	{
		optimizeImages = 0;
	}
	
	if(optTextFields == "true")
	{
		optimizeTextFields = 1
	}
	else
	{
		optimizeTextFields = 0;
	}
	
	if(optVideos == "true")
	{
		optimizeVideos = 1
	}
	else
	{
		optimizeVideos = 0;
	}
	
	verbose = verboseOutput;
	
	copyToPath =  copyPath;
	
	writeCacheFile();

	output.trace("||Generating CasparCG Flash template, version " + VERSION + "||");
	verboseTrace("-----------------------------------------------------");
	
	verboseTrace("Verbose tracing is activated (+), this can be turned off in the Settings tab.");
	
	bShowPreview = showPreview;
	
	if(filePath == undefined) 
	{
		var saveSucess = fl.saveDocumentAs(thisDocument);

		if(saveSucess)
		{
			filePath = thisDocument.path; 
			bSaveOk = true;
		} 
		else
		{
			bSaveOk = false;
		}
	} 
	else 
	{
		var saveSucess = thisDocument.save();
		bSaveOk = true;
	}
	
	if(bSaveOk) 
	{
		thickness = tn;
		sharpness = sn;
		
		//Move to _root
		thisDocument.currentTimeline = 0;
		//Save the document
		thisDocument.save();
		//Clear the compile errors
		fl.compilerErrors.clear()
	
		//Hack to make the document changed to be able to revert on errors
		
		//Set the document class
		if(thisDocument.docClass == null || thisDocument.docClass == undefined || thisDocument.docClass == "")
		{
			thisDocument.docClass = CLASS_PATH;
		}
		else
		{
			readDocumentClass(thisDocument.docClass);
		}
		
		sequence();
		var returnobj = [fileName+"<#>"+remoteCopySuccess];
		return returnobj;
	}
}

function setClassPath()
{
	var bIsFound = false;
	var classPaths = fl.as3PackagePaths.split(';');

	for (var i = 0; i < classPaths.length; i++)
	{
		if(classPaths[i] == '$(LocalData)/Classes/casparCG'){
			bIsFound = true;
		}
	}
	
	//if old version of caspar, remove class path
	for (var j = 0; j < classPaths.length; j++)
	{
		//output.trace("loopar classoaths" + classPaths[j]);
		if (classPaths[j] == '$(LocalData)/Classes/caspar') {
			//output.trace("hittar" + classPaths.length);
			classPaths.splice(j, 1);
			//output.trace("efter" + classPaths.length);
			fl.as3PackagePaths = classPaths.join(';');
		}
	}
	
	if (!bIsFound)
	{
		classPaths.push('$(LocalData)/Classes/casparCG');
		fl.as3PackagePaths = classPaths.join(';');
	}

}

function setLibraryPath()
{
	//fl.trace("LIBRARY PATHS= < " + fl.libraryPath + " >"); 
	
	var bIsFound = false;
	var libraryPaths = fl.libraryPath.split(';');
	
	for (var i = 0; i < libraryPaths.length; i++)
	{
		if(libraryPaths[i] == '$(LocalData)/Libraries/casparCG'){
			bIsFound = true;
		}
	}
	
	if (!bIsFound)
	{
		//libraryPaths.push('$(LocalData)/Libraries');
		fl.libraryPath = '$(LocalData)/Libraries/casparCG;' + fl.libraryPath;
	}
	
	//fl.trace("LIBRARY PATHS= < " + fl.libraryPath + " >"); 
}



//
//function loadServers()
//{
	//fl.trace("laddar");
	//var serverFile = fl.configURI + "/WindowSWF/FTGeneratorServers.settings";
	//return FLfile.read(serverFile);
//}
//
function saveServers(servers)
{
	var configFile = fl.configURI + "/WindowSWF/TemplateGeneratorServers.settings";
	FLfile.write(configFile, servers);
}

function writeCacheFile()
{
	var configFile = fl.configURI + "/WindowSWF/TemplateGenerator.settings";
	//FLfile.write(configFile, authorName + "#" + authorEmail + verbose);
	//fl.trace("skriver fil user" + FLfile.write(configFile, "<Settings author=\"" + authorName + "\" email=\"" + authorEmail + "\" verbose = \"" + verbose + "\" />"));
	FLfile.write(configFile, "<Settings author=\"" + authorName + "\" email=\"" + authorEmail + "\" verbose = \"" + verbose + "\" />");
}

function checkServer(serverName)
{
	
	var serverURI = "file://" + serverName + "/Caspar";
	if (FLfile.exists(serverURI)) 
	{
		return "true";
	}
	else 
	{
		return "false";
	}
}

function getFolders(serverName)
{
	
	var serverURI = "file://" + serverName + "/Caspar/";
	var folderArray = FLfile.listFolder(serverURI, "directories");
	var returnString;
	for (var i = 0; i < folderArray.length; i++)
	{
		if (returnString != undefined)
		{
			if (folderArray[i] != "_TEMPLATEMEDIA" && folderArray[i] != "_MEDIA" && folderArray[i] != "_CASPARLOG" && folderArray[i] != "_DATA")
			{
				returnString = returnString + "@@" + folderArray[i];
			}
		}
		else
		{
			if (folderArray[i] != "_TEMPLATEMEDIA" && folderArray[i] != "_MEDIA" && folderArray[i] != "_CASPARLOG" && folderArray[i] != "_DATA")
			{
				returnString = folderArray[i];
			}
		}
	}
	return returnString;
}

function trimString(s)
{
	//remove spaces except for in the last string
		var inString = false;
		for(var ichar=0; ichar < s.length; ichar++)
		{
			if(s.charAt(ichar) == '"')
			{
				if(inString == true) 
				{
					inString = false
				}
				else
				{
					inString = true;
				}
			}
			if(s.charAt(ichar) == ' ' && inString == false)
			{
				
				s = s.substring(0, ichar) + '¤' + s.substring(ichar+1, s.length);
			}
		}
		
		s = s.replace(/¤/g, "");
		return s;
}


//check the document class
function readDocumentClass(docClass)
{
	var sParameterTrace = "";
	var docClassPath = docClass.replace(/\./g, "/");
	docClassPath = docClassPath + ".as";
	var path = thisDocument.path.replace(thisDocument.name, "");
	
	path = path + docClassPath;
	path = path.replace(/\\/g, "/");
	path = path.replace(/:/, "|");
	path = "file:///" + path;
	
	var endOfFile = false;
	var documentClass = FLfile.read(path);
	var documentClassArray = documentClass.split("\n");
	
	for(var i = 0; i < documentClassArray.length; i++)
	{
		var row = documentClassArray[i];
		//row = row.replace(/ /g, "");
		row = row.replace(/\t/g, "");
		row = row.replace(/\r/g, "");
		row = row.replace(/\n/g, "");
		//row = row.replace(/"\)]/g, "");
		row = row.split(/"\)]/)[0];
		var sString = "%5BParameter%28";
		
		row = trimString(row);
	
		if(escape(row).search(sString)!= -1)
		{
			sParameterTrace += "Added parameter: ";
			
			var currentName = "";
			               
			var currentType = "";
			               
			var currentInfo = "";
			
			var splittedParameters = row.split('",');
			
			for (var j = 0; j < splittedParameters.length; j++)
			{
				if (splittedParameters[j].search("name=") != -1)
				{
					currentName = splittedParameters[j].split('="')[1];
					sParameterTrace += ('name="' + currentName + '" ');
				}
				if (splittedParameters[j].search("type=") != -1)
				{
					currentType = splittedParameters[j].split('="')[1];
					sParameterTrace += ('type="' + currentType + '" ');
				}
				if (splittedParameters[j].search("info=") != -1)
				{
					currentInfo = splittedParameters[j].split('="')[1];
					if(currentInfo.charAt(currentInfo.length-1) == " ") 
					{
						currentInfo = escape(currentInfo.substring(0, currentInfo.length-1));
					}
					
					if (currentInfo.search("<") != -1 || currentInfo.search(">") != -1)
					{
						currentInfo = "";
						verboseTrace("Parameter info cannot contain < or > at line " +(i+1) + " in " +docClass);
					}
					else
					{
						sParameterTrace += ('info="' + currentInfo + '"');
					}
				}
			}
			
			if (currentInfo != "" && currentType != "" && currentName != "")
			{
				var currentParameter = '<parameter name="'+currentName+'" type="'+currentType+'" info="' + currentInfo+'" />';
				
				customParameterDescription += currentParameter;
				
				if (sParameterTrace != "")
				{
					verboseTrace(sParameterTrace);
					sParameterTrace = "";
				}
			}
			else
			{
				verboseTrace("Found an invalid/incomplete parameter at line " +(i+1) + " in " +docClass+"\n  -You must specify name, type and info");
				sParameterTrace = "";
			}
		}

	}
	
	customParameterDescription = "<parameters>"+customParameterDescription+"</parameters>";
}

//Find the frame to dispose at
function setDisposeFrame(timeline)
{
	
	if(nFoundOutro != -1)
	{
		var allLayers = timeline.layers;
		
		for (var layerNr = 0; layerNr < allLayers.length; layerNr++)
		{

			// get all frames in layer
			var allFrames = allLayers[layerNr].frames;
			// cycle through frames ...
			var frameNr = 0;
			while (frameNr < allFrames.length) 
			{
				//Reference to the current frame
				var thisFrame = thisDocument.getTimeline().layers[layerNr].frames[frameNr];

				if(frameNr > nFoundOutro)
				{
					if(bFoundStopAfterOutro)
					{
						if(frameNr < nDisposeAt)
						{
							var actionscript = thisFrame.actionScript;

							if(actionscript.search("stop()") != -1)
							{
								nDisposeAt = frameNr;
								bFoundStopAfterOutro = true;
							}
						}
					}
					else
					{
						var actionscript = thisFrame.actionScript;

						if(actionscript.search("stop()") != -1)
						{
							nDisposeAt = frameNr;
							bFoundStopAfterOutro = true;
						}
					}
				}
				
				frameNr += allFrames[frameNr].duration;
			}
		}
	}
}

//Search through the main timeline for named keyframes
function scanRoot(timeline) 
{
	verboseTrace("Scanning the main timeline for keyframes with labels...");
	var allLayers = timeline.layers;
	
	for (var layerNr = 0; layerNr < allLayers.length; layerNr++)
	{

		// get all frames in layer
		var allFrames = allLayers[layerNr].frames;
		// cycle through frames ...
		var frameNr = 0;
		
		while (frameNr < allFrames.length) 
		{
			//Reference to the current frame
			var thisFrame = thisDocument.getTimeline().layers[layerNr].frames[frameNr];
			//Check if the current frame has a label
			if(thisFrame.name != "") 
			{
				if(thisFrame.name == "outro") 
				{
					verboseTrace(" >Found an outro label on frame nr. " + frameNr+1);
					//nFoundOutro = getLastFrame(timeline, thisFrame.);
					nDisposeAt = allLayers[layerNr].frameCount -1;
					nFoundOutro = frameNr;	
				}
				else 
				{
					verboseTrace(" >Found the label \""+thisFrame.name+"\" on frame nr. "+frameNr+1);
					xmlKeyframes += "<keyframe name='" + thisFrame.name + "' />";
				}
			}
						
			scanElements(thisFrame, frameNr, allLayers, layerNr);

			// get next frame with new content
			frameNr += allFrames[frameNr].duration;
		}

	}
	
	//End the keyframe xml node
	xmlKeyframes += "</keyframes>";
	
}

function scanTimeline(timeline)
{
	verboseTrace("Scanning a timeline for caspar components...");
	var allLayers = timeline.layers;
	
	for (var layerNr = 0; layerNr < allLayers.length; layerNr++)
	{
		// get all frames in layer
		var allFrames = allLayers[layerNr].frames;
		// cycle through frames ...
		var frameNr = 0;
		while (frameNr < allFrames.length) 
		{
			//Reference to the current frame
			var thisFrame = timeline.layers[layerNr].frames[frameNr];

			scanElements(thisFrame, frameNr, allLayers, layerNr);

			// get next frame with new content
			frameNr += allFrames[frameNr].duration;
		}
	}
}

function addInstance(instanceName, instanceType)
{
	
	var addComponentInstance = true;
	for (var i = 0;  i < ComponentInstances.length; i++)
	{
		if (ComponentInstances[i].name == instanceName)
		{
			addComponentInstance = false;
			if (ComponentInstances[i].type != instanceType)
			{
				bWarningFoundDuplicateInstanceNames = true;
			}
		}
	}
	
	if (addComponentInstance)
	{
		var o = new Object();
		o.name = instanceName;
		o.type = instanceType;
		ComponentInstances.push(o);
		verboseTrace(" >Adding a caspar component instance " + instanceName + ":" + instanceType);
	}
	
	return addComponentInstance;	
}

function scanElements(thisFrame, frameNr, allLayers, layerNr)
{
	verboseTrace("Scanning elements...");
	
	var addImports = true;
	// get all elements in frame
	var allElements = thisFrame.elements;
	
	//Loop trough the elements of the current frame
	for (var elementNr = 0; elementNr < allElements.length; elementNr++)
	{
		
		if (optimizeTextFields == 1)
		{
			if (allElements[elementNr].elementType == "text")
			{
				allElements[elementNr].x = Math.round(allElements[elementNr].x);
				allElements[elementNr].y = Math.round(allElements[elementNr].y);
			}
		}
		
		if (optimizeVideos == 1)
		{
			if (allElements[elementNr].instanceType == "embedded video")
			{
				if (allElements[elementNr].name == "")
				{
					output.trace("***Info***\nFound an unnamed embedded video instance at frame nr. " + frameNr + " on layer \"" + allLayers[layerNr].name + "\".\n");
					bWarningFoundUnnamedVideo = true;
				}
				else
				{
					thisFrame.actionScript += "\nimport flash.media.*; \n\n" + allElements[elementNr].name + ".smoothing = true; \n" + allElements[elementNr].name + ".deblocking = 4;";
				}
			}
		}
		
		if(allElements[elementNr].elementType == "text" && allElements[elementNr].textType == "dynamic" && (allElements[elementNr].name.length == 0 || allElements[elementNr].name[0] != 'x')) 
		{
			if(allElements[elementNr].name == "")
			{
				output.trace("***Info***\nFound an unnamed dynamic text field at frame nr. " + frameNr + " on layer \"" + allLayers[layerNr].name + "\".\n");
				bWarningFoundUnnamedTextField = true;
			} 
			else 
			{
				var addComponentDescription = true;
				
				if(addImports)
				{
					thisFrame.actionScript += "import se.svt.caspar.template.components.CasparTextField;\n";
					thisFrame.actionScript += "import se.svt.caspar.template.CasparTemplate;\n\n";
					addImports = false
				}
				//allElements[elementNr].setTextString("");
				allElements[elementNr].fontRenderingMode = "customThicknessSharpness";
				allElements[elementNr].antiAliasSharpness = sharpness;
				allElements[elementNr].antiAliasThickness = thickness;
				allElements[elementNr].embedRanges = EMBED_RANGES;
				//allElements[elementNr].embeddedCharacters = "ĄąĘęÓóĆćŁłŃńŚśŹźŻżšťúůýžáčďéěíňřŠŤÚŮÝŽÁČĎÉĚÍŇŘ„”—æøÆØåäöÅÄÖßüÜ";
				
				if(addInstance(allElements[elementNr].name, "CasparTextField"))
				{
					xmlComponentInstances += "<instance name=\"" + allElements[elementNr].name + "\" type=\"" + "CasparTextField" +"\" />";
				}
				
				thisFrame.actionScript +=  "\n(root as CasparTemplate).registerComponent(new CasparTextField(" + allElements[elementNr].name + ", Number("+ allElements[elementNr].name + ".getTextFormat().letterSpacing)));"
				foundCasparTextField = true;
				
				//.getTextFormat().letterSpacing)
				
				if(CasparComponentTypes.length > 0) 
				{
					for(var componentTypes = 0; componentTypes < CasparComponentTypes.length; componentTypes++)
					{
						if(CasparComponentTypes[componentTypes] == "CasparTextField") 
						{
							addComponentDescription = false;
						}
					}
				} 
				else 
				{
					CasparComponentTypes.push("CasparTextField");
				}
				
				if(addComponentDescription) 
				{
					CasparComponentTypes.push("CasparTextField");
					xmlComponentDefinitions += CasparTextFieldDescription;
				}
			}
		} 
		//Check for caspar components
		else if(allElements[elementNr].instanceType == "compiled clip") {
			if(allElements[elementNr].libraryItem.name.search("Caspar") != -1 || allElements[elementNr].libraryItem.name.search("caspar") != -1)
			{
				if(allElements[elementNr].name == "")
				{
					output.trace("***Info***\nFound an unnamed caspar component at frame nr. " + frameNr + " on layer \"" + allLayers[layerNr].name + "\".\n");
					bWarningFoundUnnamedCasparComponent = true;
				} 
				else
				{
					addComponentDescription = true;
					
					var parameters = allElements[elementNr].parameters;
					var componentDescription = "";
					
					if(parameters.length > 0)
					{
						
						for(var i = 0; i < parameters.length; i++)
						{
							if(parameters[i].name == "description")
							{
								if(parameters[i].value.search("<component") != -1)
								{
									componentDescription = parameters[i].value;
								}
							}
						}
					}
					
					if(componentDescription != "")
					{
						if(CasparComponentTypes.length > 0) 
						{
							for(var componentTypes = 0; componentTypes < CasparComponentTypes.length; componentTypes++)
							{
								if(CasparComponentTypes[componentTypes] == allElements[elementNr].libraryItem.name) 
								{
									addComponentDescription = false;
								}
							}
						} 
						else 
						{
							CasparComponentTypes.push(allElements[elementNr].libraryItem.name);
						}
						
						if(addComponentDescription) 
						{
							CasparComponentTypes.push(allElements[elementNr].libraryItem.name);
							xmlComponentDefinitions += componentDescription;
						}
					}
					else
					{
						bWarningFoundInvalidComponent = true;
					}
					
					if (addInstance(allElements[elementNr].name, allElements[elementNr].libraryItem.name))
					{
						xmlComponentInstances += "<instance name=\"" + allElements[elementNr].name + "\" type=\"" + allElements[elementNr].libraryItem.name +"\" />";
					}
					if(addImports)
					{
						thisFrame.actionScript += "import se.svt.caspar.template.components.CasparTextField;\n";
						thisFrame.actionScript += "import se.svt.caspar.template.CasparTemplate;\n\n";
						addImports = false
					}
					thisFrame.actionScript +=  "\n(root as CasparTemplate).registerComponent(" + allElements[elementNr].name + ");"
				}
			}		
		}
	}
}

// Scan the library for CasparComponent instances
function scanLibrary() 
{
	verboseTrace("Scanning the library for movie clips and bitmaps...");
	var libraryItems = thisDocument.library.items;
	
	for(i=0; i<libraryItems.length; i++) 
	{
		if(libraryItems[i].itemType == "movie clip") 
		{
			verboseTrace(" >Found a movie clip, scan it's timeline...");
			scanTimeline(libraryItems[i].timeline);
		} 
		else if(libraryItems[i].itemType == "bitmap") 
		{
			if (optimizeImages == 1)
			{
				libraryItems[i].allowSmoothing = true;
				libraryItems[i].compressionType = "lossless";
			}
		}
	}
}

function setBrewData() 
{
	xmlComponentDefinitions += "</components>";
	xmlComponentInstances += "</instances>";
	
	xmlOutput = "<template version=\"" + VERSION + "\" authorName=\"" + authorName +  "\" authorEmail=\"" + authorEmail + "\" templateInfo=\"" + templateInfo + "\" originalWidth=\"" + thisDocument.width + "\" originalHeight=\"" + thisDocument.height + "\" originalFrameRate=\"" + thisDocument.frameRate + "\" >"
	
	xmlOutput += "\n" + xmlComponentDefinitions;
	xmlOutput += "\n" + xmlKeyframes;
	xmlOutput += "\n" + xmlComponentInstances;
	xmlOutput += "\n" + customParameterDescription;
	xmlOutput += "\n </template>";
	//var escapedXmlOutput = xmlOutput.replace(/"/g, "\\\"");
	//xmlOutput = escapedXmlOutput;
	
	thisDocument.currentTimeline = 0;

	// True if the document already contains brew data
	var checkForBrewLayer = false;
	var allLayersInRoot = thisDocument.getTimeline().layers;
	
	//Check if the document already has brew data in it.
	for(var i = 0; i < allLayersInRoot.length; i++) 
	{
		if(allLayersInRoot[i].name == "brewActions") 
		{
			checkForBrewLayer = true;
			thisDocument.getTimeline().currentLayer = i;
			allLayersInRoot[i].locked = true;
			thisDocument.getTimeline().reorderLayer(i, 0, true);
			break;
		}
	}
	
	// The document contains no brew data
	if(!checkForBrewLayer) 
	{
		thisDocument.getTimeline().currentLayer = 0;
		thisDocument.getTimeline().addNewLayer("brewActions", "normal", true);
		thisDocument.getTimeline().layers[0].locked = true;
	}
	
	thisDocument.getTimeline().layers[0].frames[0].actionScript = "";
	if(foundCasparTextField) 
	{
		thisDocument.getTimeline().layers[0].frames[0].actionScript +=  "\nimport se.svt.caspar.template.components.CasparTextField;\n\n";
	}
	
	for(var i=0; i<thisDocument.getTimeline().layers.length; i++)
	{
		if(thisDocument.getTimeline().layers[i].layerType != "folder") 
		{
			var actionscript = thisDocument.getTimeline().layers[i].frames[0].actionScript;
			
			if(actionscript.search("stop()") != -1)
			{
				stopOnFirstFrame = true;
			}
		}		
	}
	
	thisDocument.getTimeline().layers[0].frames[0].actionScript += DESCRIPTION_XML_VAR_NAME + " = new XML(\n"+ xmlOutput + "\n);\n";
	thisDocument.getTimeline().layers[0].frames[0].actionScript += ORIGINAL_WIDTH_VAR_NAME + " = " + thisDocument.width + ";\n";
	thisDocument.getTimeline().layers[0].frames[0].actionScript += ORIGINAL_HEIGHT_VAR_NAME + " = " + thisDocument.height + ";\n";
	thisDocument.getTimeline().layers[0].frames[0].actionScript += ORIGINAL_FRAME_RATE_VAR_NAME + " = " + thisDocument.frameRate + ";\n";
	thisDocument.getTimeline().layers[0].frames[0].actionScript += STOP_ON_FIRST_FRAME_VAR_NAME + " = " + stopOnFirstFrame + ";\n";

	verboseTrace("Writing template data: originalWidth=" + thisDocument.width + " originalHeight=" + thisDocument.height + " originalFramerate=" + thisDocument.frameRate + " stopOnFirstFrame=" + stopOnFirstFrame);
	verboseTrace("Template metadata:\n " + xmlOutput);
	
	//var metadataXML = '<rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" ><rdf:Description rdf:about = "" xmlns:dc = "http://purl.org/dc/1.1/" ><dc:title>CasparCG Template metadata</dc:title><dc:description>Här kommer xml:en tjollahopp tjollahej tjollahooppsansa</dc:description></rdf:Description></rdf:RDF>';
	
	//thisDocument.setMetadata(metadataXML);
	
	if(nDisposeAt != -1)
	{
		thisDocument.getTimeline().insertKeyframe(nDisposeAt);
		thisDocument.getTimeline().layers[0].frames[nDisposeAt].actionScript = "stop();\nremoveTemplate();";
	}
}

function displayFeedback() 
{
	// Some feedback to the user
	var noErrors = true;
	if(bWarningFoundUnnamedCasparComponent) 
	{
		output.trace("***Error***\n All Caspar Components need to have an instance name.");
		noErrors = false;
	}
	if(bWarningFoundUnnamedMovieClip)
	{
		output.trace("***Error***\n If you have dynamic textfields inside a MovieClip you must name both the textfield and the MovieClip instance.");
		noErrors = false;
	}
	if(bWarningFoundUnnamedTextField)
	{
		output.trace("***Error***\n All dynamic TextFields need to have an instance name. If you want to create a text that is static, please choose static for the text field type.");
		noErrors = false;
	}
	if(bWarningFoundInvalidComponent)
	{
		output.trace("***Error***\n You use a Caspar component that has invalid description data. Try to remove the component instance(s) and then drag them back to the stage.");
		noErrors = false;
	}
	if(bWarningFoundDuplicateInstanceNames)
	{
		output.trace("***Error***\n Found two or more instances that uses the same instance name but that are of different types.");
		noErrors = false;
	}
	if(bWarningFoundMissingAuthorName)
	{
		output.trace("***Error***\n You must enter your name in the Author field in the \"FT Generator\" panel.");
		noErrors = false;
	}
	if(bWarningFoundUnnamedVideo)
	{
		output.trace("***Error***\n If you want to optimize embedded video instances they need to have instance names.");
		noErrors = false;
	}
	
	if (!noErrors)
	{
		output.trace("\n***Info***\n The process was terminated.");
	}
		
	return noErrors;
}

//function browseForDirectory()
//{
	//var folderURI = fl.browseForFolderURL("Select the template folder e.g. ..\\Caspar\\Templates\\");  
	//return folderURI;
//}

function checkIfFolderExists(fileURI)
{
	return FLfile.exists(fileURI);
}

function convertToURIPath(fp) 
{
	if (fp.search("file:/") == -1)
	{
		var prefix;
		
		if (fp.search(":") == -1)
		{
			prefix = "file://";
		}
		else
		{
			prefix = "file:///";
		}
		
		var newPath = fp.replace(/\.fla/, "");
		var newPath = newPath.replace(/\\/g, "/");
		//var newPath = newPath.replace(/:/, "|");
		newPath = prefix + newPath;
		return newPath;
	}
	else
	{
		return fp;
	}
}

function convertToOSPath(fp) 
{
	if (fp.search("file:/") == -1)
	{
		var newPath = fp.replace(/file:\/\/\//, "");
		var newPath = newPath.replace(/\//g, "\\");
		var newPath = newPath.replace(/\|/, ":");
		return newPath;
	}
	else
	{
		return fp;
	}
}

//Test the movie, create a .ft file and revert
function handleFiles() 
{
	
	var path = convertToURIPath(filePath, true);
	var fileURI = path + ".swf";
	var copyURI = path + ".ft";
	var errorLog = path + ".log";
	
	
	
	thisDocument.currentPublishProfile = "Caspar";
	thisDocument.exportSWF(fileURI, true);
	
	fl.compilerErrors.save(errorLog);
	
	var errors = FLfile.read(errorLog);

	if(errors.search("\\*\\*Error\\*\\*") != -1) 
	{
		if(FLfile.exists(fileURI)) 
		{
			FLfile.remove(fileURI);
		}
		
		output.trace("-----------------------------------------------------");
		output.trace("***Error***\n Your file did not compile. The errors are logged at: "+ convertToOSPath(errorLog) + "\n\nYou can view the errors in the \"Compiler Errors\" -panel.");
	}
	else 
	{
		
		if(FLfile.exists(copyURI)) 
		{
			FLfile.remove(copyURI);
			FLfile.copy(fileURI, copyURI);
		} 
		else
		{
			FLfile.copy(fileURI, copyURI);
		}
		
		fileName = thisDocument.name.replace(/\.fla/, "");
		
		//remmote copy
		if (copyToPath != "") 
		{
			var remoteFilePath = copyToPath + fileName + ".ft";
			
			remoteFilePath = convertToURIPath(remoteFilePath);
			copyToPath = convertToURIPath(copyToPath);
			
			
			//if(!FLfile.exists(copyToPath))
			//{
				//verboseTrace("The folder targeted for copying wa not found. Creating..." + copyToPath);
				//FLfile.createFolder(copyToPath);
				//verboseTrace("The folder targeted for copying was created");
			//}
			verboseTrace("Trying to copy the template to target: " + remoteFilePath);
			if(FLfile.exists(remoteFilePath))
			{
				verboseTrace("Removing old .ft-file...");
				FLfile.remove(remoteFilePath);
				remoteCopySuccess = FLfile.copy(fileURI, remoteFilePath);
				verboseTrace("Copy success?: " + remoteCopySuccess);
			}
			else
			{
				remoteCopySuccess = FLfile.copy(fileURI, remoteFilePath);
				verboseTrace("Copy success?: " + remoteCopySuccess);
			}
		}
		
		
		FLfile.remove(fileURI);
		output.trace("-----------------------------------------------------");
		output.trace("***Info***\nThe CasparCG flash template was successfully created! You can find the .ft file in:\n"+ convertToOSPath(copyURI));
		
		
		FLfile.remove(errorLog);
		
		//if(bShowPreview)
		//{
			//var pathToApp = "\"" + fl.configDirectory+"\WindowSWF\\ftPreviewer\\ftPreviewer.exe" + "\"";
			//var pathToFile = "\"" + convertToOSPath(copyURI) + "\"";	
			//var command = "start " + pathToApp + " " + pathToFile;
		//
			//FLfile.runCommandLine(command);
		//}
		
	}
	
}

function verboseTrace(msg)
{
	if(verbose == "true")
	{
		output.trace("\n +" + msg);
	}	
}

/**
 * Writes permanent metadata to the .fla
 */
function writeMetadata(metadata)
{
	thisDocument.addDataToDocument("templateInfo", "string", metadata)
}

/**
 * reads permanent metadata from the .fla
 */
function readMetadata()
{
	return thisDocument.getDataFromDocument("templateInfo");
}

//SEQUENCE

//init(20, -100, false, "Andreas Jeansson", "andreas.jeansson@svt.se", "Detta är info om mallen va.", true, true, true, "file://d40020/Caspar/test/");

function sequence() 
{

	scanRoot(allTimeLines[0]);
	setDisposeFrame(allTimeLines[0]);
	
	scanLibrary();
	
	var processOK = displayFeedback();
	
	//Start the procedure of writing brew data to the document
	
	if(processOK) 
	{
		setBrewData();
		handleFiles();
	}
	
	thisDocument.revert();
	//writeMetadata();
	
}

