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

// TO DO
// *. Only expose component instances that are used in the document?
// *. Add features (tools) to the toolbar and gfx
// *. Check if it is possible to go into the document class and check for illegal override of Stop (it is possible so just do it)
// *. Make brew undo instead of revert to be able to keep the undo history for the .fla (not possible?)
// * Rebuild error reporting system
// * Create invisible .dat file with export settings stored (check auto update)
// * Publish to custom .ft name
// * Remove errorlog if compile
// * Remove swf after build

//CONSTANTS
const SOMETHING_WENT_WRONG_PLEASE_CLOSE_AND_REOPEN_THE_FILE_AND_TRY_AGAIN = "";
const VERSION 						= "1.8.0";
const EMBED_RANGES 					= "1|2|3|4|5|17|18";
const CLASS_PATH 					= "se.svt.caspar.template.CasparTemplate";
const DESCRIPTION_XML_VAR_NAME		= "description";
const ORIGINAL_WIDTH_VAR_NAME 		= "originalWidth";
const ORIGINAL_HEIGHT_VAR_NAME 		= "originalHeight";
const ORIGINAL_FRAME_RATE_VAR_NAME 	= "originalFrameRate";
const STOP_ON_FIRST_FRAME_VAR_NAME 	= "stopOnFirstFrame";

//VARIABLES

var output = fl.outputPanel;
output.clear();

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

// FUNCTIONS

function init()
{
	//fl.removeEventListener("documentChanged", onDocumentChanged);
	//fl.addEventListener("documentChanged", onDocumentChanged);
}

function onDocumentChanged()
{
	//försök läsa .dat-fil
	var path = convertToURIPath(fl.getDocumentDOM().path, true);
	var fileURI = path + ".dat";
	fl.trace("Läser: " + fileURI);
	fl.trace(FLfile.read( fileURI));
}

function deactivate() {
	fl.trace( "Tool is no longer active" );
}

function generate(tn, sn, showPreview, aname, aemail, tinfo, optImages, optTextFields, optVideos, copyPath) 
{
	
	setClassPath();
	
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
	
	copyToPath =  copyPath;
	
	writeCacheFile();

	output.trace("||Generating CasparCG flash template, version 1.8.0||\n");
	
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
		
		//thisDocument.getTimeline().addNewLayer("Processing", "normal", true);
		sequence();
		output.trace("RETURNERAR " + fileName);
		return fileName;
	}
}

function setClassPath()
{
	var bIsFound = false;
	var classPaths = fl.as3PackagePaths.split(';');

	for (var i = 0; i < classPaths.length; i++)
	{
		if(classPaths[i] == '$(LocalData)/Classes/Caspar'){
			bIsFound = true;
		}
	}
	
	if (!bIsFound)
	{
		classPaths.push('$(LocalData)/Classes/Caspar');
		fl.as3PackagePaths = classPaths.join(';');
	}

}

function writeCacheFile()
{
	var configFile = fl.configURI + "/WindowSWF/FTGeneratorCache.dat";
	FLfile.write(configFile, authorName + "#" + authorEmail);
}

function checkServer(serverName)
{
	
	var serverURI = "file://" + serverName + "/Caspar";
	fl.trace("kollar server... " + serverName + " | " + serverURI);
	if (FLfile.exists(serverURI)) 
	{
		fl.trace("TERUW");
		return "true";
	}
	else 
	{
		fl.trace("FAELSE");
		return "false";
	}
}

function getFolders(serverName)
{
	
	var serverURI = "file://" + serverName + "/Caspar/";
	fl.trace("kollar server för list... " + serverName + " | " + serverURI);
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
	//fl.trace("returnString: " + returnString);
	return returnString;
}


//check the document class
function readDocumentClass(docClass)
{

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
		row = row.replace(/ /g, "");
		row = row.replace(/\t/g, "");
		
		if(row.search("privateconstcustomParameterDescription:XML") != -1)
		{
			var descriptionDone = false;
			var j = i+1;
			
			while(!descriptionDone)
			{
				if(j >= documentClassArray.length)
				{
					customParameterDescription = "";
					break;
				}
				if(documentClassArray[j] != undefined)
				{
				
					var searchstring = documentClassArray[j].replace(/ /g, "");
					searchstring = searchstring.replace(/\t/g, "");
					if(searchstring.search("</parameters>") != -1)
					{
						descriptionDone = true;
						break;
					}
					else if(searchstring.search("<parameter") != -1 && searchstring.search("<parameters>") == -1)
					{
						customParameterDescription = customParameterDescription + documentClassArray[j].replace(/(\r\n|\r|\n|\t)/g, "");
					}
				}				
				
				j++;
			}
			break;
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
					//nFoundOutro = getLastFrame(timeline, thisFrame.);
					nDisposeAt = allLayers[layerNr].frameCount -1;
					nFoundOutro = frameNr;	
				}
				else 
				{
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
	}
	
	return addComponentInstance;	
}

function scanElements(thisFrame, frameNr, allLayers, layerNr)
{
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
					for(componentTypes = 0; componentTypes < CasparComponentTypes.length; componentTypes++)
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
							for(componentTypes = 0; componentTypes < CasparComponentTypes.length; componentTypes++)
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
	var libraryItems = thisDocument.library.items;
	
	for(i=0; i<libraryItems.length; i++) 
	{
		if(libraryItems[i].itemType == "movie clip") 
		{
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

function convertToURIPath(fp) 
{
	var newPath = fp.replace(/\.fla/, "");
	var newPath = newPath.replace(/\\/g, "/");
	var newPath = newPath.replace(/:/, "|");
	newPath = "file:///" + newPath;
	return newPath;
}

function convertToOSPath(fp) 
{
	var newPath = fp.replace(/file:\/\/\//, "");
	var newPath = newPath.replace(/\//g, "\\");
	var newPath = newPath.replace(/\|/, ":");
	return newPath;
}

//Test the movie, create a .ft file and revert
function handleFiles() 
{
	
	var path = convertToURIPath(filePath, true);
	var fileURI = path + ".swf";
	var copyURI = path + ".ft";
	var errorLog = path + ".log";
	var datURI = path + ".dat";
	
	thisDocument.currentPublishProfile = "Caspar";

	thisDocument.exportSWF(fileURI, true);
	
	fl.compilerErrors.save(errorLog);
	
	var errors = FLfile.read(errorLog);

	if(errors.search("\\*\\*Error\\*\\*") != -1) 
	{
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
		
		//remmote copy
		
		fileName = thisDocument.name.replace(/\.fla/, "");
		
		var remoteFilePath = copyToPath + fileName + ".ft";
		
		writeDatFile(datURI);
		
		if(copyToPath != "")
		{
			if(!FLfile.exists(copyToPath))
			{
				FLfile.createFolder(copyToPath);
			}
			if(FLfile.exists(remoteFilePath))
			{
				FLfile.remove(remoteFilePath);
				FLfile.copy(fileURI, remoteFilePath);
			}
			else
			{
				FLfile.copy(fileURI, remoteFilePath);
			}
		}
		
		FLfile.remove(fileURI);
		
		output.trace("\n***Info***\nThe CasparCG flash template was successfully created! You can find the .ft file in:\n"+ convertToOSPath(copyURI));
		
		
		FLfile.remove(errorLog);
		
		if(bShowPreview)
		{
			var pathToApp = "\"" + fl.configDirectory+"\WindowSWF\\ftPreviewer\\ftPreviewer.exe" + "\"";
			var pathToFile = "\"" + convertToOSPath(copyURI) + "\"";	
			var command = "start " + pathToApp + " " + pathToFile;
		
			FLfile.runCommandLine(command);
		}
		
	}
	
}

function writeDatFile(fileURI)
{
	var data;
	
	data = templateInfo + "@@" + optimizeImages;
	
	FLfile.write(fileURI, data);
	FLfile.setAttributes(fileURI, "H");
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
	
}

