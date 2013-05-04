<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="text" indent="yes" />

<xsl:template match="/">
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Nephrite.Meta;
using Nephrite.Web;
using Nephrite.Web.Controls;

namespace Solution
{
	public sealed class Meta : MetaElement, IMetaSystem
	{
		static Dictionary&lt;string, IMetaClass&gt; _classesbyname = new Dictionary&lt;string, IMetaClass&gt;(<xsl:value-of select="count(//Class)"/>);
		static Dictionary&lt;Guid, IMetaClass&gt; _classesbyid = new Dictionary&lt;Guid, IMetaClass&gt;(<xsl:value-of select="count(//Class)"/>);

		static Dictionary&lt;string, IMetaPackage&gt; _packagesbyname = new Dictionary&lt;string, IMetaPackage&gt;(<xsl:value-of select="count(//Package)"/>);
		static Dictionary&lt;Guid, IMetaPackage&gt; _packagesbyid = new Dictionary&lt;Guid, IMetaPackage&gt;(<xsl:value-of select="count(//Package)"/>);
		
		public string EntryPoint { get { return ""; } }
		public Dictionary&lt;string, IMetaPackage&gt; Packages { get { return null; } }
		
		<xsl:for-each select="//Class">
		static Meta<xsl:value-of select="@Name"/> _<xsl:value-of select="@Name"/> = null;
		public static Meta<xsl:value-of select="@Name"/>&#160;<xsl:value-of select="@Name"/> { get { if (_<xsl:value-of select="@Name"/> == null) _<xsl:value-of select="@Name"/> = new Meta<xsl:value-of select="@Name"/>(); return _<xsl:value-of select="@Name"/>; } }
		</xsl:for-each>
		
		<xsl:for-each select="//Package">
		static Meta<xsl:value-of select="@Name"/> _<xsl:value-of select="@Name"/> = null;
		public static Meta<xsl:value-of select="@Name"/>&#160;<xsl:value-of select="@Name"/>Package { get { if (_<xsl:value-of select="@Name"/> == null) _<xsl:value-of select="@Name"/> = new Meta<xsl:value-of select="@Name"/>(); return _<xsl:value-of select="@Name"/>; } }
		</xsl:for-each>

		static Meta _system;
		public static Meta System 
		{ 
			get 
			{
				if (_system == null)
				{
					_system = new Meta(); 
				}
				return _system; 
			} 
		}		
		
		public Meta()
		{
			ID = Guid.Empty;
			Name = "System";
			Caption = "System";
			<xsl:for-each select="//Package">
			_packagesbyname.Add(Meta.<xsl:value-of select="@Name"/>Package.Name.ToLower(), Meta.<xsl:value-of select="@Name"/>Package);
			_packagesbyid.Add(Meta.<xsl:value-of select="@Name"/>Package.ID, Meta.<xsl:value-of select="@Name"/>Package);
			</xsl:for-each>
			<xsl:for-each select="//Class">
			_classesbyname.Add(Meta.<xsl:value-of select="@Name"/>.Name.ToLower(), Meta.<xsl:value-of select="@Name"/>);
			_classesbyid.Add(Meta.<xsl:value-of select="@Name"/>.ID, Meta.<xsl:value-of select="@Name"/>);
			</xsl:for-each>
		}
		
		public IMetaClass GetClass(string name)
		{
			string s = name.ToLower();
			return _classesbyname.ContainsKey(s) ? _classesbyname[s] : null;
		}
		public IMetaClass GetClass(Guid id)
		{
			return _classesbyid.ContainsKey(id) ? _classesbyid[id] : null;
		}
		public IMetaOperation GetOperation(string className, string operationName)
		{
			IMetaClass c = GetClass(className);
			if (c == null) return null;
			return c.Operations.ContainsKey(operationName) ? c.Operations[operationName] : null;
		}

		public IMetaPackage GetPackage(string name)
		{
			string s = name.ToLower();
			return _packagesbyname.ContainsKey(s) ? _packagesbyname[s] : null;
		}
		public IMetaPackage GetPackage(Guid id)
		{
			return _packagesbyid.ContainsKey(id) ? _packagesbyid[id] : null;
		}
	}
	<xsl:for-each select="//Class">
	public sealed class Meta<xsl:value-of select="@Name"/> : MetaClass
	{
		public Meta<xsl:value-of select="@Name"/>()
		{
			ID = new Guid("<xsl:value-of select="@ID"/>");
			Name = "<xsl:value-of select="@Name"/>";
			Caption = "<xsl:value-of select="@Caption"/>";
			Description = "<xsl:value-of select="@Description"/>";
			CLRType = typeof(Solution.Model.<xsl:value-of select="@Name"/>);
			Parent = Meta.<xsl:value-of select="../../@Name"/>Package;
			<xsl:for-each select="./Properties/Attribute">
			Properties.Add("<xsl:value-of select="@Name"/>", P.<xsl:value-of select="@Name"/>);
			</xsl:for-each>
			<xsl:for-each select="./Properties/Reference">
			Properties.Add("<xsl:value-of select="@Name"/>", P.<xsl:value-of select="@Name"/>);
			</xsl:for-each>
			<xsl:for-each select="./Operations/Operation">
			Operations.Add("<xsl:value-of select="@Name"/>", O.<xsl:value-of select="@Name"/>);
			</xsl:for-each>
		}
		
		public static class P
		{
			<xsl:for-each select="./Properties/Attribute">
			static MetaAttribute _<xsl:value-of select="@Name"/> = new MetaAttribute { ID = new Guid("<xsl:value-of select="@ID"/>"), Name = "<xsl:value-of select="@Name"/>", Caption = "<xsl:value-of select="@Caption"/>", Parent = Meta.<xsl:value-of select="../../@Name"/> };
			public static MetaAttribute <xsl:value-of select="@Name"/> { get { return _<xsl:value-of select="@Name"/>; } }	
			</xsl:for-each>
			<xsl:for-each select="./Properties/Reference">
			static MetaReference _<xsl:value-of select="@Name"/> = new MetaReference { ID = new Guid("<xsl:value-of select="@ID"/>"), Name = "<xsl:value-of select="@Name"/>", Caption = "<xsl:value-of select="@Caption"/>", Parent = Meta.<xsl:value-of select="../../@Name"/> };
			public static MetaReference <xsl:value-of select="@Name"/> { get { return _<xsl:value-of select="@Name"/>; } }	
			</xsl:for-each>
		}
		
		public static class O
		{
			<xsl:for-each select="./Operations/Operation">
			static MetaOperation _<xsl:value-of select="@Name"/> = new MetaOperation { ID = new Guid("<xsl:value-of select="@ID"/>"), Name = "<xsl:value-of select="@Name"/>", Caption = "<xsl:value-of select="@Caption"/>", Image = "<xsl:value-of select="@Image"/>", Parent = Meta.<xsl:value-of select="../../@Name"/> };
			public static MetaOperation <xsl:value-of select="@Name"/> { get { return _<xsl:value-of select="@Name"/>; } }	
			</xsl:for-each>
		}

		public static class L
		{
			<xsl:for-each select="./Operations/Operation">
			static ActionLink _<xsl:value-of select="@Name"/> = ActionLink.To(Meta<xsl:value-of select="../../@Name"/>.O.<xsl:value-of select="@Name"/>);
			public static ActionLink <xsl:value-of select="@Name"/>(<xsl:value-of select="@Parameters"/>)
			{
				HtmlParms p = new HtmlParms();
				<xsl:for-each select="./Parameters/Parameter">
				p.Add("<xsl:value-of select="@Name"/>", <xsl:value-of select="@Name"/>.ToString());
				</xsl:for-each>	
				return _<xsl:value-of select="@Name"/>.With(p, false); 
			}
			</xsl:for-each>	
		}
	}
	</xsl:for-each>

	<xsl:for-each select="//Package">
	public sealed class Meta<xsl:value-of select="@Name"/> : MetaPackage
	{
		public Meta<xsl:value-of select="@Name"/>()
		{
			ID = new Guid("<xsl:value-of select="@ID"/>");
			Name = "<xsl:value-of select="@Name"/>";
			Caption = "<xsl:value-of select="@Caption"/>";
			<xsl:if test="@ParentID != ''">
				ParentID = new Guid("<xsl:value-of select="@ParentID"/>");
			</xsl:if>
		}
	}
	</xsl:for-each>	
}
</xsl:template>
</xsl:stylesheet>
