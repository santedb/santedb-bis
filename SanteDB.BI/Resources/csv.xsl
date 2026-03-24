<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:html="http://www.w3.org/1999/xhtml"
>
    <xsl:output method="text" indent="no"/>
  <xsl:strip-space elements="*"/>
	
  <xsl:preserve-space elements="td th"/>
	<xsl:template match="@* | node()" priority="-1">
		<xsl:apply-templates select="@* | node()" />
	</xsl:template>
	<xsl:template match="//html:table">
      <xsl:apply-templates select="html:thead" /><xsl:apply-templates select="html:tbody" /><xsl:apply-templates select="html:tfoot" />
    </xsl:template>
  <xsl:template match="html:thead"><xsl:apply-templates select="html:tr"/></xsl:template>
  <xsl:template match="html:tbody"><xsl:apply-templates select="html:tr"/></xsl:template>
  <xsl:template match="html:tfoot"><xsl:apply-templates select="html:tr"/></xsl:template>
  <xsl:template match="html:tr"><xsl:apply-templates select="html:th|html:td"></xsl:apply-templates>
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>
  <xsl:template name="blank_hdr">
	  <xsl:param name="offset"/>
	  <xsl:if test="$offset &gt; 0">"",<xsl:call-template name="blank_hdr">
			  <xsl:with-param name="offset" select="$offset - 1"/>
		  </xsl:call-template>
	  </xsl:if>
  </xsl:template>
  <xsl:template match="html:td[position()&lt;last()]|html:th[position()&lt;last()]">
	  <xsl:call-template name="blank_hdr">
		  <xsl:with-param name="offset" select="@_offset"/>
	  </xsl:call-template>"<xsl:value-of select="." disable-output-escaping="yes"/>",<xsl:call-template name="blank_hdr">
	  <xsl:with-param name="offset" select="@colspan - 1"/>
	  </xsl:call-template>
	</xsl:template>
  <xsl:template match="html:td[position()=last()]|html:th[position()=last()]">"<xsl:value-of select="." disable-output-escaping="yes"/>"</xsl:template>
</xsl:stylesheet>
