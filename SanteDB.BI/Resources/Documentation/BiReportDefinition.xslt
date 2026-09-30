<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl xsi"
				xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
				xmlns:bi="http://santedb.org/bi"
				xmlns:xhtml="http://www.w3.org/1999/xhtml"
				xmlns:biv="http://santedb.org/bi/view"
>


	<xsl:template name="reportContents">
		<xsl:param name="headingClass" />

		<xsl:element name="{$headingClass}">Source</xsl:element>
		<xsl:apply-templates select="bi:dataSources" mode="dataSource" />
		<xsl:element name="{$headingClass}">Views / Renders</xsl:element>
		<xsl:apply-templates select="bi:views/bi:add" />
	</xsl:template>

	<xsl:template match="bi:BiReportDefinition" mode="body">
		<h1>
			<a name="{@id}">
				Report -
				<xsl:value-of select="@id"/>
			</a>
		</h1>


		<xsl:apply-templates select="bi:meta" />
		<xsl:call-template name="reportContents">
			<xsl:with-param name="headingClass" select="'h2'"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="bi:resource[@xsi:type = 'BiReportDefinition']">
		<h2>
			<a name="{@id}">
				Report -
				<xsl:value-of select="@id"/>
			</a>
		</h2>

		<xsl:apply-templates select="bi:meta" />
		<xsl:call-template name="reportContents">
			<xsl:with-param name="headingClass" select="'h3'"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="bi:view" mode="dataSource">
		<table border="1">
			<caption>
				VIEW - <xsl:value-of select="@name" /> (<xsl:value-of select="@id"/>)
			</caption>
			<tr>
				<td>TODO</td>
			</tr>
		</table>
	</xsl:template>

	<xsl:template match="bi:query" mode="dataSource">
		<table border="1">
			<caption>
				QUERY - <xsl:value-of select="@name" /> (<xsl:value-of select="@id"/>)
			</caption>
			<tr>
				<th>Database Connection(s)</th>
				<td>
					<ul>
						<xsl:for-each select="bi:dataSources/bi:add">
							<li>
								<strong>
									<xsl:value-of select="@name"/>
								</strong>
								<xsl:value-of select="@id"/>
							</li>
						</xsl:for-each>
					</ul>
				</td>
			</tr>
			<tr>
				<th>Parameters</th>
				<td>
					<dl>
						<xsl:apply-templates select="bi:parameters/bi:add" />
					</dl>
				</td>
			</tr>
			<tr>
				<th>Definition</th>
				<td>
					<ul>
						<xsl:for-each select="bi:definitions/bi:add/bi:providers/bi:invariant">
							<li>
								<xsl:value-of select="."/>
							</li>
						</xsl:for-each>
					</ul>

					<xsl:apply-templates select="bi:definitions/bi:add/bi:meta" mode="simple" />
				</td>
			</tr>
		</table>
	</xsl:template>

	<xsl:template match="bi:add[parent::bi:views]">
		<h3>
			<xsl:value-of select="@label"/> (
			<xsl:choose>
				<xsl:when test="@type = 'tabular'">TABULAR</xsl:when>
				<xsl:when test="@type = 'chart'">CHART</xsl:when>
				<xsl:otherwise>OTHER</xsl:otherwise>
			</xsl:choose>)
		</h3>
		<xsl:apply-templates select="bi:meta"/>

		<div class="preview">
			<xsl:apply-templates select="xhtml:div" mode="stripHtmlNamespace" />
		</div>
	</xsl:template>
	<xsl:template match="bi:add[parent::bi:parameters]">
		<dt>
			<code class="variable">
				<xsl:value-of select="@name"/>
			</code>
			: <code class="type">
				<xsl:value-of select="@type"/>
			</code>

			<xsl:if test="@required='true'">
				<strong>(REQUIRED)</strong>
			</xsl:if>
		</dt>
		<dd>
			<xsl:if test="bi:meta/bi:annotation">
				<xsl:apply-templates select="bi:meta" mode="simple"/>
			</xsl:if>
			<xsl:if test="bi:query">
				Dropdown is populated via query:
				<pre>
					<xsl:value-of select="bi:query/bi:definitions/bi:add/text()[last()]" />
				</pre>
			</xsl:if>
		</dd>
	</xsl:template>

	<xsl:template match="biv:chart" mode="stripHtmlNamespace">
		<div class="chart-placeholder">
			<div class="chart-type">
				<xsl:choose>
					<xsl:when test="@type = 'bar'">BAR CHART</xsl:when>
					<xsl:when test="@type = 'pie'">PIE CHART</xsl:when>
					<xsl:when test="@type = 'radar'">RADAR CHART</xsl:when>
					<xsl:when test="@type = 'line'">LINE CHART</xsl:when>
				</xsl:choose> FROM
				<code type="variable">
					<xsl:value-of select="@source" />
				</code>
			</div>
			<div class="chart-header">
				<xsl:value-of select="biv:title"/>
			</div>
			<div class="chart-body">
				<xsl:if test="biv:xAxis">
					<div class="chart-xaxis">
						<strong>X Axis:</strong>

						<strong>
							<xsl:value-of select="biv:xAxis/@label"/> =
						</strong>
						<code class="column">
							<xsl:value-of select="biv:xAxis/text()"/>
						</code>
					</div>
				</xsl:if>
				<xsl:if test="biv:yAxis">
					<div class="chart-yaxis">
						<strong>Y Axis:</strong>
						<strong>
							<xsl:value-of select="biv:yAxis/@label"/> =
						</strong>
						<code class="column">
							<xsl:choose>
								<xsl:when test="biv:yAxis/@max">
									0..<xsl:value-of select="biv:yAxis/@max"/>
								</xsl:when>
								<xsl:when test="biv:yAxis/text()">
									<xsl:value-of select="biv:yAxis/text()"/>
								</xsl:when>
								<xsl:otherwise>
									-
								</xsl:otherwise>
							</xsl:choose>
						</code>
					</div>
				</xsl:if>
				<div class="chart-datasets">
					<strong>Data Series:</strong>
					<ul>
						<xsl:for-each select="biv:dataset|biv:refset">
							<li>
								<strong>
									<xsl:value-of select="@label"/> = 
								</strong>
								<xsl:value-of select="."/>
							</li>
						</xsl:for-each>
					</ul>
				</div>
				<xsl:if test="biv:labels">
					<div class="chart-labels">
						<strong>Labels:</strong>
						<xsl:value-of select="biv:labels"/>
					</div>
				</xsl:if>
			</div>
		</div>
	</xsl:template>
	<xsl:template match="biv:dataTable" mode="stripHtmlNamespace">
		<table border="1">
			<caption>
				DATA_TABLE_RENDER FROM <xsl:value-of select="@source"/>
			</caption>
			<thead>
				<tr>
					<xsl:for-each select="biv:column/biv:header">
						<th>
							<xsl:apply-templates select="@* | node()"  mode="stripHtmlNamespace" />
						</th>
					</xsl:for-each>
				</tr>
			</thead>
			<tbody>
				<tr>
					<xsl:for-each select="biv:column/biv:cell">
						<td>
							<xsl:apply-templates select="@* | node()"  mode="stripHtmlNamespace" />
						</td>
					</xsl:for-each>
				</tr>
			</tbody>
		</table>
	</xsl:template>
</xsl:stylesheet>