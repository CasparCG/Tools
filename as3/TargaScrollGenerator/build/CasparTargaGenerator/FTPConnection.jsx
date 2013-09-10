/*
 * FTPConnection.jsx
 * Author:	Peter Torpey
 *        	www.petertorpey.com
 * Version:	1.1	01-21-24-05
 *         	Original code Copyright 2005 Peter Torpey.
 *         	This code may be used, distributed, and modified for personal use
 *         	provided the name and URL of the original author, above, remain
 *         	intact.
 *
 *         	Portions of this file are based loosely on email_methods.jsx
 *         	from Adobe Systems Inc.
 *
 * Description:
 * 	A rudimentary FTP client implementation using Adobe JavaScript Sockets.
 *
 * Usage:
 *  Include this file in your script.
 *  Create an FTPConnection object. The constructor takes an optional argument
 *  	"verbose" which is a boolean. When true, the interaction between this
 *  	client and the server will be printed in the info palette (probably too
 *  	fast to be of any use).
 *  Call <FTPConnection>.open() passing it the host name or IP address, port
 *  	(usually 21), username, and password for the FTP server to which you
 *  	are connecting. Returns true if the FTP connection was successfully
 *  	opened.
 *  Use <FTPConnection>.put() and <FTPConnection>.get() to upload and download
 *  	files from the server, respectively. Both functions take the same two
 *  	arguments. The first argument is either a string representing a full
 *  	path and name for the file or a File object for the file. The second
 *  	argument is an absolute or relative (to the initial working directory
 *  	for the logged-in account) string representing the path and file name
 *  	on the server. Returns true if the transfer was successfully completed.
 *  <FTPConnection>.close() terminates any open connection.
 *  <FTPConnection>.setEncodingASCII() and <FTPConnection>.setEncodingBinary()
 *  	change the transfer mode accordingly. The current encoding is available
 *  	from the <FTPConnection>.encoding property.
 *  A message that may describe the last error can be accessed with the
 *  	<FTPConnection>.error property.
 *  If a connection to a server is alive, <FTPConnection>.connected will be
 *  	true.
 *  While most errors cause a function to return false, fatal errors cause
 *  	exceptions to be thrown. Thus it is recommended that calls to an open
 *  	FTPConnection be made within a try-catch block.
 *
 * Notes:
 *  This script and the scripts been tested under AE6.5 on WinXP. This script
 *  	rough and has NOT been tested thoroughly and MAY CONTAIN BUGS.
 *
 * Known Issues:
 *  The directory of the file to be written to (in a get(), on the client; in a
 *  	put(), on the server), must already exist.
 *
 * For the Future:
 *  Implement CWD, APPE, MKD, RMD, DELE, LIST, etc.
 *
 */

// var FTPC_NUM_RETRY = 5;
var FTPC_ENC_ASCII = "ASCII";
var FTPC_ENC_BINARY = "BINARY";

var FTPC_ANON_UNAME = "anonymous";
var FTPC_ANON_UPASS = "aescript@";

var _FTPC_DO_LOG = false;
var _FTPC_LOG_FILE = "f:\\reel\\aeftptestlog.txt";

// constructor
function FTPConnection(verbose) {
	var self = this;
	var controlChannel = new Socket();
	var dataChannel = new Socket();
	var writeLogB = (verbose && verbose == true);
	var logFile;

	if (_FTPC_DO_LOG) {
		logFile = new File(_FTPC_LOG_FILE);
	}

	this.connected = false;
	this.host = "";
	this.port = "";
	this.error = "";
//	this.retry = FTPC_NUM_RETRY;
	this.encoding = FTPC_ENC_ASCII;

	this.openAnonymous = function (hostS, port) {
		return this.open(hostS, port, FTPC_ANON_UNAME, FTPC_ANON_UPASS);
	}

	// connect
	this.open = function (hostS, port, unameS, upassS) {
		if (this.connected) {
			this.error = "Error: Already connected.";
			return false;
		}

		if (_FTPC_DO_LOG) {
			if (!logFile.open("w")) {
				alert("Didn't open log file." + "\n" + logFile.error + "\n" + logFile.exists + "\n" + logFile.fsName);
			}
		}

		this.host = hostS;
		this.port = port;

//		for (var i = this.retry + 1; i > 0; i--) {
			if (controlChannel.open(this.host + ":" + parseInt(this.port))) {
				this.connected = controlChannel.connected;
//				break;
			}
//		}

		if (!this.connected) {
			this.error = controlChannel.error;
			return false;
		}

		try {
			// S: 220 ...
			if (!response(220)) throw "Error: FTP connection refused.";
			// C: USER <username>
			command("USER " + unameS);
			// S: 331 ...
			if (!response(230, 332)) throw "Login failed: Server rejected username.";
			// C: PASS <password>
			command("PASS " + upassS);
			// S: 230 ...
			if (!response(202, 230)) throw "Login failed: Server rejected username or password.";
		}
		catch (e) {
			this.close();
			this.error = e.toString();
			throw e;
		}
		finally {
			this.connected = controlChannel.connected;
		}

		return true;
	}

	// get
	this.get = function (localPath, remotePathS) {
		var localF;
		if (localPath instanceof File) {
			localF = localPath;
		}
		else if (localPath instanceof String) {
			localF = new File(localPath);
		}
		else {
			throw "Error: Local file name is not a string or File object";
		}

		try {
			// set type
			command("TYPE " + ((this.encoding == FTPC_ENC_ASCII) ? "A" : "I"));
			if (!response(200, 250) && writeLogB) writeLn("Warning: Server failed to change type.");
			// open local file
			if (!localF.open("w")) {
				this.error = "Error: Could not open local file for writing.";
				return false;
			}
			localF.encoding = ((this.encoding == FTPC_ENC_ASCII) ? "ASCII" : "BINARY");

			openPassiveDataChannel();

			// RETR remote file
			command("RETR " + encodePath(remotePathS));
			if (response(100, 199)) {
				// read / write
//				while (dataChannel.connected && !dataChannel.eof) {
//					if (!localF.write(dataChannel.read())) break;
//				}
				if (!localF.write(dataChannel.read())) {
					this.error = "Error: The file transfer could not be completed (IO).";
					return false;
				}

				dataChannel.close();

				if (!response(226)) {
					this.error = "Error: The file transfer could not be completed (Server226).";
					return false;
				}
			}
			else {
				this.error = "Error: The file transfer could not be completed (Server150).";
				return false;
			}
		}
		catch (e) {
			// close file if open
			// quit
			this.close();
			// this.error = e.toString();
			throw e;
		}
		finally {
			this.connected = controlChannel.connected;
			if (dataChannel.connected) {
				dataChannel.close();
			}
			localF.close();
		}

		return true;
	}

	// put
	this.put = function (localPath, remotePathS) {
		var localF;
		if (localPath instanceof File) {
			localF = localPath;
		}
		else if (localPath instanceof String) {
			localF = new File(localPath);
		}
		else {
			throw "Error: Local file name is not a string or File object";
		}

		try {
			// set type
			command("TYPE " + ((this.encoding == FTPC_ENC_ASCII) ? "A" : "I"));
			if (!response(200, 250) && writeLogB) writeLn("Warning: Server failed to change type.");
			// open local file
			if (!localF.open("r")) {
				this.error = "Error: Could not open local file for reading.";
				return false;
			}
			localF.encoding = ((this.encoding == FTPC_ENC_ASCII) ? "ASCII" : "BINARY");

			openPassiveDataChannel();

			// STOR remote file
			command("STOR " + encodePath(remotePathS));
			if (response(100, 199)) {
				var r;
				// read / write
//				while (dataChannel.connected && !localF.eof) {
//					if (!dataChannel.write(localF.read())) break;
//				}
				if (!dataChannel.write(localF.read())) {
					this.error = "Error: The file transfer could not be completed (IO).";
					return false;
				}

				dataChannel.close();

				if (!response(226)) {
					this.error = "Error: The file transfer could not be completed (Server226).";
					return false;
				}
			}
			else {
				this.error = "Error: The file transfer could not be completed (Server150).";
				return false;
			}
		}
		catch (e) {
			// close file if open
			// quit
			this.close();
			// this.error = e.toString();
			throw e;
		}
		finally {
			this.connected = controlChannel.connected;
			if (dataChannel.connected) {
				dataChannel.close();
			}
			localF.close();
		}

		return true;
	}

	this.close = function () {
		// send quit
		command("QUIT");
		response(221);
		// close controlChannel
		controlChannel.close();
		// dataChannel should be closed anyway
		dataChannel.close();
		this.connected = controlChannel.connected;

		if (_FTPC_DO_LOG) logFile.close();
	}

	// passive mode
	function openPassiveDataChannel() {
		var respS, subm, addrS;

		// PASV
		command("PASV");
		respS = response(227);
		if (!respS) throw "Error: Could not enter passive mode.";

		// parse addr
		subm = respS.match(/(\d{1,3}),(\d{1,3}),(\d{1,3}),(\d{1,3}),(\d*),(\d*)/);
		addrS = subm[1] + "." + subm[2] + "." + subm[3] + "." + subm[4] + ":" + (parseInt(subm[5]) * 256 + parseInt(subm[6]));
//		if (_FTPC_DO_LOG) logFile.writeln("--Client connecting to " + addrS);

		// open data channel
//		for (var i = this.retry + 1; i > 0; i--) {
			if (dataChannel.open(addrS, ((this.encoding == FTPC_ENC_ASCII) ? "ASCII" : "binary"))) {
//				break;
			}
//		}

		if (!dataChannel.connected) {
			this.error = dataChannel.error;
			throw "Error: Could not open passive data channel.";
		}

		return true;
	}

	// set retries
	/*
	this.setNumberOfRetries = function (num) {
		this.retry = parseInt(num);
		if (isNaN(this.retry)) {
			this.retry = FTPC_NUM_RETRY;
		}
		return this.retry;
	}
	*/

	this.setEncodingASCII = function () {
		this.encoding = FTPC_ENC_ASCII;
	}

	this.setEncodingBinary = function () {
		this.encoding = FTPC_ENC_BINARY;
	}

	function command(cmdS) {
		if (cmdS) {
			if (_FTPC_DO_LOG) logFile.writeln(":" + cmdS);
			if (writeLogB) writeLn(cmdS);
			return controlChannel.write(cmdS + "\r\n");
		}
		else {
			return controlChannel.write("\r\n");
		}
	}

	function response(expectLow, expectHigh) {
		var replyS;
		var code;

		// bypass any marks (and empty lines)
		do {
			replyS = controlChannel.read();
			if (_FTPC_DO_LOG) logFile.write("+" + replyS);
			code = replyS.match(/^(\d{3})\s/m);
		}
		while (!code);

		if (writeLogB) {clearOutput(); writeLn(replyS);}

		// this should be the actual response
//		code = replyS.match(/^(\d{3})\s/m);
		if (code && code.length == 2) {
//		if (code.length == 2) {
			code = parseInt(code[1]);
			if (code == expectLow || (expectHigh && code > expectLow && code <= expectHigh)) {
//				if (_FTPC_DO_LOG) logFile.writeln("--Found code: " + code);
				return replyS;
			}
		}

		return null;
	}

	function encodePath(pathS) {
		if (pathS.charAt(0) != "/") {
			var r;
			command("PWD");
			r = response(257);
			if (r) {
				var wd = r.match(/^\d{3}\s\"(.*?\/?)\"/m);
				pathS = wd[1] + pathS;
			}
		}
		return pathS;
		// not only shouldn't this be needed, it breaks
//		return pathS.replace("\0", "\n");
	}

	this.toString = function () {
		return "[object FTPConnection]";
	}

}
