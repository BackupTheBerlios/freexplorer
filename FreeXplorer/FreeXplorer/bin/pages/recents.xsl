<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/1999/xhtml">
	
<!-- following code taken from http://www.stylusstudio.com/xsllist/200208/post00750.html 
	because MSXML is not yet XSLT 2.0 compliant and has no escape-uri() function -->
<xsl:variable name="ascii-charset"> !&quot;#$%&amp;&apos;()*+,-./0123456789:;&lt;=&gt;?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~&#127;</xsl:variable>
<xsl:variable name="uri-ok">-_.!~*&apos;()0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz</xsl:variable>
<xsl:variable name="hex">0123456789ABCDEFabcdef</xsl:variable>

<xsl:template name="do-escape-uri">
 <xsl:param name="str"/>
 <xsl:param name="allow-utf8"/>

 <xsl:if test="$str">
  <xsl:variable name="first-char" select="substring($str,1,1)"/>
  <xsl:choose>
   <xsl:when test="$first-char = '%' and string-length($str) &gt;= 3 and contains($hex, substring($str,2,1)) and contains($hex, substring($str,3,1))">
    <!-- The percent char is ok IF it followed by a valid hex pair -->
    <xsl:value-of select="$first-char"/>
   </xsl:when>
   <xsl:when test="contains($uri-ok, $first-char)">
    <!-- This char is ok inside urls -->
    <xsl:value-of select="$first-char"/>
   </xsl:when>
   <xsl:when test="not(contains($ascii-charset, $first-char))">
    <!-- Non-ascii output raw based on utf8 allowed or not -->
    <xsl:choose>
     <xsl:when test="$allow-utf8">
      <xsl:value-of select="$first-char"/>
     </xsl:when>
     <xsl:otherwise>
      <xsl:text>%3F</xsl:text>
     </xsl:otherwise>
    </xsl:choose>
   </xsl:when>
   <xsl:otherwise>
    <!-- URL escape this char -->
    <xsl:variable name="ascii-value" select="string-length(substring-before($ascii-charset,$first-char)) + 32"/>
    <xsl:text>%</xsl:text>
    <xsl:value-of select="substring($hex,floor($ascii-value div 16) + 1,1)"/>
    <xsl:value-of select="substring($hex,$ascii-value mod 16 + 1,1)"/>
   </xsl:otherwise>
  </xsl:choose>
  
  <xsl:call-template name="do-escape-uri">
   <xsl:with-param name="str" select="substring($str,2)"/>
   <xsl:with-param name="allow-utf8" select="$allow-utf8"/>
  </xsl:call-template>
 </xsl:if>
</xsl:template>
<!-- preceding code taken from http://www.stylusstudio.com/xsllist/200208/post00750.html -->
	
<xsl:template match="/">
<html>
	<head>
	<meta name="home_page" content="home.html"/>
	<link rel="help"       href=   "help.html"/>
	<link rel="guide"      href=   "$explore"/>
	<link rel="info"       href=   "playing.html"/>
	<link rel="yellow"     href=   "back"/>
</head>
<body text="#2F2F5F3F" link="#3F5F3F3F" alink="#007FFF3F">
<table>
<tr>
	<td width="4%"></td>
	<td bgcolor="#FFFFFF28" align="center">
	<font size="+2" color="#3F3FFF3F">Fichiers recents</font>
	</td>
	<td width="4%"></td>
</tr>
<tr>
	<td width="4%"></td>
	<td width="92%" bgcolor="#FFFFFF30" height="450" valign="top">
        <xsl:for-each select="Recents/MRL">
        	<xsl:value-of select="@date"/>
        	<xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
        	<xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
        	<xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
        	<a>
        		<xsl:attribute name="href">play.html?action=add&amp;_file=<xsl:call-template name="do-escape-uri">
   					<xsl:with-param name="str" select="."/>
  					</xsl:call-template>&amp;param=Lecture+du+fichier...</xsl:attribute>
				<xsl:value-of select="."/></a>
			<br></br>
		</xsl:for-each>
	</td>
	<td width="4%"></td>
</tr>
</table>
</body>
</html>
</xsl:template>
</xsl:stylesheet>
