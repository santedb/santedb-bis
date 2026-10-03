<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl xsi"
				xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
				xmlns:bi="http://santedb.org/bi"
>
	<xsl:template match="bi:BiDefinitionCollection" mode="body">

		<h1>BI Definition Collection</h1>
		<xsl:apply-templates select="bi:resource" />
	</xsl:template>
</xsl:stylesheet>