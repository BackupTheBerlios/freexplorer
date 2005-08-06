<xsl:stylesheet version = '1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>

    <xsl:template match="wml">
        <html>
            <head>
				<link rel="yellow"     href=   "back"/>
            </head>
            <body bgcolor="#00000028" text="#FFFFFF3F" link="#80C0FF3F" alink="#FF00003F" vlink="#FF00FF3F">
		<hr/>
		<table>
			<tr>
				<td width="3%"></td>
				<td width="92%" height="470" valign="top"><font size="+1">
					<xsl:apply-templates select="card"/>
				</font></td>
				<td width="3%"></td>
			</tr>
		</table>
            </body>
        </html>
    </xsl:template>

    <xsl:template match="card">
	<xsl:if test="@ontimer != ''">
		<meta name="refresh" content="{timer/@value};url={@ontimer}"/>
	</xsl:if>

	<table bgcolor="#6060603f" width="100%"><tr><td align="center"><font color="#EFB70F3F" size="+2">
		<xsl:value-of select="@title"/>
	</font></td></tr></table>
	<xsl:apply-templates/>
    </xsl:template>

    <xsl:template match="p">
	<p>
		<xsl:apply-templates/>
        </p>
    </xsl:template>

    <xsl:template match="a">
	<xsl:if test="(name(preceding::*[1]) != 'input') and (name(preceding::*[2]) != 'input')">
	<a href="{@href}">
		<xsl:value-of select="."/>
	</a>
	</xsl:if>
    </xsl:template>

    <xsl:template match="anchor">
	<form method="GET" action="{substring-before(concat(go/@href,'?'),'?')}">
		<input type="hidden" name="($$dummy)" value="{concat('&amp;',substring-after(go/@href,'?'))}"/>
         	<input type="text" size="40" name="{go/setvar/@name}" value="{go/setvar/@value}"/>
		<input type="submit" value="{.}"/>
	</form>
    </xsl:template>

    <xsl:template match="br">
        <br/>
    </xsl:template>

	<xsl:template match="do[@type='options']">
		<p>
			[<a href="{go/@href}">
				<xsl:value-of select="@label"/>
			</a>]
		</p>
	</xsl:template>

	<xsl:template match="input">
	<form method="GET" action="{substring-before(concat(following::a/@href,'?'),'?')}">
		<input type="hidden" name="($$dummy)" value="{concat('&amp;',substring-after(following::a/@href,'?'))}"/>
        <input type="text" size="40" name="({@name})" value="{@value}">
			<xsl:if test="@maxlength">
				<xsl:attribute name="maxlength">
					<xsl:value-of select="@maxlength" />
				</xsl:attribute>
			</xsl:if>
		</input>
		<input type="submit" value="{following::a}"/>
	</form>
    </xsl:template>

</xsl:stylesheet>
