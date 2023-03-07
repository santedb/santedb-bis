<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:html="http://www.w3.org/1999/xhtml"
    xmlns:sdb="http://santedb.org/bi"
>
  <xsl:strip-space elements="*" />
  <xsl:param name="sdb:current-date" />
  <xsl:output method="xml" indent="yes" />
  <xsl:template match="@* | node()" priority="-1">
    <xsl:apply-templates select="@* | node()" />
  </xsl:template>
  <xsl:template match="//html:table">
    <sdb:Worksheet>
      <xsl:apply-templates select="html:thead" />
      <xsl:apply-templates select="html:tbody" />
      <xsl:apply-templates select="html:tfoot" />
    </sdb:Worksheet>
  </xsl:template>
  <xsl:template match="html:thead">
    <xsl:apply-templates select="html:tr" />
  </xsl:template>
  <xsl:template match="html:tbody">
    <xsl:apply-templates select="html:tr" />
  </xsl:template>
  <xsl:template match="html:tfoot">
    <xsl:apply-templates select="html:tr" />
  </xsl:template>
  <xsl:template match="html:tr">
    <sdb:Row>
      <xsl:apply-templates select="html:th|html:td"></xsl:apply-templates>
    </sdb:Row>
  </xsl:template>
  <xsl:template match="html:td">
    <sdb:Cell>
      <xsl:if test="@colspan">
        <xsl:attribute name="colspan">
          <xsl:value-of select="@colspan" />
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="." />
    </sdb:Cell>
  </xsl:template>
  <xsl:template match="html:th">
    <sdb:Header>
      <xsl:if test="@colspan">
        <xsl:attribute name="colspan">
          <xsl:value-of select="@colspan" />
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="." />
    </sdb:Header>
  </xsl:template>
</xsl:stylesheet>