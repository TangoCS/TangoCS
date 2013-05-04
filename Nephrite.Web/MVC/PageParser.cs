using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Nephrite.Web
{
	public class CustomViewTypeParserFilter : PageParserFilter
	{
		private string _viewBaseType;
		private DirectiveType _directiveType = DirectiveType.Unknown;
		//private bool _viewTypeControlAdded;

		public override void PreprocessDirective(string directiveName, IDictionary attributes)
		{
			base.PreprocessDirective(directiveName, attributes);

			string defaultBaseType = null;

			// If we recognize the directive, keep track of what it was. If we don't recognize
			// the directive then just stop.
			switch (directiveName)
			{
				//case "page":
				//	_directiveType = DirectiveType.Page;
				//	defaultBaseType = typeof(JG.ParserFilter.CustomViewPage).FullName;  // JG: inject custom types here
				//	break;
				case "control":
					_directiveType = DirectiveType.UserControl;
					defaultBaseType = typeof(ViewControl).FullName; // JG: inject custom types here
					break;
				//case "master":
				//	_directiveType = DirectiveType.Master;
				//	defaultBaseType = typeof(System.Web.Mvc.ViewMasterPage).FullName;
				//	break;
			}

			if (_directiveType == DirectiveType.Unknown)
			{
				// If we're processing an unknown directive (e.g. a register directive), stop processing
				return;
			}


			// Look for an inherit attribute
			string inherits = (string)attributes["inherits"];
			if (!String.IsNullOrEmpty(inherits))
			{
				// If it doesn't look like a generic type, don't do anything special,
				// and let the parser do its normal processing
				if (IsGenericTypeString(inherits))
				{
					// Remove the inherits attribute so the parser doesn't blow up
					attributes["inherits"] = defaultBaseType;

					// Remember the full type string so we can later give it to the ControlBuilder
					_viewBaseType = inherits;
				}
			}
		}

		private static bool IsGenericTypeString(string typeName)
		{
			// Detect C# and VB generic syntax
			// REVIEW: what about other languages?
			return typeName.IndexOfAny(new char[] { '<', '(' }) >= 0;
		}

		public override void ParseComplete(ControlBuilder rootBuilder)
		{
			base.ParseComplete(rootBuilder);

			// If it's our page ControlBuilder, give it the base type string
			//CustomViewPageControlBuilder pageBuilder = rootBuilder as CustomViewPageControlBuilder; // JG: inject custom types here
			//if (pageBuilder != null)
			//{
			//	pageBuilder.PageBaseType = _viewBaseType;
			//}
			CustomViewUserControlControlBuilder userControlBuilder = rootBuilder as CustomViewUserControlControlBuilder; // JG: inject custom types here
			if (userControlBuilder != null)
			{
				userControlBuilder.UserControlBaseType = _viewBaseType;
			}
		}

		public override bool ProcessCodeConstruct(CodeConstructType codeType, string code)
		{
			//if (codeType == CodeConstructType.ExpressionSnippet &&
			//	!_viewTypeControlAdded &&
			//	_viewBaseType != null &&
			//	_directiveType == DirectiveType.Master)
			//{

			//	// If we're dealing with a master page that needs to have its base type set, do it here.
			//	// It's done by adding the ViewType control, which has a builder that sets the base type.

			//	// The code currently assumes that the file in question contains a code snippet, since
			//	// that's the item we key off of in order to know when to add the ViewType control.

			//	Hashtable attribs = new Hashtable();
			//	attribs["typename"] = _viewBaseType;
			//	AddControl(typeof(System.Web.Mvc.ViewType), attribs);
			//	_viewTypeControlAdded = true;
			//}

			return base.ProcessCodeConstruct(codeType, code);
		}

		// Everything else in this class is unrelated to our 'inherits' handling.
		// Since PageParserFilter blocks everything by default, we need to unblock it

		public override bool AllowCode
		{
			get
			{
				return true;
			}
		}

		public override bool AllowBaseType(Type baseType)
		{
			return true;
		}

		public override bool AllowControl(Type controlType, ControlBuilder builder)
		{
			return true;
		}

		public override bool AllowVirtualReference(string referenceVirtualPath, VirtualReferenceType referenceType)
		{
			return true;
		}

		public override bool AllowServerSideInclude(string includeVirtualPath)
		{
			return true;
		}

		public override int NumberOfControlsAllowed
		{
			get
			{
				return -1;
			}
		}

		public override int NumberOfDirectDependenciesAllowed
		{
			get
			{
				return -1;
			}
		}

		public override int TotalNumberOfDependenciesAllowed
		{
			get
			{
				return -1;
			}
		}

		private enum DirectiveType
		{
			Unknown,
			Page,
			UserControl,
			Master,
		}
	}

	public sealed class CustomViewUserControlControlBuilder : FileLevelUserControlBuilder
	{
		public string UserControlBaseType
		{
			get;
			set;
		}

		public override void ProcessGeneratedCode(
			CodeCompileUnit codeCompileUnit,
			CodeTypeDeclaration baseType,
			CodeTypeDeclaration derivedType,
			CodeMemberMethod buildMethod,
			CodeMemberMethod dataBindingMethod)
		{

			// If we find got a base class string, use it
			if (UserControlBaseType != null)
			{
				derivedType.BaseTypes[0] = new CodeTypeReference(UserControlBaseType);
			}
		}
	}
}