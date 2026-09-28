<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
				xmlns:xhtml="http://www.w3.org/1999/xhtml"
				xmlns:bi="http://santedb.org/bi"
>
	<xsl:output method="html" indent="yes"/>

	<xsl:namespace-alias stylesheet-prefix="xhtml" result-prefix="#default"/>
	
	<!-- Include documentation links -->
	<xsl:include href="BiDatamartDefinition.xslt" />

	<!-- Styles Template -->
	<xsl:template name="Styles">
		<style type="text/css">
			<![CDATA[
			
			
			]]>
		</style>
	</xsl:template>

	<!-- Root Entry -->
	<xsl:template match="/bi:*">

		<xhtml:html>
			<head>
				<title>
					Documentation - <xsl:value-of select="@name"/>
				</title>
				<xsl:call-template name="Styles" />
			</head>
			<body>
				<xsl:apply-templates select="." mode="body" />
			</body>

		</xhtml:html>
	</xsl:template>

	<!-- Metadata -->
	<xsl:template match="bi:meta">
		<xsl:if test="bi:authors">
			<p>
				<strong>Authors:</strong>
				<ul>
					<xsl:for-each select="bi:authors/bi:add">
						<xsl:value-of select="."/>
					</xsl:for-each>
				</ul>
			</p>
		</xsl:if>
		<xsl:if test="bi:annotation/xhtml:div">
			<xsl:apply-templates select="bi:annotation/xhtml:div/xhtml:*" mode="stripHtmlNamespace"/>
		</xsl:if>
		<xsl:if test="bi:annotation/text()">
			<p>
				<xsl:value-of select="bi:annotation/text()"/>
			</p>
		</xsl:if>
		<xsl:if test="bi:policies">
			<p>
				<strong>Policies / Demands:</strong>
				<ul>
					<xsl:for-each select="bi:policies/bi:demand">
						<li>
							<xsl:value-of select="."/>
						</li>
					</xsl:for-each>
				</ul>
			</p>

		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:*" mode="stripHtmlNamespace">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@* | node()"  mode="stripHtmlNamespace" />
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>
