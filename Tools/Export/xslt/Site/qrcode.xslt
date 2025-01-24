<?xml version="1.0"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:output method="xml" indent="yes"/>
  <xsl:param name="style"/>


  <xsl:template match="/">
    <html>
      <xsl:apply-templates/>
    </html>
  </xsl:template>

  <xsl:template match="/*">
    <head>
      <meta charset="utf-8"/>
      <title>JUDO</title>

      <style type="text/css">
        .header1 {
            text-align: left;
            vertical-align: bottom;
            font-size: 2.5em;
            font-weight: 400;
            margin: 15px;
        }</style>
    </head>
    <body>
      <div class="header1"> Suivi de la compétition de JUDO </div>


      <center>
        <div>
          <xsl:value-of select="./@address"/>
          <!--<xsl:text>http://</xsl:text>
          <xsl:value-of select="./@address"/>
          <xsl:text>:</xsl:text>
          <xsl:value-of select="./@port"/>
          <xsl:text>/site/qrcode.png</xsl:text>-->
        </div>

        <img border="0" align="center" height="500px" width="500px">
          <xsl:attribute name="src">
            <xsl:value-of select="./@image"/>
          </xsl:attribute>
          <xsl:text>&#32;</xsl:text>
        </img>
      </center>
    </body>
  </xsl:template>
</xsl:stylesheet>
