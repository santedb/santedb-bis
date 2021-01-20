<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:s="urn:schemas-microsoft-com:office:spreadsheet"
    xmlns:o="urn:schemas-microsoft-com:office:office"
    xmlns:x="urn:schemas-microsoft-com:office:excel"
    xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
    xmlns:html="http://www.w3.org/1999/xhtml"
                xmlns:sdb="http://santedb.org/bi"
>
  <xsl:strip-space elements="*"/>
  <xsl:param name="sdb:current-date" />
  <xsl:output method="xml" indent="yes"/>
  <xsl:template match="@* | node()" priority="-1">
    <xsl:apply-templates select="@* | node()"/>
  </xsl:template>
  <xsl:template match="//html:table">
    <xsl:processing-instruction name="mso-application">progid="Excel.Sheet"</xsl:processing-instruction>
    <s:Workbook>
      <o:DocumentProperties>
        <o:LastAuthor>SanteDB</o:LastAuthor>
        <o:Created>
          <xsl:value-of select="sdb:current-date"/>
        </o:Created>
        <o:Version>16.00</o:Version>
      </o:DocumentProperties>
      <o:OfficeDocumentSettings>
        <o:AllowPNG/>
      </o:OfficeDocumentSettings>
      <s:ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">
        <s:WindowHeight>12165</s:WindowHeight>
        <s:WindowWidth>28800</s:WindowWidth>
        <s:WindowTopX>32767</s:WindowTopX>
        <s:WindowTopY>32767</s:WindowTopY>
        <s:ProtectStructure>False</s:ProtectStructure>
        <s:ProtectWindows>False</s:ProtectWindows>
      </s:ExcelWorkbook>
      <s:Styles>
        <s:Style ss:ID="Normal" ss:Name="Normal">
          <s:Alignment ss:Vertical="Bottom"/>
          <s:Borders/>
          <s:Font ss:FontName="Calibri" x:Family="Swiss" ss:Size="11" ss:Color="#000000"/>
          <s:Interior/>
          <s:NumberFormat ss:Format="@"/>
          <s:Protection/>
        </s:Style>
        <s:Style ss:ID="Header" ss:Name="Header">
          <s:Alignment ss:Vertical="Bottom"/>
          <s:Borders/>
          <s:Font ss:Bold="1" ss:FontName="Calibri" x:Family="Swiss" ss:Size="11" ss:Color="#000000"/>
          <s:Interior/>
          <s:NumberFormat ss:Format="@"/>
          <s:Protection/>
        </s:Style>
      </s:Styles>
      <s:Worksheet ss:Name="santedb-output">
        <x:WorksheetOptions>
          <x:PageSetup>
            <x:Header x:Margin="0.3"/>
            <x:Footer x:Margin="0.3"/>
            <x:PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>
          </x:PageSetup>
          <x:Unsynced/>
          <x:Selected/>
          <x:FreezePanes/>
          <x:FrozenNoSplit/>
          <x:SplitHorizontal>1</x:SplitHorizontal>
          <x:TopRowBottomPane>1</x:TopRowBottomPane>
          <x:ActivePane>2</x:ActivePane>
          <x:ProtectObjects>False</x:ProtectObjects>
          <x:ProtectScenarios>False</x:ProtectScenarios>
        </x:WorksheetOptions>
        <s:Table ss:DefaultRowHeight="15">
          <xsl:apply-templates select="html:thead" />
          <xsl:apply-templates select="html:tbody" />
          <xsl:apply-templates select="html:tfoot" />
        </s:Table>
      </s:Worksheet>

    </s:Workbook>
  </xsl:template>
  <xsl:template match="html:thead">
    <xsl:apply-templates select="html:tr"/>
  </xsl:template>
  <xsl:template match="html:tbody">
    <xsl:apply-templates select="html:tr"/>
  </xsl:template>
  <xsl:template match="html:tfoot">
    <xsl:apply-templates select="html:tr"/>
  </xsl:template>
  <xsl:template match="html:tr">
    <s:Row ss:AutoFitHeight="0">
      <xsl:apply-templates select="html:th|html:td"></xsl:apply-templates>
    </s:Row>
  </xsl:template>
  <xsl:template match="html:td">
    <s:Cell ss:StyleID="Normal">
      <xsl:if test="@colspan">
        <xsl:attribute name="ss:MergeAcross">
          <xsl:value-of select="@colspan"/>
        </xsl:attribute>
      </xsl:if>
      <s:Data ss:Type="String">
        <xsl:value-of select="."/>
      </s:Data>
    </s:Cell>
  </xsl:template>
  <xsl:template match="html:th">
    <s:Cell ss:StyleID="Header">
      <xsl:if test="@colspan">
        <xsl:attribute name="ss:MergeAcross">
          <xsl:value-of select="@colspan - 1"/>
        </xsl:attribute>
      </xsl:if>
      <s:Data ss:Type="String">
        
        <xsl:value-of select="."/>
      </s:Data>
    </s:Cell>
  </xsl:template>
</xsl:stylesheet>
