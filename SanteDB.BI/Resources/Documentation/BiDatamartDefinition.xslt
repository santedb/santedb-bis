<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
				xmlns:bi="http://santedb.org/bi"
>

	<xsl:template name="continueFlow">
		<xsl:param name="inputName"/>
		<xsl:apply-templates select="(../bi:connection|../bi:pipeline|../bi:log|../bi:transaction|../bi:reader|../bi:writer|../bi:union|../bi:filter|../bi:crosstab|../bi:halt|../bi:call|../bi:transform)[bi:input/@ref = $inputName]" mode="flowdoc"/>
	</xsl:template>

	<xsl:template match="bi:BiDatamartDefinition" mode="body">
		<h2>
			<a name="{@id}">
				Datamart -
				<xsl:value-of select="@id"/>
			</a>

		</h2>
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

		<h3>Data Dictionary / Schema</h3>
		<xsl:apply-templates select="bi:schema" />
		<h3>Data Flows</h3>
		<xsl:apply-templates select="bi:dataFlows/bi:flow" />
	</xsl:template>

	<xsl:template match="bi:flow">
		<h4>
			<a name="flwObj{@name}">
				<xsl:value-of select="@name"/>
			</a>
		</h4>
		<xsl:apply-templates select="bi:meta" />
		<ul>
			<xsl:if test="bi:parameters">
				<li>
					<strong>Parameters</strong>
					<ul>
						<xsl:for-each select="bi:parameters/bi:ref">
							<li>
								<xsl:value-of select="@name"/>
							</li>
						</xsl:for-each>
					</ul>
				</li>
			</xsl:if>
			<xsl:apply-templates select="(bi:connection|bi:pipeline|bi:log|bi:transaction|bi:reader|bi:writer|bi:union|bi:filter|bi:crosstab|bi:halt|bi:call)[not(./bi:input)]" mode="flowdoc"/>
		</ul>
	</xsl:template>

	<xsl:template match="bi:log" mode="flowdoc">
		<li>
			<strong>LOG </strong>
			<em>
				<xsl:value-of select="@priority"/>
			</em>
			<code>
				<xsl:value-of select="."/>
			</code>
		</li>
	</xsl:template>


	<xsl:template match="bi:reader" mode="flowdoc">
		<li>
			<strong>OPEN READER </strong>
			<code class="variable">
				<xsl:value-of select="@name" />
			</code>
			ON <em>
				<xsl:value-of select="bi:connection/@ref" />
			</em>
			FROM QUERY FOR
			<xsl:for-each select="bi:sql/bi:add/bi:providers/bi:invariant">
				<code class="invariant">
					<xsl:value-of select="text()"/>
				</code>
			</xsl:for-each>
		</li>
		<xsl:call-template name="continueFlow">
			<xsl:with-param name="inputName" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="bi:writer" mode="flowdoc">
		<li>
			<strong>OPEN WRITER </strong>
			<code class="variable">
				<xsl:value-of select="@name" />
			</code>
			ON <em>
				<xsl:value-of select="bi:connection/@ref" />
			</em>
			STREAM INPUT FROM <code class="variable">
				<xsl:value-of select="bi:input/@ref"/>
			</code>
			WRITE OUTPUT TO 
			<a href="#schObj{bi:target/@name}">
			<code class="table">
				<xsl:value-of select="bi:target/@name"/>
			</code>
			</a>
			<xsl:if test="@truncate = 'true'">
				TRUNCATING TABLE
			</xsl:if>
		</li>
		<xsl:call-template name="continueFlow">
			<xsl:with-param name="inputName" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="bi:crosstab" mode="flowdoc">
		<li>
			<strong>CROSSTAB </strong>
			<code class="variable">
				<xsl:value-of select="@name" />
			</code>
			STREAMING INPUT FROM <em>
				<xsl:value-of select="bi:input/@ref" />
			</em>
			PIVOT ON <code class="column">
				<xsl:value-of select="bi:pivot/@columnDef"/>
			</code>
			SELECTING
			<code class="fn">
				<xsl:value-of select="bi:pivot/@fn"/>
			</code>
			<code  class="column">
				<xsl:value-of select="bi:pivot/@value"/>
			</code>
		</li>
		<xsl:call-template name="continueFlow">
			<xsl:with-param name="inputName" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="bi:transform" mode="flowdoc">
		<li>
			<strong>TRANSFORM </strong>
			<code class="variable">
				<xsl:value-of select="@name" />
			</code>
			STREAMING INPUT FROM <code class="variable">
				<xsl:value-of select="bi:input/@ref" />
			</code>
			<table border="1">
				<caption>MAPPING</caption>
				<thead>
					<tr>
						<th>FROM</th>
						<th>TO</th>
					</tr>
				</thead>
				<tbody>
					<xsl:for-each select="bi:map">
						<tr>
							<td>
								<xsl:choose>
									<xsl:when test="bi:source/@name">
										<code class="column">
											<xsl:value-of select="bi:source/@name"/>
										</code>
									</xsl:when>
									<xsl:when test="bi:source/bi:fixed">
										<code class="string">
											<xsl:value-of select="bi:source/bi:fixed"/>
										</code>
									</xsl:when>
								</xsl:choose>
							</td>
							<td>
								<code class="column">
									<xsl:value-of select="bi:target/@name"/>
								</code>
							</td>
						</tr>
					</xsl:for-each>
				</tbody>
			</table>
		</li>
		<xsl:call-template name="continueFlow">
			<xsl:with-param name="inputName" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="bi:filter" mode="flowdoc">
		<li>
			<strong>FILTER </strong>
			<code class="variable">
				<xsl:value-of select="@name" />
			</code>
				STREAMING INPUT FROM <code class="variable">
				<xsl:value-of select="bi:input/@ref" />
			</code>
			TODO: SHOW THE FILTER LOGIC HERE 
		</li>
		<xsl:call-template name="continueFlow">
			<xsl:with-param name="inputName" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="bi:halt" mode="flowdoc">
		<li>
			<strong>HALT EXECUTION </strong>
			<code class="variable">
				<xsl:value-of select="@name" />
			</code>
			WHEN ANY INPUT FROM <code class="variable">
				<xsl:value-of select="bi:input/@ref" />
			</code>
		</li>
	</xsl:template>
	<xsl:template match="bi:transaction" mode="flowdoc">
		<li>
			<strong>BEGIN TRANSACTION </strong>
			<em>
				<xsl:value-of select="@name"/>
			</em>
			ON
			<a href="#flwCon{bi:connection/@ref}">
				<xsl:value-of select="@bi:connection/@ref"/>
			</a>
		</li>
		<xsl:apply-templates select="(bi:pipeline|bi:log|bi:transaction|bi:reader|bi:writer|bi:union|bi:filter|bi:crosstab|bi:halt|bi:call)[not(./bi:input)]" mode="flowdoc"/>
		<li>
			<strong>COMMIT</strong>
		</li>
	</xsl:template>
	<xsl:template match="bi:call" mode="flowdoc">
		<li>
			<strong>CALL </strong>
			<a href="#flwObj{bi:dataFlow/@ref}">
				<xsl:value-of select="bi:dataFlow/@ref"/>
			</a>
			<ul>
				<xsl:for-each select="bi:args/bi:ref">
					<li>
						<xsl:value-of select="@name"/> = <xsl:value-of select="bi:value/@ref"/>
					</li>
				</xsl:for-each>
			</ul>
		</li>
	</xsl:template>
	<xsl:template match="bi:connection[not(./bi:dataSource)]" mode="flowdoc"></xsl:template>
	<xsl:template match="bi:connection[./bi:dataSource]" mode="flowdoc">
		<li>
			<a name="flwCon{@name}">
				<strong>OPEN </strong>
				<xsl:value-of select="bi:dataSource/@ref"/> AS <em>
					<xsl:value-of select="@name"/>
				</em>
			</a>
		</li>
	</xsl:template>
	<xsl:template match="bi:pipeline" mode="flowdoc">
		<li>
			<strong>PIPELINE</strong>
			<ol>
				<xsl:apply-templates select="(bi:connection|bi:pipeline|bi:log|bi:transaction|bi:reader|bi:writer|bi:union|bi:filter|bi:crosstab|bi:halt|bi:call)[not(./bi:input)]" mode="flowdoc"/>
			</ol>
		</li>
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
