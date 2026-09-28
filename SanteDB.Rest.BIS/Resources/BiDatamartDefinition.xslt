<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
				xmlns:bi="http://santedb.org/bi"
>

	<xsl:template match="bi:BiDatamartDefinition" mode="body">
		<h1>Datamart - <xsl:value-of select="@id"/></h1>
		<xsl:apply-templates select="bi:meta" />

		<table border="1">
			<tr>
				<th>Refresh Frequency</th>
				<td>
					<xsl:value-of select="bi:refreshFrequency/text()"/>
				</td>
			</tr>
			<tr>
				<th>Produces Datasource:</th>
				<td>
					<xsl:value-of select="bi:produces/@id"/> (<xsl:value-of select="bi:produces/@name"/>)
					<xsl:apply-templates select="bi:produces/bi:meta"/>
				</td>
			</tr>
		</table>

		<h2>Data Dictionary / Schema</h2>
		<xsl:apply-templates select="bi:schema" />
	</xsl:template>


	<xsl:template match="bi:schema">
		<table border="1">
			<thead>
				<tr>
					<th>Entity</th>
					<th>Column</th>
					<th>Type</th>
					<!--<th>Attributes</th>-->
					<th>Description</th>
				</tr>
			</thead>
			<tbody>
				<xsl:apply-templates select="bi:table|bi:view" mode="schemaRow" />
			</tbody>
		</table>
	</xsl:template>

	<xsl:template match="bi:table|bi:view" mode="schemaRow">
		<tr>
			<th rowspan="{count(bi:column) + 1}" valign="top">
				<a name="schObj{@name}">
				<xsl:value-of select="@name"/>
				</a>
				<xsl:if test="bi:parent">
					: <a href="#schObj{bi:parent/@ref}">
						<xsl:value-of select="bi:parent/@ref"/>
					</a>
				</xsl:if>
			</th>
			<td colspan="3" align="left" valign="top">
				<xsl:apply-templates select="bi:meta"/>
			</td>
		</tr>
		<xsl:apply-templates select="bi:column" mode="schemaRow"/>
	</xsl:template>

	<xsl:template match="bi:column" mode="schemaRow">
		<tr>
			<td align="center">
				<xsl:value-of select="@name"/>
				<xsl:if test="@key or @index or @unique or @notNull">
					<br/>
				<strong>
					<xsl:if test="@key = 'true'">[PK]</xsl:if>
					<xsl:if test="@index = 'true'">[IX]</xsl:if>
					<xsl:if test="@unique = 'true'">[UQ]</xsl:if>
					<xsl:if test="@notNull = 'true'">[NN]</xsl:if>
				</strong>
				</xsl:if>
			</td>
			<td align="center">
				<xsl:choose>
					<xsl:when test="@type = 'uuid'">UUID</xsl:when>
					<xsl:when test="@type = 'date-time'">TIMESTAMP</xsl:when>
					<xsl:when test="@type = 'date'">DATE</xsl:when>
					<xsl:when test="@type = 'time'">TIME</xsl:when>
					<xsl:when test="@type = 'blob'">VARBINARY</xsl:when>
					<xsl:when test="@type = 'string'">VARCHAR</xsl:when>
					<xsl:when test="@type = 'bool'">BOOLEAN</xsl:when>
					<xsl:when test="@type = 'decimal'">NUMERIC</xsl:when>
					<xsl:when test="@type = 'float'">REAL</xsl:when>
					<xsl:when test="@type = 'int'">BIGINT</xsl:when>
					<xsl:when test="@type = 'ref'">REF</xsl:when>
				</xsl:choose>
				<xsl:if test="bi:otherTable">
					- <a href="#schObj{bi:otherTable/@ref}">
						<xsl:value-of select="bi:otherTable/@ref"/>
					</a>
				</xsl:if>
			</td>
			<td>
				<xsl:apply-templates select="bi:meta"/>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>
