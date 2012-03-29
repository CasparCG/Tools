using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CasparCG.Conformer.Core
{
    public class Specification
    {
        /// <summary>
        /// Finds the target extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static bool FindTargetExtension(string extension)
        {
            return (XDocument.Load("Specification.xml").XPathSelectElements(string.Format("/Targets/Target[@extension='{0}']", extension)).Count() > 0) ? true : false;
        }

        /// <summary>
        /// Gets the target command.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static string GetTargetCommand(string extension)
        {
            return XDocument.Load("Specification.xml").XPathSelectElement(string.Format("/Targets/Target[@extension='{0}']", extension)).Attribute("command").Value;
        }
    }
}
