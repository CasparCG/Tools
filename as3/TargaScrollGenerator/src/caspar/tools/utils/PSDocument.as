package caspar.tools.utils 
{
	/**
	 * ...
	 * @author Andreas Peter Andersson SVT AB Grafiskt Center
	 */
	public class PSDocument 
	{
		
		private var _originalPath:String;
		private var _fileNameOnCaspar:String;
		private var _id:String;
		
		public function PSDocument() 
		{
			
		}
		
		public function get originalPath():String 
		{
			return _originalPath;
		}
		
		public function set originalPath(value:String):void 
		{
			_originalPath = value;
		}
		
		public function get fileNameOnCaspar():String 
		{
			return _fileNameOnCaspar;
		}
		
		public function set fileNameOnCaspar(value:String):void 
		{
			_fileNameOnCaspar = value;
		}
		
		public function get id():String 
		{
			return _id;
		}
		
		public function set id(value:String):void 
		{
			_id = value;
		}
		
	}

}